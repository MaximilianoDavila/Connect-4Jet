// Made by Jet Brainers that are Daniel E. Cintron, Maximiliano Davila, George Lopez to create connect - 4Jetusing System;





using System.Text;


// This class holds our entire Connect Four game
class Program
{
    // --- Game Settings ---
    const int Columns = 7;
    const int Rows = 6;

    // --- Internal Representation ---
    const char EmptyChar = '_'; // How we track empty spots internally
    const char Chip1Char = 'X'; // How we track player 1 internally
    const char Chip2Char = 'O'; // How we track player 2 internally

    // --- Visual Representation ---
    const string Player1Emoji = "🟡"; // Yellow Disc emoji
    const ConsoleColor Player1Color = ConsoleColor.Yellow;
    const string Player2Emoji = "🔴"; // Red Disc emoji
    const ConsoleColor Player2Color = ConsoleColor.Red;
    const string EmptySlotDisplay = " "; // What to show for an empty space visually
    const ConsoleColor GridFrameColor = ConsoleColor.DarkBlue; // Frame color (Darkest standard blue)
    const ConsoleColor EmptySlotBackColor = ConsoleColor.Black; // Background for empty slots
    const ConsoleColor DefaultBackColor = ConsoleColor.Black; // Default console background
    const ConsoleColor DefaultForeColor = ConsoleColor.Gray;  // Default console text color

    // --- Game State ---
    static char[][] board = new char[Columns][]; // Stores internal chars: '_', 'X', 'O'

    // Player/AI or Player2 visual and internal details
    static char playerChipChar;
    static string playerChipEmoji;
    static ConsoleColor playerChipColor;
    static char aiChipChar;
    static string aiChipEmoji;
    static ConsoleColor aiChipColor;
    
    // Game mode flag: false = Player vs AI, true = Player vs Player
    static bool isPlayerVsPlayer = false;

    // Current game status
    static char currentPlayerChipChar;
    static int cursorPosition; // Current column selected (0-6)
    static bool isPlayerTurn;
    static bool gameEnded;
    static int playerScore = 0;
    static int aiScore = 0;
    static Random random = new Random(); // From System namespace


    // --- Main Program Entry Point ---
    static void Main(string[] args)
    {
        ConsoleColor originalBg = Console.BackgroundColor;
        ConsoleColor originalFg = Console.ForegroundColor;

        Console.OutputEncoding = Encoding.UTF8;
        
        //Add the splash screen animation here before the game starts
        ShowSplashScreen();
        
        Console.WriteLine(">> Welcome to Connect Four! <<");

        // NEW: Choose game mode
        ChooseGameMode();

        ChoosePlayerChip();

        bool playAgain = true;
        while (playAgain)
        {
            InitializeBoard();
            gameEnded = false;
            isPlayerTurn = true;
            currentPlayerChipChar = playerChipChar;

            // *** Game Turn Loop ***
            while (!gameEnded)
            {
                DisplayGameStatus();

                bool turnCompleted = false;

                if (isPlayerTurn)
                {
                    turnCompleted = HandlePlayerInput();
                }
                else
                {
                    // If in player vs player mode, have the second player use input
                    if (isPlayerVsPlayer)
                    {
                        turnCompleted = HandlePlayerInput();
                    }
                    else
                    {
                        HandleAITurn();
                        turnCompleted = true;
                    }
                }

                if (turnCompleted && !gameEnded)
                {
                    if (CheckWin(currentPlayerChipChar))
                    {
                        gameEnded = true;
                        DisplayGameStatus();
                        Console.Write($"\n--- ");
                        if (currentPlayerChipChar == playerChipChar) {
                            WriteWithColor(playerChipEmoji, playerChipColor);
                            Console.Write(" (Player) WINS! ---");
                            playerScore++;
                        } else {
                            WriteWithColor(aiChipEmoji, aiChipColor);
                            // Change win message based on game mode
                            Console.Write(isPlayerVsPlayer ? " (Player 2) WINS! ---" : " (AI) WINS! ---");
                            aiScore++;
                        }
                        Console.WriteLine();
                    }
                    else if (IsBoardFull())
                    {
                        gameEnded = true;
                        DisplayGameStatus();
                        Console.WriteLine("\n--- DRAW! Board is full. ---");
                    }
                    else
                    {
                        SwitchPlayer();
                    }
                }
            } // End turn loop

            Console.Write("\nPlay again? (y/n): ");
            string response = Console.ReadLine();
            playAgain = response?.Trim().ToLower() == "y";

        } // End multiple games loop

        Console.WriteLine("\nThanks for playing!");
        ResetConsoleColors(originalFg, originalBg);
    }


