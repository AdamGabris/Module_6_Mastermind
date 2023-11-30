using System.ComponentModel;
using System.Net.NetworkInformation;
using Utils;
using System.Text;

namespace MasterMind
{


    public enum GAME_TYPE : int
    {
        PLAYER_VS_NPC,
        PLAYER_VS_PLAYER
    }

    public enum COLORS : int
    {
        RED,
        YELLOW,
        GREEN,
        BLUE,
        MAGENTA,
        CYAN,
    }
    public class MasterMindGame : GameEngine.IScene
    {

        const int CORRECT_COLOR_CORRECT_SPOT = 1;
        const int JUST_CORECT_COLOR = 0;
        const int WRONG = -1;

        GAME_TYPE type;
        public Action<Type, object[]> OnExitScreen { get; set; }

        int[] solution;
        int[] guess;
        int[] evaluation;

        List<int[]> guesses;
        List<int[]> evaluations;

        bool dirty = false;
        bool isGameOver;
        bool isWinner;
        int maxAttempts = 0;
        int numberOfTries;


        public MasterMindGame(GAME_TYPE gameType)
        {
            type = GAME_TYPE.PLAYER_VS_NPC;
            evaluations = new List<int[]>();
            guesses = new List<int[]>();
        }

        #region MasterMind game functions

        public static int[] CreateSequence(int[] source, int length = 4, bool duplicates = true)
        {
            List<int> sequence = new();
            Random rnd = new((int)DateTime.Now.Ticks);

            while (sequence.Count < length)
            {
                int index = rnd.Next(0, source.Length);
                int peg = source[index];

                if (duplicates == false)
                {
                    if (sequence.IndexOf(peg) == -1)
                    {
                        sequence.Add(peg);
                    }
                }
                else
                {
                    sequence.Add(peg);
                }
            }


            return sequence.ToArray();
        }


