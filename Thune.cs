using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thunghiem1
{
    internal class Thune
    {
        public static string blockArea = "■";   //1 khối
        public static int rows = 0, score = 0, level = 1;   
        public static int[,] grid = new int[23, 16];

        //Repeated statics Atributes
        private static TetrisFigure tFig;   //khối đang rơi
        private static TetrisFigure nextTFig;   //khối tiếp theo
        public static bool isDropped = false;
        public static int[,] spawnedBlockLocation = new int[23, 16];    //vị trí khối đang rơi

        //Timers
        public static Stopwatch timer = new Stopwatch();
        public static Stopwatch dropTimer = new Stopwatch();
        public static Stopwatch inputTimer = new Stopwatch();
        public static int dropTime, dropRate = 300;

        //Movement
        public static ConsoleKeyInfo pressedKey;
        public static bool isKeyPressed = false;

        static void Main()
        {

            DrawBorder();
            GetMenu();

            timer.Start();
            dropTimer.Start();

            nextTFig = new TetrisFigure();
            tFig = nextTFig;
            tFig.DisplayFigure();
            nextTFig = new TetrisFigure();

            RefreshConsole();

        }

        //Vẽ map
        public static void DrawBorder()
        {
            for (int lengthCount = 2; lengthCount <= 25; ++lengthCount)
            {
                Console.SetCursorPosition(40, lengthCount);
                Console.Write("||");
                Console.SetCursorPosition(72, lengthCount);
                Console.Write("||");
            }
            Console.SetCursorPosition(40, 25);
            for (int widthCount = 0; widthCount <= 16; widthCount++)
            {
                Console.Write("--");
            }
            Console.SetCursorPosition(40, 2);
            for (int widthCount = 0; widthCount <= 16; widthCount++)
            {
                Console.Write("--");
            }

        }

        //Nhấn esc để thoát
        public static void GetMenu()
        {

            Console.SetCursorPosition(4, 5);
            Console.WriteLine("Nhan bat ky phim nao");
            Console.SetCursorPosition(5, 6);
            Console.WriteLine("De bat dau");
            Console.SetCursorPosition(9, 7);
            Console.WriteLine("Game");
            Console.SetCursorPosition(3, 9);
            Console.WriteLine("Hoac phim  ESC De thoat");
            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
            {
                Environment.Exit(0);
            }
        }

        //Dashboard that shows score, level and the ammount of rows that you eliminate
        public static void GetDashboard(int levels, int scores, int rowss)
        {
            Console.SetCursorPosition(80, 5);
            Console.WriteLine("Level : " + levels);
            Console.SetCursorPosition(80, 7);
            Console.WriteLine("Score : " + scores);
            Console.SetCursorPosition(80, 9);
            Console.WriteLine("Rows cleared : " + rowss);
            Console.SetCursorPosition(80, 11);
            Console.WriteLine("Next figure : ");
        }

        private static void RefreshConsole()
        {
            while (true)//Update Loop
            {
                dropTime = (int)dropTimer.ElapsedMilliseconds;
                if (dropTime > dropRate)
                {
                    dropTime = 0;
                    dropTimer.Restart();
                    tFig.Drop();
                }
                if (isDropped == true)
                {
                    tFig = nextTFig;
                    nextTFig = new TetrisFigure();
                    tFig.DisplayFigure();

                    isDropped = false;
                }
                for (int j = 0; j < 16; j++)
                {
                    if (spawnedBlockLocation[0, j] == 1)
                        return;
                }
                ClearBlock();
                Input();
            } //end Update

        }

        //Nút để chơi
        private static void Input()
        {
            if (Console.KeyAvailable)
            {
                pressedKey = Console.ReadKey();
                isKeyPressed = true;
            }
            else
                isKeyPressed = false;

            if (pressedKey.Key == ConsoleKey.LeftArrow & !tFig.isSomethingLeft() & isKeyPressed)
            {
                for (int i = 0; i < 4; i++)
                {
                    tFig.location[i][1] -= 1;
                }
                tFig.Update();
            }
            else if (pressedKey.Key == ConsoleKey.RightArrow & !tFig.isSomethingRight() & isKeyPressed)
            {
                for (int i = 0; i < 4; i++)
                {
                    tFig.location[i][1] += 1;
                }
                tFig.Update();
            }
            if (pressedKey.Key == ConsoleKey.DownArrow & isKeyPressed)
            {
                tFig.Drop();
            }
            if (pressedKey.Key == ConsoleKey.UpArrow & isKeyPressed)
            {
                for (; tFig.isSomethingBelow() != true;)
                {
                    tFig.Drop();
                }
            }
            if (pressedKey.Key == ConsoleKey.Spacebar & isKeyPressed)
            {
                //rotate
                tFig.Rotate();
                tFig.Update();
            }
        }

        //Dòng đã phá
        private static void ClearBlock()
        {
            int combo = 0;
            for (int i = 0; i < 23; i++)
            {
                int j;
                for (j = 0; j < 16; j++)
                {
                    if (spawnedBlockLocation[i, j] == 0)
                        break;
                }
                if (j == 16)
                {
                    rows++;
                    combo++;
                    for (j = 0; j < 16; j++)
                    {
                        spawnedBlockLocation[i, j] = 0;
                    }
                    int[,] newTetLocation = new int[23, 16];
                    for (int k = 1; k < i; k++)
                    {
                        for (int l = 0; l < 16; l++)
                        {
                            newTetLocation[k + 1, l] = spawnedBlockLocation[k, l];
                        }
                    }
                    for (int k = 1; k < i; k++)
                    {
                        for (int l = 0; l < 16; l++)
                        {
                            spawnedBlockLocation[k, l] = 0;
                        }
                    }
                    for (int k = 0; k < 23; k++)
                        for (int l = 0; l < 16; l++)
                            if (newTetLocation[k, l] == 1)
                                spawnedBlockLocation[k, l] = 1;
                    TetrisFigure.Draw();
                }
            }

            lvlModifier(combo);

            GetDashboard(level, score, rows);

            dropRate = 300 - 22 * level;

        }

        public static void lvlModifier(int combo)
        {
            if (combo == 1)
                score += 50 * level;
            else if (combo == 2)
                score += 100 * level;
            else if (combo == 3)
                score += 300 * level;
            else if (combo > 3)
                score += 500 * combo * level;

            if (rows < 10) level = 1;
            else if (rows < 20) level = 2;
            else if (rows < 35) level = 3;
            else if (rows < 45) level = 4;
            else if (rows < 55) level = 5;
            else if (rows < 70) level = 6;
            else if (rows < 90) level = 7;
            else if (rows < 110) level = 8;
            else if (rows < 130) level = 9;
            else if (rows < 150) level = 10;
        }
    }
}
