using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GGimp
{
    public partial class MainWindow : Window
    {
        private EditModes SelectedEditMode = EditModes.Drag;
        private Shapes SelectedShape = Shapes.Line;

        private Point? ClickPoint;
        private Shape ShapeToEdit = null;
        private LineDimensions SelectedLineDim = null;
        private ShapeDimensions SelectedShapeDim = null;

        private bool IsProcessing = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ChangeTool(object sender, RoutedEventArgs e)
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

        public void DrawNewShape(string x, string y, string width, string height)
        {
            int parsedX, parsedY, parsedWidth, parsedHeight;
            int.TryParse(x, out parsedX);
            int.TryParse(y, out parsedY);
            int.TryParse(width, out parsedWidth);
            int.TryParse(height, out parsedHeight);
            DrawNewShape(parsedX, parsedY, parsedWidth, parsedHeight);
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
                    ShapeToEdit = toSave;
                }
            };
            ShapeToEdit = toSave;
            canvas.Children.Add(toSave);
        }

        private void MouseRelease(object sender, MouseButtonEventArgs e)
        {
            ClickPoint = null;
            ShapeToEdit = null;
            IsProcessing = false;
            SelectedLineDim = null;
        }

        private void MouseClick(object sender, MouseButtonEventArgs e)
        {
            IsProcessing = true;
            if (SelectedEditMode == EditModes.Draw)
            {
                ClickPoint = e.GetPosition(canvas);
                DrawNewShape((int)ClickPoint.Value.X, (int)ClickPoint.Value.Y);
            }
            else if (SelectedEditMode == EditModes.Resize && ShapeToEdit != null)
            {
                ClickPoint = new Point((int)Canvas.GetLeft(ShapeToEdit), (int)Canvas.GetTop(ShapeToEdit));
            }
            else if (SelectedEditMode == EditModes.Drag && ShapeToEdit != null)
            {
                ClickPoint = e.GetPosition(canvas);
                if (ShapeToEdit.GetType() == typeof(Line))
                {
                    SelectedLineDim = new LineDimensions((Line)ShapeToEdit);
                    return;
                }
                SelectedShapeDim = new ShapeDimensions(ShapeToEdit);

            }
        }

        private void MouseMoving(object sender, MouseEventArgs e)
        {
            if (ClickPoint != null && (SelectedEditMode == EditModes.Draw || SelectedEditMode == EditModes.Resize))
            {
                Point currentPosition = e.GetPosition(canvas);
                if (ShapeToEdit.GetType() == typeof(Line))
                {
                    Line toEdit = (Line)ShapeToEdit;
                    toEdit.X2 = currentPosition.X;
                    toEdit.Y2 = currentPosition.Y;
                    return;
                }
                if (currentPosition.X < ClickPoint.Value.X)
                { ShapeToEdit.SetValue(LeftProperty, (double)currentPosition.X); }
                if (currentPosition.Y < ClickPoint.Value.Y)
                { ShapeToEdit.SetValue(TopProperty, (double)currentPosition.Y); }

                ShapeToEdit.Height = Math.Abs((int)ClickPoint.Value.Y - (int)currentPosition.Y);
                ShapeToEdit.Width = Math.Abs((int)ClickPoint.Value.X - (int)currentPosition.X);
            }
            else if (ClickPoint != null && (SelectedEditMode == EditModes.Drag))
            {
                Point currentPosition = e.GetPosition(canvas);

                if (ShapeToEdit.GetType() == typeof(Line))
                {
                    Line toEdit = (Line)ShapeToEdit;
                    toEdit.X1 = Math.Abs(SelectedLineDim.X1 - ClickPoint.Value.X + currentPosition.X);
                    toEdit.Y1 = Math.Abs(SelectedLineDim.Y1 - ClickPoint.Value.Y + currentPosition.Y);
                    toEdit.X2 = Math.Abs(SelectedLineDim.X2 - ClickPoint.Value.X + currentPosition.X);
                    toEdit.Y2 = Math.Abs(SelectedLineDim.Y2 - ClickPoint.Value.Y + currentPosition.Y);
                    return;
                }
                ShapeToEdit.SetValue(LeftProperty, Math.Abs(SelectedShapeDim.X1 - ClickPoint.Value.X + currentPosition.X));
                ShapeToEdit.SetValue(TopProperty, Math.Abs(SelectedShapeDim.Y1 - ClickPoint.Value.Y + currentPosition.Y));

            }
        }

        private void TextEditShape(object sender, MouseButtonEventArgs e)
        {
            TextEditDialog dialog = new TextEditDialog(ShapeToEdit);
            dialog.Show();
        }

        private void TextAddShape(object sender, RoutedEventArgs e)
        {
            if (SelectedEditMode != EditModes.Draw)
            {
                MessageBox.Show("Musisz wybrać kształt który chcesz dodać!", "Błąd");
                return;
            }
            TextEditDialog dialog = new TextEditDialog(this);
            dialog.Show();
        }
    }
}
