using System;
using Windows.ApplicationModel.Background;
using System.Diagnostics;
#if WINUI3
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Shared
{
    public sealed partial class SharedUserControl : UserControl
    {
        public SharedUserControl()
        {
            this.InitializeComponent();
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
#if WINUI3
            var displayInfo = DisplayInfo.GetDisplayInfoForElement(this);
            myButton.Content = $"Screen: Width={displayInfo.ScreenWidth},Height={displayInfo.ScreenHeight}";
#else
            var displayInfo = DisplayInformation.GetForCurrentView();
            myButton.Content = $"Screen: Width={displayInfo.ScreenWidthInRawPixels},Height={displayInfo.ScreenHeightInRawPixels}";
#endif
        }
    }
}
