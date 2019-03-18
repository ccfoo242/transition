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

            foreach (var component in node.ConnectedComponentTerminals.Where(tup => !(tup.Item1 is IMeterComponent)))
            {
                otherTerminals = component.Item1.GetOtherTerminals(component.Item2);
                foreach (var other in otherTerminals)
                {
                    node2 = ElectricNodes.Find(node3 => node3.HasComponentTerminal(component.Item1, other));
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

            foreach (var component in node.ConnectedComponentTerminals.Where(tup => !(tup.Item1 is IMeterComponent)))
            {
                if (component.Item1 is IIsolateSection)
                    otherTerminals = ((IIsolateSection)component.Item1).getOtherTerminalsIsolated(component.Item2).ToList();
                else
                    otherTerminals = component.Item1.GetOtherTerminals(component.Item2);
                
                foreach (var other in otherTerminals)
                {
                    node2 = ElectricNodes.Find(node3 => node3.HasComponentTerminal(component.Item1, other));
                    if (node2 != null)
                        if (!output.Contains(node2) && (node2 != node)) output.Add(node2);
                }

                if (component.Item1 is IImplicitGroundedComponent)
                    if (component.Item1 is OpAmp)
                    {/* opamp is floating at input, and grounded at output */
                        if (component.Item2 == 2)
                            if (!output.Contains(GroundNode)) output.Add(GroundNode);
                    }
                    else
                            if (!output.Contains(GroundNode)) output.Add(GroundNode);
            }

            return output;

        }

        public void MakeUpNodesSectionsCircuits()
        {
            ElectricNodes.Clear();

            GroundNode.NodeWires.Clear();
            GroundNode.ConnectedComponentTerminals.Clear();
            //GroundNode.NodeWires.AddRange(GetGroundedWires());

            ElectricNodes.Add(GroundNode);

            Func<SerializableComponent, byte, ElectricNode> compTerminalBelongsToNode =
                (comp, terminal) =>
                {
                    if (comp.IsTerminalGrounded(terminal)) return GroundNode;

                    foreach (var node in ElectricNodes)
                        if (node.ConnectedComponentTerminals.Contains(new Tuple<SerializableComponent, byte>(comp, terminal)))
                            return node;

                    return null;
                };

            ElectricNode newNode;
            ElectricNode node2;
            int nodeNumber = 1;

            var compTerminals = GetAllComponentsTerminals();

            foreach (var compTerm in compTerminals)
            {
                node2 = compTerminalBelongsToNode(compTerm.Item1, compTerm.Item2);

                if (node2 == null)
                {
                    newNode = new ElectricNode() { groundNode = false, NodeNumber = nodeNumber };
                    nodeNumber++;
                    newNode.ConnectedComponentTerminals.AddRange(compTerm.Item1.GetOtherConnectedComponents(compTerm.Item2));
                    ElectricNodes.Add(newNode);
                }
                else
                    if (node2 == GroundNode)
                        GroundNode.ConnectedComponentTerminals.Add(compTerm);
            }


            foreach (var node in ElectricNodes)
                if (GetOtherNodesConnectedToThisNode(node).Count == 0)
                    node.isIsolatedFromOtherNodes = true;
            

            Circuits.Clear();
           
            Circuit currentCircuit;

            foreach (var node in ElectricNodes.Where(node => !node.groundNode))
            {
                currentCircuit = Circuits.Find(circuit => circuit.Nodes.Contains(node));
                if (currentCircuit == null)
                    Circuits.Add(getCircuit(node));
            }
            
            CircuitSection currentSection;

            foreach (var circuit in Circuits)
                foreach (var node in circuit.Nodes)
                {
                    currentSection = circuit.Sections.Find(section => section.Nodes.Contains(node));
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
                if (node == GroundNode) return;
                foreach (var nextNode in GetOtherNodesConnectedToThisNodeSection(node))
                    if (!output.Nodes.Contains(nextNode)) visit(nextNode);
            };

            visit(startingNode);
            
            return output;
        }


        private Circuit getCircuit(ElectricNode startingNode)
        {
            var output = new Circuit();

            Action<ElectricNode> visit = null;
            
            visit = (node) => {
                output.Nodes.Add(node);
                if (node == GroundNode) return;
                foreach (var nextNode in GetOtherNodesConnectedToThisNode(node))
                    if (!output.Nodes.Contains(nextNode)) visit(nextNode);
            };

            visit(startingNode);
            
            return output;
        }
        

        public List<Tuple<SerializableComponent, byte>> GetAllComponentsTerminals()
        {
            var output = new List<Tuple<SerializableComponent, byte>>();
            
            foreach (var comp in CurrentDesign.Components)
                for (byte i = 0; i < comp.QuantityOfTerminals; i++)
                    output.Add(new Tuple<SerializableComponent, byte>(comp, i));
            
            return output;
        }


        public async Task<Tuple<bool, string>> Calculate()
        {

            MakeUpNodesSectionsCircuits();

         
            Func<IPassive, byte, ElectricNode> getOtherNodeInPassiveComponent =
                (component, compTerminal) =>
                {
                    var terminals = component.GetImpedanceTerminals;
                    
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
            
            if (Circuits.Count == 0) return new Tuple<bool, string>(false, "There is no circuit");

            foreach (var circuit in Circuits)
                foreach (var section in circuit.Sections)
                    if (section.HasGroundReferenceVoltageOutput && !section.IsGrounded)
                        return new Tuple<bool, string>(false, "There is a floating section with a grounded voltage output node, use the differential voltage output");

            var NodeMatrix = new Common.Matrix(QuantityOfNodes);
            /* 
             * inside this matrix we put admittances we solve this matrix along with a vector, 
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
        public bool groundNode;

        public bool HasGroundReferenceVoltageOutput => MeterComponents.Where(comp => comp is VoltageOutputNode).Count() > 0;

        public bool IsEssential { get => NonMeterComponents.Count > 2; }

        public bool HasNoComponentsConnected { get => NonMeterComponents.Count > 0; }
        public bool HasVoltageSource { get => NonMeterComponents.Where(comp => comp is VoltageSource).Count() > 0; }

        public List<SerializableComponent> NonMeterComponents => 
            ConnectedComponentTerminals.Select(tup => tup.Item1).Where(comp => !(comp is IMeterComponent)).ToList();

        public List<SerializableComponent> MeterComponents =>
            ConnectedComponentTerminals.Select(tup => tup.Item1).Where(comp => (comp is IMeterComponent)).ToList();

        public bool HasComponentTerminal(SerializableComponent comp, byte terminal)
        {
            var tuple = new Tuple<SerializableComponent, byte>(comp, terminal);
            return ConnectedComponentTerminals.Contains(tuple);
        }

        public bool HasComponent(SerializableComponent comp)
        {
            return ConnectedComponentTerminals.Select(tup => tup.Item1).Contains(comp);
        }

        public override string ToString()
        {
            if (groundNode) return "Ground Node"; else
            return "Node " + NodeNumber.ToString();
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

        public override string ToString()
        {
            return "Circuit, Mode: " + this.CircuitMode.ToString() + ", " + Sections.Count.ToString() + " Sections, " + Nodes.Count.ToString() + " Nodes";
        }
    }



    public class CircuitSection
    {
        public List<ElectricNode> Nodes { get; } = new List<ElectricNode>();
        public bool IsGrounded => Nodes.Where(node => node.groundNode).Count() > 0;

        public override string ToString()
        {
            return "Circuit section: " + Nodes.Count.ToString() + " nodes, " +
                (IsGrounded ? "Grounded" : "Floating");
        }

        public bool HasGroundReferenceVoltageOutput => Nodes.Where(node => node.HasGroundReferenceVoltageOutput).Count() > 0;


    }
    
}
