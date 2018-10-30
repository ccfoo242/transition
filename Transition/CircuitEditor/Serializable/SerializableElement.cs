using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.OnScreenComponents;
using Transition.Common;
using Windows.UI.Xaml.Controls;

namespace Transition.CircuitEditor.Serializable
{
    public enum Precision { Arbitrary, p05, p1, p2, p5, p10, p20, p50 }

    public abstract class SerializableElement : BindableBase
    {
        private string elementName = "";
        public string ElementName
        {
            get { return elementName; }
            set { SetProperty(ref elementName, value); }
        }

        public abstract string ElementLetter { get; }
        public abstract string ElementType { get; }
        public abstract byte QuantityOfTerminals { get; set; }
        public ScreenComponentBase OnScreenComponent { get; set; }

        public delegate void ElementDeletedHandler();
        public event ElementDeletedHandler ElementDeleted;
        public event ElementDeletedHandler UnBindElement;

        public virtual void SetProperty(string property, object value)
        {
            switch (property)
            {
                case "ElementName": ElementName = (string)value;break;
            }
        }

        public void deletedElement()
        {
            ElementDeleted?.Invoke();
        }

        public void unbindElement()
        {
            ElementDeleted?.Invoke();
        }

        public override string ToString()
        {
            return "Element: " + ElementName;
        }

    }

    public abstract class SerializableComponent : SerializableElement
    {
        private double rotation;
        public double Rotation { get { return rotation; }
            set { SetProperty(ref rotation, value);
                ComponentLayoutChanged?.Invoke();
            } }

        private bool flipX;
        public bool FlipX { get { return flipX; }
            set { SetProperty(ref flipX, value);
                ComponentLayoutChanged?.Invoke();
            } }

        private bool flipY;
        public bool FlipY { get { return flipY; }
            set { SetProperty(ref flipY, value);
                ComponentLayoutChanged?.Invoke();
            } }

        private double positionX;
        public double PositionX { get { return positionX; }
            set { SetProperty(ref positionX, value);
                ComponentPositionChanged?.Invoke();
            } }

        private double positionY;
        public double PositionY { get { return positionY; }
            set { SetProperty(ref positionY, value);
                ComponentPositionChanged?.Invoke();
            } }

        public delegate void ComponentLayoutChangedHandler();
        public event ComponentLayoutChangedHandler ComponentLayoutChanged;

        public delegate void ComponentPositionChangedHandler();
        public event ComponentPositionChangedHandler ComponentPositionChanged;

        // ParametersControl is the UI Control that allows user to
        // configure the component parameters
        public UserControl ParametersControl { get; set; }

        // OnScreenComponent is the class that has responsability
        // for showing the component on screen of the CircuitEditor
        // it manages position of textboxes like component values, names
        // or other parameters.
        // it is a Canvas that must be added to the canvas of CircuitEditor
        // 

        public void rotate()
        {
            Rotation += 90;
            if (Rotation == 360) Rotation = 0;
        }

        public void doFlipX()
        {
            FlipX ^= true;
        }

        public void doFlipY()
        {
            FlipY ^= true;
        }

        public override void SetProperty(string property, object value)
        {
            base.SetProperty(property, value);

            switch (property)
            {
                case "Rotation":  Rotation = (double)value; break;
                case "FlipX":     FlipX = (bool)value; break;
                case "FlipY":     FlipY = (bool)value; break;
                case "PositionX": PositionX = (double)value; break;
                case "PositionY": PositionY = (double)value; break;
                case "QuantityOfTerminals":
                                  QuantityOfTerminals = (byte)value; break;
            }
        }
        
    }
}