    /// <summary>
    /// Asks the player to choose the game mode: Player vs AI or Player vs Player.
    /// Clears the screen after a valid selection.
    /// </summary>
    static void ChooseGameMode()
    {
        Console.Write("Choose game mode: (1) Player vs AI, (2) Player vs Player: ");
        string input = Console.ReadLine()?.Trim();
        while (input != "1" && input != "2")
        {
            Console.Write("Invalid choice. Please enter 1 or 2: ");
            input = Console.ReadLine()?.Trim();
        }
        isPlayerVsPlayer = input == "2";
        Console.Clear(); // Clear the screen after mode selection
    }


    /// <summary>
    /// Resets console colors to the provided defaults.
    /// </summary>
    static void ResetConsoleColors(ConsoleColor fg, ConsoleColor bg)
    {
        Console.ForegroundColor = fg;
        Console.BackgroundColor = bg;
    }


    /// <summary>
    /// Asks the player to choose 'X' or 'O' and sets up player/AI (or Player 2) chip details.
    /// </summary>
    static void ChoosePlayerChip()
    {
        Console.Write($"Choose your representation ({Chip1Char} or {Chip2Char}): ");
        char choice = ' ';

        while (choice != Chip1Char && choice != Chip2Char)
        {
            string input = Console.ReadLine()?.ToUpper();
            if (input?.Length == 1 && (input[0] == Chip1Char || input[0] == Chip2Char))
            {
                choice = input[0];
            }
            else
            {
                Console.Write($"Invalid choice. Please enter {Chip1Char} or {Chip2Char}: ");
            }
        }

        playerChipChar = choice;
        aiChipChar = (playerChipChar == Chip1Char) ? Chip2Char : Chip1Char;

        if (playerChipChar == Chip1Char) {
            playerChipEmoji = Player1Emoji; playerChipColor = Player1Color;
            aiChipEmoji = Player2Emoji; aiChipColor = Player2Color;
        } else {
            playerChipEmoji = Player2Emoji; playerChipColor = Player2Color;
            aiChipEmoji = Player1Emoji; aiChipColor = Player1Color;
        }

        Console.Write($"You are ");
        WriteWithColor(playerChipEmoji, playerChipColor);
        if (isPlayerVsPlayer)
        {
            Console.Write(". Second player is ");
            WriteWithColor(aiChipEmoji, aiChipColor);
            Console.WriteLine(".");
        }
        else
        {
            Console.Write(". AI is ");
            WriteWithColor(aiChipEmoji, aiChipColor);
            Console.WriteLine(".");
        }
        Console.WriteLine("---------------------------------");
    }


    /// <summary>
    /// Sets up the internal board state with empty characters.
    /// </summary>
    static void InitializeBoard()
    {
        cursorPosition = Columns / 2;

        for (int c = 0; c < Columns; c++)
        {
            board[c] = new char[Rows];
            for (int r = 0; r < Rows; r++)
            {
                board[c][r] = EmptyChar;
            }
        }
    }


