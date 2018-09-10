using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.Serializable;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;

namespace Transition.CircuitEditor.OnScreenComponents
{
    public abstract class ScreenElementBase : FrameworkElement
    {

    }

    public abstract class ScreenComponentBase : ScreenElementBase
    {
        public Canvas ComponentCanvas { get; }
        public SerializableComponent SerializableComponent { get; }

        public double SchematicWidth { get; set; }
        public double SchematicHeight { get; set; }
        
        public double ActualRotation { get { return SerializableComponent.Rotation; } }
        public bool FlipX { get { return SerializableComponent.FlipX; } } 
        public bool FlipY { get { return SerializableComponent.FlipY; } }

        public ScreenComponentBase(SerializableComponent component)
        {
            SerializableComponent = component;

            ComponentCanvas = new Canvas()
                { IsTapEnabled = true };

            ComponentCanvas.PointerPressed += Element_PointerPressed;
            DataContext = SerializableComponent;

        }

        protected void postConstruct()
        {
            ComponentCanvas.Width = SchematicWidth;
            ComponentCanvas.Height = SchematicHeight;
        }

        private void Element_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                var prop = e.GetCurrentPoint(ComponentCanvas).Properties;
                if (prop.IsRightButtonPressed) return;
                if (prop.IsMiddleButtonPressed) return;
            }
            CircuitEditor.currentInstance.clickElement(this.SerializableComponent);
        }
        
    }
}
