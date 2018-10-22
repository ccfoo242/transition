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
            CircuitEditor.CircuitEditor ce = new CircuitEditor.CircuitEditor();

            ce.loadDesign(newDesign);
            
        }

        private void NavItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            string ItemContent = args.InvokedItem as string;

            if (ItemContent != null)
            {
                switch (ItemContent)
                {
                    case "Design":
                        ContentFrame.Navigate(typeof(Pages.DesignPage));
                        break;
                    case "Library":
                        ContentFrame.Navigate(typeof(Pages.Library));
                        break;
                    case "Settings":
                        ContentFrame.Navigate(typeof(Pages.Settings));
                        break;
                }
            }    
        }

        private void NavLoaded(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(Pages.DesignPage));
        }
    }
}
