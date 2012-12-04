using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaDouDou
{
    class Game
    {
        private int[,] gameZoneMatrix;
        private int columAmount;
        private int rowAmount;
        private int blockType;
        private int beanAmount;

        private int score;
        private double remainTime;

        public Game(int columAmount, int rowAmount, int blockType, int beanAmount)
        {
            this.columAmount = columAmount;
            this.rowAmount = rowAmount;
            this.blockType = blockType;
            this.beanAmount = beanAmount;

            gameZoneMatrix = new int[rowAmount, columAmount];
            initGameZoneMatrix();

            score = 0;
            remainTime = 150;
        }

        private void initGameZoneMatrix()
        {
            int type = blockType + 10;
            Random random = new Random();
            for (int i = 0; i < rowAmount; i++)
            {
                for (int j = 0; j < columAmount; j++)
                {
                    int value = random.Next() % type;
                    if (value <= 0 || value > blockType)
                    {
                        value = 0;
                    }
                    gameZoneMatrix[i, j] = value;
                }
            }
        }

        public int[,] getGameZoneMatrix()
        {
            return gameZoneMatrix;
        }

        public void restart()
        {
            initGameZoneMatrix();
        }
    }
}
