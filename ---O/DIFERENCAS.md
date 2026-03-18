# OCP (Open/Closed Principle)

## Visual Rápido
```text
Before
SalaryCalculator
└─ if tipo == FullTime
└─ else if tipo == PartTime
└─ else if tipo == Intern

After
Employee (abstração)
├─ FullTimeEmployee.CalculateSalary()
├─ PartTimeEmployee.CalculateSalary()
└─ InternEmployee.CalculateSalary()
```

## Diff-Chave
```diff
- if (employee.Type == "FullTime") ...
- else if (employee.Type == "PartTime") ...
- else if (employee.Type == "Intern") ...
+ employee.CalculateSalary()
```

## Onde a Violação Aparece
- `Before/Program.cs:33` regra central por condicional.
- `After/Program.cs:21` polimorfismo para extensão sem editar cálculo central.
