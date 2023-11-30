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
        int previousYPos = -1;
        int previousXPos = -1;
        int bottomY = 0;
        int instructionY = 0;
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
            bottomY = (int)((Console.WindowHeight - numberOfLines) * 0.5);
            instructionY = bottomY + 3;
            currentRow = artRows.Length - 1;
            ypos = 0;
            previousYPos = -1;
            tickCount = TICKS_PER_FRAME;
            animate = true;
        }

        public void update()
        {
            if (tickCount == TICKS_PER_FRAME)
            {
                if (animate)
                {
                    if (ypos > bottomY)
                    {
                        currentRow--;
                        bottomY--;
                        ypos = 0;
                        previousYPos = -1;

                        if (currentRow < 0)
                        {
                            animate = false;
                        }
                    }
                    else
                    {
                        previousYPos = ypos;
                        ypos++;
                    }

                    dirty = true;
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
            if (animate && dirty)
            {
                dirty = false;
                Console.Write(ANSICodes.Positioning.SetCursorPos(previousYPos, 0));
                Console.Write(ANSICodes.Positioning.ClearLine);
                Console.Write(ANSICodes.Positioning.SetCursorPos(ypos, 0));
                Console.Write(Output.Align(artRows[currentRow], Alignment.CENTER));
            }
        } 

    }

}