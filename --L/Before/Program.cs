namespace L.Before
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var shapes = new List<Rectangle>
            {
                new(),
                new Square()
            };

            foreach (var shape in shapes)
            {
                Console.WriteLine($"Tipo: {shape.GetType().Name}");
                ResizeAndPrintArea(shape);
                Console.WriteLine();
            }
        }

        private static void ResizeAndPrintArea(Rectangle rectangle)
        {
            rectangle.Width = 5;
            rectangle.Height = 10;

            Console.WriteLine($"Largura: {rectangle.Width}");
            Console.WriteLine($"Altura: {rectangle.Height}");
            Console.WriteLine($"Área esperada: 50");
            Console.WriteLine($"Área calculada: {rectangle.GetArea()}");
        }
    }

    public class Rectangle
    {
        public virtual int Width { get; set; }
        public virtual int Height { get; set; }

        public int GetArea()
        {
            return Width * Height;
        }
    }

    public class Square : Rectangle
    {
        public override int Width
        {
            get => base.Width;
            set
            {
                base.Width = value;
                base.Height = value;
            }
        }

        public override int Height
        {
            get => base.Height;
            set
            {
                base.Width = value;
                base.Height = value;
            }
        }
    }
}
