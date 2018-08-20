using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Transition.CircuitEditor.Components
{
    public sealed partial class VoltageSource : UserControl, IComponentParameter, INotifyPropertyChanged
    {
       
        public double SchematicWidth  { get { return 120; } }
        public double SchematicHeight { get { return 120; } }


        private String componentName;
        public String ComponentName
        {
            get { return componentName; }
            set
            {
                componentName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ComponentName"));
            }
        }

        public string ComponentLetter { get { return "V"; } }

        public Canvas CnvLabels { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private bool flipX;
        private bool flipY;
        private double actualRotation;

        public VoltageSource()
        {
            this.InitializeComponent();
            init();
        }

        public void init()
        {
            CnvLabels = new Canvas();
        }

        public void setFlipX(bool flip)
        {
            flipX = flip;
            setPositionTextBoxes();
        }

        public void setFlipY(bool flip)
        {
            flipY = flip;
            setPositionTextBoxes();
        }

        public void setRotation(double rotation)
        {
            actualRotation = rotation % 360;
            setPositionTextBoxes();
        }

        public void setPositionTextBoxes()
        {


        }
    }
}
