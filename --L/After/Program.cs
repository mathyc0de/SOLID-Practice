namespace L.Before
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var shapes = new List<Shape>
            {
                new Rectangle { Width = 10, Height = 5 },
                new Square { Side = 10 }
            };

            foreach (var shape in shapes)
            {
                Console.WriteLine($"Tipo: {shape.GetType().Name}");
                Console.WriteLine($"Área: {shape.GetArea()}");
            }
        }
    }

    public abstract class Shape
    {
            public abstract int GetArea();
    }

    public class Rectangle(int Width, int Height) : Shape
    {
        public int Width { public get; private set; } = Width;
        public int Height { public get; private set; } = Height;

        public override int GetArea()
        {
            return Width * Height;
        }
    }

    public class Square(int Side) : Shape
    {
        public int Side { public get; private set; } = Side;

        public override int GetArea()
        {
            return Side * Side;
        }
    }
}
