using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        // return game zone matrix
        public int[,] getGameZoneMatrix()
        {
            return gameZoneMatrix;
        }

        // restart the game, reset game properties
        public void restart()
        {
            initPlayerGameInfo();
            initGameZoneMatrix();
        }
    }
}
