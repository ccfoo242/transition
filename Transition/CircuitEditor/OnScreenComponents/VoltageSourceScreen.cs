﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.Serializable;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Transition.CircuitEditor.OnScreenComponents
{
    public class VoltageSourceScreen : ScreenComponentBase
    {
        public override double SchematicWidth => 120;
        public override double SchematicHeight => 120;
        public override int[,] TerminalPositions
        {
            get => new int[,] { { 60, 20 }, { 60, 100 } };
        }
        public TextBlock txtComponentName;
        public TextBlock txtVoltage;
        public TextBlock txtImpedance;

        public ContentControl SymbolGenerator { get; }
  
        public VoltageSourceScreen(VoltageSource vs) : base(vs)
        {
            SymbolGenerator = new ContentControl()
            {
                ContentTemplate = (DataTemplate)Application.Current.Resources["symbolGenerator"]
            };
            ComponentCanvas.Children.Add(SymbolGenerator);
            Canvas.SetTop(SymbolGenerator, 19);
            Canvas.SetLeft(SymbolGenerator, 19);

            txtComponentName = new TextBlock() { FontWeight = FontWeights.ExtraBold };
            Binding b1 = new Binding()
            {
                Path = new PropertyPath("ElementName"),
                Mode = BindingMode.OneWay
            };
            txtComponentName.SetBinding(TextBlock.TextProperty, b1);
            txtComponentName.RenderTransform = new TranslateTransform() { };
            txtComponentName.SizeChanged += delegate { setPositionTextBoxes(SerializableComponent); };
            Children.Add(txtComponentName);

            txtVoltage = new TextBlock()
            {

            };
            Children.Add(txtVoltage);

            txtImpedance = new TextBlock()
            {

            };
            Children.Add(txtImpedance);

            postConstruct();
        }

        public override void setPositionTextBoxes(SerializableElement el)
        {
            
        }


    }
}
