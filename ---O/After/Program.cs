namespace O.After
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var employees = new List<Employee>
            {
                new FullTimeEmployee("Ana", 5000m),
                new PartTimeEmployee("Bruno", 2500m),
                new InternEmployee("Carlos", 1500m)
            };

            foreach (var employee in employees)
            {
                Console.WriteLine($"{employee.Name} ({employee.GetType().Name}) => R$ {employee.CalculateSalary():F2}");
            }
        }
    }

    public abstract class Employee(string name, decimal baseSalary)
    {
        public string Name { get; } = name;
        public decimal BaseSalary { get; } = baseSalary;

        public abstract decimal CalculateSalary();
    }

    public class FullTimeEmployee(string name, decimal baseSalary) : Employee(name, baseSalary)
    {
        public override decimal CalculateSalary()
        {
            return BaseSalary + 2000m;
        }
    }

    public class PartTimeEmployee(string name, decimal baseSalary) : Employee(name, baseSalary)
    {
        public override decimal CalculateSalary()
        {
            return BaseSalary;
        }
    }

    public class InternEmployee(string name, decimal baseSalary) : Employee(name, baseSalary)
    {
        public override decimal CalculateSalary()
        {
            return BaseSalary - 500m;
        }
    }
}