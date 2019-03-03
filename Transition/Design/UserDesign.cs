using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Easycoustics.Transition.CircuitEditor;
using Easycoustics.Transition.CircuitEditor.OnScreenComponents;
using Easycoustics.Transition.CircuitEditor.Serializable;
using Easycoustics.Transition.Common;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Easycoustics.Transition.Design
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

        public static UserDesign CurrentDesign { get; set; }

        public GraphParameters CurveGraphParameters { get; set; } = new GraphParameters();
        public AnalysisParameters AnalysisParameters { get; set; } = new AnalysisParameters();

        public ObservableCollection<SerializableComponent> Components { get; } = new ObservableCollection<SerializableComponent>();
        public ObservableCollection<SerializableWire> Wires { get; } = new ObservableCollection<SerializableWire>();

        public ObservableCollection<ScreenComponentBase> ScreenComponents { get; } = new ObservableCollection<ScreenComponentBase>();
        public ObservableCollection<WireScreen> ScreenWires { get; } = new ObservableCollection<WireScreen>();

        private readonly List<ElectricNode> ElectricNodes = new List<ElectricNode>();
        private ElectricNode GetGroundNode { get { return ElectricNodes.Where(node => node.groundNode).ToArray()[0]; } }


        public delegate void ElementDelegate(UserDesign sender, SerializableElement element);
        public event ElementDelegate ElementAdded;
        public event ElementDelegate ElementRemoved;

       
        public SchematicParameters CircuitSchematicParameters { get; set; } = new SchematicParameters();
      
        public CurveLibrary.LibraryFolder SystemCurves = new CurveLibrary.LibraryFolder("Design Result Curves");
        public CurveLibrary.LibraryFolder UserCurves = new CurveLibrary.LibraryFolder("User Curves");

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
            CurrentDesign = this;

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
                        {
                            if (el is WireScreen)
                            {
                                WireScreen ws = (WireScreen)el;
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
        
        public async Task<Tuple<bool,string>> Calculate()
        {
          
            CalculateNodes();

            var NodesToWork = ElectricNodes.Where(node => !node.hasNoComponentsConnected && !node.isIsolatedFromOtherNodes && !node.groundNode).ToList();

            Func<IPassive, byte, decimal, List<Tuple<ElectricNode, ComplexDecimal>>> getOtherNodesInPassiveComponent =
                (component, compTerminal, frequency) =>
                {
                    var output = new List<Tuple<ElectricNode, ComplexDecimal>>();
                    var impedances = component.getImpedance(frequency);

                    byte otherTerminal;
                    ElectricNode otherNode;

                    foreach (var impedance in impedances)
                    {
                        if ((impedance.Item1 == compTerminal) || (impedance.Item2 == compTerminal))
                        {
                            otherTerminal = (impedance.Item1 == compTerminal) ? impedance.Item2 : impedance.Item1;
                            /* some impedances are fixed grounded, cannot be floating, those return 255 as component terminal
                             that means connected to the node 0, that is always ground */
                            if (otherTerminal == 255)
                                otherNode = GetGroundNode;
                            else
                                otherNode = GetComponentTerminalNode((SerializableComponent)component, otherTerminal);

                            if (otherNode != null)
                                output.Add(new Tuple<ElectricNode, ComplexDecimal>(otherNode, impedance.Item3));
                        }
                    }

                    return output;
                };

            if (NodesToWork.Count == 0) return new Tuple<bool, string>(false, "Circuit has no nodes");

            int QuantityOfNodes = NodesToWork.Count;

            
            var NodeMatrix = new Common.Matrix(QuantityOfNodes);
            /* inside this matrix we put admittances  we solve this matrix along with a vector, 
             as a equation system of currents for getting the voltages at every node
             admittance is the reciprocal of impedance.
             both admittance and impedance are complex.
             */

            var CVMatrix = new Common.Matrix(QuantityOfNodes, 1);

            Common.Matrix nodeVoltages;
            //node 0 is always the ground

            var outputVoltageNodes = Components.OfType<VoltageOutputNode>();
            var outputVoltageCurrentComponents = Components.OfType<IVoltageCurrentOutput>();

            SystemCurves.Clear();

            foreach (var outputNode in outputVoltageNodes)
            {
                outputNode.ResultVoltageCurve.Clear();
                SystemCurves.AddIfNotAdded(outputNode.ResultVoltageCurve);
             /*   outputNode.ResultVoltageCurve.AdaptFunctionTo(MinimumFrequency, MaximumFrequency,
                    QuantityOfFrequencyPoints, FrequencyScale);*/
            }

            var OutputVoltagesComponents = new List<IVoltageCurrentOutput>();
            var OutputCurrentsComponents = new List<IVoltageCurrentOutput>();
            var OutputResistorsPower = new List<Resistor>();

            foreach (var outputVoltCurrComponent in outputVoltageCurrentComponents)
            {
                if (outputVoltCurrComponent.OutputVoltageAcross)
                {
                    outputVoltCurrComponent.resultVoltageCurve.Clear();
                    SystemCurves.AddIfNotAdded(outputVoltCurrComponent.resultVoltageCurve);

                    //outputVoltCurrComponent.resultVoltageCurve.AdaptFunctionTo(MinimumFrequency, MaximumFrequency,
                    //QuantityOfFrequencyPoints, FrequencyScale);

                    OutputVoltagesComponents.Add(outputVoltCurrComponent);
                }

                if (outputVoltCurrComponent.OutputCurrentThrough)
                {
                    outputVoltCurrComponent.resultCurrentCurve.Clear();
                    SystemCurves.AddIfNotAdded(outputVoltCurrComponent.resultCurrentCurve);

                    //outputVoltCurrComponent.resultCurrentCurve.AdaptFunctionTo(MinimumFrequency, MaximumFrequency,
                    //QuantityOfFrequencyPoints, FrequencyScale);

                    OutputCurrentsComponents.Add(outputVoltCurrComponent);
                }
            }

            foreach (var resistor in Components.OfType<Resistor>())
            {

                SystemCurves.AddIfNotAdded(resistor.resultPowerCurve);
            //    resistor.resultPowerCurve.AdaptFunctionTo(MinimumFrequency, MaximumFrequency, QuantityOfFrequencyPoints, FrequencyScale);
                OutputResistorsPower.Add(resistor);
            }


            ElectricNode nodePositive;
            ElectricNode nodeNegative;
            ElectricNode node2;
            int nodePositiveNumber;
            int nodeNegativeNumber;
            ComplexDecimal voltPositive;
            ComplexDecimal voltNegative;
            ComplexDecimal totalVoltage;
            IPassive passive;
            ComplexDecimal sourceAdmittance;
            ComplexDecimal voltage;
            ComplexDecimal voltagePolarity;
            bool positiveTerminal;

            

            var freqPoints = getFrequencyPoints();
            foreach (var FreqPoint in freqPoints)
            {
                /* whole circuit is solved for each frequency point */
                NodeMatrix.Clear();
                CVMatrix.Clear();
                /* here we populate the node matrix */
                for (int nodeNumber = 0; nodeNumber < QuantityOfNodes; nodeNumber++)
                {
                    foreach (var component in NodesToWork[nodeNumber].ConnectedComponentTerminals)
                    {
                        if (component.Item1 is IPassive)
                        {
                            passive = (IPassive)component.Item1;
                            /* the impedance is calculated for a certain frequency */
                            var impedances = getOtherNodesInPassiveComponent(passive, component.Item2, FreqPoint);

                            foreach (var impedance in impedances)
                            {
                                NodeMatrix.addAtCoordinate(nodeNumber, nodeNumber, impedance.Item2.Reciprocal);
                                
                                if (impedance.Item1 != GetGroundNode) /* impedance is floating */
                                     NodeMatrix.addAtCoordinate(nodeNumber, NodesToWork.IndexOf(impedance.Item1), -1 * impedance.Item2.Reciprocal);
                            }
                        }

                        if (component.Item1 is VoltageSource)
                        {
                            var source = (VoltageSource)component.Item1;
                            sourceAdmittance = source.getSourceImpedance(FreqPoint).Reciprocal;

                            voltage = source.getSourceVoltage(FreqPoint);

                            positiveTerminal = (component.Item2 == 0);
                            voltagePolarity = positiveTerminal ? 1 : -1;

                            byte otherTerminal = positiveTerminal ? source.NegativeTerminal : source.PositiveTerminal;
                            var otherNode = GetComponentTerminalNode(source, otherTerminal);

                            if (otherNode != null)
                            {
                                CVMatrix.addAtCoordinate(nodeNumber, 0, voltage * sourceAdmittance * voltagePolarity);
                                NodeMatrix.addAtCoordinate(nodeNumber, nodeNumber, sourceAdmittance);

                                if (otherNode != GetGroundNode)
                                    NodeMatrix.addAtCoordinate(nodeNumber, NodesToWork.IndexOf(otherNode), -1 * sourceAdmittance);
                            }
                        }
                    }
                }

                try
                {
                    nodeVoltages = NodeMatrix.Solve(CVMatrix);
                }
                catch (Exception e)
                {
                    return new Tuple<bool, string>(false, "Voltage Matrix solving failed: " + Environment.NewLine + e.ToString());
                }

                foreach (var nodeV in outputVoltageNodes)
                {
                    node2 = GetComponentTerminalNode(nodeV, 0);

                    if (node2 != null)
                        nodeV.ResultVoltageCurve.addSample(FreqPoint, nodeVoltages.Data[NodesToWork.IndexOf(node2), 0]);
                }


                foreach (var comp in OutputVoltagesComponents)
                {
                    nodePositive = GetComponentTerminalNode((SerializableComponent)comp, 0);
                    nodeNegative = GetComponentTerminalNode((SerializableComponent)comp, 1);

                    if (nodePositive != null && nodeNegative != null)
                    {
                        nodePositiveNumber = NodesToWork.IndexOf(nodePositive);
                        nodeNegativeNumber = NodesToWork.IndexOf(nodeNegative);

                        voltPositive = (nodePositive != GetGroundNode) ? nodeVoltages.Data[nodePositiveNumber, 0] : 0;
                        voltNegative = (nodeNegative != GetGroundNode) ? nodeVoltages.Data[nodeNegativeNumber, 0] : 0;

                        totalVoltage = voltPositive - voltNegative;
                        
                        comp.resultVoltageCurve.addSample(FreqPoint, totalVoltage);
                      
                    }
                }

                foreach (var comp in OutputCurrentsComponents)
                {
                     nodePositive = GetComponentTerminalNode((SerializableComponent)comp, 0);
                     nodeNegative = GetComponentTerminalNode((SerializableComponent)comp, 1);

                    if (nodePositive != null && nodeNegative != null)
                    {
                        nodePositiveNumber = NodesToWork.IndexOf(nodePositive);
                        nodeNegativeNumber = NodesToWork.IndexOf(nodeNegative);

                        voltPositive = (nodePositive != GetGroundNode) ? nodeVoltages.Data[nodePositiveNumber, 0] : 0;
                        voltNegative = (nodeNegative != GetGroundNode) ? nodeVoltages.Data[nodeNegativeNumber, 0] : 0;

                        totalVoltage = voltPositive - voltNegative;
                        
                        var current = totalVoltage / comp.getImpedance(FreqPoint);
                        comp.resultCurrentCurve.addSample(FreqPoint, current);
                    }
                }
            }

            return new Tuple<bool, string>(true, "Circuit solved OK");
        }


        public void CalculateNodes()
        {
            //var output = new List<ElectricNode>();
            ElectricNodes.Clear();

            var GroundNode = new ElectricNode() { groundNode = true };
            GroundNode.NodeWires.AddRange(GetGroundedWires());

            ElectricNodes.Add(GroundNode);

            Func<SerializableWire, ElectricNode> wireBelongsTo =
                (wire) => 
                {
                    foreach (var node in ElectricNodes)
                        foreach (var existingWire in node.NodeWires)
                            if (wire.isThisWireBoundedToOtherWire(existingWire)) return node;
                
                    return null;
                };

            ElectricNode x;

            foreach (var wire in GetNonGroundedWires())
            {
                x = wireBelongsTo(wire);

                if (x == null)
                {
                    x = new ElectricNode();
                    ElectricNodes.Add(x);
                }
                    
                x.NodeWires.Add(wire);
            }

            foreach (var node in ElectricNodes)
                foreach (var wire in node.NodeWires)
                    foreach (var tuple in wire.GetBoundedComponents)
                        if (!node.ConnectedComponentTerminals.Contains(tuple))
                            node.ConnectedComponentTerminals.Add(tuple);

            foreach (var node in ElectricNodes)
            {
                if (node.ConnectedComponentTerminals.Count == 0) node.hasNoComponentsConnected = true;
                if (GetOtherNodesConnectedToThisNode(node).Count == 0) node.isIsolatedFromOtherNodes = true;
            }
            

        }

        private ElectricNode GetComponentTerminalNode(SerializableComponent comp, byte terminal)
        {
            return ElectricNodes.Find(node => node.HasComponentTerminal(comp, terminal));
        }

        private List<ElectricNode> GetOtherNodesConnectedToThisNode(ElectricNode node)
        {
            var output = new List<ElectricNode>();
            List<Tuple<SerializableComponent, byte>> otherTerminals;
            ElectricNode node2;

            foreach (var component in node.ConnectedComponentTerminals)
            {
                otherTerminals = component.Item1.GetOtherTerminals(component.Item2);
                foreach (var other in otherTerminals)
                {
                    node2 = GetComponentTerminalNode(other.Item1, other.Item2);
                    if (node2 != null)
                        if (!output.Contains(node2) && node2 != node) output.Add(node2);
                }
            }

            return output;

        }

        public List<SerializableWire> GetNonGroundedWires()
        {
            var grounded = GetGroundedWires();
            
            return Wires.Where(wire => !grounded.Contains(wire)).ToList(); 
        }

        public List<SerializableWire> GetGroundedWires()
        {
            var output = Wires.Where((wire) => wire.IsWireGrounded).ToList();

            Func<SerializableWire, bool> isConnected =
                (wir) =>
                {
                    foreach (var wire in output)
                        if (wir.isThisWireBoundedToOtherWire(wire))
                            return true; 

                    return false;
                };

            bool wireAdded = true;

            while (wireAdded)
            {
                wireAdded = false;

                foreach (var wire in Wires)
                    if (!output.Contains(wire))
                        if (isConnected(wire))
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

            Func<SerializableWire, bool> isConnected =
                (wir) =>
                {
                    foreach (var wire in ConnectedWires)
                        if (wir.isThisWireBoundedToOtherWire(wire))
                            return true; 
                    return false;
                };

            while (added)
            {
                added = false;
                foreach (var wire in Wires)
                    if (isConnected(wire))
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
            return getFrequencyPoints(AnalysisParameters.AnalysisFrequencyScale,
                                      AnalysisParameters.AnalysisMaximumFrequency,
                                      AnalysisParameters.AnalysisMinimumFrequency, 
                                      AnalysisParameters.AnalysisQuantityOfFrequencyPoints);
        }

        public static List<decimal> getFrequencyPoints(AxisScale scale, decimal maximumFreq, decimal minimumFreq, uint quantityOfPoints)
        {
            var output = new List<decimal>();

            if (scale == AxisScale.Logarithmic)
            {
                var c = maximumFreq / minimumFreq;
                var pitch = DecimalMath.Power(c, 1 / (quantityOfPoints - 1m));

                for (int x = 0; x < quantityOfPoints; x++)
                    output.Add(minimumFreq * DecimalMath.PowerN(pitch, x));
            }

            if (scale == AxisScale.Linear)
            {
                var pitch = (maximumFreq - minimumFreq) / (quantityOfPoints - 1);

                for (int x = 0; x < quantityOfPoints; x++)
                    output.Add(minimumFreq + x * pitch);
            }

            return output;
        }
     


    }


    public class ElectricNode
    {
        public int NodeNumber;
        public List<Tuple<SerializableComponent, byte>> ConnectedComponentTerminals = new List<Tuple<SerializableComponent, byte>>();
        public List<SerializableWire> NodeWires = new List<SerializableWire>();
        public bool isIsolatedFromOtherNodes;
        public bool hasNoComponentsConnected;
        public bool groundNode;

        public bool IsEssential { get => ConnectedComponentTerminals.Count > 2; }

        public bool HasComponentTerminal(SerializableComponent comp, byte terminal)
        {
            var tuple = new Tuple<SerializableComponent, byte>(comp, terminal);

            return ConnectedComponentTerminals.Contains(tuple);
        }
    }
}
