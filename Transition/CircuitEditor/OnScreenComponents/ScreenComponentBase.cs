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
using Windows.UI.Xaml.Media;

namespace Transition.CircuitEditor.OnScreenComponents
{

    public abstract class ScreenComponentBase : Canvas
    {
        public Canvas ComponentCanvas { get; }
        public CompositeTransform ComponentTransform { get; }

        public SerializableComponent SerializableComponent { get; }

        public abstract double SchematicWidth { get; }
        public abstract double SchematicHeight { get; }

        public double ActualRotation { get { return SerializableComponent.Rotation; } }
        public bool FlipX { get { return SerializableComponent.FlipX; } }
        public bool FlipY { get { return SerializableComponent.FlipY; } }

        public abstract void setPositionTextBoxes();
        public abstract int[,] TerminalPositions { get; }

        public ScreenComponentBase(SerializableComponent component) : base()
        {
            SerializableComponent = component;

            ComponentTransform = new CompositeTransform();
            ComponentTransform.CenterX = SchematicWidth / 2;
            ComponentTransform.CenterY = SchematicHeight / 2;

            ComponentCanvas = new Canvas()
            {
                IsTapEnabled = true,
                Width = SchematicWidth,
                Height = SchematicHeight,
                RenderTransform = ComponentTransform
            };
            
            Children.Add(ComponentCanvas);
            
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
