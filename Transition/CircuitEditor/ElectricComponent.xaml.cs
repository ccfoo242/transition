﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Transition.CircuitEditor
{
    public sealed partial class ElectricComponent : UserControl, IElectricElement
    {
        /*
          public IComponentParameterControl parameters { get; set; }

          public String elementName
          {
              get
              {
                  if (parameters != null)
                      return parameters.ComponentName;
                  else return "";
              }
          }

          public ElectricComponent()
          {
              this.InitializeComponent();
          }

          public ElectricComponent(String tipoElto)
          {
              InitializeComponent();

              grd.RenderTransform = new CompositeTransform();
              labelsgrd.RenderTransform = new CompositeTransform();

              parameters = new Components.ResistorParametersControl();

              switch (tipoElto)
              {
                  case "resistor":
                      parameters = new Components.ResistorParametersControl();
                      break;

                  case "capacitor":
                      parameters = new Components.CapacitorParametersControl();
                      break;

                  case "inductor":
                      parameters = new Components.InductorParametersControl();
                      break;

                  case "ground":
                      parameters = new Components.GroundParameters();
                      break;

                  case "fdnr":
                      parameters = new Components.FDNRParametersControl();
                      break;

                  case "potentiometer":
                      parameters = new Components.PotentiometerParametersControl();
                      break;

                  case "transformer":
                      parameters = new Components.TransformerParametersControl();
                      break;

                  case "generator":
                      parameters = new Components.VoltageSourceComponentParameters();
                      break;

                  case "impedance":
                      grd.Width = 140;
                      grd.Height = 80;
                      break;

                  case "scn":
                      grd.Width = 140;
                      grd.Height = 80;
                      break;

                  case "opamp":
                      grd.Width = 180;
                      grd.Height = 200;
                      break;

                  case "transferfunction":
                      grd.Width = 180;
                      grd.Height = 200;
                      break;

                  case "summer":
                      grd.Width = 160;
                      grd.Height = 160;
                      break;

                  case "buffer":
                      grd.Width = 140;
                      grd.Height = 120;
                      break;

                  case "speaker":
                      grd.Width = 120;
                      grd.Height = 140;
                      break;

                  default:
                      break;
              }

              cont.ContentTemplate = (DataTemplate)((UserControl)parameters).Resources["SchematicComponentLayout"];
              cont.DataContext = parameters;

              this.Height = parameters.SchematicHeight;
              this.Width = parameters.SchematicWidth;
              grd.Height = this.Height;
              grd.Width = this.Width;

              CompositeTransform t = (CompositeTransform)grd.RenderTransform;
              t.CenterX = grd.Width / 2;
              t.CenterY = grd.Height / 2;

              parameters.ComponentName = parameters.ComponentLetter + ce.getNextNumberLetter(parameters.ComponentLetter);
              labelsgrd.Children.Add(parameters.CnvLabels);
          }

          public bool isInside(Rectangle rect)
          {
              if (rect == null) return false;

              double x1 = Canvas.GetLeft(rect);
              double y1 = Canvas.GetTop(rect);
              double x2 = Canvas.GetLeft(rect) + rect.Width;
              double y2 = Canvas.GetTop(rect) + rect.Height;

              if ((x1 < Canvas.GetLeft(this)) &&
                  (x2 > Canvas.GetLeft(this)) &&
                  (y1 < Canvas.GetTop(this)) &&
                  (y2 > Canvas.GetTop(this)))
                  return true;
              else
                  return false;
          }


          public void moveRelative(Point point)
          {
              Canvas.SetLeft(this, Statics.round20(PressedPoint.X - point.X));
              Canvas.SetTop(this, Statics.round20(PressedPoint.Y - point.Y));
          }




          private void rightTap(object sender, RightTappedRoutedEventArgs e)
          {
              CircuitEditor.currentInstance.showParameters(this);
          }

          public void Rotate()
          {
              CompositeTransform t = (CompositeTransform)grd.RenderTransform;

              t.Rotation += 90;
              parameters.setRotation(t.Rotation);

          }

          public void FlipX()
          {
              CompositeTransform t = (CompositeTransform)grd.RenderTransform;

              t.ScaleX *= -1;
              parameters.setFlipX(t.ScaleX == -1);
          }

          public void FlipY()
          {
              CompositeTransform t = (CompositeTransform)grd.RenderTransform;

              t.ScaleY *= -1;
              parameters.setFlipY(t.ScaleY == -1);
          }
          */
        public void deselected()
        {
            throw new NotImplementedException();
        }

        public bool isInside(Rectangle rect)
        {
            throw new NotImplementedException();
        }

        public void moveRelative(Point point)
        {
            throw new NotImplementedException();
        }

        public void selected()
        {
            throw new NotImplementedException();
        }

        public void updateOriginPoint()
        {
            throw new NotImplementedException();
        }

        private void Element_PointerPressed(object sender, PointerRoutedEventArgs e)
        {

        }

        private void rightTap(object sender, RightTappedRoutedEventArgs e)
        {

        }
    }
}
