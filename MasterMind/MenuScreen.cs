using Utils;
using static Utils.Output;

namespace MasterMind
{
    public class MenuScreen : GameEngine.IScene
    {
        #region Constants And Variables 
        const String NEW_GAME = "Start New Game";
        const String CONTINUE_GAME = "Continue Game";
        const string DISPLAY_SETTINGS = "Settings";
        const string QUIT = "Quit";
        const int MENU_ITEM_WIDTH = 50;
        readonly string[] menuItems = { NEW_GAME, QUIT };
        int currentMenuIndex = 0;
        int startRow = 0;
        int startColumn = 0;
        int menuChange = 0;


        #endregion
        public Action<Type, Object[]> OnExitScreen { get; set; }

        public MenuScreen()
        {

        }
        public MenuScreen(int start)
        {

            startRow = start;

        }
        public void init()
        {
            startColumn = 0;
        }
        public void input()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKey keyCode = Console.ReadKey(true).Key;
                if (keyCode == ConsoleKey.DownArrow)
                {
                    menuChange = 1;
                }
                else if (keyCode == ConsoleKey.UpArrow)
                {
                    menuChange = -1;
                }
                else if (keyCode == ConsoleKey.Enter)
                {

                    if (menuItems[currentMenuIndex] == QUIT)
                    {
                        OnExitScreen(null, null);
                    }
                    else if (menuItems[currentMenuIndex] == NEW_GAME)
                    {
                        OnExitScreen(typeof(MasterMindGame), new object[] { GAME_TYPE.PLAYER_VS_NPC });
                    }
                }
            }
            else
            {
                menuChange = 0;
            }

        }
        public void update()
        {
            currentMenuIndex += menuChange;
            currentMenuIndex = Math.Clamp(currentMenuIndex, 0, menuItems.Length - 1);
            menuChange = 0;
        }
        public void draw()
        {
            int consoleHeight = Console.WindowHeight;
            int menuHeight = menuItems.Length;
            int startRow = (consoleHeight - menuHeight) / 2;
            Console.WriteLine(ANSICodes.Positioning.SetCursorPos(startRow + 2, startColumn));
            for (int index = 0; index < menuItems.Length; index++)
            {
                if (index == currentMenuIndex)
                {
                    printActiveMenuItem($"* {menuItems[index]} *");
                }
                else
                {
                    printMenuItem($"  {menuItems[index]}  ");
                }
            }
        }
        void printActiveMenuItem(string item)
        {
            Output.Write(Reset(Bold(Align(item, Alignment.CENTER))), newLine: true);
        }
        void printMenuItem(string item)
        {
            Output.Write(Reset(Align(item, Alignment.CENTER)), newLine: true);
        }

    }
}

// Make sure that reset the position of newLine before drawing a new row. Instead of writing a new line, move the cursor