    /// <summary>
    /// Helper function to write text with specific colors, then resets to defaults.
    /// </summary>
    static void WriteWithColor(string text, ConsoleColor foreground, ConsoleColor background = DefaultBackColor)
    {
        ConsoleColor originalFg = Console.ForegroundColor;
        ConsoleColor originalBg = Console.BackgroundColor;

        Console.ForegroundColor = foreground;
        Console.BackgroundColor = background;
        Console.Write(text);

        Console.ForegroundColor = originalFg;
        Console.BackgroundColor = originalBg;
    }


    /// <summary>
    /// Clears the screen and displays the entire game status: Score, Board, Turn.
    /// Draws a grid with Dark Blue frame/lines and Black empty slots.
    /// </summary>
    static void DisplayGameStatus()
    {
        ConsoleColor originalBg = Console.BackgroundColor;
        ConsoleColor originalFg = Console.ForegroundColor;
        Console.Clear();

        // Display Score
        Console.Write($"Score: Player ");
        WriteWithColor(playerChipEmoji, playerChipColor);
        Console.Write($"={playerScore} | ");
        if (isPlayerVsPlayer)
        {
            Console.Write("Player 2 ");
            Console.Write($"={aiScore}");
        }
        else
        {
            Console.Write("AI ");
            Console.Write($"={aiScore}");
        }
        Console.WriteLine();
        Console.WriteLine("--------------------------------");

        // Display Cursor ('V')
        Console.Write(" ");
        for (int c = 0; c < Columns; c++)
        {
            WriteWithColor(c == cursorPosition ? "  V   " : "    ", ConsoleColor.Green);
        }
        Console.WriteLine();

        // Display Column Numbers
        Console.Write(" ");
        for (int c = 0; c < Columns; c++)
        {
            Console.Write($"  {c + 1} ");
        }
        Console.WriteLine();
        
        // Display Top Border of Grid
        WriteWithColor("+", DefaultForeColor, GridFrameColor);
        for (int c = 0; c < Columns; c++)
        {
            WriteWithColor("----", DefaultForeColor, GridFrameColor);
        }
        WriteWithColor("+", DefaultForeColor, GridFrameColor);
        Console.WriteLine();

        // Display the Board Grid (Top row first)
        for (int r = Rows - 1; r >= 0; r--)
        {
            // Draw Row Content with vertical separators
            WriteWithColor("|", DefaultForeColor, GridFrameColor); // Left edge

            for (int c = 0; c < Columns; c++)
            {
                char internalChip = board[c][r];
                ConsoleColor currentBg = EmptySlotBackColor; // Black background for slots

                Console.BackgroundColor = currentBg;
                Console.Write(" "); // Left padding

                // Write chip or empty space
                if (internalChip == playerChipChar)
                {
                    WriteWithColor(playerChipEmoji, playerChipColor, currentBg);
                }
                else if (internalChip == aiChipChar)
                {
                    WriteWithColor(aiChipEmoji, aiChipColor, currentBg);
                }
                else
                {
                    WriteWithColor(EmptySlotDisplay, DefaultForeColor, currentBg);
                }

                Console.BackgroundColor = currentBg;
                Console.Write(" "); // Right padding
                Console.BackgroundColor = originalBg; // Reset for the separator line

                // Draw vertical separator between columns
                WriteWithColor("|", DefaultForeColor, GridFrameColor);
            }
            Console.WriteLine();

            // Draw Horizontal Row Separator
            WriteWithColor("+", DefaultForeColor, GridFrameColor);
            for (int c = 0; c < Columns; c++)
            {
                WriteWithColor("----", DefaultForeColor, GridFrameColor);
            }
            WriteWithColor("+", DefaultForeColor, GridFrameColor);
            Console.WriteLine();
        }

        // Reset colors fully before writing turn info
        Console.ForegroundColor = originalFg;
        Console.BackgroundColor = originalBg;

        // Display Turn Info
        Console.Write($"\nTurn: ");
        if (isPlayerTurn)
        {
            WriteWithColor(playerChipEmoji, playerChipColor);
            Console.Write(" (Player)");
        }
        else
        {
            WriteWithColor(aiChipEmoji, aiChipColor);
            Console.Write(isPlayerVsPlayer ? " (Player 2)" : " (AI)");
        }
        Console.WriteLine();
    }


