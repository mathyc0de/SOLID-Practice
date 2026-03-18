namespace D.After
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var service = new CheckoutService(new EmailSender(), new SmsSender());
            service.FinishOrder("cliente@email.com", "Pedido #1234 finalizado com sucesso.");
        }
    }

    public interface IEmailSender
    {
        void Send(string to, string message);
    }

    public interface ISmsSender
    {
        void Send(string phoneNumber, string message);
    }

    public class EmailSender : IEmailSender
    {
        public void Send(string to, string message)
        {
            Console.WriteLine($"Enviando e-mail para {to}: {message}");
        }
    }

    public class SmsSender : ISmsSender
    {
        public void Send(string phoneNumber, string message)
        {
            Console.WriteLine($"Enviando SMS para {phoneNumber}: {message}");
        }
    }

    public class CheckoutService(IEmailSender emailSender, ISmsSender smsSender)
    {
        private readonly IEmailSender _emailSender = emailSender;
        private readonly ISmsSender _smsSender = smsSender;

        public void FinishOrder(string customerEmail, string message)
        {
            Console.WriteLine("Finalizando pedido...");
            Console.WriteLine("Salvando dados do pedido...");

            _emailSender.Send(customerEmail, message);
            _smsSender.Send("11999999999", message);
        }
    }
}