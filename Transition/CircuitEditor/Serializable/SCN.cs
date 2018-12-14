using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.ParametersControls;

namespace Transition.CircuitEditor.Serializable
{
    public class SCN : SerializableComponent
    {
        public override string ElementLetter => "Q";
        public override string ElementType => "SCN";

        public override byte QuantityOfTerminals
            { get => 2; set => throw new NotImplementedException(); }

        private bool positivePolarity;
        public bool PositivePolarity { get => positivePolarity;
            set { SetProperty(ref positivePolarity, value);
                raiseLayoutChanged();
                OnPropertyChanged("PolarityString");
            }
        }

        private EngrNumber r;
        public EngrNumber R { get => r; set {
                SetProperty(ref r, value);
                reCalculateC();
                OnPropertyChanged("ResistanceString");
                raiseLayoutChanged();
            } }

        private EngrNumber c;
        public EngrNumber C { get => c; set {
                SetProperty(ref c, value);
                reCalculateFs();
            } }

        private EngrNumber fs;
        public EngrNumber Fs { get => fs; set {
                SetProperty(ref fs, value);
                reCalculateC();
            } }

        public string PolarityString { get => PositivePolarity ? "+SCN" : "-SCN"; }
        public string ResistanceString { get => R.ToString(); }

        public SCN() : base()
        {
            r = "1K";
            c = "1n";
            fs = 1 / (r * c);

            PositivePolarity = true;

            ParametersControl = new SCNParametersControl(this);
            OnScreenElement = new OnScreenComponents.SCNScreen(this);
        }

        private void reCalculateC()
        {
            EngrNumber newC = 1 / (R * Fs);
            SetProperty(ref c, newC, "C");
        }

        private void reCalculateFs()
        {
            EngrNumber newFs = 1 / (R * C);
            SetProperty(ref fs, newFs, "Fs");
        }

        public override void SetProperty(string property, object value)
        {
            base.SetProperty(property, value);

            switch (property)
            {
                case "R": R = (EngrNumber)value; break;
                case "C": C = (EngrNumber)value; break;
                case "Fs": Fs = (EngrNumber)value; break;
                case "PositivePolarity": PositivePolarity = (bool)value; break;
            }
        }
    }
}
