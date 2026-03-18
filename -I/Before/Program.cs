namespace I.Before
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var workers = new List<IWorker>
            {
                new HumanWorker(),
                new RobotWorker()
            };

            foreach (var worker in workers)
            {
                worker.Work();

                try
                {
                    worker.Eat();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao comer: {ex.Message}");
                }

                try
                {
                    worker.Sleep();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao dormir: {ex.Message}");
                }

                Console.WriteLine();
            }
        }
    }

    public interface IWorker
    {
        void Work();
        void Eat();
        void Sleep();
    }

    public class HumanWorker : IWorker
    {
        public void Work()
        {
            Console.WriteLine("Humano trabalhando...");
        }

        public void Eat()
        {
            Console.WriteLine("Humano comendo...");
        }

        public void Sleep()
        {
            Console.WriteLine("Humano dormindo...");
        }
    }

    public class RobotWorker : IWorker
    {
        public void Work()
        {
            Console.WriteLine("Robô trabalhando...");
        }

        public void Eat()
        {
            throw new NotImplementedException("Robôs não comem.");
        }

        public void Sleep()
        {
            throw new NotImplementedException("Robôs não dormem.");
        }
    }
}