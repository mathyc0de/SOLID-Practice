namespace D.Before
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var service = new CheckoutService();
            service.FinishOrder("cliente@email.com", "Pedido #1234 finalizado com sucesso.");
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

    public class CheckoutService
    {
        private readonly EmailSender _emailSender;
        private readonly SmsSender _smsSender;

        public CheckoutService()
        {
            _emailSender = new EmailSender();
            _smsSender = new SmsSender();
        }

        public void FinishOrder(string customerEmail, string message)
        {
            Console.WriteLine("Finalizando pedido...");
            Console.WriteLine("Salvando dados do pedido...");

            _emailSender.Send(customerEmail, message);
            _smsSender.Send("11999999999", message);
        }
    }
}