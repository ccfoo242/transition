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

        public delegate void EmptyDelegate();
        public event ElementDelegate ElementDeleted;

        public delegate void ElementDelegate(SerializableElement el);
        public event ElementDelegate LayoutChanged;

        
        public virtual void SetProperty(string property, object value)
        {
            switch (property)
            {
                case "ElementName": ElementName = (string)value; break;
            }
        }

        public void deletedElement()
        {
            ElementDeleted?.Invoke(this);
        }

        public void raiseLayoutChanged()
        {
            LayoutChanged?.Invoke(this);
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
            set {
                double correctedValue = value;

                while ((correctedValue < 0) || (correctedValue >= 360))
                {
                    if (correctedValue < 0) correctedValue += 360;
                    if (correctedValue >= 360) correctedValue -= 360;
                }

                SetProperty(ref rotation, correctedValue);
           //     ComponentLayoutChanged?.Invoke();
                raiseLayoutChanged();
            } }

        private bool flipX;
        public bool FlipX { get { return flipX; }
            set { SetProperty(ref flipX, value);
          //      ComponentLayoutChanged?.Invoke();
                raiseLayoutChanged();
            } }

        private bool flipY;
        public bool FlipY { get { return flipY; }
            set { SetProperty(ref flipY, value);
           //     ComponentLayoutChanged?.Invoke();
                raiseLayoutChanged();
            } }

        private Point2D componentPosition;
        public Point2D ComponentPosition
        {
            get { return componentPosition; }
            set
            {
                SetProperty(ref componentPosition, value);
                raiseLayoutChanged();
            }
        }
        /*
        private double positionX;
        public double PositionX { get { return positionX; }
            set { SetProperty(ref positionX, value);
           //     ComponentPositionChanged?.Invoke();
                raiseLayoutChanged();
            } }

        private double positionY;
        public double PositionY { get { return positionY; }
            set { SetProperty(ref positionY, value);
            //    ComponentPositionChanged?.Invoke();
                raiseLayoutChanged();
            } }
            */

     //   public delegate void ComponentLayoutChangedHandler();
     //   public event ComponentLayoutChangedHandler ComponentLayoutChanged;

      //  public delegate void ComponentPositionChangedHandler();
      //  public event ComponentPositionChangedHandler ComponentPositionChanged;

        // ParametersControl is the UI Control that allows user to
        // configure the component parameters
        public UserControl ParametersControl { get; set; }

        // OnScreenComponent is the class that has responsability
        // for showing the component on screen of the CircuitEditor
        // it manages position of textboxes like component values, names
        // or other parameters.
        // it is a Canvas that must be added to the canvas of CircuitEditor
        // 
        
        public override void SetProperty(string property, object value)
        {
            base.SetProperty(property, value);

            switch (property)
            {
                case "Rotation":  Rotation = (double)value; break;
                case "FlipX":     FlipX = (bool)value; break;
                case "FlipY":     FlipY = (bool)value; break;
                case "Position":  ComponentPosition = (Point2D)value; break;
                case "QuantityOfTerminals":
                                  QuantityOfTerminals = (byte)value; break;
            }
        }
        
    }
}
