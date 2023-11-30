using Utils;

namespace MasterMind
{

    public class SplashScreen : GameEngine.IScene
    {
        const int TICKS_PER_FRAME = 1;
        const string art = """
███    ███  █████  ███████ ████████ ███████ ██████  ███    ███ ██ ███    ██ ██████ 
████  ████ ██   ██ ██         ██    ██      ██   ██ ████  ████ ██ ████   ██ ██   ██
██ ████ ██ ███████ ███████    ██    █████   ██████  ██ ████ ██ ██ ██ ██  ██ ██   ██
██  ██  ██ ██   ██      ██    ██    ██      ██   ██ ██  ██  ██ ██ ██  ██ ██ ██   ██
██      ██ ██   ██ ███████    ██    ███████ ██   ██ ██      ██ ██ ██   ████ ██████ 
""";

        int numberOfLines = 0;
        string[] artRows;
        int currentRow = 0;
        int ypos = 0;
        int xpos = 0;
        int middleX = 0;
        int tickCount = 0;
        bool animate = false;
        bool dirty = false;
        int countdownToExit = 3;


        public Action<Type, object[]> OnExitScreen { get; set; }

        public void init()
        {
            Console.Clear();
            artRows = art.Split("\n");
            numberOfLines = artRows.Length;
            middleX = (int)((Console.WindowWidth - artRows[0].Length) * 0.5);
            currentRow = 0;
            ypos = Console.WindowHeight / 3;
            xpos = 0;
            tickCount = TICKS_PER_FRAME;
            animate = true;
        }


        public void update()
        {
            if (tickCount == TICKS_PER_FRAME)
            {
                if (animate)
                {
                    if (xpos < middleX)
                    {
                        xpos++;
                        dirty = true;
                    }
                    else if (currentRow < numberOfLines - 1)
                    {
                        currentRow++;
                        xpos = 0;
                        ypos++;
                        dirty = true;
                    }
                    else
                    {
                        animate = false;
                    }
                }
                tickCount = 0;
            }
            else
            {
                if (!animate)
                {
                    if (countdownToExit == 0)
                    {
                        OnExitScreen(typeof(MenuScreen), new object[] { artRows.Length });
                    }
                    countdownToExit--;
                }

                tickCount++;
            }
        }



        public void input()
        {
        }


        public void draw()
        {
            if (dirty)
            {
                Console.SetCursorPosition(0, ypos);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(xpos, ypos);
                Console.Write(artRows[currentRow]);

                dirty = false;
            }
        }

    }

}




