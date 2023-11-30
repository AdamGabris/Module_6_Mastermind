using Utils;
public class GameEngine
{
    private IScene currentScene = null;
    private IScene nextScreen = null;
    private long ticksPerFrame = 20;
    private bool isRuning = false;
    public Action<Type, Object[]> OnExitScreen;


    public GameEngine(Type startScreen, int fps = 60)
    {
        if (!typeof(IScene).IsAssignableFrom(startScreen))
        {
            throw new ArgumentException("The start screen must implement the GameEngine.IScene interface");
        }
        ticksPerFrame = (Constants.MS_IN_SECONDS / fps) * TimeSpan.TicksPerMillisecond;
        init();
        ExitAndSwapScreen(startScreen, null);
    }

    private void init()
    {
        Console.Write(ANSICodes.HideCursor);
        OnExitScreen = ExitAndSwapScreen;
        Console.CancelKeyPress += new ConsoleCancelEventHandler(Exit);
    }
    public void Run()
    {
        isRuning = true;
        while (isRuning)
        {
            long start = DateTime.Now.Ticks;
            if (currentScene != null)
            {
                currentScene.input();
                currentScene.update();
                currentScene.draw();
            }
            long end = DateTime.Now.Ticks;

            int delta = (int)((start + ticksPerFrame - end) / TimeSpan.TicksPerMillisecond);
            if (delta > 0)
            {
                Thread.Sleep(delta);
            }

            if (nextScreen != null)
            {
                currentScene = nextScreen;
                nextScreen = null;
                currentScene.init();
            }
        }

        Exit(null, null);
    }

    void ExitAndSwapScreen(Type nextScreenType, Object[] args)
    {
        if (nextScreenType == null)
        {
            isRuning = false;
            return;
        }

        if (!typeof(IScene).IsAssignableFrom(nextScreenType))
        {
            throw new ArgumentException("next screen must implement IGameScreen.");
        }

        nextScreen = (IScene)Activator.CreateInstance(nextScreenType, args);
        nextScreen.OnExitScreen = ExitAndSwapScreen;
    }

    private void Exit(object sender, ConsoleCancelEventArgs args)
    {
        Console.Title = "";
        Console.Write(ANSICodes.ShowCursor);
        Console.Clear();
    }
    public interface IScene
    {
        public Action<Type, object[]> OnExitScreen { get; set; }
        void init();
        void input();
        void update();
        void draw();
    }

    private static class Constants
    {
        public const int MS_IN_SECONDS = 1000;
    }
}

