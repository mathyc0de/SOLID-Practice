# DIP (Dependency Inversion Principle)

## Visual Rápido
```text
Before
CheckoutService
└─ new EmailSender()
└─ new SmsSender()

After
CheckoutService(IEmailSender, ISmsSender)
└─ recebe implementações externas (injeção)
```

## Diff-Chave
```diff
- private readonly EmailSender _emailSender;
- private readonly SmsSender _smsSender;
- _emailSender = new EmailSender();
- _smsSender = new SmsSender();
+ private readonly IEmailSender _emailSender;
+ private readonly ISmsSender _smsSender;
```

## Onde a Violação Aparece
- `Before/Program.cs:28` alto nível depende de concretos.
- `Before/Program.cs:33` construção interna das dependências.
- `After/Program.cs:12` e `After/Program.cs:38` inversão via abstrações + injeção.
