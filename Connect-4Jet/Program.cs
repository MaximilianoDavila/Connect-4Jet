// Made by Jet Brainers that are Daniel E. Cintron, Maximiliano Davila, George Lopez to create connect - 4Jetusing System;
// Add necessary using directives back for cleaner code
using System;
using System.Text;       // For Encoding
using System.Threading;    // For Thread.Sleep
using System.Collections.Generic; // For List

// Note: System.Runtime.InteropServices is not needed for this version

// This class holds our entire Connect Four game
class Program
{
    // --- Game Settings ---
    const int Columns = 7;
    const int Rows = 6;

    // --- Internal Representation ---
    const char EmptyChar = '_';
    const char Chip1Char = 'X';
    const char Chip2Char = 'O';

    // --- Visual Representation ---
    const string Player1Emoji = "🟡";
    const ConsoleColor Player1Color = ConsoleColor.Yellow;
    const string Player2Emoji = "🔴";
    const ConsoleColor Player2Color = ConsoleColor.Red;
    const string EmptySlotDisplay = " ";
    const ConsoleColor GridFrameColor = ConsoleColor.DarkBlue;
    const ConsoleColor EmptySlotBackColor = ConsoleColor.Black;
    const ConsoleColor DefaultBackColor = ConsoleColor.Black;
    const ConsoleColor DefaultForeColor = ConsoleColor.Gray;


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
    static char[][] board = new char[Columns][];

    // Player/AI or Player2 visual and internal details
    static char playerChipChar;
    static string playerChipEmoji;
    static ConsoleColor playerChipColor;
    static char aiChipChar;
    static string aiChipEmoji;
    static ConsoleColor aiChipColor;

    // Game mode flag
    static bool isPlayerVsPlayer = false;

    // Current game status
    static char currentPlayerChipChar;
    static int cursorPosition;
    static bool isPlayerTurn;
    static bool gameEnded;
    static int playerScore = 0; // Player 1 score
    static int aiScore = 0;     // Player 2 or AI score
    static Random random = new Random();


    // --- Main Program Entry Point ---
    static void Main(string[] args)
    {
        ConsoleColor originalBg = Console.BackgroundColor;
        ConsoleColor originalFg = Console.ForegroundColor;

        Console.OutputEncoding = Encoding.UTF8;

        ShowSplashScreen();

        Console.WriteLine(">> Welcome to Connect Four! <<");
        GetPlayerNames();
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
                // No need to draw here, HandleInput/HandleAI will draw before animation starts
                // DisplayGameStatus(); // Display state *before* getting input

                bool turnCompleted = false; // Determines if turn action (drop/quit) happened

                if (isPlayerTurn)
                {
                    // Display before waiting for input
                    DisplayGameStatus();
                    turnCompleted = HandlePlayerInput(); // Handles input, drop attempt, and animation
                }
                else // AI or Player 2 turn
                {
                    if (isPlayerVsPlayer)
                    {
                         // Display before waiting for input
                        DisplayGameStatus();
                        turnCompleted = HandlePlayerInput(); // Player 2 uses same input logic
                    }
                    else
                    {
                        // Display before AI "thinks"
                        DisplayGameStatus();
                        HandleAITurn(); // Handles AI logic, drop attempt, and animation
                        turnCompleted = true; // AI always completes an action if possible
                    }
                }

                // Post-Turn Logic (Win/Draw Check, Switch Player)
                if (turnCompleted && !gameEnded) // Check gameEnded again in case of Quit (Q)
                {
                    if (CheckWin(currentPlayerChipChar))
                    {
                        gameEnded = true;
                        if (currentPlayerChipChar == playerChipChar) { playerScore++; player1Wins++; player2Losses++; }
                        else { aiScore++; player2Wins++; player1Losses++; }

                        DisplayGameStatus(); // Show final board after win
                        Console.Write($"\n--- ");
                        if (currentPlayerChipChar == playerChipChar) {
                            WriteWithColor(playerChipEmoji, playerChipColor); Console.Write($" ({player1Name}) WINS! ---");
                        } else {
                            WriteWithColor(aiChipEmoji, aiChipColor); Console.Write(isPlayerVsPlayer ? $" ({player2Name}) WINS! ---" : " (AI) WINS! ---");
                        }
                        Console.WriteLine();
                    }
                    else if (IsBoardFull())
                    {
                        gameEnded = true;
                        player1Draws++; player2Draws++;
                        DisplayGameStatus(); // Show final board after draw
                        Console.WriteLine("\n--- DRAW! Board is full. ---");
                    }
                    else
                    {
                        SwitchPlayer(); // Only switch if game is continuing
                    }
                }
                // If turn wasn't completed (e.g., player only moved cursor), loop continues
            } // End turn loop

