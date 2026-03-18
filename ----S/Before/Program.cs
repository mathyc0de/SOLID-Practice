namespace S.Before {
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

            var service = new OrderService();
            service.ProcessOrder(order);
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

    public class OrderService
    {
        public static void ProcessOrder(Order order)
        {
            if (string.IsNullOrWhiteSpace(order.CustomerEmail))
            {
                Console.WriteLine("Pedido inválido: e-mail do cliente é obrigatório.");
                return;
            }

            if (order.Items == null || order.Items.Count == 0)
            {
                Console.WriteLine("Pedido inválido: o pedido precisa ter itens.");
                return;
            }

            decimal total = 0;

            foreach (var item in order.Items)
            {
                total += item.Quantity * item.UnitPrice;
            }

            if (total > 1000)
            {
                total *= 0.9m; // 10% de desconto
            }

            Console.WriteLine("Salvando pedido no banco de dados...");
            Console.WriteLine($"Pedido salvo com total de R$ {total:F2}");

            Console.WriteLine($"Enviando e-mail para {order.CustomerEmail}...");
            Console.WriteLine("E-mail enviado com sucesso.");

            Console.WriteLine($"[LOG] Pedido processado em {DateTime.Now}");
        }
    }}