using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor;
using Transition.CircuitEditor.OnScreenComponents;
using Transition.CircuitEditor.Serializable;
using Transition.Common;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Transition.Design
{
    public class UserDesign
    {
        /* the UserDesign class stores all circuit information, components, 
         * component parameters, wires and the bounds between them. Also it handles
         * the Canvas where the Design is drawn, and has the responsability for drawing 
         * the circuit on this canvas. This allows the UserDesign class to show a circuit 
         * on screen. This is useful for showing circuit examples on different UI dialogs
         * (and not only to draw the circuit that is being edited by the user in CircuitEditor)
         * On the other hand the CircuitEditor has all operations for manipulating
         * the design, to add components, wires, move and link them.
         * That means there is a strong coupling between UserDesign and CircuitEditor */

        public ObservableCollection<SerializableComponent> Components { get; } = new ObservableCollection<SerializableComponent>();
        public ObservableCollection<SerializableWire> Wires { get; } = new ObservableCollection<SerializableWire>();

        public ObservableCollection<ScreenComponentBase> ScreenComponents { get; } = new ObservableCollection<ScreenComponentBase>();
        public ObservableCollection<WireScreen> ScreenWires { get; } = new ObservableCollection<WireScreen>();

        public delegate void ElementDelegate(UserDesign sender, SerializableElement element);
        public event ElementDelegate ElementAdded;
        public event ElementDelegate ElementRemoved;

        public Color Background { get; set; } = Color.FromArgb(255, 255, 255, 255);

        private decimal minimumFrequency = 10;
        public decimal MinimumFrequency
        {
            get => minimumFrequency;
            set
            {
                if (value > MaximumFrequency)
                    throw new ArgumentException();
                minimumFrequency = value;
            }
        }


        private decimal maximumFrequency = 40000;
        public decimal MaximumFrequency
        {
            get => maximumFrequency;
            set
            {
                if (value < MinimumFrequency)
                    throw new ArgumentException();
                maximumFrequency = value;
            }
        }

        public int NumberOfFrequencyPoints { get; set; } = 400;

        public enum AxisScale { Logarithmic, Linear };
        public AxisScale FrequencyScale { get; set; } = AxisScale.Logarithmic;


        public double RadiusNear => 15;

        public Canvas CanvasCircuit { get; } = new Canvas()
        {
            Background = new SolidColorBrush(Colors.Transparent),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Width = 1080,
            Height = 720,
            AllowDrop = true
        };

        public bool SnapToGrid { get; set; } = true;

        public UserDesign()
        {
            Components.CollectionChanged += Components_CollectionChanged;
            Wires.CollectionChanged += Wires_CollectionChanged;
        }

        private void Components_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (SerializableComponent component in e.NewItems)
                    {
                        CanvasCircuit.Children.Add(component.OnScreenElement);
                        ScreenComponents.Add((ScreenComponentBase)component.OnScreenElement);
                        ElementAdded?.Invoke(this, component);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (SerializableComponent component in e.OldItems)
                    {
                        component.deletedElement();
                        CanvasCircuit.Children.Remove(component.OnScreenElement);
                        ScreenComponents.Remove((ScreenComponentBase)component.OnScreenElement);
                        ElementRemoved?.Invoke(this, component);
                    }
                    break;
            }

        }

        private void Wires_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (SerializableWire wire in e.NewItems)
                    {
                        CanvasCircuit.Children.Add(wire.OnScreenWire);
                        ScreenWires.Add(wire.OnScreenWire);
                        ElementAdded?.Invoke(this, wire);
                    };
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (SerializableWire wire in e.OldItems)
                    {
                        wire.deletedElement();
                        CanvasCircuit.Children.Remove(wire.OnScreenWire);
                        ScreenWires.Remove(wire.OnScreenWire);
                        ElementRemoved?.Invoke(this, wire);
                    }
                    break;
            }
        }


        public void removeElement(SerializableElement element)
        {
            if (element is SerializableComponent) Components.Remove((SerializableComponent)element);
            if (element is SerializableWire) Wires.Remove((SerializableWire)element);
        }

        public List<ICircuitSelectable> getAllSelectable()
        {
            var output = new List<ICircuitSelectable>();

            output.AddRange(ScreenComponents);
            output.AddRange(getAllUnboundedWireTerminals());

            return output;
        }

        public List<ScreenElementBase> getAllScreenElements()
        {
            var output = new List<ScreenElementBase>();
            output.AddRange(ScreenWires);
            output.AddRange(ScreenComponents);

            return output;
        }

        public List<SerializableWire> getBoundedWires(SerializableElement el, byte terminal)
        {
            var output = new List<SerializableWire>();

            foreach (SerializableWire wire in Wires)
                if (wire.isThisWireBoundedTo(el, terminal))
                    output.Add(wire);
            
            return output;
        }


        public SerializableWire bindComponentTerminal(SerializableComponent comp1, byte terminal1, SerializableElement el2, byte terminal2)
        {
            if (areTwoElementsTerminalsBounded(comp1, terminal1, el2, terminal2))
                return null;

            if ((comp1 == el2) && (terminal1 == terminal2)) return null;

            SerializableWire wire = new SerializableWire()
            {
                PositionTerminal0 = new Point2D(40, 40),
                PositionTerminal1 = new Point2D(140, 40)
            };

            wire.Bind0 = new Tuple<SerializableElement, byte>(comp1, terminal1);
            wire.Bind1 = new Tuple<SerializableElement, byte>(el2, terminal2);

            wire.ElementName = "W" + (getMaximumNumberWire() + 1).ToString();

            return wire;
        }

        public bool areTwoElementsTerminalsBounded(SerializableElement el1, byte terminal1, SerializableElement el2, byte terminal2)
        {
            foreach (SerializableWire wire in Wires)
            {
                if (wire.isThisWireBoundedTo(el1, terminal1) &&
                    wire.isThisWireBoundedTo(el2, terminal2))
                    return true;
            }
            return false;
        }


        public ICircuitSelectable getClickedElement(Point2D clickedPoint)
        {
            foreach (WireTerminal wt in getAllUnboundedWireTerminals())
                if (wt.isClicked(clickedPoint)) return wt as ICircuitSelectable;

            foreach (WireTerminal wt in getAllBoundedWireTerminals())
                if (wt.isClicked(clickedPoint)) return wt as ICircuitSelectable;

            foreach (ScreenComponentBase comp in ScreenComponents)
                if (comp.isClicked(clickedPoint)) return comp as ICircuitSelectable;

            foreach (WireScreen wire in ScreenWires)
                if (wire.isClicked(clickedPoint)) return wire as ICircuitSelectable;

            return null;
        }


        public int getMaximumNumberElement(string ElementLetter)
        {
            if (ElementLetter == null) return 0;
            if (ElementLetter == "") return 0;

            int maximum = 0;
            int result;

            foreach (SerializableElement element in Components)
                if (element.ElementName != null)
                    if (element.ElementName.Substring(0, ElementLetter.Length) == ElementLetter)
                        if (int.TryParse(element.ElementName.Substring(ElementLetter.Length, element.ElementName.Length - ElementLetter.Length), out result))
                            if (result > maximum) maximum = result;

            return maximum;
        }

        public int getMaximumNumberWire()
        {
            int result;
            int maximum = 0;

            foreach (SerializableWire wire in Wires)
                if (wire.ElementName != null)
                    if (wire.ElementName.Substring(0, 1) == "W")
                        if (int.TryParse(wire.ElementName.Substring(1, wire.ElementName.Length - 1), out result))
                            if (result > maximum) maximum = result;

            return maximum;
        }

        public int getNextNumberLetter(string ElementLetter)
        {
            return getMaximumNumberElement(ElementLetter) + 1;
        }

        public List<ElementTerminal> getAllTerminals()
        {
            var output = new List<ElementTerminal>();
            foreach (WireScreen wire in ScreenWires)
            {
                output.Add(wire.Terminals[0]);
                output.Add(wire.Terminals[1]);
            }

            foreach (ScreenComponentBase comp in ScreenComponents)
                foreach (ElementTerminal t in comp.Terminals)
                    output.Add(t);

            return output;
        }

        public List<ElementTerminal> getAllComponentTerminals()
        {
            var output = new List<ElementTerminal>();

            foreach (ScreenComponentBase comp in ScreenComponents)
                foreach (ElementTerminal t in comp.Terminals)
                    output.Add(t);

            return output;
        }

        public List<WireTerminal> getAllWireTerminals()
        {
            var output = new List<WireTerminal>();
            foreach (WireScreen wire in ScreenWires)
            {
                output.Add(wire.Terminals[0] as WireTerminal);
                output.Add(wire.Terminals[1] as WireTerminal);
            }

            return output;
        }

        public List<WireTerminal> getAllUnboundedWireTerminals()
        {
            var output = new List<WireTerminal>();
            foreach (WireTerminal wt in getAllWireTerminals())
                if (!wt.isBounded) output.Add(wt);
            return output;
        }

        public List<ElementTerminal> getAllUnboundedWireAndComponentTerminals()
        {
            var output = new List<ElementTerminal>();

            foreach (WireTerminal wt in getAllWireTerminals())
                if (!wt.isBounded) output.Add(wt);

            foreach (ScreenComponentBase comp in ScreenComponents)
                foreach (ElementTerminal t in comp.Terminals)
                    output.Add(t);

            return output;
        }

        public List<WireTerminal> getAllBoundedWireTerminals()
        {
            var output = new List<WireTerminal>();
            foreach (WireTerminal wt in getAllWireTerminals())
                if (wt.isBounded) output.Add(wt);
            return output;
        }

        public List<ICircuitSelectable> enclosingElementsForGroupSelect(Rectangle rect)
        {
            var output = new List<ICircuitSelectable>();

            foreach (ICircuitSelectable element in ScreenComponents.OfType<ICircuitSelectable>())
                if (element.isInside(rect))
                    output.Add(element);

            foreach (ICircuitSelectable element in getAllUnboundedWireTerminals().OfType<ICircuitSelectable>())
                if (element.isInside(rect))
                    output.Add(element);

            return output;
        }

        public ElementTerminal getNearestElementTerminalExcept(Point2D point, ScreenElementBase removedElement)
        {
            /* when user is dragging a wire terminal across the screen
               nearby components or wires terminals are highlighted,
               but we need the dragged terminal not to be highlighted.
               so the dragging wire terminal is the removed element */

            ElementTerminal nearestTerminal = null;
            double nearestDistance = double.MaxValue;

            foreach (var el in getAllScreenElements())
                if (el != removedElement)
                    for (byte i = 0; i < el.QuantityOfTerminals; i++)
                        if ((el.getAbsoluteTerminalPosition(i).getDistance(point) < nearestDistance) &&
                            el.getAbsoluteTerminalPosition(i).getDistance(point) < RadiusNear)
                        { if (el is WireScreen)
                            { WireScreen ws = (WireScreen)el;
                                if (!ws.isTerminalBounded(i))
                                {
                                    nearestTerminal = el.Terminals[i];
                                    nearestDistance = el.getAbsoluteTerminalPosition(i).getDistance(point);
                                }
                            }
                            else
                            {
                                nearestTerminal = el.Terminals[i];
                                nearestDistance = el.getAbsoluteTerminalPosition(i).getDistance(point);
                            }

                        }


            return nearestTerminal;
        }

        public List<Tuple<ElementTerminal, ElementTerminal>> getListPairedComponentTerminals()
        {
            var output = new List<Tuple<ElementTerminal, ElementTerminal>>();

            var alreadyAddedTerminals = new List<ElementTerminal>();
            var allTerminals = getAllUnboundedWireAndComponentTerminals();

            foreach (var t1 in allTerminals)
                foreach (var t2 in allTerminals)
                    if (t1.ScreenElement != t2.ScreenElement)
                        if (t1.TerminalPosition.getDistance(t2.TerminalPosition) < RadiusNear)
                            if (!alreadyAddedTerminals.Contains(t1) &&
                                !alreadyAddedTerminals.Contains(t2))
                            {
                                output.Add(new Tuple<ElementTerminal, ElementTerminal>(t1, t2));
                                alreadyAddedTerminals.Add(t1);
                                alreadyAddedTerminals.Add(t2);
                            }

            return output;
        }

        public Dictionary<byte, ElementTerminal> getListPairedComponentTerminals(ScreenComponentBase component)
        {
            var output = new Dictionary<byte, ElementTerminal>();

            var allTerminals = getAllUnboundedWireAndComponentTerminals();

            for (byte i = 0; i < component.QuantityOfTerminals; i++)
                foreach (var t1 in allTerminals)
                    if (t1.ScreenElement != component)
                        if (t1.getAbsoluteTerminalPosition().getDistance(component.getAbsoluteTerminalPosition(i)) < RadiusNear)
                        { if (!areTwoElementsTerminalsBounded(component.SerializableComponent, i, t1.ScreenElement.Serializable, t1.TerminalNumber))
                                output.Add(i, t1); }

            return output;
        }

        public void lowlightAllTerminalsAllElements()
        {
            foreach (ElementTerminal t in getAllTerminals())
                t.lowlight();
        }

        public IList<SerializableWire> GetAllIndependentWires
        {
            get
            {
                var output = new List<SerializableWire>();
                foreach (var wire in Wires)
                    if (wire.IsIndependent) output.Add(wire);

                return output;
            }
        }

        public void Calculate()
        {
          //  var WiresNodes = new Dictionary<SerializableWire, int>();

            var nodes = GetNodes();

            var componentsTerminals = new Dictionary<int, List<Tuple<SerializableComponent, byte>>>();
            
            foreach (var kvp in nodes)
            {
                componentsTerminals[kvp.Key] = new List<Tuple<SerializableComponent, byte>>();

                foreach (var wire in kvp.Value)
                    foreach (var tuple in wire.GetBoundedComponents)
                        if (!componentsTerminals[kvp.Key].Contains(tuple))
                            componentsTerminals[kvp.Key].Add(tuple);
            }

            int QuantityOfNodes = nodes.Count - 1;

            //node 0 is always the ground

            var freqPoints = getFrequencyPoints();
            

            foreach (var FreqPoint in freqPoints)
            {
                for (int node = 1; node <= QuantityOfNodes; node++)
                {

                }
            }

            var A = new Common.Matrix(4);

            A.Data[0, 0] = 6;  A.Data[0, 1] = -2;  A.Data[0, 2] = 2; A.Data[0, 3] = 4;
            A.Data[1, 0] = 12; A.Data[1, 1] = -8;  A.Data[1, 2] = 6; A.Data[1, 3] = 10;
            A.Data[2, 0] = 3;  A.Data[2, 1] = -13; A.Data[2, 2] = 9; A.Data[2, 3] = 3;
            A.Data[3, 0] = -6; A.Data[3, 1] = 4;   A.Data[3, 2] = 1; A.Data[3, 3] = -18;

            var B = new Common.Matrix(4, 1);

            B.Data[0, 0] = 16;
            B.Data[1, 0] = 26;
            B.Data[2, 0] = -19;
            B.Data[3, 0] = -34;


            var X = A.Solve(B);
        }


        public Dictionary<int, List<SerializableWire>> GetNodes()
        {
            var output = new Dictionary<int, List<SerializableWire>>();
            int currentNode = 1;
            int x;

            output.Add(0, GetGroundedWires());

            Func<SerializableWire, Dictionary<int, List<SerializableWire>>, int> wireBelongsTo =
                (wire, dictOutput) => 
                {
                    foreach (var kvp in dictOutput)
                        foreach (var existingWire in kvp.Value)
                            if (wire.isThisWireBoundedToOtherWire(existingWire)) return kvp.Key;
                
                    return -1;
                };

            foreach (var wire in GetNonGroundedWires())
            {
                x = wireBelongsTo(wire, output);
                if (x == -1)
                {
                    output.Add(currentNode, new List<SerializableWire>());
                    output[currentNode].Add(wire);
                    currentNode++;
                }
                else
                    output[x].Add(wire);
            }

            return output;
        }

        public List<SerializableWire> GetNonGroundedWires()
        {
            var output = new List<SerializableWire>();

            var grounded = GetGroundedWires();

            foreach (var wire in Wires)
                if (!grounded.Contains(wire))
                    output.Add(wire);

            return output;
        }

        public List<SerializableWire> GetGroundedWires()
        {
            var output = new List<SerializableWire>();
            
            foreach (var wire in Wires)
                if (wire.IsWireGrounded)
                    output.Add(wire);

            Func<SerializableWire, List<SerializableWire>, bool> isConnected =
                (wir, lis) =>
                {
                    foreach (var wire in lis)
                    { if (wir.isThisWireBoundedToOtherWire(wire)) return true; }
                    return false;
                };

            bool wireAdded = true;

            while (wireAdded)
            {
                wireAdded = false;

                foreach (var wire in Wires)
                    if (!output.Contains(wire))
                        if (isConnected(wire, output))
                        {
                            output.Add(wire);
                            wireAdded = true;
                        }
            }

            return output;
        }

        private bool areTwoWiresConnected(SerializableWire w1, SerializableWire w2)
        {
            if (w1 == null || w2 == null) throw new NullReferenceException();
            var groupW1 = WireGroup(w1);

            return groupW1.Contains(w2);
        }

        private List<SerializableWire> WireGroup(SerializableWire w1)
        {
            var ConnectedWires = new List<SerializableWire>();
           
            ConnectedWires.Add(w1);
            bool added = true;

            Func<SerializableWire, List<SerializableWire>, bool> isConnected =
                (wir, lis) =>
                {foreach (var wire in lis)
                    { if (wir.isThisWireBoundedToOtherWire(wire)) return true; }
                    return false;};

            while (added)
            {
                added = false;
                foreach (var wire in Wires)
                    if (isConnected(wire, ConnectedWires))
                    {
                        added = true;
                        if (!ConnectedWires.Contains(wire))
                            ConnectedWires.Add(wire);
                        break;
                    }
            }
            
            return ConnectedWires;
        }
        

        public List<decimal> getFrequencyPoints()
        {
            var output = new List<decimal>();

            if (FrequencyScale == AxisScale.Logarithmic)
            {
                var c = MaximumFrequency / MinimumFrequency;
                var pitch = DecimalMath.Power(c, 1 / (NumberOfFrequencyPoints - 1m));

                for (int x = 0; x < NumberOfFrequencyPoints; x++)
                    output.Add(MinimumFrequency * DecimalMath.PowerN(pitch, x));
            }

            if (FrequencyScale == AxisScale.Linear)
            {
                var pitch = (MaximumFrequency - MinimumFrequency) / (NumberOfFrequencyPoints - 1);

                for (int x = 0; x < NumberOfFrequencyPoints; x++)
                    output.Add(MinimumFrequency + x * pitch);
            }

            return output;
        }

     
    }

}
