﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Text;
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
    public sealed partial class FDNRParametersControl : UserControl, IComponentParameterControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public double SchematicWidth { get { return 120; } }
        public double SchematicHeight { get { return 80; } }
        public String ComponentLetter { get { return "D"; } }

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

        private EngrNumber fdnrValue;
        public EngrNumber FdnrValue
        {
            get { return fdnrValue; }
            set
            {
                fdnrValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FdnrValue"));
            }
        }

        public TextBlock txtComponentName;
        public TextBlock txtFdnrValue;

        public Canvas CnvLabels { get; set; }

        private double actualRotation;

        public FDNRParametersControl()
        {
            this.InitializeComponent();
            init();
        }

        private void init()
        {
            componentValueBox.ComponentValue = EngrNumber.One;

            CnvLabels = new Canvas();

            txtComponentName = new TextBlock() { FontWeight = FontWeights.ExtraBold };
            Binding b1 = new Binding()
            {
                Path = new PropertyPath("ComponentName"),
                Source = this,
                Mode = BindingMode.OneWay
            };
            txtComponentName.SetBinding(TextBlock.TextProperty, b1);
            txtComponentName.RenderTransform = new TranslateTransform() { };
            txtComponentName.SizeChanged += delegate { setPositionTextBoxes(); };
            CnvLabels.Children.Add(txtComponentName);


            txtFdnrValue = new TextBlock() { FontWeight = FontWeights.ExtraBold };
            Binding b2 = new Binding()
            {
                Path = new PropertyPath("ValueString"),
                Source = componentValueBox,
                Mode = BindingMode.OneWay
            };
            txtFdnrValue.SetBinding(TextBlock.TextProperty, b2);
            txtFdnrValue.RenderTransform = new TranslateTransform() { };
            txtFdnrValue.SizeChanged += delegate { setPositionTextBoxes(); };
            CnvLabels.Children.Add(txtFdnrValue);

            setPositionTextBoxes();
        }

        private void setPositionTextBoxes()
        {
            double leftRV; double topRV;
            double leftCN; double topCN;

            if ((actualRotation == 0) || (actualRotation == 180))
            {
                topCN = 0;
                leftCN = (SchematicWidth / 2) - (txtComponentName.ActualWidth / 2);
                topRV = 60;
                leftRV = (SchematicWidth / 2) - (txtFdnrValue.ActualWidth / 2);
            }
            else
            {
                topCN = (SchematicHeight / 2) - (txtComponentName.ActualHeight / 2);
                leftCN = 40 - (txtComponentName.ActualWidth);
                topRV = (SchematicHeight / 2) - (txtFdnrValue.ActualHeight / 2);
                leftRV = 80;
            }

            ((TranslateTransform)txtComponentName.RenderTransform).X = leftCN;
            ((TranslateTransform)txtComponentName.RenderTransform).Y = topCN;

            ((TranslateTransform)txtFdnrValue.RenderTransform).X = leftRV;
            ((TranslateTransform)txtFdnrValue.RenderTransform).Y = topRV;
        }

        public void setRotation(double rotation)
        {
            rotation = rotation % 360;
          
            actualRotation = rotation;
            setPositionTextBoxes();
        }

        private void changeD(object sender, PropertyChangedEventArgs e)
        {
            FdnrValue = componentValueBox.ComponentValue;
        }


        public void setFlipX(bool flip)
        {
        }

        public void setFlipY(bool flip)
        {
        }
    }
}