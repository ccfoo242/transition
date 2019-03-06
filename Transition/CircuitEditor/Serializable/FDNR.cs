﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Easycoustics.Transition.CircuitEditor.Components;
using Easycoustics.Transition.CircuitEditor.OnScreenComponents;
using Easycoustics.Transition.Common;
using Easycoustics.Transition.Functions;

namespace Easycoustics.Transition.CircuitEditor.Serializable
{
    public class FDNR : SerializableComponent, IPassive, IVoltageCurrentOutput
    {
        public override string ElementLetter => "D";
        public override string ElementType => "Frequency Dependent Negative Resistor";

        private bool outputVoltageAcross;
        private bool outputCurrentThrough;

        public bool OutputVoltageAcross
        {
            get => outputVoltageAcross;
            set { SetProperty(ref outputVoltageAcross, value); raiseLayoutChanged(); }
        }

        public bool OutputCurrentThrough
        {
            get => outputCurrentThrough;
            set { SetProperty(ref outputCurrentThrough, value); raiseLayoutChanged(); }
        }

        private decimal fdnrValue;
        public decimal FdnrValue
        {
            get { return fdnrValue; }
            set { SetProperty(ref fdnrValue, value);
                OnPropertyChanged("ValueString");
            }
        }

        private Precision componentPrecision;
        public Precision ComponentPrecision
        {
            get { return componentPrecision; }
            set
            {
                SetProperty(ref componentPrecision, value);
                OnPropertyChanged("ValueString");
            }
        }

        public bool AnyPrecisionSelected { get { return (ComponentPrecision == Precision.Arbitrary); } }

        public override byte QuantityOfTerminals { get => 2; set => throw new NotImplementedException(); }

        public FDNR() : base()
        {
            FdnrValue = 1m;
            
            ParametersControl = new FDNRParametersControl(this);
            OnScreenElement = new FDNRScreen(this);
        }


        public string ValueString
        {
            get
            {
                // this one sets de String for the component in the schematic window
                string returnString;
                var conv = new DecimalEngrConverter() { ShortString = false };
                var convShort = new DecimalEngrConverter() { ShortString = true };

                if (AnyPrecisionSelected)
                    returnString = (string)conv.Convert(FdnrValue, typeof(string), null, "");
                else
                    returnString = (string)convShort.Convert(FdnrValue, typeof(string), null, "");

                return returnString + "F²";
            }
        }

        public SampledFunction resultVoltageCurve { get; set; } = new SampledFunction() { FunctionQuantity = "Voltage", FunctionUnit = "Volt" };
        public SampledFunction resultCurrentCurve { get; set; } = new SampledFunction() { FunctionQuantity = "Current", FunctionUnit = "Amper" };

        public override void SetProperty(string property, object value)
        {
            base.SetProperty(property, value);

            switch (property)
            {
                case "FdnrValue": FdnrValue = (decimal)value; break;
                case "ComponentPrecision": ComponentPrecision = (Precision)value; break;

                case "OutputVoltageAcross": OutputVoltageAcross = (bool)value; break;
                case "OutputCurrentThrough": OutputCurrentThrough = (bool)value; break;

            }

            resultVoltageCurve.Title = "Voltage Across FDNR " + ElementName;
            resultCurrentCurve.Title = "Current Through FDNR " + ElementName;
        }

        public ComplexDecimal getImpedance(decimal frequency)
        {
            /* w is angular frequency
             w= 2 * Pi * F 
             */
            decimal w = 2 * DecimalMath.Pi * frequency;
            return -1 / (w * w * FdnrValue);
        }

        List<Tuple<byte, byte, ComplexDecimal>> IPassive.getImpedance(decimal frequency)
        {
            var output = new List<Tuple<byte, byte, ComplexDecimal>>();
            output.Add(new Tuple<byte, byte, ComplexDecimal>(0, 1, getImpedance(frequency)));

            return output;
        }
    }
}
