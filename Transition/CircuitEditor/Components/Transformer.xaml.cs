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
    public sealed partial class Transformer : UserControl, IComponentParameter, INotifyPropertyChanged
    {

        public double SchematicWidth { get { return 120; } }
        public double SchematicHeight { get { return 140; } }

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

        public double actualRotation = 0;

        public string ComponentLetter { get { return "T"; } }
        public Canvas CnvLabels { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public TextBlock txtPri;
        public TextBlock txtSec;
        public TextBlock txtCN;
        public TextBlock txtTR;

        public EngrNumber TurnsRatio
        {
            get { return turnsRatio; }
            set
            {
                turnsRatio = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TurnsRatio"));
            }
        }
        private EngrNumber turnsRatio;

        public EngrNumber Lpri
        {
            get { return lpri; }
            set
            {
                lpri = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Lpri"));
            }
        }
        private EngrNumber lpri;

        public EngrNumber Lsec
        {
            get { return lsec; }
            set
            {
                lsec = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Lsec"));
            }
        }
        private EngrNumber lsec;

        public EngrNumber MutualL
        {
            get { return mutualL; }
            set
            {
                mutualL = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MutualL"));
            }
        }
        private EngrNumber mutualL;

        public EngrNumber LpLeak
        {
            get { return lpLeak; }
            set
            {
                lpLeak = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LpLeak"));
            }
        }
        private EngrNumber lpLeak;

        public EngrNumber LsLeak
        {
            get { return lsLeak; }
            set
            {
                lsLeak = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LsLeak"));
            }
        }
        private EngrNumber lsLeak;


        public Transformer()
        {
            this.InitializeComponent();
            init();
        }

        private void init()
        {
            CnvLabels = new Canvas();
        }

        public void setFlipX(bool flip)
        {
        }

        public void setFlipY(bool flip)
        {
        }

        public void setPositionTextBoxes(double rotation)
        {
            actualRotation = rotation % 360;
            setPositionTextBoxes();
        }

        public void setPositionTextBoxes()
        {

        }

        private void changeTR(object sender, PropertyChangedEventArgs e)
        {

        }

        private void changeLpri(object sender, PropertyChangedEventArgs e)
        {

        }


        private void changeLsec(object sender, PropertyChangedEventArgs e)
        {

        }

        private void changeKc(object sender, PropertyChangedEventArgs e)
        {

        }
    }
}
