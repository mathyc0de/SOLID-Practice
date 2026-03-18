namespace OCP.Before
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var employees = new List<Employee>
            {
                new("Ana", "FullTime", 5000m),
                new("Bruno", "PartTime", 2500m),
                new("Carlos", "Intern", 1500m)
            };

            var calculator = new SalaryCalculator();

            foreach (var employee in employees)
            {
                var salary = calculator.Calculate(employee);
                Console.WriteLine($"{employee.Name} ({employee.Type}) => R$ {salary:F2}");
            }
        }
    }

    public class Employee(string name, string type, decimal baseSalary)
    {
        public string Name { get; } = name;
        public string Type { get; } = type;
        public decimal BaseSalary { get; } = baseSalary;
    }

    public class SalaryCalculator
    {
        public static decimal Calculate(Employee employee)
        {
            if (employee.Type == "FullTime")
            {
                return employee.BaseSalary + 2000m;
            }
            else if (employee.Type == "PartTime")
            {
                return employee.BaseSalary;
            }
            else if (employee.Type == "Intern")
            {
                return employee.BaseSalary - 500m;
            }
            else
            {
                throw new InvalidOperationException("Tipo de funcionário não suportado.");
            }
        }
    }
}