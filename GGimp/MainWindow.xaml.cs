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
    public partial class MainWindow : Window
    {

        private enum Shapes { Rectangle, Circle, Line };
        private enum EditModes { Resize, Draw, Drag };

        private EditModes SelectedEditMode = EditModes.Drag;
        private Shapes SelectedShape = Shapes.Line;

        private List<Shape> DrawedShapes { get; set; } = new List<Shape>();

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
                        toSave.SetValue(LeftProperty, (double)x);
                        toSave.SetValue(TopProperty, (double)y);
                        toSave.Height = height;
                        toSave.Width = width;
                        break;
                    case Shapes.Circle:
                        toSave = new Ellipse() { Fill = Brushes.Red };
                        toSave.SetValue(LeftProperty, (double)x);
                        toSave.SetValue(TopProperty, (double)y);
                        toSave.Height = height + 50;
                        toSave.Width = width + 50;
                        break;
                    case Shapes.Line:
                        toSave = new Line
                        {
                            Stroke = Brushes.Blue,
                            X1 = x,
                            X2 = width,
                            Y1 = y,
                            Y2 = height
                        };
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                MessageBox.Show("Wybrałeś nieprawidłowe wartości", "Błąd");
                return;
            }
            DrawedShapes.Add(toSave);
            canvas.Children.Add(toSave);
        }

        private void MouseRelease(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(canvas);
            DrawNewShape((int)point.X, (int)point.Y);
        }

    }

}
