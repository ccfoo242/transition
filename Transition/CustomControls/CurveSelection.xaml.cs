using Easycoustics.Transition.CurveLibrary;
using Easycoustics.Transition.Design;
using Easycoustics.Transition.Functions;
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

namespace Easycoustics.Transition.CustomControls
{
    public sealed partial class CurveSelection : UserControl
    {
        public CurveSelection()
        {
            this.InitializeComponent();

            var systemCurveLibrary = UserDesign.CurrentDesign.SystemCurves;
            var userCurveLibrary = UserDesign.CurrentDesign.UserCurves;

            treeSystemCurves.ItemsSource = systemCurveLibrary.Children;
            treeUserCurves.ItemsSource = userCurveLibrary.Children;
        }

        public List<Function> selectedCurves()
        {
            var output = new List<Function>();

            foreach (var node in treeSystemCurves.SelectedNodes)
                if (node.Content is LibraryItem)
                    output.Add(((LibraryItem)node.Content).Curve);

            return output;
        }
    }

    public class ExplorerItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FolderTemplate { get; set; }
        public DataTemplate FileTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            var explorerItem = (LibraryBase)item;
            if (explorerItem is LibraryFolder) return FolderTemplate;

            return FileTemplate;
        }
    }
}
