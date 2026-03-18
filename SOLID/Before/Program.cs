namespace SOLID.Before
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var orderService = new OrderService();

            var order = new Order
            {
                Id = 1,
                CustomerEmail = "cliente@email.com",
                CustomerPhone = "11999999999",
                Total = 1500m,
                PaymentType = "Pix"
            };

            orderService.PlaceOrder(order);

            Console.WriteLine("\nTentando cancelar pedido com pagamento Pix...");

            try
            {
                PaymentMethod payment = new PixPayment();
                orderService.CancelOrder(order, payment);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no cancelamento: {ex.Message}");
            }

            Console.WriteLine("\nExecutando rotina do robô de backoffice...");

            IBackOfficeWorker worker = new RobotBackOfficeWorker();
            worker.ProcessOrder(order);

            try
            {
                worker.PrintInvoice(order);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao imprimir nota: {ex.Message}");
            }
        }
    }

    public class Order
    {
        public int Id { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string PaymentType { get; set; } = string.Empty;
    }

    // Viola S, O e D ao mesmo tempo.
    public class OrderService
    {
        private readonly SqlServerOrderRepository _repository;
        private readonly EmailSender _emailSender;
        private readonly SmsSender _smsSender;
        private readonly ConsoleLogger _logger;

        public OrderService()
        {
            _repository = new SqlServerOrderRepository();
            _emailSender = new EmailSender();
            _smsSender = new SmsSender();
            _logger = new ConsoleLogger();
        }

        public void PlaceOrder(Order order)
        {
            if (string.IsNullOrWhiteSpace(order.CustomerEmail))
            {
                throw new InvalidOperationException("E-mail do cliente é obrigatório.");
            }

            if (order.Total <= 0)
            {
                throw new InvalidOperationException("Total do pedido deve ser maior que zero.");
            }

            decimal finalTotal = order.Total;
            if (order.Total > 1000m)
            {
                finalTotal *= 0.9m;
            }

            if (order.PaymentType == "CreditCard")
            {
                var card = new CreditCardPayment();
                card.Pay(finalTotal);
            }
            else if (order.PaymentType == "Pix")
            {
                var pix = new PixPayment();
                pix.Pay(finalTotal);
            }
            else if (order.PaymentType == "Boleto")
            {
                var boleto = new BoletoPayment();
                boleto.Pay(finalTotal);
            }
            else
            {
                throw new InvalidOperationException("Tipo de pagamento não suportado.");
            }

            _repository.Save(order, finalTotal);
            _emailSender.Send(order.CustomerEmail, $"Pedido {order.Id} confirmado.");
            _smsSender.Send(order.CustomerPhone, $"Pedido {order.Id} recebido.");
            _logger.Log($"Pedido {order.Id} processado em {DateTime.Now}.");
        }

        public void CancelOrder(Order order, PaymentMethod paymentMethod)
        {
            paymentMethod.Refund(order.Total);
            _repository.MarkAsCanceled(order.Id);
            _logger.Log($"Pedido {order.Id} cancelado.");
        }
    }

    public abstract class PaymentMethod
    {
        public abstract void Pay(decimal amount);
        public abstract void Refund(decimal amount);
    }

    public class CreditCardPayment : PaymentMethod
    {
        public override void Pay(decimal amount)
        {
            Console.WriteLine($"Pagamento em cartão: R$ {amount:F2}");
        }

        public override void Refund(decimal amount)
        {
            Console.WriteLine($"Estorno no cartão: R$ {amount:F2}");
        }
    }

    // Viola LSP: não consegue cumprir contrato de Refund.
    public class PixPayment : PaymentMethod
    {
        public override void Pay(decimal amount)
        {
            Console.WriteLine($"Pagamento via Pix: R$ {amount:F2}");
        }

        public override void Refund(decimal amount)
        {
            throw new NotSupportedException("Pix não permite estorno por este fluxo.");
        }
    }

    public class BoletoPayment : PaymentMethod
    {
        public override void Pay(decimal amount)
        {
            Console.WriteLine($"Boleto gerado: R$ {amount:F2}");
        }

        public override void Refund(decimal amount)
        {
            Console.WriteLine($"Cancelamento de boleto: R$ {amount:F2}");
        }
    }

    // Viola ISP: interface ampla demais.
    public interface IBackOfficeWorker
    {
        void ProcessOrder(Order order);
        void PrintInvoice(Order order);
        void SendPromotionalSms(string phoneNumber);
        void GenerateMonthlyReport();
    }

    public class HumanBackOfficeWorker : IBackOfficeWorker
    {
        public void ProcessOrder(Order order)
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

    public class RobotBackOfficeWorker : IBackOfficeWorker
    {
        public void ProcessOrder(Order order)
        {
            Console.WriteLine($"Robô processando pedido {order.Id}...");
        }

        public void PrintInvoice(Order order)
        {
            throw new NotImplementedException("Robô não imprime notas físicas.");
        }

        public void SendPromotionalSms(string phoneNumber)
        {
            throw new NotImplementedException("Robô não envia campanhas de marketing.");
        }

        public void GenerateMonthlyReport()
        {
            throw new NotImplementedException("Robô não gera relatórios manuais.");
        }
    }

    public class SqlServerOrderRepository
    {
        public static void Save(Order order, decimal finalTotal)
        {
            Console.WriteLine($"Salvando pedido {order.Id} no SQL Server com total R$ {finalTotal:F2}...");
        }

        public static void MarkAsCanceled(int orderId)
        {
            Console.WriteLine($"Marcando pedido {orderId} como cancelado no SQL Server...");
        }
    }

    public class EmailSender
    {
        public static void Send(string to, string message)
        {
            Console.WriteLine($"Enviando e-mail para {to}: {message}");
        }
    }

    public class SmsSender
    {
        public static void Send(string phoneNumber, string message)
        {
            Console.WriteLine($"Enviando SMS para {phoneNumber}: {message}");
        }
    }

    public class ConsoleLogger
    {
        public static void Log(string message)
        {
            Console.WriteLine($"[LOG] {message}");
        }
    }
}