    /// <summary>
    /// Handles player input. Returns true if Enter or Q is pressed, false otherwise.
    /// </summary>
    static bool HandlePlayerInput()
    {
        Console.Write("Move Column (LEFT/RIGHT), Drop Chip (ENTER), Quit (Q): ");
        ConsoleKeyInfo keyInfo = Console.ReadKey(true);

        bool turnActionCompleted = false;

        switch (keyInfo.Key)
        {
            case ConsoleKey.LeftArrow:
                if (cursorPosition > 0) cursorPosition--;
                break;

            case ConsoleKey.RightArrow:
                if (cursorPosition < Columns - 1) cursorPosition++;
                break;

            case ConsoleKey.Enter:
                if (!TryDropChip(cursorPosition, currentPlayerChipChar))
                {
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write("Column full! Try again.        ");
                    Thread.Sleep(1000);
                }
                turnActionCompleted = true;
                break;

            case ConsoleKey.Q:
                Console.WriteLine("\nQuitting game.");
                gameEnded = true;
                turnActionCompleted = true;
                break;

            // Default case ignores other keys silently
        }

        return turnActionCompleted;
    }


    /// <summary>
    /// Handles the AI's turn: Gets move, places chip.
    /// </summary>
    static void HandleAITurn()
    {
        Console.WriteLine("\nAI is thinking...");
        Thread.Sleep(1000);

        int aiColumn = ChooseAIColumn();

        if (aiColumn != -1)
        {
            Console.WriteLine($"AI chooses column {aiColumn + 1}.");
            Thread.Sleep(500);
            TryDropChip(aiColumn, aiChipChar);
        }
        else
        {
            Console.WriteLine("AI Error: No valid column found? Board might be full.");
            gameEnded = true;
        }
        Thread.Sleep(1000);
    }


    /// <summary>
    /// AI Logic: Check win, block win, pick preferred/random. Uses internal characters.
    /// </summary>
    static int ChooseAIColumn()
    {
        // 1. Check AI win
        for (int c = 0; c < Columns; c++)
        {
            if (CanPlace(c))
            {
                int r = FindLowestRow(c);
                board[c][r] = aiChipChar;
                bool won = CheckWin(aiChipChar);
                board[c][r] = EmptyChar;
                if (won) return c;
            }
        }
        // 2. Check Player win block
        for (int c = 0; c < Columns; c++)
        {
            if (CanPlace(c))
            {
                int r = FindLowestRow(c);
                board[c][r] = playerChipChar;
                bool blocked = CheckWin(playerChipChar);
                board[c][r] = EmptyChar;
                if (blocked) return c;
            }
        }
        // 3. Pick preferred
        List<int> preferred = new List<int> { 3, 4, 2, 5, 1, 6, 0 };
        foreach (int c in preferred)
        {
            if (CanPlace(c)) return c;
        }
        // 4. Pick any valid (fallback)
        for (int c = 0; c < Columns; c++)
        {
            if (CanPlace(c)) return c;
        }
        return -1;
    }


    /// <summary>
    /// Finds the lowest empty row (0-based) in a column, or -1 if full/invalid.
    /// </summary>
    static int FindLowestRow(int column)
    {
        if (column < 0 || column >= Columns) return -1;
        for (int r = 0; r < Rows; r++)
        {
            if (board[column][r] == EmptyChar) return r;
        }
        return -1;
    }


    /// <summary>
    /// Checks if a column is valid and not full.
    /// </summary>
    static bool CanPlace(int column)
    {
        return column >= 0 && column < Columns && board[column][Rows - 1] == EmptyChar;
    }


    /// <summary>
    /// Places internal chip char in lowest slot of column. Returns true if successful.
    /// </summary>
    static bool TryDropChip(int column, char chipChar)
    {
        int row = FindLowestRow(column);
        if (row != -1)
        {
            board[column][row] = chipChar;
            return true;
        }
        return false;
    }


