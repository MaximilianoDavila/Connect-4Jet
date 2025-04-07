// Made by Daniel E. Cintron, Maximiliano Davila, George Lopez
using System;
using System.Text;       
using System.Threading;    
using System.Collections.Generic; 

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

    // Player/AI visual and internal details
    static char playerChipChar;
    static string playerChipEmoji;
    static ConsoleColor playerChipColor;
    static char aiChipChar;
    static string aiChipEmoji;
    static ConsoleColor aiChipColor;

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

        Console.WriteLine(">> Welcome to Connect Four! <<");

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
                    HandleAITurn();
                    turnCompleted = true;
                }

                if (turnCompleted && !gameEnded)
                {
                    if (CheckWin(currentPlayerChipChar))
                    {
                        gameEnded = true;
                        DisplayGameStatus();
                        Console.Write($"\n--- ");
                        if (currentPlayerChipChar == playerChipChar) {
                            WriteWithColor(playerChipEmoji, playerChipColor); Console.Write(" (Player) WINS! ---"); playerScore++;
                        } else {
                            WriteWithColor(aiChipEmoji, aiChipColor); Console.Write(" (AI) WINS! ---"); aiScore++;
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
    /// Resets console colors to the provided defaults.
    /// </summary>
    static void ResetConsoleColors(ConsoleColor fg, ConsoleColor bg)
    {
        Console.ForegroundColor = fg;
        Console.BackgroundColor = bg;
    }


    /// <summary>
    /// Asks the player to choose 'X' or 'O' and sets up player/AI chip details.
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

        Console.Write($"You are "); WriteWithColor(playerChipEmoji, playerChipColor);
        Console.Write(". AI is "); WriteWithColor(aiChipEmoji, aiChipColor);
        Console.WriteLine(".");
        Console.WriteLine("-----------------------------");
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
        Console.Write($"Score: Player "); WriteWithColor(playerChipEmoji, playerChipColor);
        Console.Write($"={playerScore} | AI "); WriteWithColor(aiChipEmoji, aiChipColor);
        Console.WriteLine($"={aiScore}");
        Console.WriteLine("-----------------------------");

        // Display Cursor ('V')
        Console.Write(" ");
        for (int c = 0; c < Columns; c++) { WriteWithColor(c == cursorPosition ? " V " : "   ", ConsoleColor.Green); }
        Console.WriteLine();

        // Display Column Numbers
        Console.Write(" ");
        for (int c = 0; c < Columns; c++) { Console.Write($" {c + 1} "); }
        Console.WriteLine();

        // Display Top Border of Grid
        WriteWithColor("+", DefaultForeColor, GridFrameColor);
        for (int c = 0; c < Columns; c++) { WriteWithColor("---", DefaultForeColor, GridFrameColor); } // Column content width is 3 (" E ")
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

                Console.BackgroundColor = currentBg; Console.Write(" "); // Left padding

                // Write chip or empty space
                if (internalChip == playerChipChar) { WriteWithColor(playerChipEmoji, playerChipColor, currentBg); }
                else if (internalChip == aiChipChar) { WriteWithColor(aiChipEmoji, aiChipColor, currentBg); }
                else { WriteWithColor(EmptySlotDisplay, DefaultForeColor, currentBg); }

                Console.BackgroundColor = currentBg; Console.Write(" "); // Right padding
                Console.BackgroundColor = originalBg; // Reset for the separator line

                // *** ADDED: Draw vertical separator between columns ***
                WriteWithColor("|", DefaultForeColor, GridFrameColor);
            }
            // No need for the final right edge "|" because the loop adds one after the last cell
            Console.WriteLine(); // Move to the next line

            // Draw Horizontal Row Separator using '+' and '---'
            // (This remains the same as the cell content width is still 3)
            WriteWithColor("+", DefaultForeColor, GridFrameColor);
            for (int c = 0; c < Columns; c++) { WriteWithColor("---", DefaultForeColor, GridFrameColor); }
            WriteWithColor("+", DefaultForeColor, GridFrameColor);
            Console.WriteLine();
        }

        // Reset colors fully before writing turn info
        Console.ForegroundColor = originalFg;
        Console.BackgroundColor = originalBg;

        // Display Turn Info
        Console.Write($"\nTurn: ");
        if (isPlayerTurn) { WriteWithColor(playerChipEmoji, playerChipColor); Console.Write(" (Player)"); }
        else { WriteWithColor(aiChipEmoji, aiChipColor); Console.Write(" (AI)"); }
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
                if (!TryDropChip(cursorPosition, playerChipChar))
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
        for (int c = 0; c < Columns; c++) {
            if (CanPlace(c)) {
                int r = FindLowestRow(c); board[c][r] = aiChipChar;
                bool won = CheckWin(aiChipChar);
                board[c][r] = EmptyChar;
                if (won) return c;
            }
        }
        // 2. Check Player win block
        for (int c = 0; c < Columns; c++) {
            if (CanPlace(c)) {
                int r = FindLowestRow(c); board[c][r] = playerChipChar;
                bool blocked = CheckWin(playerChipChar);
                board[c][r] = EmptyChar;
                if (blocked) return c;
            }
        }
        // 3. Pick preferred
        List<int> preferred = new List<int> { 3, 4, 2, 5, 1, 6, 0 };
        foreach (int c in preferred) { if (CanPlace(c)) return c; }

        // 4. Pick any valid (fallback)
        for (int c = 0; c < Columns; c++) { if (CanPlace(c)) return c; }

        return -1;
    }


    /// <summary>
    /// Finds the lowest empty row (0-based) in a column, or -1 if full/invalid.
    /// </summary>
    static int FindLowestRow(int column) {
        if (column < 0 || column >= Columns) return -1;
        for (int r = 0; r < Rows; r++) { if (board[column][r] == EmptyChar) return r; }
        return -1;
    }


    /// <summary>
    /// Checks if a column is valid and not full.
    /// </summary>
    static bool CanPlace(int column) {
        return column >= 0 && column < Columns && board[column][Rows - 1] == EmptyChar;
    }


    /// <summary>
    /// Places internal chip char in lowest slot of column. Returns true if successful.
    /// </summary>
    static bool TryDropChip(int column, char chipChar) {
        int row = FindLowestRow(column);
        if (row != -1) { board[column][row] = chipChar; return true; }
        return false;
    }


    /// <summary>
    /// Checks for 4-in-a-row for the given internal chip char.
    /// </summary>
    static bool CheckWin(char chipChar) {
        // Check horizontal (--)
        for (int r = 0; r < Rows; r++) { for (int c = 0; c <= Columns - 4; c++) { if (board[c][r] == chipChar && board[c + 1][r] == chipChar && board[c + 2][r] == chipChar && board[c + 3][r] == chipChar) return true; } }
        // Check vertical (|)
        for (int c = 0; c < Columns; c++) { for (int r = 0; r <= Rows - 4; r++) { if (board[c][r] == chipChar && board[c][r + 1] == chipChar && board[c][r + 2] == chipChar && board[c][r + 3] == chipChar) return true; } }
        // Check diagonal (/) up-right
        for (int c = 0; c <= Columns - 4; c++) { for (int r = 0; r <= Rows - 4; r++) { if (board[c][r] == chipChar && board[c + 1][r + 1] == chipChar && board[c + 2][r + 2] == chipChar && board[c + 3][r + 3] == chipChar) return true; } }
        // Check diagonal (\) down-right
        for (int c = 0; c <= Columns - 4; c++) { for (int r = 3; r < Rows; r++) { if (board[c][r] == chipChar && board[c + 1][r - 1] == chipChar && board[c + 2][r - 2] == chipChar && board[c + 3][r - 3] == chipChar) return true; } }
        return false;
    }


    /// <summary>
    /// Checks if the board is completely full.
    /// </summary>
    static bool IsBoardFull() {
        for (int c = 0; c < Columns; c++) { if (board[c][Rows - 1] == EmptyChar) return false; }
        return true;
    }


    /// <summary>
    /// Switches the turn between player and AI.
    /// </summary>
    static void SwitchPlayer() {
        isPlayerTurn = !isPlayerTurn;
        currentPlayerChipChar = isPlayerTurn ? playerChipChar : aiChipChar;
    }
}