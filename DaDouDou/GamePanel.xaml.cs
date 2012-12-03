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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace DaDouDou
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class GamePanel : Page
    {
        private BitmapImage greenImage;

        public GamePanel()
        {
            this.InitializeComponent();
            //greenImage = new BitmapImage(new Uri("Assets/game_page/green_bean.png", UriKind.Relative));
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。Parameter
        /// 属性通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void BackHome_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void Image_PointerEntered_1(object sender, PointerRoutedEventArgs e)
        {
            Image image = (Image)sender;
            image.Source = purple.Source;
        }

        private void Image_PointerExited_1(object sender, PointerRoutedEventArgs e)
        {
            Image image = (Image)sender;
            image.Source = greenImage;
        }

        private void green_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            Image image = (Image)sender;
            image.Source = red.Source;
        }

    }
}
