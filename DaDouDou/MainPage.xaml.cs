using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace DaDouDou
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。Parameter
        /// 属性通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void start_game_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
             this.Frame.Navigate(typeof(GamePanel));
        }

        private void start_game_PointerEntered_1(object sender, PointerRoutedEventArgs e)
        {
            Image image = (Image)sender;
            Thickness myThickness = new Thickness(5, 105, 0, 0);
            image.Margin = myThickness;
        }
        private void start_game_PointerExited_1(object sender, PointerRoutedEventArgs e)
        {
            Image image = (Image)sender;
            Thickness myThickness = new Thickness(0, 100, 0, 0);
            image.Margin = myThickness;
        }
    }
}
