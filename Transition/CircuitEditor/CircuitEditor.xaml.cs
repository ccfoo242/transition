using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Transition.CircuitEditor.OnScreenComponents;
using Transition.CircuitEditor.Serializable;
using Transition.Design;
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

        public UserDesign currentDesign { get; set; }

        public ObservableCollection<ScreenComponentBase> selectedElements;
        public ObservableCollection<ScreenComponentBase> elements => currentDesign.visibleElements;
        public List<Line> gridLines;

        private bool SnapToGrid => currentDesign.SnapToGrid;

        public bool groupSelect = false;
        private bool movingComponents = false;
        public Point pointStartGroupSelect;
        public Rectangle rectGroupSelect;

        public Point clickedPoint;

        public Grid grdNoSelectedElement;

        public CircuitEditor()
        {
            InitializeComponent();
            selectedElements = new ObservableCollection<ScreenComponentBase>();
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

        public void loadDesign(UserDesign design)
        {
            cnvCircuit.Children.Clear();
            currentDesign = design;
            design.visibleElements.CollectionChanged += updateDesign;
        }

        public void updateDesign(object sender, NotifyCollectionChangedEventArgs e)
        {

        }

        public void addToCanvas(UIElement element)
        {
            cnvCircuit.Children.Add(element);
        }

        public bool isElementOnCanvas(UIElement element)
        {
            return cnvCircuit.Children.Contains(element);
        }

        private void clickDeleteComponent(object sender, RoutedEventArgs e)
        {
            foreach (ScreenComponentBase element in selectedElements)
                currentDesign.removeElement(element.SerializableComponent);
        }

        public void refreshSelectedElements(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (ScreenComponentBase element in currentDesign.visibleElements)
                element.deselected();

            foreach (ScreenComponentBase element in selectedElements)
            {
                element.selected();
                scrComponentParameters.Content = element.SerializableComponent.ParametersControl;
                enableComponentEdit();
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

        public void selectElement(ScreenComponentBase element)
        {
            selectedElements.Clear();
            selectedElements.Add(element);

        }
        
        public void addElement(string element)
        {
            //tap event invoques an element on center of drawboard
            addElement(element, new Point(
                snapCoordinate(cnvCircuit.Width / 2),
                snapCoordinate(cnvCircuit.Height / 2)));
        }

        public void addElement(string stringComponent, Point ptCanvas)
        {
            if (stringComponent != "wire")
            {
                SerializableComponent component = getElement(stringComponent);
                component.PositionX = snapCoordinate(ptCanvas.X);
                component.PositionY = snapCoordinate(ptCanvas.Y);

                component.ElementName = component.ElementLetter + getNextNumberLetter(component.ElementLetter).ToString();

                currentDesign.addComponent(component);

                addToCanvas(component.OnScreenComponent);
            }
            else
            {
                Wire wire = new Wire();
                wire.X1 = snapCoordinate(ptCanvas.X);
                wire.Y1 = snapCoordinate(ptCanvas.Y);
                wire.X2 = snapCoordinate(ptCanvas.X + 100);
                wire.Y2 = snapCoordinate(ptCanvas.Y);
                currentDesign.addWire(wire);
                addToCanvas(wire.OnScreenWire);
            }
        }

        private void clickRotate(object sender, RoutedEventArgs e)
        {
            foreach (ScreenComponentBase comp in selectedElements)
                comp.SerializableComponent.rotate();
        }

        private void clickFlipX(object sender, RoutedEventArgs e)
        {
            foreach (ScreenComponentBase comp in selectedElements)
                comp.SerializableComponent.doFlipX();
        }

        private void clickFlipY(object sender, RoutedEventArgs e)
        {
            foreach (ScreenComponentBase comp in selectedElements)
                comp.SerializableComponent.doFlipY();
        }

        private void tapCloseButton(object sender, TappedRoutedEventArgs e)
        {
            splitter.IsPaneOpen = false;
        }

        private async void drop(object sender, DragEventArgs e)
        {
      
            string element = await e.DataView.GetTextAsync();
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

        private double snapCoordinate(double coordinate)
        {
            return SnapToGrid ? Statics.round20(coordinate) : coordinate;
        }
        
        private void cnvPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            clickedPoint = e.GetCurrentPoint(cnvCircuit).Position;

            ScreenComponentBase clickedElement = null;

            foreach (ScreenComponentBase element in elements)
                if (element.isInside(clickedPoint.X, clickedPoint.Y))
                    clickedElement = element;

            if (e.GetCurrentPoint(cnvCircuit).Properties.IsLeftButtonPressed)
                if (clickedElement == null)
                {
                    if (rectGroupSelect != null)
                        if (cnvGeneral.Children.Contains(rectGroupSelect))
                            cnvGeneral.Children.Remove(rectGroupSelect);

                    groupSelect = true;
                    pointStartGroupSelect = e.GetCurrentPoint(cnvCircuit).Position;
                    selectedElements.Clear();

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
                else
                {
                    movingComponents = true;

                    if (selectedElements.Contains(clickedElement))
                    { }
                    else
                    {
                        selectElement(clickedElement);
                    }
                }

            if (e.GetCurrentPoint(cnvCircuit).Properties.IsRightButtonPressed)
            {
                if (clickedElement != null)
                {
                    selectElement(clickedElement);
                    splitter.IsPaneOpen = true;
                }
            }
        }

        private void cnvPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (e.GetCurrentPoint(cnvCircuit).Properties.IsLeftButtonPressed)
            {
                Point ptCanvas = e.GetCurrentPoint(cnvCircuit).Position;

                if (groupSelect)
                {
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
                else
                
                   if (movingComponents)
                        foreach (ScreenComponentBase element in selectedElements)
                            element.moveRelative(snapCoordinate(clickedPoint.X) - snapCoordinate(ptCanvas.X),
                                                 snapCoordinate(clickedPoint.Y) - snapCoordinate(ptCanvas.Y));
            }
        }

        private void cnvPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            
            if (groupSelect)
            {
                if (rectGroupSelect != null) cnvGeneral.Children.Remove(rectGroupSelect);
                groupSelect = false;

                selectedElements.Clear();

                foreach (ScreenComponentBase element in currentDesign.visibleElements)
                    if (element.isInside(rectGroupSelect))
                        selectedElements.Add(element);
            }
            movingComponents = false;
            foreach (ScreenComponentBase element in elements)
                element.updateOriginPoint();
        }

        public void showParameters(ScreenComponentBase component)
        {
            //  scrComponentParameters.Content = element.parameters;
            splitter.IsPaneOpen = true;
            selectElement(component);
        }

        public int getMaximumNumberElement(string ElementLetter)
        {
            if (ElementLetter == null) return 0;
            if (ElementLetter == "") return 0;
            
            int maximum = 0;
            int result;

            foreach (SerializableElement element in currentDesign.components)
                if (element.ElementName != null)
                    if (element.ElementName.Substring(0, ElementLetter.Length) == ElementLetter)
                        if (int.TryParse(element.ElementName.Substring(ElementLetter.Length, element.ElementName.Length - ElementLetter.Length), out result))
                            if (result > maximum) maximum = result;

            return maximum;

        }

        public int getNextNumberLetter(String ElementLetter)
        {
            return getMaximumNumberElement(ElementLetter) + 1;
        }

        public static SerializableComponent getElement(string element)
        {
            switch (element)
            {
                case "resistor":       return new Resistor();
                case "capacitor":      return new Capacitor();
                case "inductor":       return new Inductor();
                case "fdnr":           return new FDNR();
                case "ground":         return new Ground();
                case "potentiometer":  return new Potentiometer();
                case "transformer":    return new Transformer();
                case "generator":      return new VoltageSource();
            }

            throw new NotSupportedException();
        }
    }
}
