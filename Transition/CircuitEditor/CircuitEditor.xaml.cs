using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Transition.CircuitEditor.OnScreenComponents;
using Transition.CircuitEditor.Serializable;
using Transition.Commands;
using Transition.Common;
using Transition.Design;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
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

        public ObservableCollection<ScreenElementBase> selectedElements;
        public List<Line> gridLines;

        private bool SnapToGrid { get {
                if (Window.Current.CoreWindow.GetKeyState(VirtualKey.Control).HasFlag(CoreVirtualKeyStates.Down))
                    return !currentDesign.SnapToGrid;
                else
                    return currentDesign.SnapToGrid; } }

        public bool groupSelect = false;
        private bool movingComponents = false;
        public Point pointStartGroupSelect;
        public Rectangle rectGroupSelect;

        public Point clickedPoint;

        public Grid grdNoSelectedElement;
        public Grid grdWiresHaveNoParameters;

        public ObservableStack<ICircuitCommand> UndoStack = new ObservableStack<ICircuitCommand>();
        public ObservableStack<ICircuitCommand> RedoStack = new ObservableStack<ICircuitCommand>();

        public CircuitEditor()
        {
            InitializeComponent();

            init();
            selectedElements = new ObservableCollection<ScreenElementBase>();
            selectedElements.CollectionChanged += refreshSelectedElements;

            gridLines = new List<Line>();

            CircuitEditor.currentInstance = this;   //XAML constructed singleton?

            lstStackUndo.ItemsSource = UndoStack;
            lstStackRedo.ItemsSource = RedoStack;
            
        }

        private void init()
        {
            grdNoSelectedElement = new Grid();
            grdNoSelectedElement.Children.Add(new TextBlock()
            {
                Text = "No selected element...",
                FontStyle = Windows.UI.Text.FontStyle.Italic,
                Margin = new Thickness(4)
            });

            grdWiresHaveNoParameters = new Grid();
            grdWiresHaveNoParameters.Children.Add(new TextBlock()
            {
                Text = "Wires have no parameters...",
                FontStyle = Windows.UI.Text.FontStyle.Italic,
                Margin = new Thickness(4)
            });
        }

        public void loadDesign(UserDesign design)
        {
            currentDesign = design;

            design.CanvasCircuit.PointerPressed += cnvPointerPressed;
            design.CanvasCircuit.PointerMoved += cnvPointerMoved;
            design.CanvasCircuit.PointerReleased += cnvPointerReleased;
            design.CanvasCircuit.Drop += drop;
            design.CanvasCircuit.DragOver += dragOver;

            cnvGeneral.Children.Add(design.CanvasCircuit);

        }

        public void updateDesign(object sender, NotifyCollectionChangedEventArgs e)
        {

        }

        private void clickDeleteComponent(object sender, RoutedEventArgs e)
        {
            foreach (ScreenComponentBase element in selectedElements.OfType<ScreenComponentBase>())
                executeCommand(new CommandRemoveComponent(element.SerializableComponent));

            foreach (WireTerminal wt in selectedElements.OfType<WireTerminal>())
                executeCommand(new CommandRemoveWire()
                     { Wire = wt.SerializableWire });
        }
        
        public void refreshSelectedElements(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (ScreenElementBase element in currentDesign.CanvasCircuit.Children.OfType<ScreenElementBase>())
                element.deselected();

            foreach (ScreenElementBase element in selectedElements)
            {
                element.selected();

                if (element is ScreenComponentBase)
                    scrComponentParameters.Content = ((ScreenComponentBase)element).SerializableComponent.ParametersControl;

                if (element is WireTerminal)
                    scrComponentParameters.Content = grdWiresHaveNoParameters;
            }

            if (selectedElements.Count == 0)
                disableComponentEdit(); else
                enableComponentEdit();
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

        public void selectElement(ScreenElementBase element)
        {
            selectedElements.Clear();
            selectedElements.Add(element);
        }

        public void addElement(string element)
        {
            //tap event invoques an element on center of drawboard
            addElement(element, new Point(
                snapCoordinate(cnvGeneral.Width / 2),
                snapCoordinate(cnvGeneral.Height / 2)));
        }

        public void addElement(string stringComponent, Point ptCanvas)
        {
            ICircuitCommand command;

            if (stringComponent != "wire")
            {
                var component = getElement(stringComponent);
                component.PositionX = snapCoordinate(ptCanvas.X);
                component.PositionY = snapCoordinate(ptCanvas.Y);

                component.ElementName = component.ElementLetter + currentDesign.getNextNumberLetter(component.ElementLetter).ToString();

                command = new CommandAddComponent() { Component = component };
            }
            else
            {
                var wire = new Wire()
                {
                    X0 = snapCoordinate(ptCanvas.X),
                    Y0 = snapCoordinate(ptCanvas.Y),
                    X1 = snapCoordinate(ptCanvas.X + 100),
                    Y1 = snapCoordinate(ptCanvas.Y),
                    ElementName = "W" + (currentDesign.getMaximumNumberWire() + 1).ToString()
                };

                command = new CommandAddWire() { Wire = wire };
            }

            executeCommand(command);
        }

        private void bindComponentTerminalPair(ElementTerminal et1, ElementTerminal et2)
        {
            Wire wire = currentDesign.bindTwoComponentsTerminals(
                ((ScreenComponentBase)et1.element).SerializableComponent, et1.terminal,
                ((ScreenComponentBase)et2.element).SerializableComponent, et2.terminal);

            if (wire == null) return;
        }

        private void clickRotate(object sender, RoutedEventArgs e)
        {
            foreach (ScreenComponentBase comp in selectedElements.OfType<ScreenComponentBase>())
                comp.SerializableComponent.rotate();
        }

        private void clickFlipX(object sender, RoutedEventArgs e)
        {
            foreach (ScreenComponentBase comp in selectedElements.OfType<ScreenComponentBase>())
                comp.SerializableComponent.doFlipX();
        }

        private void clickFlipY(object sender, RoutedEventArgs e)
        {
            foreach (ScreenComponentBase comp in selectedElements.OfType<ScreenComponentBase>())
                comp.SerializableComponent.doFlipY();
        }

        private void tapCloseButton(object sender, TappedRoutedEventArgs e)
        {
            splitter.IsPaneOpen = false;
        }

        private async void drop(object sender, DragEventArgs e)
        {
            string element = await e.DataView.GetTextAsync();
            Point ptCanvas = e.GetPosition(cnvGeneral);

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
            {
                cnvGeneral.Children.Add(l);
                Canvas.SetZIndex(l, -1);
            }
        }

        private double snapCoordinate(double coordinate)
        {
            return SnapToGrid ? Statics.round20(coordinate) : coordinate;
        }

        private ScreenElementBase getClickedElement(Point clickedPoint)
        {
            List<ScreenElementBase> clickedElements = new List<ScreenElementBase>();

            foreach (ScreenElementBase element in currentDesign.CanvasCircuit.Children.OfType<ScreenElementBase>())
                if (element.isClicked(clickedPoint.X, clickedPoint.Y))
                    clickedElements.Add(element);

            if (clickedElements.Count == 0) return null;

            bool isThereOneOrMoreWireTerminals = false;

            foreach (ScreenElementBase element in clickedElements)
                if (element is WireTerminal)
                    isThereOneOrMoreWireTerminals = true;

            if (isThereOneOrMoreWireTerminals)
            {
                /* if is there one or more wireterminals, we give them priority
                 but we cannot select a wire terminal that is already bounded
                 to other wire terminal, in a point where many wire terminals coexist
                 there is one free terminal, and the others are bounded to the free
                 We do select the free terminal, that one can be moved freely
                 and the others terminal will follow it.*/
                foreach (WireTerminal wt in clickedElements.OfType<WireTerminal>())
                { if (!wt.isBoundedToOtherWire) return wt; }
                /* if all wt's were bounded, we do select whatever else is present on the click point*/
                foreach (ScreenComponentBase cm in clickedElements.OfType<ScreenComponentBase>())
                { return cm; }
                /* if we reach here is because all click elements are bounded wt's, so we return null
                 but this is unlikely*/
                return null;
            }
            // if there are no wt's in the clicked point we select whatever is there.

            return clickedElements[0];
        }

        private void lowlightAllTerminalsAllElements()
        {
            foreach (ScreenElementBase el in currentDesign.CanvasCircuit.Children.OfType<ScreenElementBase>())
                el.lowlightAllTerminals();
        }

        private ElementTerminal getNearestElementTerminal(double pointX, double pointY)
        {
            byte terminalNumber = 0;
            ScreenElementBase nearestElement = null;
            double nearestDistance = double.MaxValue;

            foreach (ScreenElementBase el in currentDesign.CanvasCircuit.Children.OfType<ScreenElementBase>())
                for (byte x = 0; x < el.QuantityOfTerminals; x++)
                    if (el.isPointNearTerminal(x, pointX, pointY))
                        if (el.getDistance(x, pointX, pointY) < nearestDistance)
                        {
                            nearestDistance = el.getDistance(x, pointX, pointY);
                            terminalNumber = x;
                            nearestElement = el;
                        }

            if (nearestElement != null)
                return new ElementTerminal()
                { element = nearestElement,
                    terminal = terminalNumber };
            else
                return null;
        }

        private List<ElementTerminal> getListPairedComponentTerminals()
        {
            List<ElementTerminal> output = new List<ElementTerminal>();
            double distance;

            foreach (ScreenComponentBase comp in currentDesign.CanvasCircuit.Children.OfType<ScreenComponentBase>())
                for (byte i = 0; i < comp.QuantityOfTerminals; i++)
                    foreach (ScreenComponentBase comp2 in currentDesign.CanvasCircuit.Children.OfType<ScreenComponentBase>())
                        for (byte j = 0; j < comp2.QuantityOfTerminals; j++)
                            if (comp != comp2)
                            {
                                distance = comp.getDistance(i,
                                  comp2.getAbsoluteTerminalPosition(j).X,
                                  comp2.getAbsoluteTerminalPosition(j).Y);
                                if (distance < 15)
                                    output.Add(new ElementTerminal()
                                    { element = comp, terminal = i });
                            }

            return output;
        }

        private Dictionary<ElementTerminal, ElementTerminal> getPairedTerminalsForBinding()
        {
            Dictionary<ElementTerminal, ElementTerminal> output = new Dictionary<ElementTerminal, ElementTerminal>();
            double distance;

            foreach (ScreenComponentBase comp in currentDesign.CanvasCircuit.Children.OfType<ScreenComponentBase>())
                for (byte i = 0; i < comp.QuantityOfTerminals; i++)
                    foreach (ScreenComponentBase comp2 in currentDesign.CanvasCircuit.Children.OfType<ScreenComponentBase>())
                        for (byte j = 0; j < comp2.QuantityOfTerminals; j++)
                            if (comp != comp2)
                            {
                                distance = comp.getDistance(i,
                                  comp2.getAbsoluteTerminalPosition(j).X,
                                  comp2.getAbsoluteTerminalPosition(j).Y);

                                if (distance < 15)
                                {
                                    ElementTerminal elt1 = new ElementTerminal() { element = comp, terminal = i };
                                    ElementTerminal elt2 = new ElementTerminal() { element = comp2, terminal = j };
                                    bool existsAlready = false;
                                    foreach (KeyValuePair<ElementTerminal, ElementTerminal> kvp in output)
                                    {
                                        if (kvp.Key.Equals(elt1)) existsAlready = true;
                                        if (kvp.Value.Equals(elt1)) existsAlready = true;
                                        if (kvp.Key.Equals(elt2)) existsAlready = true;
                                        if (kvp.Value.Equals(elt2)) existsAlready = true;
                                    }
                                    if (!existsAlready) output.Add(elt1, elt2);
                                }
                            }

            return output;
        }

        private List<ElementTerminal> getAllFreeElementTerminals()
        {
            List<ElementTerminal> output = new List<ElementTerminal>();

            return output;
        }

        private ElementTerminal getNearestElementTerminalExcept(double pointX, double pointY, ScreenElementBase removedElement)
        {
            /* when user is dragging a wire terminal across the screen
             nearby components or wires terminals are highlighted,
             but we need the dragged terminal not to be highlighted.
             so the dragging wire terminal is the removed element */

            byte terminalNumber = 0;
            ScreenElementBase nearestElement = null;
            double nearestDistance = double.MaxValue;

            foreach (ScreenElementBase el in currentDesign.CanvasCircuit.Children.OfType<ScreenElementBase>())
                if (el != removedElement)
                    for (byte x = 0; x < el.QuantityOfTerminals; x++)
                        if (el.isPointNearTerminal(x, pointX, pointY))
                            if (!(el is WireTerminal))
                            {   /* nearby terminal is a component terminal*/
                                if (el.getDistance(x, pointX, pointY) < nearestDistance)
                                {
                                    nearestDistance = el.getDistance(x, pointX, pointY);
                                    terminalNumber = x;
                                    nearestElement = el;
                                }
                            }
                            else
                            {   /* nearby terminal is an unbounded wire terminal */
                                WireTerminal wt = (WireTerminal)el;
                                if (!wt.isBoundedToOtherWire)
                                    if (wt.getDistance(x, pointX, pointY) < nearestDistance)
                                    {
                                        nearestDistance = wt.getDistance(x, pointX, pointY);
                                        terminalNumber = wt.TerminalNumber;
                                        nearestElement = wt;
                                    }
                            }

            if (nearestElement != null)
                return new ElementTerminal()
                { element = nearestElement, terminal = terminalNumber };
            else
                return null;
        }


        private void cnvPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            clickedPoint = e.GetCurrentPoint(cnvGeneral).Position;

            ScreenElementBase clickedElement = getClickedElement(clickedPoint);

            if (e.GetCurrentPoint(cnvGeneral).Properties.IsLeftButtonPressed)
                if (clickedElement == null)
                {
                    if (rectGroupSelect != null)
                        if (cnvGeneral.Children.Contains(rectGroupSelect))
                            cnvGeneral.Children.Remove(rectGroupSelect);

                    groupSelect = true;
                    pointStartGroupSelect = e.GetCurrentPoint(cnvGeneral).Position;
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

                    if (!selectedElements.Contains(clickedElement))
                        selectElement(clickedElement);
                }

            if (e.GetCurrentPoint(cnvGeneral).Properties.IsRightButtonPressed)
                if (clickedElement != null)
                {
                    selectElement(clickedElement);
                    splitter.IsPaneOpen = true;
                }
        }

        private void cnvPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (e.GetCurrentPoint(cnvGeneral).Properties.IsLeftButtonPressed)
            {
                Point ptCanvas = e.GetCurrentPoint(cnvGeneral).Position;

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
                {
                    if (selectedElements.Count > 1)
                    {
                        foreach (ScreenElementBase element in selectedElements)
                            element.moveRelative(snapCoordinate(clickedPoint.X - ptCanvas.X),
                                                 snapCoordinate(clickedPoint.Y - ptCanvas.Y));
                    }

                    if (selectedElements.Count == 1)
                        if (selectedElements[0] is WireTerminal)
                        {
                            WireTerminal wt = (WireTerminal)selectedElements[0];
                            if (!wt.isBounded)
                            {
                                /* we are dragging around an unbounded wire terminal */
                                ElementTerminal nearest = getNearestElementTerminalExcept(ptCanvas.X, ptCanvas.Y, selectedElements[0]);
                                lowlightAllTerminalsAllElements();
                                if (nearest != null)
                                {
                                    nearest.element.highlightTerminal(nearest.terminal);
                                    wt.moveAbsolute(
                                        nearest.element.getAbsoluteTerminalPosition(nearest.terminal).X,
                                        nearest.element.getAbsoluteTerminalPosition(nearest.terminal).Y);

                                }
                                else
                                    wt.moveRelative(snapCoordinate(clickedPoint.X - ptCanvas.X),
                                                    snapCoordinate(clickedPoint.Y - ptCanvas.Y));
                            }/* else, the wt is bounded, and for now, it is not movable */
                        }
                        else
                        {   /* here, the one selected element is a component.
                            while dragging, if one its terminals is very close to 
                            some other terminal of other component,
                            the two terminals will be highlighted.
                            */
                            selectedElements[0].moveRelative(snapCoordinate(clickedPoint.X - ptCanvas.X),
                                                             snapCoordinate(clickedPoint.Y - ptCanvas.Y));

                            lowlightAllTerminalsAllElements();
                            foreach (ElementTerminal elt in getListPairedComponentTerminals())
                                ((ScreenComponentBase)elt.element).highlightTerminal(elt.terminal);

                        }
                }
            }
        }

        private void cnvPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (groupSelect)
            {
                if (rectGroupSelect != null) cnvGeneral.Children.Remove(rectGroupSelect);
                groupSelect = false;

                selectedElements.Clear();

                foreach (ScreenElementBase element in currentDesign.CanvasCircuit.Children.OfType<ScreenElementBase>())
                    if (element.isInside(rectGroupSelect))
                        selectedElements.Add(element);
            }

            if (movingComponents)
                foreach (ScreenElementBase el in selectedElements)
                    if ((el.originalPositionX != el.PositionX) ||
                        (el.originalPositionY != el.PositionY))
                    {
                        var command = new CommandMoveElement()
                        {
                            OldPositionX = el.originalPositionX,
                            OldPositionY = el.originalPositionY,
                            NewPositionX = el.PositionX,
                            NewPositionY = el.PositionY,
                            Element = el
                        };

                        executeCommand(command);
                    }
                
            if (movingComponents)
                if (selectedElements.Count == 1)
                    if (selectedElements[0] is WireTerminal)
                    {
                        WireTerminal wt = (WireTerminal)selectedElements[0];
                        if (!wt.isBounded)
                        {
                            Point ptCanvas = e.GetCurrentPoint(cnvGeneral).Position;
                            ElementTerminal nearest = getNearestElementTerminalExcept(ptCanvas.X, ptCanvas.Y, wt);

                            if (nearest != null) /* if there is some terminal nearby, we bind the wire to it*/
                                wt.wireScreen.wire.bind(nearest.element.Serializable, nearest.terminal, wt.TerminalNumber);
                        }
                    }
                    else
                    {
                        /* the dropped element is a component */
                        var pairs = getPairedTerminalsForBinding();
                        foreach (KeyValuePair<ElementTerminal, ElementTerminal> kvp in pairs)
                            bindComponentTerminalPair(kvp.Key, kvp.Value);

                    }

            movingComponents = false;

         /*   foreach (ScreenElementBase element in currentDesign.CanvasCircuit.Children.OfType<ScreenElementBase>())
                element.updateOriginalPosition(); */
        }

        public void showParameters(ScreenComponentBase component)
        {
            //  scrComponentParameters.Content = element.parameters;
            splitter.IsPaneOpen = true;
            selectElement(component);
        }


        public static SerializableComponent getElement(string element)
        {
            switch (element)
            {
                case "resistor":        return new Resistor();
                case "capacitor":       return new Capacitor();
                case "inductor":        return new Inductor();
                case "fdnr":            return new FDNR();
                case "ground":          return new Ground();
                case "potentiometer":   return new Potentiometer();
                case "transformer":     return new Transformer();
                case "generator":       return new VoltageSource();
            }

            throw new NotSupportedException();
        }


        private void dragElement(UIElement sender, DragStartingEventArgs args)
        {

            Border b = (Border)sender;
            String element = (String)b.Tag;

            args.DragUI.SetContentFromDataPackage();
            args.Data.RequestedOperation = DataPackageOperation.Move;
            args.Data.SetText(element);
        }

        private void tapElement(object sender, TappedRoutedEventArgs e)
        {

            Border bd = (Border)sender;
            String element = (String)bd.Tag;

            addElement(element);
        }

        public void executeCommand(ICircuitCommand command)
        {
            RedoStack.Clear();
            command.execute();
            UndoStack.Push(command);
        }

        public void undo()
        {
            if (UndoStack.Count == 0) return;

            ICircuitCommand command = UndoStack.Pop();
            command.unExecute();
            RedoStack.Push(command);
        }

        public void redo()
        {
            if (RedoStack.Count == 0) return;
            
            ICircuitCommand command = RedoStack.Pop();
            command.execute();
            UndoStack.Push(command);
        }

        private void tapUndo(object sender, TappedRoutedEventArgs e)
        {
            undo();
        }

        private void tapRedo(object sender, TappedRoutedEventArgs e)
        {
            redo();
        }
    }
}
