using Easycoustics.Transition.CircuitEditor;
using Easycoustics.Transition.CircuitEditor.Serializable;
using Easycoustics.Transition.Common;
using Easycoustics.Transition.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easycoustics.Transition
{
    public class Analyzer
    {

        public static Analyzer CurrentInstance { get; } = new Analyzer();

        private AnalysisParameters Parameters { get => CurrentDesign.AnalysisParameters; }
        private UserDesign CurrentDesign { get => UserDesign.CurrentDesign; }

        private readonly List<ElectricNode> ElectricNodes = new List<ElectricNode>();
        private ElectricNode GroundNode = new ElectricNode() { groundNode = true, NodeNumber = 0 };

        private IEnumerable<SerializableComponent> Components => UserDesign.CurrentDesign.Components;
        private CurveLibrary.LibraryFolder SystemCurves => UserDesign.CurrentDesign.SystemCurves;

        private readonly List<Circuit> Circuits = new List<Circuit>();

        public Analyzer()
        {

        }

        

        private ElectricNode GetComponentTerminalNode(SerializableComponent comp, byte terminal)
        {
            return ElectricNodes.Find(node => node.HasComponentTerminal(comp, terminal));
        }
        


        private List<ElectricNode> GetOtherNodesConnectedToThisNode(ElectricNode node)
        {
            var output = new List<ElectricNode>();
            List<byte> otherTerminals;
            ElectricNode node2;

            foreach (var component in node.ConnectedComponentTerminals)
            {
                otherTerminals = component.Item1.GetOtherTerminals(component.Item2);
                foreach (var other in otherTerminals)
                {
                    node2 = GetComponentTerminalNode(component.Item1, other);
                    if (node2 != null)
                        if (!output.Contains(node2) && (node2 != node)) output.Add(node2);
                }

                if (component.Item1 is IImplicitGroundedComponent)
                    if (!output.Contains(GroundNode)) output.Add(GroundNode);
            }

            return output;
        }

        private List<ElectricNode> GetOtherNodesConnectedToThisNodeSection(ElectricNode node)
        {
            var output = new List<ElectricNode>();

            List<byte> otherTerminals;
            ElectricNode node2;

            foreach (var component in node.ConnectedComponentTerminals)
            {
                if (component.Item1 is IIsolateSection)
                    otherTerminals = ((IIsolateSection)component.Item1).getOtherTerminalsIsolated(component.Item2).ToList();
                else
                    otherTerminals = component.Item1.GetOtherTerminals(component.Item2);
                
                foreach (var other in otherTerminals)
                {
                    node2 = GetComponentTerminalNode(component.Item1, other);
                    if (node2 != null)
                        if (!output.Contains(node2) && (node2 != node) && (node2 != GroundNode)) output.Add(node2);

                    if (component.Item1 is IImplicitGroundedComponent)
                        if (component.Item1 is OpAmp)
                        {
                            if (component.Item2 == 2)
                                if (!output.Contains(GroundNode)) output.Add(GroundNode);
                        } else
                                if (!output.Contains(GroundNode)) output.Add(GroundNode);

                }
            }

            return output;

        }

        public void MakeUpNodesSectionsCircuits()
        {
            //var output = new List<ElectricNode>();
            ElectricNodes.Clear();

            GroundNode.NodeWires.Clear();
            GroundNode.NodeWires.AddRange(GetGroundedWires());

            ElectricNodes.Add(GroundNode);

            Func<SerializableWire, ElectricNode> wireBelongsTo =
                (wire) =>
                {
                    foreach (var node in ElectricNodes)
                        if (node.NodeWires.Contains(wire)) return node;

                    return null;
                };

            ElectricNode newNode;

            foreach (var wire in GetNonGroundedWires())
                if (wireBelongsTo(wire) == null)
                {
                    newNode = new ElectricNode() { groundNode = false };
                    ElectricNodes.Add(newNode);
                    newNode.NodeWires.AddRange(WireGroup(wire));
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

            Circuits.Clear();

            Func<ElectricNode, Circuit> nodeBelongsToCircuit = (node) => {
                foreach (var circuit in Circuits)
                { if (circuit.Nodes.Contains(node)) return circuit; }

                return null;
            };

            Circuit currentCircuit;

            foreach (var node in ElectricNodes)
            {
                currentCircuit = nodeBelongsToCircuit(node);
                if (currentCircuit == null)
                    Circuits.Add(getCircuit(node));
            }

            Func<ElectricNode, Circuit, CircuitSection> nodeBelongsToSection = (node, circuit) =>
            {
                foreach (var section in circuit.Sections)
                { if (section.Nodes.Contains(node)) return section; }

                return null;
            };

            CircuitSection currentSection;

            foreach (var circuit in Circuits)
                foreach (var node in circuit.Nodes)
                {
                    currentSection = nodeBelongsToSection(node, circuit);
                    if (currentSection == null)
                        circuit.Sections.Add(getSection(node));
                }
            
            

        }

        private CircuitSection getSection(ElectricNode startingNode)
        {
            var output = new CircuitSection();

            Action<ElectricNode> visit = null;
            
            visit = (node) => {
                output.Nodes.Add(node);
                foreach (var nextNode in GetOtherNodesConnectedToThisNodeSection(node))
                    if (!output.Nodes.Contains(nextNode)) visit(nextNode);
            };

            visit(startingNode);
            
            output.Nodes.AddRange(output.Nodes);

            return output;
        }


        private Circuit getCircuit(ElectricNode startingNode)
        {
            var output = new Circuit();

            Action<ElectricNode> visit = null;
            
            visit = (node) => {
                output.Nodes.Add(node);
                foreach (var nextNode in GetOtherNodesConnectedToThisNode(node))
                    if (!output.Nodes.Contains(nextNode)) visit(nextNode);
            };

            visit(startingNode);
            
            return output;
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
                foreach (var wire in CurrentDesign.Wires)
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



      




        private bool areTwoWiresConnected(SerializableWire w1, SerializableWire w2)
        {
            if (w1 == null || w2 == null) throw new NullReferenceException();
            var groupW1 = WireGroup(w1);

            return groupW1.Contains(w2);
        }



        public List<SerializableWire> GetNonGroundedWires()
        {
            var grounded = GetGroundedWires();

            return CurrentDesign.Wires.Where(wire => !grounded.Contains(wire)).ToList();
        }

        public List<SerializableWire> GetGroundedWires()
        {
            var output = CurrentDesign.Wires.Where((wire) => wire.IsWireGrounded).ToList();

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

                foreach (var wire in CurrentDesign.Wires)
                    if (!output.Contains(wire))
                        if (isConnected(wire))
                        {
                            output.Add(wire);
                            wireAdded = true;
                        }
            }

            return output;
        }



        public async Task<Tuple<bool, string>> Calculate()
        {

            MakeUpNodesSectionsCircuits();

            var NodesToWork = ElectricNodes.Where(node => !node.hasNoComponentsConnected && !node.isIsolatedFromOtherNodes && !node.groundNode).ToList();

            Func<IPassive, byte, ElectricNode> getOtherNodeInPassiveComponent =
                (component, compTerminal) =>
                {
                    var terminals = component.GetImpedanceTerminals;

                    //var impedance = component.GetImpedance(frequency);

                    byte otherTerminal;
                    ElectricNode otherNode = null;

                    if ((terminals.Item1 == compTerminal) || (terminals.Item2 == compTerminal))
                    {
                        otherTerminal = (terminals.Item1 == compTerminal) ? terminals.Item2 : terminals.Item1;
                        /* some impedances are fixed grounded, cannot be floating, those return 255 as component terminal
                         that means connected to the node 0, that is always ground */
                        if (otherTerminal == 255)
                            otherNode = GroundNode;
                        else
                            otherNode = GetComponentTerminalNode((SerializableComponent)component, otherTerminal);
                    }

                    return otherNode;
                };

            if (NodesToWork.Count == 0) return new Tuple<bool, string>(false, "Circuit has no nodes");

            int QuantityOfNodes = NodesToWork.Count;


            var NodeMatrix = new Common.Matrix(QuantityOfNodes);
            /* inside this matrix we put admittances  we solve this matrix along with a vector, 
             as a equation system of currents for getting the voltages at every node.
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
            }

            var OutputVoltagesComponents = new List<IVoltageCurrentOutput>();
            var OutputCurrentsComponents = new List<IVoltageCurrentOutput>();
            var OutputResistorsPower = new List<Resistor>();

            foreach (var outputVoltCurrComponent in outputVoltageCurrentComponents)
            {
                if (outputVoltCurrComponent.OutputVoltageAcross)
                {
                    outputVoltCurrComponent.ResultVoltageCurve.Clear();
                    SystemCurves.AddIfNotAdded(outputVoltCurrComponent.ResultVoltageCurve);


                    OutputVoltagesComponents.Add(outputVoltCurrComponent);
                }

                if (outputVoltCurrComponent.OutputCurrentThrough)
                {
                    outputVoltCurrComponent.ResultCurrentCurve.Clear();
                    SystemCurves.AddIfNotAdded(outputVoltCurrComponent.ResultCurrentCurve);

                    //outputVoltCurrComponent.resultCurrentCurve.AdaptFunctionTo(MinimumFrequency, MaximumFrequency,
                    //QuantityOfFrequencyPoints, FrequencyScale);

                    OutputCurrentsComponents.Add(outputVoltCurrComponent);
                }
            }

            foreach (var resistor in Components.OfType<Resistor>())
            {

                SystemCurves.AddIfNotAdded(resistor.ResultPowerCurve);
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



            var freqPoints = UserDesign.CurrentDesign.getFrequencyPoints();
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
                            var otherNode = getOtherNodeInPassiveComponent(passive, component.Item2);
                            var impedance = passive.GetImpedance(FreqPoint);

                            NodeMatrix.addAtCoordinate(nodeNumber, nodeNumber, impedance.Reciprocal);
                            /* reciprocal of impedance is the admittance, the matrix itself is an admittance matrix */

                            if (otherNode != GroundNode) /* impedance is floating */
                                NodeMatrix.addAtCoordinate(nodeNumber, NodesToWork.IndexOf(otherNode), -1 * impedance.Reciprocal);

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

                                if (otherNode != GroundNode)
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

                        voltPositive = (nodePositive != GroundNode) ? nodeVoltages.Data[nodePositiveNumber, 0] : 0;
                        voltNegative = (nodeNegative != GroundNode) ? nodeVoltages.Data[nodeNegativeNumber, 0] : 0;

                        totalVoltage = voltPositive - voltNegative;

                        comp.ResultVoltageCurve.addSample(FreqPoint, totalVoltage);

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

                        voltPositive = (nodePositive != GroundNode) ? nodeVoltages.Data[nodePositiveNumber, 0] : 0;
                        voltNegative = (nodeNegative != GroundNode) ? nodeVoltages.Data[nodeNegativeNumber, 0] : 0;

                        totalVoltage = voltPositive - voltNegative;

                        var current = totalVoltage / comp.GetImpedance(FreqPoint);
                        comp.ResultCurrentCurve.addSample(FreqPoint, current);
                    }
                }
            }

            return new Tuple<bool, string>(true, "Circuit solved OK");
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

        public bool HasComponent(SerializableComponent comp)
        {
            return ConnectedComponentTerminals.Select(tup => tup.Item1).Contains(comp);
        }
    }



    public class Circuit
    {
        public List<CircuitSection> Sections { get; } = new List<CircuitSection>();   

        public CircuitMode CircuitMode { get {
                int quantityFloating = 0;
                int quantityGrounded = 0;
                foreach (var section in Sections)
                {
                    if (section.IsGrounded) quantityGrounded++;
                    else quantityFloating++;
                }

                if (quantityFloating > 0 && quantityGrounded == 0) return CircuitMode.Floating;
                else
                    if (quantityFloating == 0 && quantityGrounded > 0) return CircuitMode.Grounded;
                else
                    return CircuitMode.Mixed;

            } }

        public List<ElectricNode> Nodes { get; } = new List<ElectricNode>();
    }

    public class CircuitSection
    {
        public List<ElectricNode> Nodes { get; } = new List<ElectricNode>();
        public bool IsGrounded => Nodes.Where(node => node.groundNode).Count() > 0;
    }
    
}
