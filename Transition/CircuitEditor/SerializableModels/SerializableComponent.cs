﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.OnScreenComponents;
using Transition.Common;
using Windows.UI.Xaml.Controls;

namespace Transition.CircuitEditor.Serializable
{
    public abstract class SerializableComponent : BindableBase
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

        // ParametersControl is the UI Control that allows user to
        // configure the component parameters
        public UserControl ParametersControl { get; set; }

        // OnScreenComponent is the class that has responsability
        // for showing the component on screen of the CircuitEditor
        // it manages position of textboxes like component values, names
        // or other parameters.
        // it has a Canvas that must be added to the canvas of CircuitEditor
        // 
        public ScreenComponentBase OnScreenComponent { get; set; }

        public abstract string ComponentLetter { get; }
    }
}
