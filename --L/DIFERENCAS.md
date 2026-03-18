# LSP (Liskov Substitution Principle)

## Visual Rápido
```text
Before
Rectangle <- Square
Cliente assume: Width e Height independentes
Square quebra essa suposição

After
Shape
├─ Rectangle.GetArea()
└─ Square.GetArea()
Cliente depende só de contrato estável
```

## Diff-Chave
```diff
- class Square : Rectangle { override Width/Height acoplando lados }
+ abstract class Shape { GetArea() }
+ class Rectangle : Shape { GetArea() }
+ class Square : Shape { GetArea() }
```

## Onde a Violação Aparece
- `Before/Program.cs:21` método cliente assume comportamento de `Rectangle`.
- `Before/Program.cs:44` `Square` altera semântica esperada.
- `After/Program.cs:21` contrato comum estável por `Shape`.
