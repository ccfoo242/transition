using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.OnScreenComponents;
using Transition.Common;

namespace Transition.CircuitEditor.Serializable
{
    public class SerializableComponent : BindableBase
    {
        private int rotation;
        public int Rotation { get { return rotation; }
            set { SetProperty(ref rotation, value); } }

        private bool flipX;
        public bool FlipX { get { return flipX; }
            set { SetProperty(ref flipX, value); } }

        private bool flipY;
        public bool FlipY { get { return flipY; }
            set { SetProperty(ref flipY, value); } }

        private int positionX;
        public int PositionX { get { return positionX; }
            set { SetProperty(ref positionX, value); } }

        private int positionY;
        public int PositionY { get { return positionY; }
            set { SetProperty(ref positionY, value); } }

        private string componentName;
        public string ComponentName { get { return componentName; }
            set { SetProperty(ref componentName, value); } }

        public IComponentParameterControl ParametersControl { get; set; }
        public ScreenComponentBase OnScreenComponent { get; set; }

    }
}
