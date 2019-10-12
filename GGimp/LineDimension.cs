using System.Windows.Shapes;

namespace GGimp
{
    public class LineDimensions : ShapeDimensions
    {
        public int X2 { get; set; }
        public int Y2 { get; set; }

        public LineDimensions(Line line) : base()
        {

            X1 = (int)line.X1;
            Y1 = (int)line.Y1;
            X2 = (int)line.X2;
            Y2 = (int)line.Y2;
        }
    }
}
