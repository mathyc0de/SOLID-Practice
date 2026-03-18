# ISP (Interface Segregation Principle)

## Visual Rápido
```text
Before
IWorker
├─ Work()
├─ Eat()
└─ Sleep()
RobotWorker -> precisa lançar NotImplementedException

After
IWorker -> Work()
IHumanNeeds -> Eat(), Sleep()
RobotWorker implementa só IWorker
```

## Diff-Chave
```diff
- interface IWorker { Work(); Eat(); Sleep(); }
+ interface IWorker { Work(); }
+ interface IHumanNeeds { Eat(); Sleep(); }
```

## Onde a Violação Aparece
- `Before/Program.cs:40` interface gorda.
- `Before/Program.cs:72` e `Before/Program.cs:77` métodos inválidos para robô.
- `After/Program.cs:26` e `After/Program.cs:31` interfaces segregadas.
