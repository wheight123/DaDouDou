using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace DaDouDou
{
    class Game
    {
        // game basic properties
        private int[,] gameZoneMatrix;
        private int columAmount;
        private int rowAmount;
        private int blockType;
        private int beanAmount;

        // player game information properties
        private int score;
        private double remainTime;

        // define game zone matrix value exchange time
        private const int EXCHANGE_TIME = 500;

        //*******************************************************************//
        // initialize part beginning //
        // constructor functions
        public Game(int columAmount, int rowAmount, int blockType, int beanAmount)
        {
            // initialize game basic properties
            this.columAmount = columAmount;
            this.rowAmount = rowAmount;
            this.blockType = blockType;
            this.beanAmount = beanAmount;
            gameZoneMatrix = new int[rowAmount, columAmount];
            // initialize game zone matrix
            initGameZoneMatrix();

            //player game information properties
            initPlayerGameInfo();
        }

        // initialize game zone matrix
        private void initGameZoneMatrix()
        {
            // clear game zone matrix
            clearGameZoneMatrix();
            // set random value pair in game zone matrix
            setRandomValuePairInGameZoneMatrix();
            // random exchange game zone matrix
            randomExhcangeGameZoneMatrix();
        }
        // clear game zone matrix, set all to 0
        private void clearGameZoneMatrix()
        {
            for (int i = 0; i < rowAmount; i++)
            {
                for (int j = 0; j < columAmount; j++)
                {
                    gameZoneMatrix[i, j] = 0;
                }
            }
        }
        // set random value pair in game zone matrix
        private void setRandomValuePairInGameZoneMatrix()
        {
            Random random = new Random();
            int beanPairAmount = beanAmount / 2;
            for (int i = 0; i < beanPairAmount; i++)
            {
                int value = random.Next() % blockType + 1;
                int x1 = i / columAmount + 1;
                int y1 = i % columAmount;
                int x2 = rowAmount / 2 + x1 + 1;
                int y2 = y1;
                gameZoneMatrix[x1, y1] = value;
                gameZoneMatrix[x2, y2] = value;
            }
        }
        // random exchange game zone matrix
        private void randomExhcangeGameZoneMatrix()
        {
            Random random = new Random();
            for (int i = 1; i <= EXCHANGE_TIME; i++)
            {
                int x1 = random.Next() % rowAmount;
                int y1 = random.Next() % columAmount;
                int x2 = random.Next() % rowAmount;
                int y2 = random.Next() % columAmount;
                int temp = gameZoneMatrix[x1, y1];
                gameZoneMatrix[x1, y1] = gameZoneMatrix[x2, y2];
                gameZoneMatrix[x2, y2] = temp;
            }
        }

        // initialize player game information
        private void initPlayerGameInfo()
        {
            score = 0;
            remainTime = 150;
        }

        // initialize part ending //
        //*******************************************************************//


        //*******************************************************************//
        // properties set and get method beginning //

        // return game zone matrix
        public int[,] getGameZoneMatrix()
        {
            return gameZoneMatrix;
        }
        // return score
        public int getScore()
        {
            return score;
        }
        // return remain time
        public double getRemainTime()
        {
            return remainTime;
        }

        // set and get method ending //
        //*******************************************************************//


        //*******************************************************************//
        // service functions beginning //

        // restart the game, reset game properties
        public void restart()
        {
            // initialize player game information
            initPlayerGameInfo();
            // initialize game zone matrix
            initGameZoneMatrix();
        }
        
        // find point list
        public List<Point> findPointList(int x, int y)
        {
            List<Point> originPointList = new List<Point>();
            originPointList = findOriginPointList(x, y);
            if (originPointList.Count < 2)
            {
                return new List<Point>();
            }
            else
            {
                List<Point> resultPointList = new List<Point>();
                resultPointList = findResultPointList(originPointList);
                return resultPointList;
            }
        }
        // find origin point list, search click point four direction
        private List<Point> findOriginPointList(int x, int y)
        {
            List<Point> originPointList = new List<Point>();
            int i, j;
            // find in click point right direction
            if (y < columAmount - 1)
            {
                i = x;
                j = y + 1;
                for (; j < columAmount; j++)
                {
                    if (gameZoneMatrix[i, j] != 0)
                    {
                        Point point = new Point(i, j);
                        originPointList.Add(point);
                        break;
                    }
                }
            }
            // find in click point left direction
            if (y > 0)
            {
                i = x;
                j = y - 1;
                for (; j >= 0; j--)
                {
                    if (gameZoneMatrix[i, j] != 0)
                    {
                        Point point = new Point(i, j);
                        originPointList.Add(point);
                        break;
                    }
                }
            }
            // find in click point down direction
            if (x < rowAmount -1)
            {
                i = x + 1;
                j = y;
                for (; i < rowAmount; i++)
                {
                    if (gameZoneMatrix[i, j] != 0)
                    {
                        Point point = new Point(i, j);
                        originPointList.Add(point);
                        break;
                    }
                }
            }
            // find in click point up direction
            if (x > 0)
            {
                i = x - 1;
                j = y;
                for (; i >= 0; i--)
                {
                    if (gameZoneMatrix[i, j] != 0)
                    {
                        Point point = new Point(i, j);
                        originPointList.Add(point);
                        break;
                    }
                }
            }
            return originPointList;
        }
        // find result point list according to origin point list
        private List<Point> findResultPointList(List<Point> originPointList)
        {
            List<Point> resultPointList = new List<Point>();
            while(originPointList.Count > 0) 
            {
                Point point = originPointList.ElementAt(0);
                originPointList.Remove(point);
                Boolean flag = false;
                List<Point> tempPointList = new List<Point>();
                foreach(Point tempPoint in originPointList)
                {

                    if(isTwoPointEqual(point,tempPoint))
                    {
                        flag = true;
                        resultPointList.Add(tempPoint);
                        tempPointList.Add(tempPoint);
                    }
                }
                foreach (Point ttPoint in tempPointList)
                {
                    originPointList.Remove(ttPoint);
                }
                if (flag)
                {
                    resultPointList.Add(point);
                }
            }
            return resultPointList;
        }
        // check whether two poing own the same value in game zone matrix
        private Boolean isTwoPointEqual(Point point1, Point point2)
        {
            int x1 = (int)point1.X;
            int y1 = (int)point1.Y;
            int x2 = (int)point2.X;
            int y2 = (int)point2.Y;
            if (gameZoneMatrix[x1, y1] == gameZoneMatrix[x2, y2])
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // decrease remain time abnormally
        public void decreaseRemainTimeAbnormal()
        {
            remainTime -= 10;
        }

        // decrease remain time normally
        public void decreaseRemainTimeNormal()
        {
            remainTime -= 1;
        }

        // clear beans value in game zone matrix
        public void clearBeansValue(List<Point> pointList)
        {
            foreach (Point point in pointList)
            {
                int x = (int)point.X;
                int y = (int)point.Y;
                gameZoneMatrix[x, y] = 0;
            }
        }

        public void updateGameInfo(List<Point> pointList)
        {
            foreach (Point point in pointList)
            {
                int x = (int)point.X;
                int y = (int)point.Y;
                gameZoneMatrix[x, y] = 0;
            }
            score += pointList.Count;
        }

        public Boolean isGameOver()
        {
            if (remainTime == 0 || score == beanAmount)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        // service functions ending //
        //*******************************************************************//
    }
}
