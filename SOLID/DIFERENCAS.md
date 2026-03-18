# SOLID Completo (Before x After)

## Mapa Visual Geral
```text
Before
OrderService (classe central)
├─ valida
├─ calcula
├─ escolhe pagamento por if/else
├─ salva
├─ notifica
├─ loga
└─ acoplada a concretos

After
OrderFacade (entrada)
└─ OrderService (orquestra)
   ├─ IOrderValidator
   ├─ IPriceCalculator + IDiscountPolicy
   ├─ IPaymentGateway + IPaymentProcessor
   ├─ IRefundGateway + IRefundProcessor
   ├─ IOrderRepository
   ├─ INotifier
   └─ ILogger
```

## Matriz de Violações
| Princípio | Before (violação) | After (ajuste) |
|---|---|---|
| S | `OrderService` faz tudo (`Before/Program.cs:57`) | serviços separados e orquestração (`After/Program.cs:160`) |
| O | `if/else` por `PaymentType` (`Before/Program.cs:91`) | gateway + processadores por tipo (`After/Program.cs:259`) |
| L | `PixPayment` lança em `Refund` (`Before/Program.cs:152`) | contratos separados para pagar/estornar (`After/Program.cs:79`, `After/Program.cs:85`) |
| I | `IBackOfficeWorker` obriga métodos indevidos (`Before/Program.cs:172`) | interfaces pequenas (`After/Program.cs:390`) |
| D | `OrderService` instancia concretos (`Before/Program.cs:65`) | dependência por abstrações no construtor (`After/Program.cs:160`) |

## Diff-Chave Consolidado
```diff
- if (order.PaymentType == "CreditCard") ... else if ...
+ paymentGateway.Pay(order.PaymentKind, total)

- abstract class PaymentMethod { Pay(); Refund(); }
- class PixPayment : PaymentMethod { Refund() => throw ... }
+ interface IPaymentProcessor { Pay(); }
+ interface IRefundProcessor { Refund(); }
+ refundGateway.TryRefund(...)

- interface IBackOfficeWorker { ProcessOrder(); PrintInvoice(); SendPromotionalSms(); GenerateMonthlyReport(); }
+ interface IOrderProcessor { Process(); }
+ interface IInvoicePrinter { PrintInvoice(); }
+ interface IPromotionalMessenger { SendPromotionalSms(); }
+ interface IMonthlyReporter { GenerateMonthlyReport(); }
```