    /// <summary>
    /// Checks for 4-in-a-row for the given internal chip char.
    /// </summary>
    static bool CheckWin(char chipChar)
    {
        // Check horizontal (--)
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c <= Columns - 4; c++)
            {
                if (board[c][r] == chipChar && board[c + 1][r] == chipChar && board[c + 2][r] == chipChar && board[c + 3][r] == chipChar)
                    return true;
            }
        }
        // Check vertical (|)
        for (int c = 0; c < Columns; c++)
        {
            for (int r = 0; r <= Rows - 4; r++)
            {
                if (board[c][r] == chipChar && board[c][r + 1] == chipChar && board[c][r + 2] == chipChar && board[c][r + 3] == chipChar)
                    return true;
            }
        }
        // Check diagonal (/) up-right
        for (int c = 0; c <= Columns - 4; c++)
        {
            for (int r = 0; r <= Rows - 4; r++)
            {
                if (board[c][r] == chipChar && board[c + 1][r + 1] == chipChar && board[c + 2][r + 2] == chipChar && board[c + 3][r + 3] == chipChar)
                    return true;
            }
        }
        // Check diagonal (\) down-right
        for (int c = 0; c <= Columns - 4; c++)
        {
            for (int r = 3; r < Rows; r++)
            {
                if (board[c][r] == chipChar && board[c + 1][r - 1] == chipChar && board[c + 2][r - 2] == chipChar && board[c + 3][r - 3] == chipChar)
                    return true;
            }
        }
        return false;
    }


    /// <summary>
    /// Checks if the board is completely full.
    /// </summary>
    static bool IsBoardFull()
    {
        for (int c = 0; c < Columns; c++)
        {
            if (board[c][Rows - 1] == EmptyChar) return false;
        }
        return true;
    }


    /// <summary>
    /// Switches the turn between player and AI (or Player 2 in PvP).
    /// </summary>
    static void SwitchPlayer()
    {
        isPlayerTurn = !isPlayerTurn;
        currentPlayerChipChar = isPlayerTurn ? playerChipChar : aiChipChar;
    }
    
    /// <summary>
    /// Displays an animated splash screen with a simulated Connect 4 game where red wins.
    /// </summary>
    static void ShowSplashScreen()
    {
        Console.Clear();

        //Set board dimensions: 6 rows x 7 columns
        int rows = 6;
        int cols = 7;

        //Create the board
        string[,] board = new string[rows, cols];

        //Initialize board with empty spaces
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                board[r, c] = " ";

        //Define the move sequence leading to red’s diagonal win
        (int col, string color)[] moves = new (int, string)[]
        {
            (0, "R"), (1, "Y"),
            (1, "R"), (2, "Y"),
            (2, "R"), (3, "Y"),
            (2, "R"), (3, "Y"),
            (3, "R"), (5, "Y"),
            (3, "R") //Final move gives red a diagonal win
        };

        //Play the moves with falling animation
        foreach (var move in moves)
        {
            AnimateChipDrop(board, move.col, move.color); //Drop chip with animation
            Thread.Sleep(50); //Pause between moves
        }
        
        //Wait before showing logo
        Thread.Sleep(1000);

        //Show animated Connect 4Jet splash
        ShowAsciiSplash();
    }

    /// <summary>
    /// Animates a chip dropping visually down the column one cell at a time.
    /// </summary>
    static void AnimateChipDrop(string[,] board, int column, string chip)
    {
        //Find the target row where the chip will land
        int targetRow = -1;
        for (int r = board.GetLength(0) - 1; r >= 0; r--)
        {
            if (board[r, column] == " ")
            {
                targetRow = r;
                break;
            }
        }

        //Return if column is full
        if (targetRow == -1) return;

        //Drop chip row by row to simulate falling
        for (int r = 0; r <= targetRow; r++)
        {
            if (r > 0) board[r - 1, column] = " "; //Clear previous chip
            board[r, column] = chip; //Set current position
            DrawBoard(board); //Redraw board
            Thread.Sleep(50); //Pause to animate fall
        }
    }

    /// <summary>
    /// Draws the Connect 4 board and optional message (e.g. Red Wins).
    /// </summary>
    static void DrawBoard(string[,] board, string message = null)
    {
        Console.Clear();

        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        //Display optional message centered above the board
        if (!string.IsNullOrEmpty(message))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"        {message}\n");
            Console.ResetColor();
        }

        //Draw from top row to bottom row
        for (int r = 0; r < rows; r++)
        {
            Console.Write("|");
            for (int c = 0; c < cols; c++)
            {
                string chip = board[r, c];

                //Set color for chips
                if (chip == "R")
                    Console.ForegroundColor = ConsoleColor.Red;
                else if (chip == "Y")
                    Console.ForegroundColor = ConsoleColor.Yellow;
                else
                    Console.ResetColor();

                Console.Write($" {chip} "); //Draw chip
                Console.ResetColor();
                Console.Write("|");
            }
            Console.WriteLine(); //Move to next line
        }

        //Draw board bottom border
        Console.WriteLine(" -----------------------------");
    }

    /// <summary>
    /// Displays the Connect 4Jet splash with animated color changes.
    /// </summary>
    static void ShowAsciiSplash()
    {
        Console.Clear();

        //Define splash ASCII lines
        string[] splashLines = new string[]
        {
            "                                                           >=>                          >=>             >=>   ",
            "                                                           >=>              >=>         >=>             >=>   ",
            "   >==>    >=>     >==>>==>  >==>>==>    >==>       >==> >=>>==>           >>=>         >=>   >==>    >=>>==> ",
            " >=>     >=>  >=>   >=>  >=>  >=>  >=> >>   >=>   >=>      >=>            > >=>         >=> >>   >=>    >=>   ",
            ">=>     >=>    >=>  >=>  >=>  >=>  >=> >>===>>=> >=>       >=>          >=> >=>         >=> >>===>>=>   >=>   ",
            " >=>     >=>  >=>   >=>  >=>  >=>  >=> >>         >=>      >=>         >===>>=>>=> >>   >=> >>          >=>   ",
            "   >==>    >=>     >==>  >=> >==>  >=>  >====>      >==>    >=>             >=>     >===>    >====>      >=>  "
        };

        string pressEnterText = "PRESS ENTER TO START THE GAME...";
        int pressEnterY = splashLines.Length + 2;

        ConsoleColor[] colors = new ConsoleColor[]
        {
            ConsoleColor.Red,
            ConsoleColor.Yellow,
            ConsoleColor.Cyan,
            ConsoleColor.Green,
            ConsoleColor.Magenta
        };

        int colorIndex = 0;

        // Make input detection non-blocking
        Console.TreatControlCAsInput = true;
        Console.CursorVisible = false;

        DateTime lastUpdate = DateTime.Now;

        // Loop until Enter is pressed
        while (true)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Enter)
                    break; // Exit animation on Enter
            }

            // Only update every 200ms
            if ((DateTime.Now - lastUpdate).TotalMilliseconds >= 200)
            {
                Console.Clear();

                Console.ForegroundColor = colors[colorIndex];
                foreach (string line in splashLines)
                {
                    Console.WriteLine(line);
                }

                Console.ResetColor();

                // Show "Press ENTER" message in white
                Console.SetCursorPosition((Console.WindowWidth - pressEnterText.Length) / 2, pressEnterY);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(pressEnterText);
                Console.ResetColor();

                // Cycle to the next color
                colorIndex = (colorIndex + 1) % colors.Length;
                lastUpdate = DateTime.Now;
            }

            Thread.Sleep(10); // Small delay to reduce CPU usage
        }

        Console.Clear();
        Console.CursorVisible = true;
    }
}
