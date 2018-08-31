using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using static Transition.App;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Transition.CircuitEditor
{
    public sealed partial class CircuitEditor : UserControl
    {
        public static CircuitEditor currentInstance;

        public ObservableCollection<IElectricElement> selectedElements;
        public List<Line> gridLines;

        public IElectricElement clickElement;

        public Wire manipulatingPnt1;
        public Wire manipulatingPnt2;

        public bool groupSelect = false;
        public Point pointStartGroupSelect;
        public Rectangle rectGroupSelect;

        public Point clickPoint;

        public Grid grdNoSelectedElement;

        public CircuitEditor()
        {
            this.InitializeComponent();
            selectedElements = new ObservableCollection<IElectricElement>();
            selectedElements.CollectionChanged += refreshSelectedElements;
            gridLines = new List<Line>();

            grdNoSelectedElement = new Grid();

            grdNoSelectedElement.Children.Add(new TextBlock()
            {
                Text = "No selected element...",
                FontStyle = Windows.UI.Text.FontStyle.Italic,
                Margin = new Thickness(4)
            });

            CircuitEditor.currentInstance = this;   //XAML constructed singleton?
        }

        private void clickDeleteComponent(object sender, RoutedEventArgs e)
        {
            bool deleted = false;
            foreach (IElectricElement element in selectedElements)
                if (cnvCircuit.Children.Contains((UIElement)element))
                {
                    cnvCircuit.Children.Remove((UIElement)element);
                    deleted = true;
                }

            if (deleted) selectedElements.Clear();
        }

        public void refreshSelectedElements(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (IElectricElement element in cnvCircuit.Children)
                element.deselected();

            foreach (IElectricElement element in selectedElements)
            {
                element.selected();
                if (element is ElectricComponent)
                {
                    scrComponentParameters.Content = ((ElectricComponent)element).parameters;
                    enableComponentEdit();
                }
            }

            if (selectedElements.Count == 0)
            {
                disableComponentEdit();
            }
        }

        private void enableComponentEdit()
        {
            btnCommonControlFlipX.IsEnabled = true;
            btnCommonControlFlipY.IsEnabled = true;
            btnCommonControlRotate.IsEnabled = true;
            btnDeleteComponent.IsEnabled = true;
        }

        private void disableComponentEdit()
        {
            btnCommonControlFlipX.IsEnabled = false;
            btnCommonControlFlipY.IsEnabled = false;
            btnCommonControlRotate.IsEnabled = false;
            btnDeleteComponent.IsEnabled = false;

            scrComponentParameters.Content = grdNoSelectedElement;
        }

        public void selectElement(IElectricElement element)
        {
            selectedElements.Clear();
            selectedElements.Add(element);

        }

        
        public void addElement(String element)
        {
            //tap event invoques an element on center of drawboard
            addElement(element, new Point(cnvCircuit.Width / 2, cnvCircuit.Height / 2));
        }

        public void addElement(String element, Point ptCanvas)
        {

            if (element == "wire")
            {
                Wire elto = new Wire(this);
                cnvCircuit.Children.Add(elto);
                Canvas.SetLeft(elto, 0);
                Canvas.SetTop(elto, 0);
                Point pt1 = new Point(Statics.round20(ptCanvas.X),
                                      Statics.round20(ptCanvas.Y));
                Point pt2 = new Point(pt1.X + 100, pt1.Y);
                elto.setPnt1(pt1);
                elto.setPnt2(pt2);

            }
            else
            {   //if it is not a wire, it is a component...
                ElectricComponent elto;
                elto = new ElectricComponent(element, this);

                cnvCircuit.Children.Add(elto);

                double x = Statics.round20(ptCanvas.X - (elto.Width / 2));
                double y = Statics.round20(ptCanvas.Y - (elto.Height / 2));

                Canvas.SetLeft(elto, x);
                Canvas.SetTop(elto, y);
            }
        }

        private void clickRotate(object sender, RoutedEventArgs e)
        {
            foreach (ElectricComponent comp in selectedElements)
                comp.Rotate();
        }

        private void clickFlipX(object sender, RoutedEventArgs e)
        {
            foreach (ElectricComponent comp in selectedElements)
                comp.FlipX();
        }

        private void clickFlipY(object sender, RoutedEventArgs e)
        {
            foreach (ElectricComponent comp in selectedElements)
                comp.FlipY();
        }

        private void tapCloseButton(object sender, TappedRoutedEventArgs e)
        {
            splitter.IsPaneOpen = false;
        }

        private async void drop(object sender, DragEventArgs e)
        {
            List<String> a = new List<String>();
            a.AddRange(e.DataView.AvailableFormats  );

            String element = await e.DataView.GetTextAsync();
            Point ptCanvas = e.GetPosition(cnvCircuit);

            addElement(element, ptCanvas);
        }

        private void dragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Move;
        }

        private void reDrawCanvas(object sender, SizeChangedEventArgs e)
        {

            double width = cnvGeneral.ActualWidth;
            double height = cnvGeneral.ActualHeight;

            int x = 0;
            int y = 0;

            foreach (Line l in gridLines)
                if (cnvGeneral.Children.Contains(l))
                    cnvGeneral.Children.Remove(l);

            gridLines.Clear();

            Line line;
            double strokeThickness = 1;
            Brush strokeColor = new SolidColorBrush(Colors.LightGray);

            while (x < width)
            {
                x += 20;
                line = new Line()
                {
                    X1 = x,
                    X2 = x,
                    Y1 = 0,
                    Y2 = height,
                    Stroke = strokeColor,
                    StrokeThickness = strokeThickness
                };
                gridLines.Add(line);
            }

            while (y < height)
            {
                y += 20;
                line = new Line()
                {
                    X1 = 0,
                    X2 = width,
                    Y1 = y,
                    Y2 = y,
                    Stroke = strokeColor,
                    StrokeThickness = strokeThickness
                };
                gridLines.Add(line);
            }

            foreach (Line l in gridLines)
                cnvGeneral.Children.Add(l);
        }

        private void cnvPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            clickPoint = e.GetCurrentPoint(cnvCircuit).Position;

            if ((clickElement == null) && (manipulatingPnt1 == null) &&
                (manipulatingPnt2 == null))
            {
                if (rectGroupSelect != null)
                    if (cnvGeneral.Children.Contains(rectGroupSelect))
                        cnvGeneral.Children.Remove(rectGroupSelect);

                groupSelect = true;
                pointStartGroupSelect = e.GetCurrentPoint(cnvCircuit).Position;
                selectedElements.Clear();
                // splitter.IsPaneOpen = false;

                rectGroupSelect = new Rectangle()
                {
                    StrokeThickness = 1,
                    Stroke = new SolidColorBrush(Colors.Black),
                    Width = 1,
                    Height = 1
                };

                rectGroupSelect.PointerReleased += cnvPointerReleased;
                cnvGeneral.Children.Add(rectGroupSelect);
            }

            if (clickElement != null)
            {
                if (selectedElements.Contains(clickElement))
                { }
                else
                {
                    selectElement(clickElement);
                }
            }
        }

        private void cnvPointerMoved(object sender, PointerRoutedEventArgs e)
        {

            if (e.GetCurrentPoint(cnvCircuit).Properties.IsLeftButtonPressed)
            {
                if (groupSelect)
                {
                    Point ptCanvas = e.GetCurrentPoint(cnvCircuit).Position;

                    double left;
                    double top;
                    double width;
                    double height;

                    left = (pointStartGroupSelect.X < ptCanvas.X) ? pointStartGroupSelect.X : ptCanvas.X;
                    top = (pointStartGroupSelect.Y < ptCanvas.Y) ? pointStartGroupSelect.Y : ptCanvas.Y;

                    width = ((pointStartGroupSelect.X - ptCanvas.X) > 0) ?
                             (pointStartGroupSelect.X - ptCanvas.X) :
                             (ptCanvas.X - pointStartGroupSelect.X);

                    height = ((pointStartGroupSelect.Y - ptCanvas.Y) > 0) ?
                             (pointStartGroupSelect.Y - ptCanvas.Y) :
                             (ptCanvas.Y - pointStartGroupSelect.Y);

                    Canvas.SetLeft(rectGroupSelect, left);
                    Canvas.SetTop(rectGroupSelect, top);
                    rectGroupSelect.Width = width;
                    rectGroupSelect.Height = height;

                    return;
                }

                if (manipulatingPnt1 != null)
                {
                    PointerPoint ptCanvas = e.GetCurrentPoint(cnvCircuit);

                    Point p1 = new Point(Statics.round20(ptCanvas.Position.X),
                                         Statics.round20(ptCanvas.Position.Y));

                    manipulatingPnt1.setPnt1(p1);
                    return;
                }

                if (manipulatingPnt2 != null)
                {
                    PointerPoint ptCanvas = e.GetCurrentPoint(cnvCircuit);

                    Point p2 = new Point(Statics.round20(ptCanvas.Position.X),
                                         Statics.round20(ptCanvas.Position.Y));

                    manipulatingPnt2.setPnt2(p2);
                    return;
                }

                if ((clickElement != null))
                {
                    Point ptCanvas = e.GetCurrentPoint(cnvCircuit).Position;
                    foreach (IElectricElement elto in selectedElements)
                    {
                        elto.moveRelative(new Point(clickPoint.X - ptCanvas.X,
                                                    clickPoint.Y - ptCanvas.Y));
                    }
                }
            }
        }

        private void cnvPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            clickElement = null;
            manipulatingPnt1 = null;
            manipulatingPnt2 = null;

            if (groupSelect)
            {
                if (rectGroupSelect != null) cnvGeneral.Children.Remove(rectGroupSelect);
                groupSelect = false;

                selectedElements.Clear();

                foreach (IElectricElement elto in cnvCircuit.Children)
                    if (elto.isInside(rectGroupSelect))
                        selectedElements.Add(elto);
            }

            foreach (IElectricElement elto in cnvCircuit.Children)
                elto.updateOriginPoint();
        }

        public void showParameters(ElectricComponent element)
        {
            //  scrComponentParameters.Content = element.parameters;
            splitter.IsPaneOpen = true;
            selectElement(element);
        }
        
        public int getMaximumNumberElement(String ElementLetter)
        {
            if (ElementLetter == null) return 0;
            if (ElementLetter == "") return 0;

            List<ElectricComponent> elements = new List<ElectricComponent>();

            foreach (Control cn in cnvCircuit.Children)
                if (cn is ElectricComponent)
                    elements.Add((ElectricComponent)cn);

            int maximum = 0;
            int result;

            foreach (ElectricComponent el in elements)
                if (el.elementName != null)
                    if (el.elementName.Substring(0, ElementLetter.Length) == ElementLetter)
                        if (int.TryParse(el.elementName.Substring(ElementLetter.Length, el.elementName.Length - ElementLetter.Length), out result))
                            if (result > maximum) maximum = result;

            return maximum;

        }

        public int getNextNumberLetter(String ElementLetter)
        {
            return getMaximumNumberElement(ElementLetter) + 1;
        }
    }
}
