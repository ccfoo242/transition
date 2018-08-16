using System;
using System.Collections.Generic;
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

namespace Transition.CustomControls
{
    public sealed partial class GroupBox : UserControl
    {
        public GroupBox()
        {
            this.InitializeComponent();
        }


        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty =
             DependencyProperty.Register("Header", typeof(string), typeof(GroupBox), new PropertyMetadata("Your Header", HeaderPropertyChangedCallback));

        public static void HeaderPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                (d as GroupBox).HeaderTitle.Text = e.NewValue?.ToString();
                //(d as GroupBox).border.Margin = new Thickness((d as GroupBox).HeaderTitle.ActualWidth, 10, 3, 3);
            }
        }

        public object CustomContent
        {
            get { return (object)GetValue(CustomContentProperty); }
            set { SetValue(CustomContentProperty, value); }
        }

        public static readonly DependencyProperty CustomContentProperty =
            DependencyProperty.Register("CustomContent", typeof(object), typeof(GroupBox), new PropertyMetadata(null, PropertyChangedCallback));

        public static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                (d as GroupBox).Content.Content = e.NewValue;
            }
        }

        private void HeaderTitle_LayoutUpdated(object sender, object e)
        {
            border.Margin = new Thickness(HeaderTitle.ActualWidth + 10, 10, 3, 3);
        }
    }
}
