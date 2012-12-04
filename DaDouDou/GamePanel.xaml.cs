using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
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
        private const int COLUM_AMOUNT = 25;
        private const int ROW_AMOUNT = 15;
        private const int BLOCK_WIDTH = 32;
        private const int BLOCK_HEIGHT = 32;
        
        private double gamePanelStartPointX;
        private double gamePanleStartPointY;
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
            // initialize game panel start point x,y coordinate
            initGamePanelStartPoint();
            // initialize game panel background
            initGamePanelBackground();
        }

        private void initGamePanelStartPoint()
        {
            CoreWindow cw = CoreWindow.GetForCurrentThread();
            gamePanelStartPointX = (cw.Bounds.Width - BLOCK_WIDTH * COLUM_AMOUNT) / 2;
            gamePanleStartPointY = (cw.Bounds.Height - BLOCK_HEIGHT * ROW_AMOUNT) / 2;
        }
        private void initGamePanelBackground()
        {
            for (int i = 1; i <= ROW_AMOUNT; i++)
            {
                for (int j = 1; j <= COLUM_AMOUNT; j++)
                {
                    Image image = new Image();
                    image.Name = "bgImage_" + i + "_" + j;
                    double x = gamePanelStartPointX + (j - 1) * BLOCK_WIDTH;
                    double y = gamePanleStartPointY + (i - 1) * BLOCK_HEIGHT;
                    Thickness myThickness = new Thickness(x, y, 0, 0);
                    image.Margin = myThickness;
                    if ((i + j) % 2 == 0)
                    {
                        image.Source = block1.Source;
                    }
                    else
                    {
                        image.Source = block2.Source;
                    }
                    image.PointerMoved += bgImage_PointerMoved;
                    image.PointerExited +=bgImage_PointerExited;
                    image.Tapped += bgImage_Tapped;
                    gameCanvas.Children.Add(image);
                }
            }
        }

        private void bgImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Image image = (Image)sender;
            image.Source = bean1.Source;
        }
        // point move across bgImage block
        private void bgImage_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Image image = (Image)sender;
            image.Source = block3.Source;
        }
        // point exit from bgImage block
        private void bgImage_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Image image = (Image)sender;
            String name = image.Name;
            String[] array = name.Split(new Char[] { '_' });
            int i = Int32.Parse(array[1]);
            int j = Int32.Parse(array[2]);
            if ((i + j) % 2 == 0)
            {
                image.Source = block1.Source;
            }
            else
            {
                image.Source = block2.Source;
            }
        }

        

        private void backHome_Click_1(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        
    }
}
