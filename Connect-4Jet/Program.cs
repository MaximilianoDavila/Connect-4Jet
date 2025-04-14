// Made by Jet Brainers that are Daniel E. Cintron, Maximiliano Davila, George Lopez to create connect - 4Jetusing System;
using System.Text;       // For Encoding

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
    const ConsoleColor DefaultForeColor = ConsoleColor.Gray; // Default console text color


    //Player Names
    static string player1Name;
    static string player2Name;

    //Scoreboard values
    static int player1Wins = 0;
    static int player1Losses = 0;
    static int player1Draws = 0;

    static int player2Wins = 0;
    static int player2Losses = 0;
    static int player2Draws = 0;


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
        //Player names
        GetPlayerNames();

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
                DisplayGameStatus(); // This now calls the coordinate-based version

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
                        DisplayGameStatus(); // Show final board state
                        Console.Write($"\n--- ");
                        if (currentPlayerChipChar == playerChipChar)
                        {
                            WriteWithColor(playerChipEmoji, playerChipColor);
                            Console.Write($" ({player1Name}) WINS! ---"); // Player 1 wins
                            playerScore++;
                        }
                        else
                        {
                            WriteWithColor(aiChipEmoji, aiChipColor);
                            Console.Write(isPlayerVsPlayer
                                ? $" ({player2Name}) WINS! ---"
                                : " (AI) WINS! ---"); // Player 2 or AI wins
                            aiScore++;
                        }

                        Console.WriteLine();
                    }
                    else if (IsBoardFull())
                    {
                        gameEnded = true;
                        DisplayGameStatus(); // Show final board state
                        Console.WriteLine("\n--- DRAW! Board is full. ---");
                    }
                    else
                    {
                        SwitchPlayer();
                    }
                }
            } // End turn loop

            //Adds wins,loses and draws to the respective players scores.
            if (currentPlayerChipChar == playerChipChar)
            {
                if (gameEnded)
                {
                    player1Wins++;
                    player2Losses++;
                }
            }
            else if (currentPlayerChipChar == aiChipChar)
            {
                if (gameEnded)
                {
                    player2Wins++;
                    player1Losses++;
                }
            }
            else
            {
                if (gameEnded)
                {
                    player1Draws++;
                    player2Draws++;
                }
            }

            //Shows scoreboard
            DisplayScoreboard();


            //Ask for replay
            Console.Write("\nPlay again? (y/n): ");
            string response = Console.ReadLine();
            playAgain = response?.Trim().ToLower() == "y";


        } // End multiple games loop

        Console.WriteLine("\nThanks for playing!");
        ResetConsoleColors(originalFg, originalBg);
    }

    //Scoreboard of each players wins, loses and draws
    static void DisplayScoreboard()
    {
        Console.WriteLine("\n--- Scoreboard ---");
        Console.WriteLine($"{player1Name}: Wins = {player1Wins}, Losses = {player1Losses}, Draws = {player1Draws}");
        Console.WriteLine($"{player2Name}: Wins = {player2Wins}, Losses = {player2Losses}, Draws = {player2Draws}");
        Console.WriteLine("------------------");
    }

    /// <summary>
    /// Asks the player to choose the game mode: Player vs AI or Player vs Player.
    /// Clears the screen after a valid selection.
    /// </summary>
    static void ChooseGameMode()
    {
        Console.WriteLine("\nWelcome!! Here are your choices in this game which are:");

        Console.WriteLine("\n________________________________________________________________");
        Console.WriteLine("|                              Menu                            |");
        Console.WriteLine("________________________________________________________________");
        Console.WriteLine("|             Number#            |           Options           |");
        Console.WriteLine("|_______________________________________________________________");
        Console.WriteLine("|                1               |       Player vs AI          |");
        Console.WriteLine("________________________________________________________________");
        Console.WriteLine("|                2               |       Player vs Player      |");
        Console.WriteLine("|_______________________________________________________________");

        Console.Write("\nChoose game mode: (1) Player vs AI, (2) Player vs Player: ");
        string input = Console.ReadLine()?.Trim();
        while (input != "1" && input != "2")
        {
            Console.Write("\nInvalid choice. Please enter 1 or 2: ");
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

        if (playerChipChar == Chip1Char)
        {
            playerChipEmoji = Player1Emoji;
            playerChipColor = Player1Color;
            aiChipEmoji = Player2Emoji;
            aiChipColor = Player2Color;
        }
        else
        {
            playerChipEmoji = Player2Emoji;
            playerChipColor = Player2Color;
            aiChipEmoji = Player1Emoji;
            aiChipColor = Player1Color;
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

    /// </summary>
    static void DisplayGameStatus()
    {
        ConsoleColor originalBg = Console.BackgroundColor;
        ConsoleColor originalFg = Console.ForegroundColor;
        Console.Clear();
        Console.CursorVisible = false; // Hide console cursor while drawing

        // --- Draw Static Info ---
        Console.SetCursorPosition(0, 0); // Ensure we start at the top left
        Console.Write($"Score: Player ");
        WriteWithColor(playerChipEmoji, playerChipColor);
        Console.Write($"={playerScore} | ");
        if (isPlayerVsPlayer)
        {
            Console.Write("Player 2 ");
            WriteWithColor(aiChipEmoji, aiChipColor);
            Console.Write($"={aiScore}");
        }
        else
        {
            Console.Write("AI ");
            WriteWithColor(aiChipEmoji, aiChipColor);
            Console.Write($"={aiScore}");
        }

        Console.WriteLine();
        Console.WriteLine("-----------------------------"); // Width 29 approx

        // Calculate where the board drawing starts vertically
        int boardTopY = Console.CursorTop; // Get the current line index

        // Display Cursor ('V') - Use SetCursorPosition for precise placement

        int cursorScreenX = 1 + cursorPosition * 4 + 1; // 1 (border) + offset + 1 (center in cell)
        try
        {
            Console.SetCursorPosition(cursorScreenX, boardTopY);
        }
        catch
        {
        }

        WriteWithColor("V", ConsoleColor.Green); // Draw the V
        boardTopY++; // Move down one line

        // Display Column Numbers - Use SetCursorPosition
        try
        {
            Console.SetCursorPosition(0, boardTopY);
        }
        catch
        {
            /* ignore error */
        }

        Console.Write(" "); // Initial indent
        for (int c = 0; c < Columns; c++)
        {
            // Center number in 4 spaces " {N} "
            Console.Write($" {c + 1}  ");
        }

        boardTopY++; // Move down one line

        // --- Draw the Grid Structure ---
        // Draw top border: +---+---+...+
        try
        {
            Console.SetCursorPosition(0, boardTopY);
        }
        catch
        {
            /* ignore error */
        }

        WriteWithColor("+", DefaultForeColor, GridFrameColor);
        for (int c = 0; c < Columns; c++)
        {
            WriteWithColor("---", DefaultForeColor, GridFrameColor);
            WriteWithColor("+", DefaultForeColor, GridFrameColor);
        }

        boardTopY++; // Move down one line

        // Draw rows with empty slots and separators
        for (int r = Rows - 1; r >= 0; r--)
        {
            // Draw row content line (| E | E |...) E=" { } "
            try
            {
                Console.SetCursorPosition(0, boardTopY);
            }
            catch
            {
                /* ignore error */
            }

            WriteWithColor("|", DefaultForeColor, GridFrameColor); // Left edge
            for (int c = 0; c < Columns; c++)
            {
                ConsoleColor currentBg = EmptySlotBackColor;
                Console.BackgroundColor = currentBg;
                Console.Write(" "); // Left pad
                WriteWithColor(EmptySlotDisplay, DefaultForeColor, currentBg); // Empty space " "
                Console.Write(" "); // Right pad
                Console.BackgroundColor = originalBg; // Reset for separator
                WriteWithColor("|", DefaultForeColor, GridFrameColor); // Separator
            }

            boardTopY++; // Move down one line

            // Draw Horizontal Row Separator: +---+---+...+
            try
            {
                Console.SetCursorPosition(0, boardTopY);
            }
            catch
            {
                /* ignore error */
            }

            WriteWithColor("+", DefaultForeColor, GridFrameColor);
            for (int c = 0; c < Columns; c++)
            {
                WriteWithColor("---", DefaultForeColor, GridFrameColor);
                WriteWithColor("+", DefaultForeColor, GridFrameColor);
            }

            boardTopY++; // Move down one line
        }

        // --- Place Emojis on the Grid ---
        // Calculate Y position of the *first content row's* character placement
        int firstContentRowY = 5; // Score(1)+Sep(1)+Cursor(1)+Numbers(1)+TopBorder(1)=5 lines above first content row
        for (int r = Rows - 1; r >= 0; r--)
        {
            for (int c = 0; c < Columns; c++)
            {
                char internalChip = board[c][r];
                if (internalChip != EmptyChar)
                {
                    // Calculate screen position for the chip emoji "E" within "| {E} |"
                    int chipScreenX = 1 + c * 4 + 1; // 1(|) + c*4 + 1(space)
                    int chipScreenY = firstContentRowY + (Rows - 1 - r) * 2; // Each board row takes 2 screen lines

                    try
                    {
                        // Add try-catch for safety
                        Console.SetCursorPosition(chipScreenX, chipScreenY);
                        if (internalChip == playerChipChar)
                        {
                            WriteWithColor(playerChipEmoji, playerChipColor, EmptySlotBackColor);
                        }
                        else if (internalChip == aiChipChar)
                        {
                            WriteWithColor(aiChipEmoji, aiChipColor, EmptySlotBackColor);
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        /* ignore if window too small */
                    }
                }
            }
        }

        // --- Display Turn Info Below Board ---
        // Ensure cursor is below the board
        try
        {
            Console.SetCursorPosition(0, boardTopY);
        }
        catch
        {
            /* ignore error */
        }

        Console.WriteLine(); // Add a blank line

        Console.ForegroundColor = originalFg; // Reset colors
        Console.BackgroundColor = originalBg;


        //Adjusted turn to say name
        Console.Write($"Turn: ");
        if (isPlayerTurn)
        {
            WriteWithColor(playerChipEmoji, playerChipColor);
            Console.Write($" ({player1Name})"); // Display Player 1's name
        }
        else
        {
            WriteWithColor(aiChipEmoji, aiChipColor);
            if (isPlayerVsPlayer)
            {
                Console.Write($" ({player2Name})"); // Display Player 2's name
            }
            else
            {
                Console.Write($" (AI)"); // For Player vs AI mode, show AI
            }
        }

        Console.WriteLine(); // Ensure newline after turn info

        Console.CursorVisible = true; // Restore console cursor
    }

    /// <summary>
    /// Handles player input. Returns true if Enter or Q is pressed, false otherwise.
    /// </summary>
    static bool HandlePlayerInput()
    {
        // Determine whose chip to use based on whose turn it is (for PvP)
        char chipToDrop = isPlayerTurn ? playerChipChar : aiChipChar;
        // Display correct player number in prompt
        string playerPrompt = isPlayerVsPlayer ? $"{(isPlayerTurn ? player1Name : player2Name)}" : "Player";

        Console.Write($"{playerPrompt} - Move (LEFT/RIGHT), Drop (ENTER), Quit to Menu(Q): ");
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
                // Use the correct chip for the current player (handles PvP)
                if (!TryDropChip(cursorPosition, chipToDrop))
                {
                    try
                    {
                        Console.SetCursorPosition(0, Console.CursorTop);
                    }
                    catch
                    {
                        /* ignore */
                    }

                    Console.Write("Column full! Try again.        ");
                    Thread.Sleep(1000);
                }

                turnActionCompleted = true;
                break;

            case ConsoleKey.Q:
                Console.WriteLine("\nReturning to main menu...\n");
                // Reset game state and return to game mode selection
                ChooseGameMode();
                Console.Clear();
                turnActionCompleted = true;
                break;
        }

        return turnActionCompleted;
    }


    /// <summary>
    /// Handles the AI's turn: Gets move, places chip.
    /// </summary>
    static void HandleAITurn()
    {
        // This function is only called if isPlayerVsPlayer is false
        Console.WriteLine("\nAI is thinking...");
        Thread.Sleep(1000);

        int aiColumn = ChooseAIColumn();

        if (aiColumn != -1)
        {
            Console.WriteLine($"AI chooses column {aiColumn + 1}.");
            Thread.Sleep(500);
            TryDropChip(aiColumn, aiChipChar); // AI always uses aiChipChar
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
        // Check horizontal (left to right)
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c <= Columns - 4; c++)
            {
                if (board[c][r] == chipChar && board[c + 1][r] == chipChar && board[c + 2][r] == chipChar &&
                    board[c + 3][r] == chipChar)
                    return true;
            }
        }

        // Check vertical (top to bottom)
        for (int c = 0; c < Columns; c++)
        {
            for (int r = 0; r <= Rows - 4; r++)
            {
                if (board[c][r] == chipChar && board[c][r + 1] == chipChar && board[c][r + 2] == chipChar &&
                    board[c][r + 3] == chipChar)
                    return true;
            }
        }

        // Check diagonal (up-right /)
        for (int c = 0; c <= Columns - 4; c++)
        {
            for (int r = 0; r <= Rows - 4; r++)
            {
                if (board[c][r] == chipChar && board[c + 1][r + 1] == chipChar && board[c + 2][r + 2] == chipChar &&
                    board[c + 3][r + 3] == chipChar)
                    return true;
            }
        }

        // Check diagonal (down-right \)
        for (int c = 0; c <= Columns - 4; c++)
        {
            for (int r = 3; r < Rows; r++)
            {
                if (board[c][r] == chipChar && board[c + 1][r - 1] == chipChar && board[c + 2][r - 2] == chipChar &&
                    board[c + 3][r - 3] == chipChar)
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
    /// Prompts the players to enter their names at the start of the game.
    /// </summary>
    static void GetPlayerNames()
    {
        Console.Write("\nEnter name for Player 1: ");
        player1Name = Console.ReadLine();
        Console.Write("\nEnter name for Player 2: ");
        player2Name = Console.ReadLine();
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
        string[,] splashBoard = new string[rows, cols]; // Use a separate board for splash

        //Initialize board with empty spaces
        for (int r = 0; r < rows; r++)
        for (int c = 0; c < cols; c++)
            splashBoard[r, c] = " ";

        //Define the move sequence leading to red’s diagonal win
        (int col, string color)[] moves = new (int, string)[]
        {
            (0, "R"), (1, "Y"),
            (1, "R"), (2, "Y"),
            (2, "R"), (3, "Y"),
            (2, "R"), (3, "Y"), // Note: This sequence has duplicate moves, maybe intentional for demo?
            (3, "R"), (5, "Y"),
            (3, "R") //Final move gives red a diagonal win
        };

        //Play the moves with falling animation
        foreach (var move in moves)
        {
            AnimateChipDrop(splashBoard, move.col, move.color); // Pass splashBoard
            Thread.Sleep(50); //Pause between moves
        }

        //Show "Red Wins!" message on the board
        DrawBoard(splashBoard, "Red Wins!"); // Pass splashBoard
        Thread.Sleep(1500); // Wait before showing logo


        //Show animated Connect 4Jet splash
        ShowAsciiSplash();
    }

    /// <summary>
    /// Animates a chip dropping visually down the column one cell at a time.
    /// Uses a specific board passed to it.
    /// </summary>
    static void AnimateChipDrop(string[,] currentBoard, int column, string chip) // Takes board as parameter
    {
        //Find the target row where the chip will land
        int targetRow = -1;
        for (int r = currentBoard.GetLength(0) - 1; r >= 0; r--) // Use currentBoard
        {
            if (currentBoard[r, column] == " ") // Use currentBoard
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
            if (r > 0) currentBoard[r - 1, column] = " "; // Clear previous chip on currentBoard
            currentBoard[r, column] = chip; // Set current position on currentBoard
            DrawBoard(currentBoard); // Redraw the passed board (currentBoard)
            Thread.Sleep(50); // Pause to animate fall
        }
    }

    /// <summary>
    /// Draws the Connect 4 board passed to it and an optional message.
    /// Uses simple R/Y characters for splash screen compatibility.
    /// </summary>
    static void DrawBoard(string[,] boardToDraw, string message = null) // Takes board as parameter
    {
        Console.Clear();

        int rows = boardToDraw.GetLength(0); // Use passed board dimensions
        int cols = boardToDraw.GetLength(1); // Use passed board dimensions

        //Display optional message centered above the board
        if (!string.IsNullOrEmpty(message))
        {
            Console.ForegroundColor = ConsoleColor.White; // Use white for message
            // Basic centering attempt
            int padding = (Console.WindowWidth - message.Length) / 2;
            Console.WriteLine($"{new string(' ', Math.Max(0, padding))}{message}\n");
            Console.ResetColor();
        }

        //Draw top border
        Console.WriteLine(" +---+---+---+---+---+---+---+");

        //Draw board content from top row to bottom row
        for (int r = 0; r < rows; r++)
        {
            Console.Write(" |"); // Left edge
            for (int c = 0; c < cols; c++)
            {
                string chip = boardToDraw[r, c]; // Use passed board

                //Set color for chips
                if (chip == "R")
                    Console.ForegroundColor = ConsoleColor.Red;
                else if (chip == "Y")
                    Console.ForegroundColor = ConsoleColor.Yellow;
                else
                    Console.ResetColor(); // Use default for empty space " "

                Console.Write($" {chip} "); // Draw chip/space (Width 3)

                Console.ResetColor();
                Console.Write("|"); // Vertical separator / Right edge of cell
            }

            Console.WriteLine(); //Move to next line
            //Draw horizontal separator
            Console.WriteLine(" +---+---+---+---+---+---+---+");
        }
        // Bottom border is drawn by the last separator loop

    }

    /// <summary>
    /// Displays the Connect 4Jet splash with animated color changes.
    /// </summary>
    static void ShowAsciiSplash()
    {
        Console.Clear();

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
        // Calculate Y position relative to console height if possible, otherwise place below ASCII art
        int pressEnterY = Math.Min(Console.WindowHeight - 2, splashLines.Length + 2);
        int pressEnterX = Math.Max(0, (Console.WindowWidth - pressEnterText.Length) / 2);


        ConsoleColor[] colors = new ConsoleColor[]
        {
            ConsoleColor.Red, ConsoleColor.Yellow, ConsoleColor.Cyan,
            ConsoleColor.Green, ConsoleColor.Magenta
        };
        int colorIndex = 0;

        Console.CursorVisible = false; // Hide cursor during animation
        DateTime lastUpdate = DateTime.Now;

        // Loop until Enter is pressed
        while (true)
        {
            // Check for key press without blocking
            if (Console.KeyAvailable)
            {
                // Read the key - true means don't display it
                if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                    break; // Exit loop on Enter
            }

            // Throttle updates to reduce flickering/CPU usage
            if ((DateTime.Now - lastUpdate).TotalMilliseconds >= 200)
            {
                Console.Clear(); // Clear screen for redraw

                // Draw ASCII Art with cycling color
                Console.ForegroundColor = colors[colorIndex];
                for (int i = 0; i < splashLines.Length; i++)
                {
                    // Basic centering attempt for ASCII art lines
                    int linePadding = Math.Max(0, (Console.WindowWidth - splashLines[i].Length) / 2);
                    try
                    {
                        Console.SetCursorPosition(linePadding, i);
                    }
                    catch
                    {
                        /* ignore */
                    }

                    Console.WriteLine(splashLines[i]);
                }

                Console.ResetColor();

                // Draw "Press ENTER" message
                try
                {
                    Console.SetCursorPosition(pressEnterX, pressEnterY);
                }
                catch
                {
                    /* ignore */
                }

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(pressEnterText);
                Console.ResetColor();

                // Cycle color and update time
                colorIndex = (colorIndex + 1) % colors.Length;
                lastUpdate = DateTime.Now;
            }

            Thread.Sleep(20); // Small sleep to prevent tight loop from eating CPU
        }
        Console.CursorVisible = true; // Restore cursor
        Console.Clear(); // Clear splash screen before starting game
    }
}