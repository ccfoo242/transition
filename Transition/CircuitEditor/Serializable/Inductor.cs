﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.Components;
using Transition.CircuitEditor.OnScreenComponents;

namespace Transition.CircuitEditor.Serializable
{
    public class Inductor : SerializableComponent
    {
        public override string ElementLetter => "L";
        
        private EngrNumber inductorValue;
        public EngrNumber InductorValue
        {
            get { return inductorValue; }
            set { SetProperty(ref inductorValue, value); }
        }

        private int inductorModel;
        public int InductorModel
        {
            get { return inductorModel; }
            set { SetProperty(ref inductorModel, value); }
        }
        
        private EngrNumber rs;
        public EngrNumber Rs
        {
            get { return rs; }
            set
            {
                SetProperty(ref rs, value);
                calculateFoQ();
            }
        }

        private EngrNumber cp;
        public EngrNumber Cp
        {
            get { return cp; }
            set
            {
                SetProperty(ref cp, value);
                calculateFoQ();
            }
        }

        private EngrNumber ew;
        public EngrNumber Ew
        {
            get { return ew; }
            set { SetProperty(ref ew, value); }
        }

        private EngrNumber fo;
        public EngrNumber Fo
        {
            get { return fo; }
            set
            {
                SetProperty(ref fo, value);
                calculateRsCp();
            }
        }

        private EngrNumber q;
        public EngrNumber Q
        {
            get { return q; }
            set
            {
                SetProperty(ref q, value);
                calculateRsCp();
            }
        }

        public override int QuantityOfTerminals { get => 2; set => throw new NotImplementedException(); }

        public Inductor()
        {
            InductorValue = EngrNumber.One;
            InductorModel = 0;
            
            SetProperty(ref rs, new EngrNumber(1, "p"), "Rs");
            SetProperty(ref cp, new EngrNumber(1, "p"), "Cp");
            calculateFoQ();

            ParametersControl = new InductorParametersControl(this);
            OnScreenComponent = new InductorScreen(this);

        }

        private void calculateFoQ()
        {
            double dL = inductorValue.ValueDouble;
            double dRs = Rs.ValueDouble;
            double dCp = Cp.ValueDouble;

            double dWop = Math.Sqrt(1 / (dL * dCp));

            double dQ = (dWop * dL) / dRs;
            double dFo = dWop / (2 * Math.PI);

            SetProperty(ref fo, dFo, "Fo");
            SetProperty(ref q, dQ, "Q");
            
        }

        private void calculateRsCp()
        {
            double dQ = Q.ValueDouble;
            double dFo = Fo.ValueDouble;
            double dL = inductorValue.ValueDouble;

            double dWo = 2 * Math.PI * dFo;

            double dRs = (dWo * dL) / dQ;
            double dCp = 1 / (dL * dWo * dWo);

            SetProperty(ref rs, dRs, "Rs");
            SetProperty(ref cp, dCp, "Cp");

        }

    }
}
