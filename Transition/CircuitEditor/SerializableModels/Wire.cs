using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.Serializable;

namespace Transition.CircuitEditor.SerializableModels
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

        public override string ComponentLetter => "W";

        public SerializableElement Bind1 { get; set; }
        public byte BoundedTerminal1 { get; set; }

        public SerializableElement Bind2 { get; set; }
        public byte BoundedTerminal2 { get; set; }

        public bool IsBounded1 { get { return Bind1 != null; } }
        public bool IsBounded2 { get { return Bind2 != null; } }

        public Wire()
        {
       
        }
    }
}
