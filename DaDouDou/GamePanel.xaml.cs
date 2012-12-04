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

            // code add image view
            Image image = new Image();
            //BitmapImage bitmapImage = new BitmapImage(new Uri("C:\\Users\\wheight123\\Documents\\Visual Studio 2012\\Projects\\DaDouDou\\DaDouDou\\Assets\\game_page\\bean\\bean10.png", UriKind.Absolute));
            image.Source = bean1.Source;
            Thickness myThickness = new Thickness(200, 200, 0, 0);
            image.Margin = myThickness;
            gameCanvas.Children.Add(image);
        }

        private void backHome_Click_1(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}
