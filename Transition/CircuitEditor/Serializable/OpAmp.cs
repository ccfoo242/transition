using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Easycoustics.Transition.CircuitEditor.ParametersControls;
using Easycoustics.Transition.Common;

namespace Easycoustics.Transition.CircuitEditor.Serializable
{
    public class OpAmp : SerializableComponent, IPassive
    {
        public override string ElementLetter => "U";
        public override string ElementType => "OpAmp";

        private decimal dcGain;
        public decimal DcGain { get => dcGain;
            set { SetProperty(ref dcGain, value); }
        }

        private decimal phaseMargin;
        public decimal PhaseMargin { get => phaseMargin;
            set { SetProperty(ref phaseMargin, value); }
        }

        private decimal gainBandwidth;
        public decimal GainBandwidth { get => gainBandwidth;
            set { SetProperty(ref gainBandwidth, value); }
        }

        private decimal rIn;
        public decimal RIn { get => rIn;
            set { SetProperty(ref rIn, value); }
        }

        private decimal rOut;
        public decimal ROut { get => rOut;
            set { SetProperty(ref rOut, value); }
        }

        private string modelName;
        public string ModelName { get => modelName;
            set { SetProperty(ref modelName, value);
                raiseLayoutChanged();
            }
        }

        private string description;
        public string Description { get => description;
            set { SetProperty(ref description, value); }
        }

        public override byte QuantityOfTerminals { get => 3; set => throw new NotImplementedException(); }

        public OpAmp() : base()
        {
            GainBandwidth = 1e6m;
            DcGain = 100;
            PhaseMargin = 45;
            RIn = 100e3m;
            ROut = 100;
            ModelName = "Default";
            Description = "Default OpAmp";

            ParametersControl = new OpAmpParametersControl(this);
            OnScreenElement = new OnScreenComponents.OpAmpScreen(this);

        }

        public override void SetProperty(string property, object value)
        {
            base.SetProperty(property, value);

            switch (property)
            {
                case "GainBandwidth" : GainBandwidth = (decimal)value; break;
                case "DcGain"        : DcGain = (decimal)value; break;
                case "PhaseMargin"   : PhaseMargin = (decimal)value; break;
                case "RIn"           : RIn = (decimal)value; break;
                case "ROut"          : ROut = (decimal)value; break;
                case "ModelName"     : ModelName = (string)value; break;
                case "Description"   : Description = (string)value; break;
            }
        }

        public List<Tuple<byte, byte, ComplexDecimal>> getImpedance(decimal frequency)
        {
            var output = new List<Tuple<byte, byte, ComplexDecimal>>();
            output.Add(new Tuple<byte, byte, ComplexDecimal>(0, 1, RIn));

            return output;
        }
    }
}
