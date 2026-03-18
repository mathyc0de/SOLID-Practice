# SRP (Single Responsibility Principle)

## Visual Rápido
```text
Before
└─ OrderService
   ├─ Valida pedido
   ├─ Calcula total/desconto
   ├─ Persiste
   ├─ Envia e-mail
   └─ Gera log

After
├─ OrderValidator
├─ OrderCalculator
├─ OrderRepository
├─ EmailNotifier
├─ Logger
└─ OrderService (orquestra)
```

## Diff-Chave
```diff
- class OrderService { validar + calcular + salvar + notificar + logar }
+ class OrderService { orquestrar fluxo }
+ class OrderValidator { validar }
+ class OrderCalculator { calcular }
+ class OrderRepository { salvar }
+ class EmailNotifier { notificar }
+ class Logger { logar }
```

## Onde a Violação Aparece
- `Before/Program.cs:34` concentra múltiplas responsabilidades.
- `After/Program.cs:34` até `After/Program.cs:104` separa responsabilidades por classe.
