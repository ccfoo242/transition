using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.ParametersControls;

namespace Transition.CircuitEditor.Serializable
{
    public class OpAmp : SerializableComponent
    {
        public override string ElementLetter => "U";
        public override string ElementType => "OpAmp";

        private EngrNumber dcGain;
        public EngrNumber DcGain { get => dcGain;
            set { SetProperty(ref dcGain, value); }
        }

        private EngrNumber phaseMargin;
        public EngrNumber PhaseMargin { get => phaseMargin;
            set { SetProperty(ref phaseMargin, value); }
        }

        private EngrNumber gainBandwidth;
        public EngrNumber GainBandwidth { get => gainBandwidth;
            set { SetProperty(ref gainBandwidth, value); }
        }

        private EngrNumber rIn;
        public EngrNumber RIn { get => rIn;
            set { SetProperty(ref rIn, value); }
        }

        private EngrNumber rOut;
        public EngrNumber ROut { get => rOut;
            set { SetProperty(ref rOut, value); }
        }

        private string modelName;
        public string ModelName { get => modelName;
            set { SetProperty(ref modelName, value); }
        }

        private string description;
        public string Description { get => description;
            set { SetProperty(ref description, value); }
        }

        public override byte QuantityOfTerminals { get => 3; set => throw new NotImplementedException(); }

        public OpAmp() : base()
        {
            GainBandwidth = "10M";
            DcGain = 100;
            PhaseMargin = 45;
            RIn = "100K";
            ROut = 100;
            ModelName = "Default";
            Description = "Default OpAmp";

            ParametersControl = new OpAmpParametersControl(this);
            OnScreenElement = new OnScreenComponents.OpAmpScreen(this);

        }
    }
}
