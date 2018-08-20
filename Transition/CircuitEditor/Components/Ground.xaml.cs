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
    public sealed partial class Ground : UserControl, IComponentParameter, INotifyPropertyChanged
    {
        public Ground()
        {
            this.InitializeComponent();
            CnvLabels = new Canvas();
        }

        public double SchematicWidth { get { return 80; } }
        public double SchematicHeight { get { return 80; } }

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
        public string ComponentLetter { get { return "G"; } }

        public Canvas CnvLabels { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void setFlipX(bool flip)
        {
        }

        public void setFlipY(bool flip)
        {
        }

        public void setRotation(double rotation)
        {
        }
    }
}
