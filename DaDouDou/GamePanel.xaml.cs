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
        private Game game;
        private const int COLUM_AMOUNT = 25;
        private const int ROW_AMOUNT = 15;
        private const int BLOCK_WIDTH = 32;
        private const int BLOCK_HEIGHT = 32;
        private const int BLOCK_TYPE = 10;
        private const int BEAN_AMOUNT = 200;
        
        private double gamePanelStartPointX;
        private double gamePanleStartPointY;

        private Image[,] gamePanelBeanMatrix;
        private Image[,] gamePanelBackgroundMatrix;

        private DispatcherTimer showPathTimer;
        private Point showPathPoint;
        private List<Point> showPathPointList;
        public GamePanel()
        {
            this.InitializeComponent();
            game = new Game(COLUM_AMOUNT, ROW_AMOUNT, BLOCK_TYPE, BEAN_AMOUNT);
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
            // initialzie game panel beans
            initGamePanelBeans();
        }

        // initialize game panel start point x,y coordinate
        private void initGamePanelStartPoint()
        {
            CoreWindow cw = CoreWindow.GetForCurrentThread();
            gamePanelStartPointX = (cw.Bounds.Width - BLOCK_WIDTH * COLUM_AMOUNT) / 2;
            gamePanleStartPointY = (cw.Bounds.Height - BLOCK_HEIGHT * ROW_AMOUNT) / 2;
        }

        // initialize game panel background
        private void initGamePanelBackground()
        {
            gamePanelBackgroundMatrix = new Image[ROW_AMOUNT, COLUM_AMOUNT];
            for (int i = 0; i < ROW_AMOUNT; i++)
            {
                double y = gamePanleStartPointY + i * BLOCK_HEIGHT;
                for (int j = 0; j < COLUM_AMOUNT; j++)
                {
                    Image image = new Image();
                    image.Name = "bgImage_" + i + "_" + j;
                    double x = gamePanelStartPointX + j * BLOCK_WIDTH;
                    Thickness myThickness = new Thickness(x, y, 0, 0);
                    image.Margin = myThickness;
                    setBackgroundImageSource(image, i, j);
                    image.PointerEntered += bgImage_PointerEntered;
                    //image.PointerMoved += bgImage_PointerMoved;
                    image.PointerExited += bgImage_PointerExited;
                    image.Tapped += bgImage_Tapped;
                    gameCanvas.Children.Add(image);

                    gamePanelBackgroundMatrix[i, j] = image;
                }
            }
        }
        // bgImage block be tapped
        private void bgImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Image image = (Image)sender;
            String name = image.Name;
            String[] array = name.Split(new Char[] { '_' });
            int i = Int32.Parse(array[1]);
            int j = Int32.Parse(array[2]);
            List<Point> resultPointList = game.findPointList(i, j);
            if (resultPointList.Count > 0)
            {
                clearBeans(resultPointList);
                showBeanPairPath(i, j, resultPointList);
            }
            else
            {
                game.decreaseRemainTime();
            }
        }
        private void clearBeans(List<Point> pointList)
        {
            game.clearBeansValue(pointList);
            clearBeanInGamePanel(pointList);
        }
        private void clearBeanInGamePanel(List<Point> pointList)
        {
            foreach (Point point in pointList)
            {
                int x = (int)point.X;
                int y = (int)point.Y;
                Image image = gamePanelBeanMatrix[x, y];
                gameCanvas.Children.Remove(image);
            }
        }
        private void showBeanPairPath(int x, int y, List<Point> pointList)
        {
            showPathTimer = new DispatcherTimer();
            showPathTimer.Interval = TimeSpan.FromSeconds(0.2);
            showPathTimer.Tick += showPathTimer_Tick;
            showPathTimer.Start();
            Point startPoint = new Point(x, y);
            showPathPoint = startPoint;
            showPathPointList = pointList;
            foreach (Point endPoint in pointList)
            {
                showPath(startPoint, endPoint);
            }
            
        }
        private void showPath(Point point1, Point point2)
        {
            int x1 = (int)point1.X;
            int y1 = (int)point1.Y;
            int x2 = (int)point2.X;
            int y2 = (int)point2.Y;
            int i, j;
            if (x1 == x2)
            {
                int endJ;
                i = x1;
                if (y1 > y2)
                {
                    j = y2 + 1;
                    endJ = y1;
                }
                else
                {
                    j = y1 + 1;
                    endJ = y2;
                }
                for (; j < endJ; j++)
                {
                    Image image = gamePanelBackgroundMatrix[i, j];
                    gameCanvas.Children.Remove(image);
                    image.Source = pathPoint.Source;
                    gameCanvas.Children.Add(image);
                    gamePanelBackgroundMatrix[i, j] = image;
                }
            }
            else
            {
                int endI;
                j = y1;
                if (x1 > x2)
                {
                    i = x2 + 1;
                    endI = x1;
                }
                else
                {
                    i = x1 + 1;
                    endI = x2;
                }
                for (; i < endI; i++)
                {
                    Image image = gamePanelBackgroundMatrix[i, j];
                    gameCanvas.Children.Remove(image);
                    image.Source = pathPoint.Source;
                    gameCanvas.Children.Add(image);
                    gamePanelBackgroundMatrix[i, j] = image;
                }
            }
        }
        private void showPathTimer_Tick(object sender, object e)
        {
            eraseBeanPairPath(showPathPoint, showPathPointList);
        }
        private void eraseBeanPairPath(Point startPoint, List<Point> pointList)
        {
            foreach (Point endPoint in pointList)
            {
                erasePath(startPoint, endPoint);
            }
        }
        private void erasePath(Point point1, Point point2)
        {
            int x1 = (int)point1.X;
            int y1 = (int)point1.Y;
            int x2 = (int)point2.X;
            int y2 = (int)point2.Y;
            int i, j;
            if (x1 == x2)
            {
                int endJ;
                i = x1;
                if (y1 > y2)
                {
                    j = y2 + 1;
                    endJ = y1;
                }
                else
                {
                    j = y1 + 1;
                    endJ = y2;
                }
                for (; j < endJ; j++)
                {
                    Image image = gamePanelBackgroundMatrix[i, j];
                    gameCanvas.Children.Remove(image);
                    setBackgroundImageSource(image, i, j);
                    gameCanvas.Children.Add(image);
                    gamePanelBackgroundMatrix[i, j] = image;
                }
            }
            else
            {
                int endI;
                j = y1;
                if (x1 > x2)
                {
                    i = x2 + 1;
                    endI = x1;
                }
                else
                {
                    i = x1 + 1;
                    endI = x2;
                }
                for (; i < endI; i++)
                {
                    Image image = gamePanelBackgroundMatrix[i, j];
                    gameCanvas.Children.Remove(image);
                    setBackgroundImageSource(image, i, j);
                    gameCanvas.Children.Add(image);
                    gamePanelBackgroundMatrix[i, j] = image;
                }
            }
        }

        // point move across bgImage block
        private void bgImage_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Image image = (Image)sender;
            image.Source = block3.Source;
        }
        // point enter into bgImage block
        private void bgImage_PointerEntered(object sender, PointerRoutedEventArgs e)
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
            setBackgroundImageSource(image, i, j);
        }
        // set background image's source
        private void setBackgroundImageSource(Image image, int i, int j)
        {
            int type = (i + j) % 2;
            switch (type)
            {
                case 0:
                    image.Source = block1.Source;
                    break;
                case 1:
                    image.Source = block2.Source;
                    break;
                default:
                    break;
            }
        }

        // initialize game panel beans
        private void initGamePanelBeans()
        {
            gamePanelBeanMatrix = new Image[ROW_AMOUNT, COLUM_AMOUNT];
            int[,] gameZoneMatrix = game.getGameZoneMatrix();
            for (int i = 0; i < ROW_AMOUNT; i++)
            {
                double y = gamePanleStartPointY + i * BLOCK_HEIGHT;
                for (int j = 0; j < COLUM_AMOUNT; j++)
                {
                    int type = gameZoneMatrix[i, j];
                    if (type != 0)
                    {
                        Image image = new Image();
                        image.Name = "beanImage_" + i + "_" + j;
                        double x = gamePanelStartPointX + j * BLOCK_WIDTH;
                        Thickness myThickness = new Thickness(x, y, 0, 0);
                        image.Margin = myThickness;
                        setBeanImageSource(image, type);
                        gameCanvas.Children.Add(image);
                        gamePanelBeanMatrix[i, j] = image;
                    }
                }
            }
        }
        // set bean image source
        private void setBeanImageSource(Image image, int type)
        {
            switch(type) 
            {
                case 1:
                    image.Source = bean1.Source;
                    break;
                case 2:
                    image.Source = bean2.Source;
                    break;
                case 3:
                    image.Source = bean3.Source;
                    break;
                case 4:
                    image.Source = bean4.Source;
                    break;
                case 5:
                    image.Source = bean5.Source;
                    break;
                case 6:
                    image.Source = bean6.Source;
                    break;
                case 7:
                    image.Source = bean7.Source;
                    break;
                case 8:
                    image.Source = bean8.Source;
                    break;
                case 9:
                    image.Source = bean9.Source;
                    break;
                case 10:
                    image.Source = bean10.Source;
                    break;
            }
        }
        
        // backHome click function
        private void backHome_Click_1(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        // restart click function
        private void restart_Click(object sender, RoutedEventArgs e)
        {
            // clear game panel beans
            clearGamePanelBeans();
            // game resetart, reset game information
            game.restart();
            // initialize game panel beans
            initGamePanelBeans();
        }

        // clear game panel beans
        private void clearGamePanelBeans()
        {
            int[,] gameZoneMatrix = game.getGameZoneMatrix();
            for (int i = 0; i < ROW_AMOUNT; i++)
            {
                for (int j = 0; j < COLUM_AMOUNT; j++)
                {
                    int type = gameZoneMatrix[i, j];
                    if (type != 0)
                    {
                        Image image = gamePanelBeanMatrix[i, j];
                        gameCanvas.Children.Remove(image);
                    }
                }
            }
        }
    }
}
