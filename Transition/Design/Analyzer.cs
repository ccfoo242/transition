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
        private ElectricNode GroundNode = new ElectricNode() { IsGroundNode = true, NodeNumber = 0 };

        private IEnumerable<SerializableComponent> Components => UserDesign.CurrentDesign.Components;
        private CurveLibrary.LibraryFolder SystemCurves => UserDesign.CurrentDesign.SystemCurves;

        private readonly List<Circuit> Circuits = new List<Circuit>();

      //  private Dictionary<Circuit, Common.Matrix> MatrixDict = new Dictionary<Circuit, Common.Matrix>();
     //   private Dictionary<Circuit, Common.Matrix> CVDict = new Dictionary<Circuit, Common.Matrix>();
     //   private Dictionary<Circuit, Common.Matrix> ResultDict = new Dictionary<Circuit, Common.Matrix>();
        private bool CircuitChanged = false;

        public Analyzer()
        {

        }

        public static void CircuitHasChanged()
        {
            CurrentInstance.CircuitChanged = true;
        }

        private ElectricNode GetComponentTerminalNode(SerializableComponent comp, byte terminal)
        {
            if (comp.IsTerminalGrounded(terminal)) return GroundNode;

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
         
            ElectricNode newNode;
            ElectricNode node2;
            int nodeNumber = 1;

            var compTerminals = GetAllComponentsTerminals();

            foreach (var compTerm in compTerminals)
            {
                node2 = GetComponentTerminalNode(compTerm.Item1, compTerm.Item2);

                if (node2 == null)
                {
                    /* we build the node */
                    newNode = new ElectricNode() { IsGroundNode = false, NodeNumber = nodeNumber };
                    nodeNumber++;
                    newNode.ConnectedComponentTerminals.AddRange(compTerm.Item1.GetOtherConnectedComponents(compTerm.Item2));
                    ElectricNodes.Add(newNode);
                }
                else
                    if (node2 == GroundNode)
                        GroundNode.ConnectedComponentTerminals.Add(compTerm);
            }

            foreach (var compMeter in CurrentDesign.Components.Where(comp => comp is VoltageOutputDifferential))
            {
                if (compMeter.IsTerminalGrounded(0))
                    GroundNode.ConnectedComponentTerminals.Add(new Tuple<SerializableComponent, byte>(compMeter, 0));
                if (compMeter.IsTerminalGrounded(1))
                    GroundNode.ConnectedComponentTerminals.Add(new Tuple<SerializableComponent, byte>(compMeter, 1));  
            }

            /*
            foreach (var node in ElectricNodes)
                if (GetOtherNodesConnectedToThisNode(node).Count == 0)
                    node.isIsolatedFromOtherNodes = true;
            */

            Circuits.Clear();
           
            Circuit currentCircuit;

            foreach (var node in ElectricNodes.Where(node => !node.IsGroundNode))
            {
                currentCircuit = Circuits.Find(circuit => circuit.Nodes.Contains(node));
                if (currentCircuit == null)
                    Circuits.Add(buildCircuit(node));
            }
            
            CircuitSection currentSection;

            foreach (var circuit in Circuits)
                foreach (var node in circuit.Nodes)
                {
                    currentSection = circuit.Sections.Find(section => section.Nodes.Contains(node));
                    if (currentSection == null)
                        circuit.Sections.Add(buildSection(node));
                }

           // MatrixDict.Clear();
            //CVDict.Clear();

            foreach (var circuit in Circuits)
            {
                foreach (var section in circuit.Sections)
                {
                    circuit.NonReferenceNodes.AddRange(section.Nodes);
                    circuit.NonReferenceNodes.Remove(section.ReferenceNode);
                }

                //  MatrixDict.Add(circuit, new Common.Matrix(circuit.NonReferenceNodes.Count));
                //   CVDict.Add(circuit, new Common.Matrix(circuit.NonReferenceNodes.Count, 1));

                circuit.CurrentMatrix = new Common.Matrix(circuit.NonReferenceNodes.Count);
                circuit.ConstValueMatrix = new Common.Matrix(circuit.NonReferenceNodes.Count, 1);

            }


            CircuitChanged = false;


        }

        private CircuitSection buildSection(ElectricNode startingNode)
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


        private Circuit buildCircuit(ElectricNode startingNode)
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
            
            foreach (var comp in CurrentDesign.Components.Where(comp => !(comp is IMeterComponent)))
                for (byte i = 0; i < comp.QuantityOfTerminals; i++)
                    output.Add(new Tuple<SerializableComponent, byte>(comp, i));
            
            return output;
        }

        public CircuitSection GetSectionForNode(ElectricNode node)
        {
            CircuitSection output;

            foreach (var circuit in Circuits)
            {
                output = circuit.Sections.Find(section => section.Nodes.Contains(node));
                if (output != null) return output;
            }

            return null;
        }

        public Circuit GetCircuitForNode(ElectricNode node)
        {
            return Circuits.Find(circuit => circuit.Nodes.Contains(node));

        }

        public bool IsNodeGroundRefereced(ElectricNode node) => GetSectionForNode(node).IsGrounded;


        private ElectricNode getOtherNodeInPassiveComponent(IPassive component, byte compTerminal)
        {
            var terminals = component.GetImpedanceTerminals;

            byte otherTerminal = (terminals.Item1 == compTerminal) ? terminals.Item2 : terminals.Item1;
            /* some impedances are fixed grounded, cannot be floating, those return 255 as component terminal
             that means connected to the node 0, that is always ground */

            return (otherTerminal == 255) ? GroundNode : GetComponentTerminalNode((SerializableComponent)component, otherTerminal);

        }

        public async Task<Tuple<bool, string>> Calculate()
        {

            if (CircuitChanged) MakeUpNodesSectionsCircuits();

            if (Circuits.Count == 0) return new Tuple<bool, string>(false, "There is no circuit");

            foreach (var circuit in Circuits)
            {
                foreach (var section in circuit.Sections)
                {
                    if (section.HasGroundReferenceVoltageOutput && !section.IsGrounded)
                        return new Tuple<bool, string>(false, "There is a floating section with a grounded voltage output node, use the differential voltage output");
                }

                if (!circuit.HasVoltageSource)
                    return new Tuple<bool, string>(false, "There is a circuit without a Voltage Source, all circuits must have at least one Voltage Source");
                
            }
            /* check the differential voltage outputs */

            ElectricNode node0;
            ElectricNode node1;

            bool node0Grounded;
            bool node1Grounded;

            foreach (var diff in Components.OfType<VoltageOutputDifferential>())
            {
                node0 = GetComponentTerminalNode(diff, 0);
                node1 = GetComponentTerminalNode(diff, 1);
                
                if (node0 == null)
                    return new Tuple<bool, string>(false, "There is a differential voltage output with a terminal unconnected");
                if (node1 == null)
                    return new Tuple<bool, string>(false, "There is a differential voltage output with a terminal unconnected");

                node0Grounded = IsNodeGroundRefereced(node0);
                node1Grounded = IsNodeGroundRefereced(node1);

                if ((!node0Grounded && node1Grounded) || (node0Grounded && !node1Grounded))
                    return new Tuple<bool, string>(false, "There is a differential voltage output with a terminal grounded and the other floating, both must be floating in the same section, or both must be  in grounded sections");

                if (!node0Grounded && !node1Grounded)
                {
                    var section1 = GetSectionForNode(node0);
                    var section2 = GetSectionForNode(node1);
                    if (section1 != section2)
                        return new Tuple<bool, string>(false, "There is a differential voltage output with terminals on different floating sections");
                }

            }

         
            // var NodeMatrix = new Common.Matrix(QuantityOfNodes);
            /* 
             * inside this matrix we put admittances we solve this matrix along with a vector, 
             as a equation system of currents for getting the voltages at every node.
             admittance is the reciprocal of impedance.
             both admittance and impedance are complex.
             */

            // var CVMatrix = new Common.Matrix(QuantityOfNodes, 1);


            //Common.Matrix nodeVoltages;
            //node 0 is always the ground

            var outputVoltageNodes = Components.OfType<VoltageOutputNode>();
            var outputVoltageDiff = Components.OfType<VoltageOutputDifferential>();

            var outputVoltageCurrentComponents = Components.OfType<IVoltageCurrentOutput>();

            SystemCurves.Clear();

            foreach (var outputNode in outputVoltageNodes)
            {
                outputNode.ResultVoltageCurve.Clear();
                SystemCurves.AddIfNotAdded(outputNode.ResultVoltageCurve);
            }

            foreach (var outputNodeDiff in outputVoltageDiff)
            {
                outputNodeDiff.ResultVoltageCurve.Clear();
                SystemCurves.AddIfNotAdded(outputNodeDiff.ResultVoltageCurve);
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
                    
                    OutputCurrentsComponents.Add(outputVoltCurrComponent);
                }
            }

            foreach (var resistor in Components.OfType<Resistor>().Where(res => res.OutputResistorPower))
            {
                resistor.ResultPowerCurve.Clear();
                SystemCurves.AddIfNotAdded(resistor.ResultPowerCurve);
                OutputResistorsPower.Add(resistor);
            }

            ElectricNode node;
            ElectricNode node2;
            ElectricNode nodePositive;
            ElectricNode nodeNegative;
            ElectricNode currentNode;
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

            CircuitSection currentSection;

            var freqPoints = UserDesign.CurrentDesign.getFrequencyPoints();
            foreach (var FreqPoint in freqPoints)
            {
                foreach (var circuit in Circuits)
                {
                    /* circuit is solved for each frequency point */
                    circuit.CurrentMatrix.Clear();
                    circuit.ConstValueMatrix.Clear();
                    
                    /* here we populate the node matrix */
                    for (int nodeNumber = 0; nodeNumber < circuit.NonReferenceNodes.Count; nodeNumber++)
                    {
                        currentNode = circuit.NonReferenceNodes[nodeNumber];
                        //currentSection = GetSectionForNode(circuit.NonReferenceNodes[nodeNumber]);

                        foreach (var component in currentNode.ConnectedComponentTerminals)
                        {
                            var admittances = component.Item1.GetAdmittancesForTerminal(component.Item2, FreqPoint);

                            for (byte z = 0; z < admittances.Count(); z++)
                            {
                                node = GetComponentTerminalNode(component.Item1, z);
                                if (node != GetSectionForNode(node).ReferenceNode)
                                    circuit.CurrentMatrix.addAtCoordinate(nodeNumber, circuit.NonReferenceNodes.IndexOf(node), admittances[z]);
                            }
                            /*
                            if (component.Item1 is IPassive)
                            {
                                passive = (IPassive)component.Item1;

                                var otherNode = getOtherNodeInPassiveComponent(passive, component.Item2);
                                var impedance = passive.GetImpedance(FreqPoint);

                                MatrixDict[circuit].addAtCoordinate(nodeNumber, nodeNumber, impedance.Reciprocal);
                               
                                if (otherNode != currentSection.ReferenceNode) 
                                    MatrixDict[circuit].addAtCoordinate(nodeNumber, circuit.NonReferenceNodes.IndexOf(otherNode), -1 * impedance.Reciprocal);

                            }
                            */

                            if (component.Item1 is VoltageSource)
                            {
                                var source = (VoltageSource)component.Item1;
                                sourceAdmittance = source.getSourceImpedance(FreqPoint).Reciprocal;

                                voltage = source.getSourceVoltage(FreqPoint);

                                positiveTerminal = (component.Item2 == source.PositiveTerminal);
                                voltagePolarity = positiveTerminal ? 1 : -1;

                               // byte otherTerminal = positiveTerminal ? source.NegativeTerminal : source.PositiveTerminal;
                               // var otherNode = GetComponentTerminalNode(source, otherTerminal);
                                
                                /* if (otherNode != null) */
                                
                                 circuit.ConstValueMatrix.addAtCoordinate(nodeNumber, 0, voltage * sourceAdmittance * voltagePolarity);
                             //   MatrixDict[circuit].addAtCoordinate(nodeNumber, nodeNumber, sourceAdmittance);

                           //     if (otherNode != currentSection.ReferenceNode)
                            //         MatrixDict[circuit].addAtCoordinate(nodeNumber, circuit.NonReferenceNodes.IndexOf(otherNode), -1 * sourceAdmittance);
                                
                            }
                        }
                    }

                    try
                    {
                        circuit.VoltageResultMatrix = circuit.CurrentMatrix.Solve(circuit.ConstValueMatrix);
                        // ResultDict[circuit] = MatrixDict[circuit].Solve(CVDict[circuit]);
                    }
                    catch (Exception e)
                    {
                        return new Tuple<bool, string>(false, "Voltage Matrix solving failed: " + Environment.NewLine + e.ToString());
                    }
                }

                foreach (var nodeV in outputVoltageNodes)
                {
                    node2 = GetComponentTerminalNode(nodeV, 0);

                    if (node2 != null)
                    {
                        var circuit = GetCircuitForNode(node2);
                        nodeV.ResultVoltageCurve.addSample(FreqPoint, circuit.VoltageResultMatrix.Data[circuit.NonReferenceNodes.IndexOf(node2), 0]);
                    }
                }

                foreach (var nodeVD in outputVoltageDiff)
                {
                    nodePositive = GetComponentTerminalNode(nodeVD, 0);
                    nodeNegative = GetComponentTerminalNode(nodeVD, 1);

                    if ((nodePositive != null) && (nodeNegative != null))
                    {
                        var circPos = GetCircuitForNode(nodePositive);
                        var circNeg = GetCircuitForNode(nodeNegative);

                        var sectPos = GetSectionForNode(nodePositive);
                        var sectNeg = GetSectionForNode(nodeNegative);
                        

                        nodePositiveNumber = circPos.NonReferenceNodes.IndexOf(nodePositive);
                        nodeNegativeNumber = circNeg.NonReferenceNodes.IndexOf(nodeNegative);

                        voltPositive = (nodePositive != sectPos.ReferenceNode) ? circPos.VoltageResultMatrix.Data[nodePositiveNumber, 0] : 0;
                        voltNegative = (nodeNegative != sectNeg.ReferenceNode) ? circNeg.VoltageResultMatrix.Data[nodeNegativeNumber, 0] : 0;

                        totalVoltage = voltPositive - voltNegative;

                        nodeVD.ResultVoltageCurve.addSample(FreqPoint, totalVoltage);
                    }
                }

                foreach (var comp in OutputVoltagesComponents)
                {
                    nodePositive = GetComponentTerminalNode((SerializableComponent)comp, 0);
                    nodeNegative = GetComponentTerminalNode((SerializableComponent)comp, 1);

                    Circuit circuit;
                    CircuitSection section;

                    if (nodePositive != GroundNode)
                    {
                        circuit = GetCircuitForNode(nodePositive);
                        section = GetSectionForNode(nodePositive);
                    }
                    else
                    {
                        circuit = GetCircuitForNode(nodeNegative);
                        section = GetSectionForNode(nodeNegative);
                    }

                    if (nodePositive != null && nodeNegative != null)
                    {
                        nodePositiveNumber = circuit.NonReferenceNodes.IndexOf(nodePositive);
                        nodeNegativeNumber = circuit.NonReferenceNodes.IndexOf(nodeNegative);

                        voltPositive = (nodePositive != section.ReferenceNode) ? circuit.VoltageResultMatrix.Data[nodePositiveNumber, 0] : 0;
                        voltNegative = (nodeNegative != section.ReferenceNode) ? circuit.VoltageResultMatrix.Data[nodeNegativeNumber, 0] : 0;

                        totalVoltage = voltPositive - voltNegative;

                        comp.ResultVoltageCurve.addSample(FreqPoint, totalVoltage);

                    }
                }

                foreach (var comp in OutputCurrentsComponents)
                {
                    nodePositive = GetComponentTerminalNode((SerializableComponent)comp, 0);
                    nodeNegative = GetComponentTerminalNode((SerializableComponent)comp, 1);

                    Circuit circuit;
                    CircuitSection section;

                    if (nodePositive != GroundNode)
                    {
                        circuit = GetCircuitForNode(nodePositive);
                        section = GetSectionForNode(nodePositive);
                    }
                    else
                    {
                        circuit = GetCircuitForNode(nodeNegative);
                        section = GetSectionForNode(nodeNegative);
                    }

                    if (nodePositive != null && nodeNegative != null)
                    {
                        nodePositiveNumber = circuit.NonReferenceNodes.IndexOf(nodePositive);
                        nodeNegativeNumber = circuit.NonReferenceNodes.IndexOf(nodeNegative);

                        voltPositive = (nodePositive != section.ReferenceNode) ? circuit.VoltageResultMatrix.Data[nodePositiveNumber, 0] : 0;
                        voltNegative = (nodeNegative != section.ReferenceNode) ? circuit.VoltageResultMatrix.Data[nodeNegativeNumber, 0] : 0;

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
       // public bool isIsolatedFromOtherNodes;
        public bool IsGroundNode;

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
            if (IsGroundNode) return "Ground Node"; else
            return "Node " + NodeNumber.ToString();
        }
    }



    public class Circuit
    {
        public List<CircuitSection> Sections { get; } = new List<CircuitSection>();   

        public Matrix CurrentMatrix { get; set; }
        public Matrix ConstValueMatrix { get; set; }
        public Matrix VoltageResultMatrix { get; set; }

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
        public List<ElectricNode> NonReferenceNodes { get; } = new List<ElectricNode>();

        public bool IsReferenceNode(ElectricNode node)
        {
            return !NonReferenceNodes.Contains(node);
        }

        public override string ToString()
        {
            return "Circuit, Mode: " + this.CircuitMode.ToString() + ", " + Sections.Count.ToString() + " Sections, " + Nodes.Count.ToString() + " Nodes";
        }

        public bool HasVoltageSource => Nodes.Find(node => node.NonMeterComponents.Find(comp => comp is VoltageSource) != null) != null;
        
    }



    public class CircuitSection
    {
        public List<ElectricNode> Nodes { get; } = new List<ElectricNode>();
        public bool IsGrounded => Nodes.Where(node => node.IsGroundNode).Count() > 0;
        public ElectricNode GroundNode => Nodes.Find(node => node.IsGroundNode);
        
        public ElectricNode ReferenceNode => IsGrounded ? GroundNode : Nodes.First();

        public override string ToString()
        {
            return "Circuit section: " + Nodes.Count.ToString() + " nodes, " +
                (IsGrounded ? "Grounded" : "Floating");
        }

        

        public bool HasGroundReferenceVoltageOutput => Nodes.Where(node => node.HasGroundReferenceVoltageOutput).Count() > 0;


    }
    
}