            DisplayScoreboard(); // Show scoreboard after game ends

            Console.Write("\nPlay again? (y/n): ");
            string response = Console.ReadLine();
            playAgain = response?.Trim().ToLower() == "y";

        } // End multiple games loop

        Console.WriteLine("\nThanks for playing!");
        ResetConsoleColors(originalFg, originalBg);
    }

    // --- Splash Screen, Game Setup, and Helper Functions ---
    #region Setup and Helpers

    static void DisplayScoreboard()
    {
        Console.WriteLine("\n--- Scoreboard ---");
        Console.WriteLine($"{player1Name}: Wins={player1Wins}, Losses={player1Losses}, Draws={player1Draws}");
        Console.WriteLine($"{player2Name}: Wins={player2Wins}, Losses={player2Losses}, Draws={player2Draws}");
        Console.WriteLine("------------------");
    }

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
        Console.Clear();
        if (!isPlayerVsPlayer) { player2Name = "AI"; } // Assign AI name if PvAI
    }

    static void ResetConsoleColors(ConsoleColor fg, ConsoleColor bg)
    {
        Console.ForegroundColor = fg;
        Console.BackgroundColor = bg;
    }

    static void ChoosePlayerChip()
    {
        Console.Write($"{player1Name}, choose your representation ({Chip1Char} or {Chip2Char}): ");
        char choice = ' ';

        while (choice != Chip1Char && choice != Chip2Char)
        {
            string input = Console.ReadLine()?.ToUpper();
            if (input?.Length == 1 && (input[0] == Chip1Char || input[0] == Chip2Char)) { choice = input[0]; }
            else { Console.Write($"Invalid choice. Please enter {Chip1Char} or {Chip2Char}: "); }
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

        Console.Write($"{player1Name} is "); WriteWithColor(playerChipEmoji, playerChipColor);
        Console.Write($". {player2Name} is "); WriteWithColor(aiChipEmoji, aiChipColor);
        Console.WriteLine(".");
        Console.WriteLine("---------------------------------");
    }

    static void InitializeBoard()
    {
        cursorPosition = Columns / 2;
        for (int c = 0; c < Columns; c++) {
            board[c] = new char[Rows];
            for (int r = 0; r < Rows; r++) { board[c][r] = EmptyChar; }
        }
    }

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

    static void GetPlayerNames()
    {
        Console.Write("Enter name for Player 1: ");
        player1Name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(player1Name)) { player1Name = "Player 1"; }

        // Ask for P2 name regardless of mode initially
        Console.Write("Enter name for Player 2 (leave blank if playing AI): ");
        player2Name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(player2Name)) {
             // We'll overwrite this with "AI" in ChooseGameMode if needed
             player2Name = "Player 2";
        }
    }
    #endregion

    // --- Board Display ---
    #region Display Logic
    static void DisplayGameStatus()
    {
        ConsoleColor originalBg = Console.BackgroundColor;
        ConsoleColor originalFg = Console.ForegroundColor;
        // Don't clear here - allow animation frames to overwrite
        Console.CursorVisible = false; // Hide console cursor while drawing

        int currentLine = 0;

        // --- Draw Static Info ---
        Console.SetCursorPosition(0, currentLine);
        // Pad end of line to overwrite previous longer scores if necessary
        string scoreLine = $"Score: {player1Name} {playerChipEmoji}={playerScore} | {player2Name} {aiChipEmoji}={aiScore}";
        Console.Write(scoreLine.PadRight(Console.WindowWidth - 1)); // Overwrite line
        currentLine++;

        Console.SetCursorPosition(0, currentLine);
        Console.WriteLine("-----------------------------".PadRight(Console.WindowWidth - 1));
        currentLine++;

        int boardTopY = currentLine;

        // Display Cursor ('V')
        Console.SetCursorPosition(0, boardTopY); // Go to start of cursor line
        Console.Write(" ".PadRight(Console.WindowWidth - 1)); // Clear the line first
        Console.SetCursorPosition(0, boardTopY); // Return to start
        int cursorScreenX = 1 + cursorPosition * 4 + 1;
        Console.Write(new string(' ', cursorScreenX)); // Pad before V
        WriteWithColor("V", ConsoleColor.Green); // Draw V
        // No need to increment currentLine for cursor, it overlays numbers line

        // Display Column Numbers
        currentLine++; // Now move to number line
        Console.SetCursorPosition(0, boardTopY + 1); // Place numbers below cursor
        Console.Write(" ");
        for(int c=0; c<Columns; c++) { Console.Write($" {c+1}  "); }
        Console.WriteLine(); // Newline after numbers
        currentLine++; // Move past numbers line


        // --- Draw the Grid Structure & Place Emojis ---
        int gridContentStartY = currentLine + 1; // Y coord where first content row starts

        // Draw top border
        try { Console.SetCursorPosition(0, currentLine); } catch { }
        WriteWithColor("+", DefaultForeColor, GridFrameColor);
        for (int c = 0; c < Columns; c++) { WriteWithColor("---", DefaultForeColor, GridFrameColor); WriteWithColor("+", DefaultForeColor, GridFrameColor); }
        currentLine++;

        // Draw content rows and separators
        for (int r = Rows - 1; r >= 0; r--)
        {
            // Draw row content line structure (|   |   |...)
            try { Console.SetCursorPosition(0, currentLine); } catch { }
            WriteWithColor("|", DefaultForeColor, GridFrameColor);
            for (int c = 0; c < Columns; c++) {
                ConsoleColor currentBg = EmptySlotBackColor;
                Console.BackgroundColor = currentBg; Console.Write(" ");
                WriteWithColor(EmptySlotDisplay, DefaultForeColor, currentBg);
                Console.Write(" "); Console.BackgroundColor = originalBg;
                WriteWithColor("|", DefaultForeColor, GridFrameColor);
            }
            currentLine++;

            // Draw Horizontal Row Separator
            try { Console.SetCursorPosition(0, currentLine); } catch { }
            WriteWithColor("+", DefaultForeColor, GridFrameColor);
            for (int c = 0; c < Columns; c++) { WriteWithColor("---", DefaultForeColor, GridFrameColor); WriteWithColor("+", DefaultForeColor, GridFrameColor); }
            currentLine++;
        }

        // --- Place Emojis (Overwriting empty slots) ---
        for (int r = Rows - 1; r >= 0; r--) {
            for (int c = 0; c < Columns; c++) {
                if (board[c][r] != EmptyChar) {
                    int chipScreenX = 1 + c * 4 + 1;
                    int chipScreenY = gridContentStartY + (Rows - 1 - r) * 2;
                    try {
                        Console.SetCursorPosition(chipScreenX, chipScreenY);
                        if (board[c][r] == playerChipChar) { WriteWithColor(playerChipEmoji, playerChipColor, EmptySlotBackColor); }
                        else if (board[c][r] == aiChipChar) { WriteWithColor(aiChipEmoji, aiChipColor, EmptySlotBackColor); }
                    } catch (ArgumentOutOfRangeException) { }
                }
            }
        }

        // --- Display Turn Info Below Board ---
        try { Console.SetCursorPosition(0, currentLine); } catch { }
        Console.WriteLine(); // Blank line

        Console.ForegroundColor = originalFg; Console.BackgroundColor = originalBg; // Reset
        string turnText;
        if (isPlayerTurn) { turnText = $"Turn: {playerChipEmoji} ({player1Name})"; }
        else { turnText = $"Turn: {aiChipEmoji} ({player2Name})"; } // Shows P2 or AI name
        Console.WriteLine(turnText.PadRight(Console.WindowWidth-1)); // Overwrite line
        currentLine+=2; // Account for blank line and turn line

        Console.CursorVisible = true; // Restore console cursor
        // Place cursor after the prompt for HandlePlayerInput
         try { Console.SetCursorPosition(0, currentLine); } catch { }
    }
    #endregion

    // --- Gameplay Logic ---
    #region Gameplay
    /// <summary>
    /// Handles player input. Returns true if Enter or Q is pressed, false otherwise.
    /// Calls animation if drop is successful.
    /// </summary>
    static bool HandlePlayerInput()
    {
        char chipToDrop = isPlayerTurn ? playerChipChar : aiChipChar;
        string currentEmoji = isPlayerTurn ? playerChipEmoji : aiChipEmoji;
        ConsoleColor currentColor = isPlayerTurn ? playerChipColor : aiChipColor;
        string playerPrompt = isPlayerVsPlayer ? $"{(isPlayerTurn ? player1Name : player2Name)}" : player1Name; // Use P1 name if PvAI

        Console.Write($"{playerPrompt} - Move (LEFT/RIGHT), Drop (ENTER), Quit (Q): ");
        ConsoleKeyInfo keyInfo = Console.ReadKey(true);

        bool turnActionCompleted = false;

        switch (keyInfo.Key)
        {
            case ConsoleKey.LeftArrow:
                if (cursorPosition > 0) cursorPosition--;
                // Redraw will happen in main loop, return false as turn action not complete
                break;

            case ConsoleKey.RightArrow:
                if (cursorPosition < Columns - 1) cursorPosition++;
                // Redraw will happen in main loop, return false as turn action not complete
                break;

            case ConsoleKey.Enter:
                int targetRow = FindLowestRow(cursorPosition); // Find where it will land
                if (targetRow != -1) // Check if column is not full
                {
                    // *** ADDED: Call animation BEFORE placing chip ***
                    AnimateGameChipDrop(cursorPosition, targetRow, currentEmoji, currentColor);
                    // Now place the chip logically
                    board[cursorPosition][targetRow] = chipToDrop;
                }
                else // Column was full
                {
                    // Show temporary message
                     try { Console.SetCursorPosition(0, Console.CursorTop); } catch { }
                    Console.Write("Column full! Try again.        ");
                    Thread.Sleep(1000);
                }
                // Whether drop succeeded or failed, pressing Enter means the player acted
                turnActionCompleted = true;
                break;

            case ConsoleKey.Q:
                Console.WriteLine("\nQuitting game.");
                gameEnded = true;
                turnActionCompleted = true;
                break;
        }

        return turnActionCompleted;
    }

    /// <summary>
    /// Handles the AI's turn: Gets move, animates, places chip.
    /// </summary>
    static void HandleAITurn()
    {
        Console.WriteLine("\nAI is thinking...");
        Thread.Sleep(1000);

        int aiColumn = ChooseAIColumn();

        if (aiColumn != -1)
        {
            Console.WriteLine($"AI chooses column {aiColumn + 1}.");
            Thread.Sleep(500); // Short pause before drop

            int targetRow = FindLowestRow(aiColumn);
            if (targetRow != -1) // Should always be true if CanPlace is correct
            {
                 // *** ADDED: Call animation BEFORE placing chip ***
                AnimateGameChipDrop(aiColumn, targetRow, aiChipEmoji, aiChipColor);
                // Place the chip logically
                board[aiColumn][targetRow] = aiChipChar;
            }
             else { // Should not happen if AI logic is correct
                 Console.WriteLine("AI Error: Tried to drop in a full column?");
                 gameEnded = true; // End game if something went wrong
             }
        }
        else
        {
            Console.WriteLine("AI Error: No valid column found? Board might be full.");
            gameEnded = true;
        }
        Thread.Sleep(500); // Shorter pause after AI drop animation finishes
    }

    // *** NEW FUNCTION FOR GAME ANIMATION ***
    /// <summary>
    /// Animates a chip dropping using coordinate placement for smoother visuals.
    /// </summary>
    /// <param name="column">The column the chip is dropped into.</param>
    /// <param name="targetRow">The final row the chip will land on.</param>
    /// <param name="chipEmoji">The visual emoji to display.</param>
    /// <param name="chipColor">The color of the emoji.</param>
    static void AnimateGameChipDrop(int column, int targetRow, string chipEmoji, ConsoleColor chipColor)
    {
        Console.CursorVisible = false; // Hide cursor during animation
        int firstContentRowY = 5; // Y coordinate of the first row of cells (adjust if layout changes)
        int chipScreenX = 1 + column * 4 + 1; // X coordinate within the cell | {E} |

        ConsoleColor originalBg = Console.BackgroundColor; // Store original background

        for (int r = 0; r <= targetRow; r++) // Loop from top (visual row 0) down to target
        {
            // Calculate current screen Y position for this animation step
            // Visual row 0 corresponds to internal board row Rows-1
            // Visual row 'r' corresponds to internal board row Rows-1-r ? No, other way.
            // Screen Y = StartY + (Visual Row Index from top) * 2 lines per slot
            int currentScreenY = firstContentRowY + r * 2;

            // Draw the chip at the current position
             try {
                Console.SetCursorPosition(chipScreenX, currentScreenY);
                WriteWithColor(chipEmoji, chipColor, EmptySlotBackColor); // Draw chip on black bg
             } catch { /* ignore */ }

            Thread.Sleep(95); // Pause for animation frame

            // Erase the chip from the current position IF it's not the final landing spot
            if (r < targetRow)
            {
                 try {
                    Console.SetCursorPosition(chipScreenX, currentScreenY);
                    WriteWithColor(EmptySlotDisplay, DefaultForeColor, EmptySlotBackColor); // Draw empty space back
                 } catch { /* ignore */ }
            }
            // The final chip position is left drawn after the loop finishes
        }
        Console.CursorVisible = true; // Restore cursor visibility
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
                bool won = CheckWin(aiChipChar); board[c][r] = EmptyChar; if (won) return c;
            }
        }
        // 2. Check Player win block
        for (int c = 0; c < Columns; c++) {
            if (CanPlace(c)) {
                int r = FindLowestRow(c); board[c][r] = playerChipChar;
                bool blocked = CheckWin(playerChipChar); board[c][r] = EmptyChar; if (blocked) return c;
            }
        }
        // 3. Pick preferred
        List<int> preferred = new List<int> { 3, 4, 2, 5, 1, 6, 0 };
        foreach (int c in preferred) { if (CanPlace(c)) return c; }
        // 4. Pick any valid (fallback)
        for (int c = 0; c < Columns; c++) { if (CanPlace(c)) return c; }
        return -1;
    }

    static int FindLowestRow(int column) {
        if (column < 0 || column >= Columns) return -1;
        for (int r = 0; r < Rows; r++) { if (board[column][r] == EmptyChar) return r; }
        return -1;
    }

    static bool CanPlace(int column) {
        return column >= 0 && column < Columns && board[column][Rows - 1] == EmptyChar;
    }

    // TryDropChip now only does the logical placement, animation called separately
    static bool TryDropChip(int column, char chipChar) {
        int row = FindLowestRow(column);
        if (row != -1) { board[column][row] = chipChar; return true; }
        return false;
    }

    static bool CheckWin(char chipChar) {
        // Check horizontal
        for (int r = 0; r < Rows; r++) { for (int c = 0; c <= Columns - 4; c++) { if (board[c][r] == chipChar && board[c + 1][r] == chipChar && board[c + 2][r] == chipChar && board[c + 3][r] == chipChar) return true; } }
        // Check vertical
        for (int c = 0; c < Columns; c++) { for (int r = 0; r <= Rows - 4; r++) { if (board[c][r] == chipChar && board[c][r + 1] == chipChar && board[c][r + 2] == chipChar && board[c][r + 3] == chipChar) return true; } }
        // Check diagonal up-right
        for (int c = 0; c <= Columns - 4; c++) { for (int r = 0; r <= Rows - 4; r++) { if (board[c][r] == chipChar && board[c + 1][r + 1] == chipChar && board[c + 2][r + 2] == chipChar && board[c + 3][r + 3] == chipChar) return true; } }
        // Check diagonal down-right
        for (int c = 0; c <= Columns - 4; c++) { for (int r = 3; r < Rows; r++) { if (board[c][r] == chipChar && board[c + 1][r - 1] == chipChar && board[c + 2][r - 2] == chipChar && board[c + 3][r - 3] == chipChar) return true; } }
        return false;
    }

    static bool IsBoardFull() {
        for (int c = 0; c < Columns; c++) { if (board[c][Rows - 1] == EmptyChar) return false; }
        return true;
    }

    static void SwitchPlayer() {
        isPlayerTurn = !isPlayerTurn;
        currentPlayerChipChar = isPlayerTurn ? playerChipChar : aiChipChar;
    }
    #endregion

    // --- Splash Screen Logic (Assumed Correct) ---
    #region SplashScreen
    static void ShowSplashScreen()
    {
        Console.Clear();
        int rows = 6; int cols = 7;
        string[,] splashBoard = new string[rows, cols];
        for (int r = 0; r < rows; r++) for (int c = 0; c < cols; c++) splashBoard[r, c] = " ";
        (int col, string color)[] moves = new (int, string)[] {
            (0, "R"), (1, "Y"), (1, "R"), (2, "Y"), (2, "R"), (3, "Y"),
            (2, "R"), (3, "Y"), (3, "R"), (5, "Y"), (3, "R") };
        foreach (var move in moves) { AnimateChipDropSplash(splashBoard, move.col, move.color); Thread.Sleep(50); } // Renamed splash animation func
        DrawBoardSplash(splashBoard, "Red Wins!"); Thread.Sleep(1500); // Renamed splash draw func
        ShowAsciiSplash();
    }

    // Renamed to avoid conflict with game animation method
    static void AnimateChipDropSplash(string[,] currentBoard, int column, string chip)
    {
        int targetRow = -1;
        for (int r = currentBoard.GetLength(0) - 1; r >= 0; r--) { if (currentBoard[r, column] == " ") { targetRow = r; break; } }
        if (targetRow == -1) return;
        for (int r = 0; r <= targetRow; r++) {
            if (r > 0) currentBoard[r - 1, column] = " ";
            currentBoard[r, column] = chip;
            DrawBoardSplash(currentBoard); // Call renamed splash draw func
            Thread.Sleep(50);
        }
    }

     // Renamed to avoid conflict with game display method
    static void DrawBoardSplash(string[,] boardToDraw, string message = null)
    {
        Console.Clear();
        int rows = boardToDraw.GetLength(0); int cols = boardToDraw.GetLength(1);
        if (!string.IsNullOrEmpty(message)) {
            Console.ForegroundColor = ConsoleColor.White;
            int padding = (Console.WindowWidth - message.Length) / 2;
            Console.WriteLine($"{new string(' ', Math.Max(0, padding))}{message}\n");
            Console.ResetColor();
        }
        Console.WriteLine(" +---+---+---+---+---+---+---+");
        for (int r = 0; r < rows; r++) {
            Console.Write(" |");
            for (int c = 0; c < cols; c++) {
                string chip = boardToDraw[r, c];
                if (chip == "R") Console.ForegroundColor = ConsoleColor.Red;
                else if (chip == "Y") Console.ForegroundColor = ConsoleColor.Yellow;
                else Console.ResetColor();
                Console.Write($" {chip} "); Console.ResetColor(); Console.Write("|");
            }
            Console.WriteLine(); Console.WriteLine(" +---+---+---+---+---+---+---+");
        }
    }

    static void ShowAsciiSplash()
    {
        Console.Clear();
        string[] splashLines = new string[] {
            "                                                           >=>                          >=>             >=>   ",
            "                                                           >=>              >=>         >=>             >=>   ",
            "   >==>    >=>     >==>>==>  >==>>==>    >==>       >==> >=>>==>           >>=>         >=>   >==>    >=>>==> ",
            " >=>     >=>  >=>   >=>  >=>  >=>  >=> >>   >=>   >=>      >=>            > >=>         >=> >>   >=>    >=>   ",
            ">=>     >=>    >=>  >=>  >=>  >=>  >=> >>===>>=> >=>       >=>          >=> >=>         >=> >>===>>=>   >=>   ",
            " >=>     >=>  >=>   >=>  >=>  >=>  >=> >>         >=>      >=>         >===>>=>>=> >>   >=> >>          >=>   ",
            "   >==>    >=>     >==>  >=> >==>  >=>  >====>      >==>    >=>             >=>     >===>    >====>      >=>  "
        };
        string pressEnterText = "PRESS ENTER TO START THE GAME...";
        int pressEnterY = Math.Min(Console.WindowHeight - 2, splashLines.Length + 2);
        int pressEnterX = Math.Max(0, (Console.WindowWidth - pressEnterText.Length) / 2);
        ConsoleColor[] colors = new ConsoleColor[] { ConsoleColor.Red, ConsoleColor.Yellow, ConsoleColor.Cyan, ConsoleColor.Green, ConsoleColor.Magenta };
        int colorIndex = 0; Console.CursorVisible = false; DateTime lastUpdate = DateTime.Now;
        while (true) {
            if (Console.KeyAvailable) { if (Console.ReadKey(true).Key == ConsoleKey.Enter) break; }
            if ((DateTime.Now - lastUpdate).TotalMilliseconds >= 200) {
                Console.Clear();
                Console.ForegroundColor = colors[colorIndex];
                for(int i = 0; i < splashLines.Length; i++) {
                     int linePadding = Math.Max(0, (Console.WindowWidth - splashLines[i].Length) / 2);
                      try { Console.SetCursorPosition(linePadding, i); } catch {/* ignore */}
                     Console.WriteLine(splashLines[i]);
                }
                Console.ResetColor();
                 try { Console.SetCursorPosition(pressEnterX, pressEnterY); } catch {/* ignore */}
                Console.ForegroundColor = ConsoleColor.White; Console.WriteLine(pressEnterText); Console.ResetColor();
                colorIndex = (colorIndex + 1) % colors.Length; lastUpdate = DateTime.Now;
            }
            Thread.Sleep(20);
        }
        Console.CursorVisible = true; Console.Clear();
    }
    #endregion
}