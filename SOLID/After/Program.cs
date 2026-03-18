namespace SOLID.After
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var orderFacade = OrderFacade.CreateDefault();

            var order = new Order(
                id: 1,
                customerEmail: "cliente@email.com",
                customerPhone: "11999999999",
                paymentKind: PaymentKind.Pix,
                items:
                [
                    new("Notebook", 1, 1400m),
                    new("Mouse", 1, 100m)
                ]
            );

            orderFacade.PlaceOrder(order);

            Console.WriteLine("\nTentando cancelar pedido com pagamento Pix...");
            orderFacade.CancelOrder(order);

            Console.WriteLine("\nExecutando rotina de backoffice...");

            IOrderProcessor robot = new RobotBackOfficeWorker();
            robot.Process(order);

            IOrderProcessor human = new HumanBackOfficeWorker();
            human.Process(order);

            if (human is IInvoicePrinter invoicePrinter)
            {
                invoicePrinter.PrintInvoice(order);
            }
        }
    }

    public enum PaymentKind
    {
        CreditCard,
        Pix,
        Boleto
    }

    public sealed record OrderItem(string ProductName, int Quantity, decimal UnitPrice);

    public sealed class Order(
        int id,
        string customerEmail,
        string customerPhone,
        PaymentKind paymentKind,
        IReadOnlyCollection<OrderItem> items)
    {
        public int Id { get; } = id;
        public string CustomerEmail { get; } = customerEmail;
        public string CustomerPhone { get; } = customerPhone;
        public PaymentKind PaymentKind { get; } = paymentKind;
        public IReadOnlyCollection<OrderItem> Items { get; } = items;
    }

    public interface IOrderValidator
    {
        void Validate(Order order);
    }

    public interface IDiscountPolicy
    {
        decimal Apply(decimal subtotal);
    }

    public interface IPriceCalculator
    {
        decimal Calculate(Order order);
    }

    public interface IPaymentProcessor
    {
        PaymentKind SupportedKind { get; }
        void Pay(decimal amount);
    }

    public interface IRefundProcessor
    {
        PaymentKind SupportedKind { get; }
        void Refund(decimal amount);
    }

    public interface IPaymentGateway
    {
        void Pay(PaymentKind paymentKind, decimal amount);
    }

    public interface IRefundGateway
    {
        bool TryRefund(PaymentKind paymentKind, decimal amount);
    }

    public interface IOrderRepository
    {
        void Save(Order order, decimal finalTotal);
        void MarkAsCanceled(int orderId);
    }

    public interface INotifier
    {
        void Notify(Order order, string message);
    }

    public interface ILogger
    {
        void Info(string message);
    }

    public sealed class OrderFacade(OrderService orderService)
    {
        private readonly OrderService _orderService = orderService;

        public static OrderFacade CreateDefault()
        {
            var orderService = new OrderService(
                new OrderValidator(),
                new PriceCalculator(new VolumeDiscountPolicy()),
                new PaymentGateway(
                [
                    new CreditCardPaymentProcessor(),
                    new PixPaymentProcessor(),
                    new BoletoPaymentProcessor()
                ]),
                new RefundGateway(
                [
                    new CreditCardRefundProcessor(),
                    new BoletoRefundProcessor()
                ]),
                new SqlServerOrderRepository(),
                new CompositeNotifier(
                [
                    new EmailNotifier(),
                    new SmsNotifier()
                ]),
                new ConsoleLogger()
            );

            return new OrderFacade(orderService);
        }

        public void PlaceOrder(Order order)
        {
            _orderService.PlaceOrder(order);
        }

        public void CancelOrder(Order order)
        {
            _orderService.CancelOrder(order);
        }
    }

    public sealed class OrderService(
        IOrderValidator validator,
        IPriceCalculator priceCalculator,
        IPaymentGateway paymentGateway,
        IRefundGateway refundGateway,
        IOrderRepository repository,
        INotifier notifier,
        ILogger logger)
    {
        private readonly IOrderValidator _validator = validator;
        private readonly IPriceCalculator _priceCalculator = priceCalculator;
        private readonly IPaymentGateway _paymentGateway = paymentGateway;
        private readonly IRefundGateway _refundGateway = refundGateway;
        private readonly IOrderRepository _repository = repository;
        private readonly INotifier _notifier = notifier;
        private readonly ILogger _logger = logger;

        public void PlaceOrder(Order order)
        {
            _validator.Validate(order);

            var finalTotal = _priceCalculator.Calculate(order);

            _paymentGateway.Pay(order.PaymentKind, finalTotal);
            _repository.Save(order, finalTotal);
            _notifier.Notify(order, $"Pedido {order.Id} confirmado.");
            _logger.Info($"Pedido {order.Id} processado em {DateTime.Now}.");
        }

        public void CancelOrder(Order order)
        {
            var total = _priceCalculator.Calculate(order);

            if (!_refundGateway.TryRefund(order.PaymentKind, total))
            {
                _logger.Info($"Pedido {order.Id} não foi estornado: pagamento {order.PaymentKind} não suporta estorno.");
                return;
            }

            _repository.MarkAsCanceled(order.Id);
            _logger.Info($"Pedido {order.Id} cancelado.");
        }
    }

    public sealed class OrderValidator : IOrderValidator
    {
        public void Validate(Order order)
        {
            if (order is null)
            {
                throw new InvalidOperationException("Pedido é obrigatório.");
            }

            if (string.IsNullOrWhiteSpace(order.CustomerEmail))
            {
                throw new InvalidOperationException("E-mail do cliente é obrigatório.");
            }

            if (string.IsNullOrWhiteSpace(order.CustomerPhone))
            {
                throw new InvalidOperationException("Telefone do cliente é obrigatório.");
            }

            if (order.Items.Count == 0)
            {
                throw new InvalidOperationException("O pedido precisa ter itens.");
            }

            if (order.Items.Any(item => item.Quantity <= 0 || item.UnitPrice <= 0))
            {
                throw new InvalidOperationException("Todos os itens devem ter quantidade e preço válidos.");
            }
        }
    }

    public sealed class VolumeDiscountPolicy : IDiscountPolicy
    {
        public decimal Apply(decimal subtotal)
        {
            if (subtotal > 1000m)
            {
                return subtotal * 0.9m;
            }

            return subtotal;
        }
    }

    public sealed class PriceCalculator(IDiscountPolicy discountPolicy) : IPriceCalculator
    {
        private readonly IDiscountPolicy _discountPolicy = discountPolicy;

        public decimal Calculate(Order order)
        {
            var subtotal = order.Items.Sum(item => item.Quantity * item.UnitPrice);
            return _discountPolicy.Apply(subtotal);
        }
    }

    public sealed class PaymentGateway(IEnumerable<IPaymentProcessor> processors) : IPaymentGateway
    {
        private readonly IReadOnlyDictionary<PaymentKind, IPaymentProcessor> _processors = processors.ToDictionary(processor => processor.SupportedKind);

        public void Pay(PaymentKind paymentKind, decimal amount)
        {
            if (!_processors.TryGetValue(paymentKind, out var processor))
            {
                throw new InvalidOperationException($"Tipo de pagamento '{paymentKind}' não suportado.");
            }

            processor.Pay(amount);
        }
    }

    public sealed class RefundGateway(IEnumerable<IRefundProcessor> processors) : IRefundGateway
    {
        private readonly IReadOnlyDictionary<PaymentKind, IRefundProcessor> _processors = processors.ToDictionary(processor => processor.SupportedKind);

        public bool TryRefund(PaymentKind paymentKind, decimal amount)
        {
            if (!_processors.TryGetValue(paymentKind, out var processor))
            {
                return false;
            }

            processor.Refund(amount);
            return true;
        }
    }

    public sealed class CreditCardPaymentProcessor : IPaymentProcessor
    {
        public PaymentKind SupportedKind => PaymentKind.CreditCard;

        public void Pay(decimal amount)
        {
            Console.WriteLine($"Pagamento em cartão: R$ {amount:F2}");
        }
    }

    public sealed class PixPaymentProcessor : IPaymentProcessor
    {
        public PaymentKind SupportedKind => PaymentKind.Pix;

        public void Pay(decimal amount)
        {
            Console.WriteLine($"Pagamento via Pix: R$ {amount:F2}");
        }
    }

    public sealed class BoletoPaymentProcessor : IPaymentProcessor
    {
        public PaymentKind SupportedKind => PaymentKind.Boleto;

        public void Pay(decimal amount)
        {
            Console.WriteLine($"Boleto gerado: R$ {amount:F2}");
        }
    }

    public sealed class CreditCardRefundProcessor : IRefundProcessor
    {
        public PaymentKind SupportedKind => PaymentKind.CreditCard;

        public void Refund(decimal amount)
        {
            Console.WriteLine($"Estorno no cartão: R$ {amount:F2}");
        }
    }

    public sealed class BoletoRefundProcessor : IRefundProcessor
    {
        public PaymentKind SupportedKind => PaymentKind.Boleto;

        public void Refund(decimal amount)
        {
            Console.WriteLine($"Cancelamento de boleto: R$ {amount:F2}");
        }
    }

    public sealed class SqlServerOrderRepository : IOrderRepository
    {
        public void Save(Order order, decimal finalTotal)
        {
            Console.WriteLine($"Salvando pedido {order.Id} no SQL Server com total R$ {finalTotal:F2}...");
        }

        public void MarkAsCanceled(int orderId)
        {
            Console.WriteLine($"Marcando pedido {orderId} como cancelado no SQL Server...");
        }
    }

    public sealed class CompositeNotifier(IEnumerable<INotifier> notifiers) : INotifier
    {
        private readonly IEnumerable<INotifier> _notifiers = notifiers;

        public void Notify(Order order, string message)
        {
            foreach (var notifier in _notifiers)
            {
                notifier.Notify(order, message);
            }
        }
    }

    public sealed class EmailNotifier : INotifier
    {
        public void Notify(Order order, string message)
        {
            Console.WriteLine($"Enviando e-mail para {order.CustomerEmail}: {message}");
        }
    }

    public sealed class SmsNotifier : INotifier
    {
        public void Notify(Order order, string message)
        {
            Console.WriteLine($"Enviando SMS para {order.CustomerPhone}: {message}");
        }
    }

    public sealed class ConsoleLogger : ILogger
    {
        public void Info(string message)
        {
            Console.WriteLine($"[LOG] {message}");
        }
    }

    public interface IOrderProcessor
    {
        void Process(Order order);
    }

    public interface IInvoicePrinter
    {
        void PrintInvoice(Order order);
    }

    public interface IPromotionalMessenger
    {
        void SendPromotionalSms(string phoneNumber);
    }

    public interface IMonthlyReporter
    {
        void GenerateMonthlyReport();
    }

    public sealed class HumanBackOfficeWorker : IOrderProcessor, IInvoicePrinter, IPromotionalMessenger, IMonthlyReporter
    {
        public void Process(Order order)
        {
            Console.WriteLine($"Humano processando pedido {order.Id}...");
        }

        public void PrintInvoice(Order order)
        {
            Console.WriteLine($"Humano imprimindo nota do pedido {order.Id}...");
        }

        public void SendPromotionalSms(string phoneNumber)
        {
            Console.WriteLine($"Humano enviando SMS para {phoneNumber}...");
        }

        public void GenerateMonthlyReport()
        {
            Console.WriteLine("Humano gerando relatório mensal...");
        }
    }

    public sealed class RobotBackOfficeWorker : IOrderProcessor
    {
        public void Process(Order order)
        {
            Console.WriteLine($"Robô processando pedido {order.Id}...");
        }
    }
}
