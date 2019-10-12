using System.Windows.Controls;
using System.Windows.Shapes;

namespace GGimp
{
    public class ShapeDimensions
    {
        public int X1 { get; set; }
        public int Y1 { get; set; }

        public ShapeDimensions(Shape shape)
        {
            X1 = (int)Canvas.GetLeft(shape);
            Y1 = (int)Canvas.GetTop(shape);
        }
        public ShapeDimensions() { }
    }
}
