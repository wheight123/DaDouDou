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
        private double gamePanelStartPointY;

        private Image[,] gamePanelBeanMatrix;
        private Image[,] gamePanelBackgroundMatrix;
        private Image[] gameScoreArray;

        private DispatcherTimer timeSliderTimer;

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
            // initialize game panel beans
            initGamePanelBeans();
            // initialize game score
            initGameScore();
            // initialize game time
            initGameTime();
        }

        //*******************************************************************//
        // initialize related function beginning //

        // initialize game panel start point x,y coordinate
        private void initGamePanelStartPoint()
        {
            CoreWindow cw = CoreWindow.GetForCurrentThread();
            gamePanelStartPointX = (cw.Bounds.Width - BLOCK_WIDTH * COLUM_AMOUNT) / 2;
            gamePanelStartPointY = (cw.Bounds.Height - BLOCK_HEIGHT * ROW_AMOUNT) / 2;
        }

        // initialize game panel background
        private void initGamePanelBackground()
        {
            gamePanelBackgroundMatrix = new Image[ROW_AMOUNT, COLUM_AMOUNT];
            for (int i = 0; i < ROW_AMOUNT; i++)
            {
                double y = gamePanelStartPointY + i * BLOCK_HEIGHT;
                for (int j = 0; j < COLUM_AMOUNT; j++)
                {
                    Image image = new Image();
                    image.Name = "bgImage_" + i + "_" + j;
                    double x = gamePanelStartPointX + j * BLOCK_WIDTH;
                    Thickness myThickness = new Thickness(x, y, 0, 0);
                    image.Margin = myThickness;
                    setBackgroundImageSource(image, i, j);
                    image.PointerEntered += bgImage_PointerEntered;
                    image.PointerExited += bgImage_PointerExited;
                    image.Tapped += bgImage_Tapped;
                    gameCanvas.Children.Add(image);

                    gamePanelBackgroundMatrix[i, j] = image;
                }
            }
        }

        // initialize game panel beans
        private void initGamePanelBeans()
        {
            gamePanelBeanMatrix = new Image[ROW_AMOUNT, COLUM_AMOUNT];
            int[,] gameZoneMatrix = game.getGameZoneMatrix();
            for (int i = 0; i < ROW_AMOUNT; i++)
            {
                double y = gamePanelStartPointY + i * BLOCK_HEIGHT;
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

        // initialize game score
        private void initGameScore()
        {
            int imageWith = 19;
            int imageHeight = 30;
            int startX = (int)gamePanelStartPointX + BLOCK_WIDTH * COLUM_AMOUNT - imageWith * 5;
            int startY = (int)gamePanelStartPointY - imageHeight * 2 + 15;
            gameScoreArray = new Image[3];
            for (int i = 0; i < 3; i++)
            {
                Image image = new Image();
                image.Name = "score_" + i;
                int x = startX + i * (imageWith + 4);
                int y = startY;
                Thickness myThickness = new Thickness(x, y, 0, 0);
                image.Margin = myThickness;
                image.Source = number0.Source;
                gameCanvas.Children.Add(image);
                gameScoreArray[i] = image;
            }
        }

        // initialize game time
        private void initGameTime()
        {
            Slider slider = timeSlider;
            gameCanvas.Children.Remove(slider);
            int x = (int)gamePanelStartPointX + 20;
            int y = (int)gamePanelStartPointY - 50;
            Thickness myThickness = new Thickness(x, y, 0, 0);
            slider.Margin = myThickness;
            slider.Maximum = game.getRemainTime();
            slider.Value = game.getRemainTime();
            gameCanvas.Children.Add(slider);

            timeSliderTimer = new DispatcherTimer();
            timeSliderTimer.Interval = TimeSpan.FromSeconds(0.5);
            timeSliderTimer.Tick += timeSliderTimer_Tick;
            timeSliderTimer.Start();
        }

        // initialize related function ending //
        //*******************************************************************//


        //*******************************************************************//
        // backgroud image block event beginning //

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
                Point point = new Point(i, j);
                // do find beans update
                doFindBeansUpdate(point, resultPointList);
            }
            else
            {
                // do beans not found update
                doBeansNotFoundUpdate();
            }
            // check whether game is over
            checkGameStatus();
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

        // backgroud image block event ending //
        //*******************************************************************//


        //*******************************************************************//
        // normal controls event response functions beginning //

        // restart game image tapped event response function
        private void image_restart_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            // clear game panel beans
            clearGamePanelBeans();
            // game resetart, reset game information
            game.restart();
            // update game panel background
            updateGamePanelBackground();
            // initialize game panel beans
            initGamePanelBeans();
            // update score
            updateScore();
            // update remain time
            updateGameTime();
            // start timeSlider timer
            timeSliderTimer.Start();
            //
            txtRemind.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        // back home page image tapped event response function
        private void image_back_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        // normal controls event response functions ending //
        //*******************************************************************//


        //*******************************************************************//

        // timeSlider timer tick callback function
        void timeSliderTimer_Tick(object sender, object e)
        {
            // decrease game remain time normally
            game.decreaseRemainTimeNormal();
            // update game time
            updateGameTime();
            // check game status
            checkGameStatus();
        }

        // do find beans update
        private void doFindBeansUpdate(Point startPoint, List<Point> pointList)
        {
            game.updateGameInfo(pointList);
            showBeanPairPath(startPoint, pointList);
            updateScore();
            updateGameTime();
        }

        // show bean pair path according to start point(x, y) and point list
        private void showBeanPairPath(Point startPoint, List<Point> pointList)
        {
            showPathPoint = startPoint;
            showPathPointList = pointList;
            foreach (Point endPoint in pointList)
            {
                showPath(startPoint, endPoint);
            }
            
            showPathTimer = new DispatcherTimer();
            showPathTimer.Interval = TimeSpan.FromSeconds(0.2);
            showPathTimer.Tick += showPathTimer_Tick;
            showPathTimer.Start();
        }
        // show bean pair path of two point
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

        // update score
        private void updateScore()
        { 
            int score = game.getScore();
            int length = gameScoreArray.Length;
            for (int i = 0; i < length; i++)
            {
                int num = score % 10;
                int temp = score / 10;
                score = temp;
                Image image = gameScoreArray[length - 1 - i];
                gameCanvas.Children.Remove(image);
                setNumberImageSource(image, num);
                gameCanvas.Children.Add(image);
            }
        }

        // showPathTimer tick callback function
        private void showPathTimer_Tick(object sender, object e)
        {
            // erase bean pair path 
            eraseBeanPairPath(showPathPoint, showPathPointList);
            // clear bean in game panel
            clearBeanInGamePanel(showPathPointList);
            // stop showPathTimer
            showPathTimer.Stop();
        }

        // clear bean image in game panel according to point list
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

        // erase bean pair path according to start point and point list
        private void eraseBeanPairPath(Point startPoint, List<Point> pointList)
        {
            foreach (Point endPoint in pointList)
            {
                erasePath(startPoint, endPoint);
            }
        }
        // erase bean pair path of two point
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
         
        // do beans not found update
        private void doBeansNotFoundUpdate()
        {
            game.decreaseRemainTimeAbnormal();
            updateGameTime();
        }

        // update game time
        private void updateGameTime()
        {
            timeSlider.Value = game.getRemainTime();
        }

        // check game status
        private void checkGameStatus()
        {
            if (game.isGameOver())
            {
                clearGamePanelBackground();
                updateGamePanelBeans();
                timeSliderTimer.Stop();
                txtRemind.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
        }

        // clear game panel background
        private void clearGamePanelBackground()
        {
            for (int i = 0; i < ROW_AMOUNT; i++)
            {
                for (int j = 0; j < COLUM_AMOUNT; j++)
                {
                    Image image = gamePanelBackgroundMatrix[i, j];
                    gameCanvas.Children.Remove(image);
                    image.Tapped -= bgImage_Tapped;
                    gameCanvas.Children.Add(image);
                    gamePanelBackgroundMatrix[i, j] = image;
                }
            }
        }
        // update game panel beans
        private void updateGamePanelBeans()
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
                        gameCanvas.Children.Add(image);
                    }
                }
            }
        }

        // update game panel background
        private void updateGamePanelBackground()
        {
            for (int i = 0; i < ROW_AMOUNT; i++)
            {
                for (int j = 0; j < COLUM_AMOUNT; j++)
                {
                    Image image = gamePanelBackgroundMatrix[i, j];
                    gameCanvas.Children.Remove(image);
                    image.Tapped += bgImage_Tapped;
                    gameCanvas.Children.Add(image);
                    gamePanelBackgroundMatrix[i, j] = image;
                }
            }
        }
        //*******************************************************************//


        //*******************************************************************//
        // auxiliary functions beginning //

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

        // set number image source
        private void setNumberImageSource(Image image, int type)
        {
            switch (type)
            {
                case 1:
                    image.Source = number1.Source;
                    break;
                case 2:
                    image.Source = number2.Source;
                    break;
                case 3:
                    image.Source = number2.Source;
                    break;
                case 4:
                    image.Source = number4.Source;
                    break;
                case 5:
                    image.Source = number5.Source;
                    break;
                case 6:
                    image.Source = number6.Source;
                    break;
                case 7:
                    image.Source = number7.Source;
                    break;
                case 8:
                    image.Source = number8.Source;
                    break;
                case 9:
                    image.Source = number9.Source;
                    break;
                case 0:
                    image.Source = number0.Source;
                    break;
            }
        }

        // auxiliary functions ending //
        //*******************************************************************//
    }
}
