using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
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

    public partial class MainWindow : Window
    {

        private enum Shapes { Rectangle, Circle, Line };
        private enum EditModes { Resize, Draw, Drag };

        private EditModes SelectedEditMode = EditModes.Drag;
        private Shapes SelectedShape = Shapes.Line;

        private Point? FirstClicked;
        private Shape FirstSelectedShape = null;
        private LineDimensions FirstLineDimensions = null;
        private ShapeDimensions FirstShapeDimensions = null;

        private bool IsProcessing = false;

        public MainWindow()
        {
            InitializeComponent();
        }
        void ChangeTool(object sender, RoutedEventArgs e)
        {
            if (sender.Equals(line))
            {
                selectedToolLabel.Content = "Linia";
                SelectedShape = Shapes.Line;
                SelectedEditMode = EditModes.Draw;
            }
            else if (sender.Equals(square))
            {
                selectedToolLabel.Content = "Prostokąt";
                SelectedShape = Shapes.Rectangle;
                SelectedEditMode = EditModes.Draw;
            }
            else if (sender.Equals(elipsys))
            {
                selectedToolLabel.Content = "Koło";
                SelectedShape = Shapes.Circle;
                SelectedEditMode = EditModes.Draw;
            }
            else if (sender.Equals(edit))
            {
                selectedToolLabel.Content = "Edycja";
                SelectedEditMode = EditModes.Resize;
            }
            else if (sender.Equals(drag))
            {
                selectedToolLabel.Content = "Suwanie";
                SelectedEditMode = EditModes.Drag;
            }
        }

        private void DrawNewShape(int x, int y, int width = 0, int height = 0)
        {
            Shape toSave = null;
            try
            {
                switch (SelectedShape)
                {
                    case Shapes.Rectangle:
                        toSave = new Rectangle() { Fill = Brushes.Green };
                        toSave.MouseLeave += (s, e) => toSave.Fill = Brushes.Green;
                        toSave.SetValue(LeftProperty, (double)x);
                        toSave.SetValue(TopProperty, (double)y);
                        toSave.Height = height;
                        toSave.Width = width;
                        break;
                    case Shapes.Circle:
                        toSave = new Ellipse() { Fill = Brushes.Red };
                        toSave.MouseLeave += (s, e) => toSave.Fill = Brushes.Red;
                        toSave.SetValue(LeftProperty, (double)x);
                        toSave.SetValue(TopProperty, (double)y);
                        toSave.Height = height;
                        toSave.Width = width;
                        break;
                    case Shapes.Line:
                        toSave = new Line
                        {
                            Stroke = Brushes.Blue,
                            X1 = x,
                            X2 = width == 0 ? x : width,
                            Y1 = y,
                            Y2 = height == 0 ? y : height
                        };
                        toSave.MouseLeave += (s, e) => toSave.Fill = Brushes.Blue;
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                MessageBox.Show("Wybrałeś nieprawidłowe wartości", "Błąd");
                return;
            }

            toSave.MouseEnter += (s, e) =>
            {
                if (!IsProcessing)
                {
                    toSave.Fill = Brushes.Gray;
                    FirstSelectedShape = toSave;
                }
            };
            FirstSelectedShape = toSave;
            canvas.Children.Add(toSave);
        }

        private void MouseRelease(object sender, MouseButtonEventArgs e)
        {
            FirstClicked = null;
            FirstSelectedShape = null;
            IsProcessing = false;
            FirstLineDimensions = null;
        }

        private void MouseClick(object sender, MouseButtonEventArgs e)
        {
            IsProcessing = true;
            if (SelectedEditMode == EditModes.Draw)
            {
                FirstClicked = e.GetPosition(canvas);
                DrawNewShape((int)FirstClicked.Value.X, (int)FirstClicked.Value.Y);
            }
            else if (SelectedEditMode == EditModes.Resize && FirstSelectedShape != null)
            {
                FirstClicked = new Point((int)Canvas.GetLeft(FirstSelectedShape), (int)Canvas.GetTop(FirstSelectedShape));
            }
            else if (SelectedEditMode == EditModes.Drag && FirstSelectedShape != null)
            {
                FirstClicked = e.GetPosition(canvas);
                if (FirstSelectedShape.GetType() == typeof(Line))
                {
                    FirstLineDimensions = new LineDimensions((Line)FirstSelectedShape);
                    return;
                }
                FirstShapeDimensions = new ShapeDimensions(FirstSelectedShape);

            }
        }

        private void MouseMoving(object sender, MouseEventArgs e)
        {
            if (FirstClicked != null && (SelectedEditMode == EditModes.Draw || SelectedEditMode == EditModes.Resize))
            {
                Point currentPosition = e.GetPosition(canvas);
                if (FirstSelectedShape.GetType() == typeof(Line))
                {
                    Line toEdit = (Line)FirstSelectedShape;
                    toEdit.X2 = currentPosition.X;
                    toEdit.Y2 = currentPosition.Y;
                    return;
                }
                if (currentPosition.X < FirstClicked.Value.X)
                { FirstSelectedShape.SetValue(LeftProperty, (double)currentPosition.X); }
                if (currentPosition.Y < FirstClicked.Value.Y)
                { FirstSelectedShape.SetValue(TopProperty, (double)currentPosition.Y); }

                FirstSelectedShape.Height = Math.Abs((int)FirstClicked.Value.Y - (int)currentPosition.Y);
                FirstSelectedShape.Width = Math.Abs((int)FirstClicked.Value.X - (int)currentPosition.X);
            }
            else if (FirstClicked != null && (SelectedEditMode == EditModes.Drag))
            {
                Point currentPosition = e.GetPosition(canvas);

                if (FirstSelectedShape.GetType() == typeof(Line))
                {
                    Line toEdit = (Line)FirstSelectedShape;
                    toEdit.X1 = Math.Abs(FirstLineDimensions.X1 - FirstClicked.Value.X + currentPosition.X);
                    toEdit.Y1 = Math.Abs(FirstLineDimensions.Y1 - FirstClicked.Value.Y + currentPosition.Y);
                    toEdit.X2 = Math.Abs(FirstLineDimensions.X2 - FirstClicked.Value.X + currentPosition.X);
                    toEdit.Y2 = Math.Abs(FirstLineDimensions.Y2 - FirstClicked.Value.Y + currentPosition.Y);
                    return;
                }
                FirstSelectedShape.SetValue(LeftProperty, Math.Abs(FirstShapeDimensions.X1 - FirstClicked.Value.X + currentPosition.X));
                FirstSelectedShape.SetValue(TopProperty, Math.Abs(FirstShapeDimensions.Y1 - FirstClicked.Value.Y + currentPosition.Y));

            }
        }

        private void TextEditShape(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