        int[] AskAndRetriveGuressFromUser()
        {
            string[] colorNames = Enum.GetNames(typeof(COLORS));
            int[] guess = new int[4];
            bool isValidInput = false;

            while (!isValidInput)
            {
                Console.WriteLine(Output.Align("Enter colors separated by a space (e.g., Red Yellow Green Blue):", Alignment.CENTER));
                string response = Console.ReadLine().Trim();
                string[] temp = response.Split(" ");

                if (temp.Length == 4)
                {
                    isValidInput = true;

                    for (int i = 0; i < 4; i++)
                    {
                        if (Enum.TryParse(temp[i], true, out COLORS color))
                        {
                            guess[i] = (int)color;
                        }
                        else
                        {
                            isValidInput = false;
                            Console.WriteLine("Invalid color input. Please enter valid color names (or numbers) separated by spaces.");
                            break;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter exactly four color names separated by spaces.");
                }
            }
            return guess;
        }


        int[] Compare(int[] sourceList, int[] guessList)
        {

            int[] output = new int[guessList.Length];

            for (int index = 0; index < guessList.Length; index++)
            {
                int score = WRONG;
                int correct = sourceList[index];
                int guess = guessList[index];

                if (correct == guess)
                {
                    score = CORRECT_COLOR_CORRECT_SPOT;
                }
                else if (IsPartOfSolution(sourceList, guess))
                {
                    score = JUST_CORECT_COLOR;
                }

                output[index] = score;
            }


            return output;
        }

        static bool IsPartOfSolution(int[] solution, int guess)
        {
            bool found = false;
            for (int index = 0; index < solution.Length; index++)
            {
                if (solution[index] == guess)
                {
                    found = true;
                    break;
                }
            }

            return found;
        }


        private string GetColoredPegs(int[] pegs)
        {
            StringBuilder coloredPegs = new StringBuilder();
            foreach (int peg in pegs)
            {
                string pegColor = GetPegColor(peg);
                coloredPegs.Append($"{pegColor}\u2606\u001b[0m ");
            }
            return coloredPegs.ToString();
        }

        private string GetEvaluationSymbols(int[] evaluations)
        {
            StringBuilder evaluationSymbols = new StringBuilder();
            foreach (int evaluation in evaluations)
            {
                string symbol = GetEvaluationSymbol(evaluation);
                evaluationSymbols.Append(symbol);
            }
            return evaluationSymbols.ToString();
        }

        private string GetPegColor(int peg)
        {
            switch (peg)
            {
                case (int)COLORS.RED:
                    return ANSICodes.Colors.Red;
                case (int)COLORS.YELLOW:
                    return ANSICodes.Colors.Yellow;
                case (int)COLORS.GREEN:
                    return ANSICodes.Colors.Green;
                case (int)COLORS.BLUE:
                    return ANSICodes.Colors.Blue;
                case (int)COLORS.MAGENTA:
                    return ANSICodes.Colors.Magenta;
                case (int)COLORS.CYAN:
                    return ANSICodes.Colors.Cyan;
                default:
                    return ANSICodes.Colors.White;
            }
        }

        private string GetEvaluationSymbol(int evaluation)
        {
            switch (evaluation)
            {
                case CORRECT_COLOR_CORRECT_SPOT:
                    return ANSICodes.Colors.Green + "\u25CF\u001b[0m ";
                case JUST_CORECT_COLOR:
                    return ANSICodes.Colors.White + "\u25C9\u001b[0m ";
                case WRONG:
                    return ANSICodes.Colors.Red + "X\u001b[0m ";
                default:
                    return "";
            }
        }

        public int GetConsoleWidth(string str)
        {
            int width = 0;
            bool insideEscapeCode = false;

            foreach (char c in str)
            {
                if (c == '\u001b') // Start of an ANSI escape code
                {
                    insideEscapeCode = true;
                }
                else if (insideEscapeCode && c == 'm') // End of an ANSI escape code
                {
                    insideEscapeCode = false;
                }
                else if (!insideEscapeCode)
                {
                    width += char.IsControl(c) || char.IsSurrogate(c) ? 0 : 1;
                }
            }

            return width;
        }


        public string CenterText(string text)
        {
            int consoleWidth = Console.WindowWidth;
            int textWidth = GetConsoleWidth(text);
            int padding = (consoleWidth - textWidth) / 2;
            return text.PadLeft(padding + text.Length).PadRight(consoleWidth);
        }

        #endregion

        #region GameEngine.IScene
        public void init()
        {

            evaluations = new List<int[]>();
            evaluation = new int[4];
            guesses = new List<int[]>();
            while (maxAttempts == 0)
            {
                Console.WriteLine("How many attemps for guessing do you want?");
                maxAttempts = int.Parse(Console.ReadLine());
                if (maxAttempts > 10)
                {
                    Console.WriteLine("Maximum number of attempts allowed is 10");
                    maxAttempts = 0;
                }
            }
            Console.WriteLine("Do you want to allow duplicates? (y/n)");
            string duplicateInput = Console.ReadLine();
            if (duplicateInput == "n")
            {
                int[] colors = new[] { (int)COLORS.RED, (int)COLORS.YELLOW, (int)COLORS.GREEN, (int)COLORS.BLUE, (int)COLORS.MAGENTA, (int)COLORS.CYAN };
                solution = CreateSequence(colors, 4, false);
                Thread.Sleep(2000);
                Console.Clear();

            }
            else
            {
                int[] colors = new[] { (int)COLORS.RED, (int)COLORS.YELLOW, (int)COLORS.GREEN, (int)COLORS.BLUE, (int)COLORS.MAGENTA, (int)COLORS.CYAN };
                solution = CreateSequence(colors, 4, true);
                Thread.Sleep(2000);
                Console.Clear();
            }
            dirty = true;
            draw();
        }

        public void input()
        {
            guess = AskAndRetriveGuressFromUser();
        }

        public void update()
        {
            if (guess != null && !isGameOver)
            {
                evaluation = Compare(solution, guess);
                guesses.Add(guess);
                evaluations.Add(evaluation);
                guess = null;
                numberOfTries++;

                isWinner = true;
                foreach (int score in evaluation)
                {
                    if (score != CORRECT_COLOR_CORRECT_SPOT)
                    {
                        isWinner = false;
                        break;
                    }
                }
                if (isWinner || guesses.Count >= maxAttempts)
                {
                    isGameOver = true;
                }
                evaluation = new int[4];
                dirty = true;

            }

        }
        public void draw()
        {
            if (dirty)
            {
                Console.Clear();
                dirty = false;
                Console.WriteLine(Output.Align("Legend:", Alignment.CENTER));
                Console.WriteLine();
                Console.WriteLine(Output.Align($"Available colors: Red 🔴 (0), Yellow 🟡 (1), Green 🟢 (2), Blue 🔵 (3), Magenta 🟣 (4), Cyan 🔵 (5)", Alignment.CENTER));
                Console.WriteLine(Output.Align($"{ANSICodes.Colors.White}☆{ANSICodes.Reset} = Peg/Guess (same color as guess)", Alignment.CENTER));
                Console.WriteLine(Output.Align($"{ANSICodes.Colors.Red}X{ANSICodes.Reset} = Incorrect space and color", Alignment.CENTER));
                Console.WriteLine(Output.Align($"{ANSICodes.Colors.White}⭘{ANSICodes.Reset} = Correct color, wrong space", Alignment.CENTER));
                Console.WriteLine(Output.Align($"{ANSICodes.Colors.Green}⭘{ANSICodes.Reset} = Correct color and space", Alignment.CENTER));
                Console.WriteLine();
                Console.WriteLine();

                for (int i = 0; i < maxAttempts; i++)
                {
                    string guessSymbols;
                    if (i < guesses.Count)
                    {
                        guessSymbols = GetColoredPegs(guesses[i]);
                    }
                    else
                    {
                        guessSymbols = "☆ ☆ ☆ ☆";
                    }

                    string evaluationSymbols;
                    if (i < evaluations.Count)
                    {
                        evaluationSymbols = GetEvaluationSymbols(evaluations[i]);
                    }
                    else
                    {
                        evaluationSymbols = "☆ ☆ ☆ ☆";
                    }

                    string line = $"{evaluationSymbols}  |  {guessSymbols}";
                    Console.WriteLine(CenterText(line));
                    Console.WriteLine();
                }
            }

            if (isGameOver && isWinner)
            {
                Console.WriteLine();
                Console.WriteLine(Output.Align("Congratulations, you win!", Alignment.CENTER));
                Console.WriteLine(Output.Align($"You took {numberOfTries} tries, and your total number of attempts was {maxAttempts}!", Alignment.CENTER));
                Console.WriteLine(Output.Align("Press any key to exit!", Alignment.CENTER));
                Console.ReadLine();
                Console.Clear();
                OnExitScreen(typeof(SplashScreen), null);
            }
            else if (isGameOver)
            {
                Console.WriteLine();
                Console.WriteLine(Output.Align("You reached the maximum attempts, you lost!", Alignment.CENTER));
                Console.WriteLine(Output.Align("Press any key to exit!", Alignment.CENTER));
                Console.ReadLine();
                Console.Clear();
                OnExitScreen(typeof(SplashScreen), null);
            }
        }


        #endregion



    }
}

