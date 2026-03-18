namespace S.After
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var order = new Order
            {
                CustomerEmail = "cliente@email.com",
                Items =
                [
                    new("Notebook", 1, 3500m),
                    new("Mouse", 2, 80m)
                ]
            };

            OrderService.ProcessOrder(order);
        }
    }

    public class Order
    {
        public string CustomerEmail { get; set; } = string.Empty;
        public List<OrderItem> Items { get; set; } = [];
    }

    public class OrderItem(string productName, int quantity, decimal unitPrice)
    {
        public string ProductName { get; } = productName;
        public int Quantity { get; } = quantity;
        public decimal UnitPrice { get; } = unitPrice;
    }

    public static class OrderValidator
    {
        private static bool IsCustomerEmailNull(string customerEmail)
        {
            if (string.IsNullOrWhiteSpace(customerEmail))
            {
                Console.WriteLine("Pedido inválido: e-mail do cliente é obrigatório.");
                return true;
            }

            return false;
        }

        private static bool IsOrderEmpty(List<OrderItem> orderItems)
        {
            if (orderItems == null || orderItems.Count == 0)
            {
                Console.WriteLine("Pedido inválido: o pedido precisa ter itens.");
                return true;
            }

            return false;
        }

        public static bool IsValidOrder(Order order) =>
            !(IsCustomerEmailNull(order.CustomerEmail) || IsOrderEmpty(order.Items));
    }

    public static class OrderCalculator
    {
        public static decimal CalculateOrder(Order order)
        {
            decimal total = 0;

            foreach (var item in order.Items)
            {
                total += item.Quantity * item.UnitPrice;
            }

            if (total > 1000)
                return total * 0.9m;

            return total;
        }
    }

    public static class OrderRepository
    {
        public static void SaveOrder(decimal total)
        {
            Console.WriteLine("Salvando pedido no banco de dados...");
            Console.WriteLine($"Pedido salvo com total de R$ {total:F2}");
        }
    }

    public static class EmailNotifier
    {
        public static void NotifyCustomer(string customerEmail)
        {
            Console.WriteLine($"Enviando e-mail para {customerEmail}...");
            Console.WriteLine("E-mail enviado com sucesso.");
        }
    }

    public static class Logger
    {
        public static void LogOrderStatus()
        {
            Console.WriteLine($"[LOG] Pedido processado em {DateTime.Now}");
        }
    }

    public class OrderService
    {
        public static void ProcessOrder(Order order)
        {
            if (!OrderValidator.IsValidOrder(order)) return;

            decimal total = OrderCalculator.CalculateOrder(order);

            OrderRepository.SaveOrder(total);
            EmailNotifier.NotifyCustomer(order.CustomerEmail);
            Logger.LogOrderStatus();
        }
    }
}