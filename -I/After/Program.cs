namespace I.After
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
                if (worker is IHumanNeeds humanNeeds)
                {
                    humanNeeds.Eat();
                    humanNeeds.Sleep();
                }
                Console.WriteLine();
            }
        }
    }

    public interface IWorker
    {
        void Work();
    }

    public interface IHumanNeeds
    {
        void Eat();
        void Sleep();
    }
    public class HumanWorker : IWorker, IHumanNeeds
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
    }
}