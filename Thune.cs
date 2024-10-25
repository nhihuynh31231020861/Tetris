
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Features;
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
        public static List<Option> options;
        public class Option
        {
            public string Name { get; }
            public Action Selected { get; }
            public Option(string name, Action selected)
            {
                Name = name;
                Selected = selected;
            }
        }
        static void Main(string[] args)
        {
            
            GetMenu();

        }
        static void GetMenu()
        {
            Console.WriteLine("Menu");
            options = new List<Option>()
            {
                new Option("Instruction", () => Huongdan()),
                new Option("StartGame", () => Game()),
                new Option("Exit", () => Environment.Exit(0)),
            };
            int index = 0;
            WriteMenu(options, options[index]);

            // Store key info in here
            ConsoleKeyInfo keyinfo;
            do
            {
                keyinfo = Console.ReadKey();

                // Handle each key input (down arrow will write the menu again with a different selected item)
                if (keyinfo.Key == ConsoleKey.DownArrow)
                {
                    if (index + 1 < options.Count)
                    {
                        index++;
                        WriteMenu(options, options[index]);
                    }
                }
                if (keyinfo.Key == ConsoleKey.UpArrow)
                {
                    if (index - 1 >= 0)
                    {
                        index--;
                        WriteMenu(options, options[index]);
                    }
                }
                // Handle different action for the option
                if (keyinfo.Key == ConsoleKey.Enter)
                {
                    options[index].Selected.Invoke();
                    index = 0;
                }
            }
            while (keyinfo.Key != ConsoleKey.X);

            Console.ReadKey();
        }
        
        static void WriteMenu(List<Option> options, Option selectedOption)
        {

            Console.Clear();
            Console.SetCursorPosition(40, 7);
            Console.WriteLine("Tetris");

            Console.SetCursorPosition(0, 10);
            foreach (Option option in options)
            {
                if (option == selectedOption)
                {
                    Console.Write("> ");
                }
                else
                {
                    Console.Write(" ");
                }

                Console.WriteLine(option.Name);
            }

        }

        static void Huongdan()
        {
            Console.Clear();
            Console.SetCursorPosition(40, 10);
           Console.WriteLine("Controls:                                " +
                "\r\n                                             " +
                "" +
                "\r\n\t\t\t\t\t    [A] or [←] move left                     " +
                "\r\n\t\t\t\t\t    [D] or [→] move right                    " +
                "\r\n\t\t\t\t\t    [S] or [↓] fall faster                   " +
                "\r\n\t\t\t\t\t    [Q] spin left                            " +
                "\r\n\t\t\t\t\t    [E] spin right                           " +
                "\r\n\t\t\t\t\t    [Spacebar] drop                          " +
                "\r\n\t\t\t\t\t    [P] pause and resume                     " +
                "\r\n\t\t\t\t\t    [Escape] close game                      " +
                "\r\n\t\t\t\t\t    [Enter] start game  ");
            ConsoleKeyInfo keyinfo;
            keyinfo = Console.ReadKey();

            if (keyinfo.Key == ConsoleKey.Escape)
            {
                GetMenu();
            }
        }
        static void Game()
        {
            do
            {
                DrawBorder();


                timer.Start();
                dropTimer.Start();
                nextTFig = new TetrisFigure();
                tFig = nextTFig;
                tFig.DisplayFigure();
                nextTFig = new TetrisFigure();

                RefreshConsole();
                ConsoleKeyInfo keyinfo;
                keyinfo = Console.ReadKey();

                Console.SetCursorPosition(40, 20);
                Console.WriteLine("Do you want to continue? Y/N");
                Console.SetCursorPosition(40, 22);
                string res = "" + Console.ReadLine();

                if (res.ToUpper().Equals("N"))
                {
                    Console.WriteLine("Bye! See you again");
                    GetMenu();
                }
                   for(int i = 0; i < 23; i++)
                    {
                        for (int j =0; j < 16; j++)
                        {
                            spawnedBlockLocation[i, j] = 0 ;
                        }    
                    }    
                    
               
            } while (true);


        }

        //Vẽ map
        public static void DrawBorder()
        {
            for (int lengthCount = 0; lengthCount <= 23; ++lengthCount)
            {
                Console.SetCursorPosition(0, lengthCount);
                Console.Write("|");
                Console.SetCursorPosition(33, lengthCount);
                Console.Write("|");
            }
            Console.SetCursorPosition(0, 23);
            for (int widthCount = 0; widthCount <= 16; widthCount++)
            {
                Console.Write("--");
            }
            Console.SetCursorPosition(0, 0);
            for (int widthCount = 0; widthCount <= 16; widthCount++)
            {
                Console.Write("--");
            }

        }

        //Nhấn esc để thoát
        /*public static void GetMenu()
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
        }*/
          

            //Dashboard that shows score, level and the ammount of rows that you eliminate
            public static void GetDashboard(int levels, int scores, int rowss)
        {
            Console.SetCursorPosition(40, 5);
            Console.WriteLine("Level : " + levels);
            Console.SetCursorPosition(40, 7);
            Console.WriteLine("Score : " + scores);
            Console.SetCursorPosition(40, 9);
            Console.WriteLine("Rows cleared : " + rowss);
            Console.SetCursorPosition(40, 11);
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
