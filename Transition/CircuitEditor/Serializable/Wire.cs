using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.OnScreenComponents;

namespace Transition.CircuitEditor.Serializable
{
    public class Wire : SerializableElement
    {
        private double x1;
        public double X1 {
            get { return x1; }
            set { SetProperty(ref x1, value); } }

        private double y1;
        public double Y1 {
            get { return y1; }
            set { SetProperty(ref y1, value); } }

        private double x2;
        public double X2 {
            get { return x2; }
            set { SetProperty(ref x2, value); } }

        private double y2;
        public double Y2 {
            get { return y2; }
            set { SetProperty(ref y2, value); } }

        public override string ElementLetter => "W";

        public override int QuantityOfTerminals { get => 2; set => throw new NotImplementedException(); }

        public SerializableElement BoundedObject1 { get; set; }
        public byte BoundedTerminal1 { get; set; }

        public SerializableElement BoundedObject2 { get; set; }
        public byte BoundedTerminal2 { get; set; }

        public bool IsBounded1 { get { return BoundedObject1 != null; } }
        public bool IsBounded2 { get { return BoundedObject2 != null; } }

        public WireScreen OnScreenWire { get; }

        public Wire()
        {
            OnScreenWire = new WireScreen(this);
        }
    }
}
