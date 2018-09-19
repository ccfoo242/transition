using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Transition.Design;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Transition
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            UserDesign newDesign = new UserDesign();

            circuitEditor.loadDesign(newDesign);
        }

        private void dragElement(UIElement sender, DragStartingEventArgs args)
        {

            Border b = (Border)sender;
            String element = (String)b.Tag;

            args.DragUI.SetContentFromDataPackage();
            args.Data.RequestedOperation = DataPackageOperation.Move;
            args.Data.SetText(element);
        }

        private void tapElement(object sender, TappedRoutedEventArgs e)
        {

            Border bd = (Border)sender;
            String element = (String)bd.Tag;

            circuitEditor.addElement(element);
        }
        
    }
}
