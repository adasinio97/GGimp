using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace GGimp
{
    public partial class TextEditDialog : Window
    {

        private MainWindow ParentWindow;
        private Shape SelectedShape;

        public TextEditDialog(MainWindow window)
        {
            InitializeComponent();
            ParentWindow = window;
        }
        public TextEditDialog(Shape shape)
        {
            InitializeComponent();
            SelectedShape = shape;
            if (shape.GetType() == typeof(Line))
            {
                Line line = (Line)shape;
                x1Value.Text = ((int)line.X1).ToString();
                y1Value.Text = ((int)line.Y1).ToString();
                x2Value.Text = ((int)line.X2).ToString();
                y2Value.Text = ((int)line.Y2).ToString();
                return;
            }
            else
            {
                x1Value.Text = ((int)Canvas.GetLeft(shape)).ToString();
                y1Value.Text = ((int)Canvas.GetTop(shape)).ToString();
                x2Value.Text = ((int)shape.Width).ToString();
                y2Value.Text = ((int)shape.Height).ToString();
                x2Label.Content = "Szerokość";
                y2Label.Content = "Wysokość";
            }
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            if (ParentWindow != null)
            {
                ParentWindow.DrawNewShape(x1Value.Text, y1Value.Text, x2Value.Text, y2Value.Text);
                this.Close();
            }
            else
            {
                int parsedX, parsedY, parsedWidth, parsedHeight;
                int.TryParse(x1Value.Text, out parsedX);
                int.TryParse(y1Value.Text, out parsedY);
                int.TryParse(x2Value.Text, out parsedWidth);
                int.TryParse(y2Value.Text, out parsedHeight);

                try
                {
                    if (SelectedShape.GetType() == typeof(Line))
                    {
                        Line line = (Line)SelectedShape;
                        line.X1 = parsedX;
                        line.Y1 = parsedY;
                        line.X2 = parsedWidth;
                        line.Y2 = parsedHeight;
                    }
                    else
                    {
                        SelectedShape.SetValue(LeftProperty, (double)parsedX);
                        SelectedShape.SetValue(TopProperty, (double)parsedY);
                        SelectedShape.Height = parsedHeight;
                        SelectedShape.Width = parsedWidth;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine(e);
                    MessageBox.Show("Wybrałeś nieprawidłowe wartości", "Błąd");
                    return;
                }
                this.Close();
            }
        }
    }
}
