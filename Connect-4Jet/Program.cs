// This code is made by Jet-Brainers that are Maximiliano Davila, George Lopez, Daniel Cintron to make connect 4Jet

using System.Threading; //Needed for Thread.Sleep

using System;
namespace Connect_4Jet;

// This class holds our entire Connect Four game
class Program
{
    // --- Game Settings ---
    const int Columns = 7;
    const int Rows = 6;
    const char Empty = '_';
    const char Chip1 = 'X'; // Default chip options
    const char Chip2 = 'O';
    

    // --- Game State ---
    // These variables keep track of the game as it's played
    static char[][] board = new char[Columns][]; // The game board grid
    static char playerChip;                      // What chip the player uses
    static char aiChip;                          // What chip the computer uses
    static char currentPlayerChip;               // Whose turn is it ('X' or 'O')
    static int cursorPosition;                   // Where the player is aiming (column index)
    static bool isPlayerTurn;                    // Is it the human player's turn?
    static bool gameEnded;                       // Has the current game finished?
    static int playerScore = 0;                  // Player's score across games
    static int aiScore = 0;                      // AI's score across games
    static Random random = new Random();         // For AI's random choices

    // --- Main Program Entry Point ---
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8; // Helps display board characters correctly
        
        //Add the Splash screen animation before game starts
        ShowSplashScreen();

        Console.WriteLine(">> Welcome to Connect Four! <<");

        ChoosePlayerChip(); // Let the player pick X or O

        // Main loop to play multiple games
        bool playAgain = true;
        while (playAgain)
        {
            InitializeBoard(); // Setup the board for a new game
            gameEnded = false;
            isPlayerTurn = true; // Player starts first
            currentPlayerChip = playerChip;

            // Game turn loop
            while (!gameEnded)
            {
                DisplayGameStatus(); // Show board, score, and whose turn

                if (isPlayerTurn)
                {
                    HandlePlayerInput();
                }
                else
                {
                    HandleAITurn();
                }

                // Check for win/draw only if the game hasn't been ended by quitting
                if (!gameEnded)
                {
                    if (CheckWin(currentPlayerChip))
                    {
                        gameEnded = true;
                        DisplayGameStatus(); // Show final board
                        Console.WriteLine($"\n--- {(currentPlayerChip == playerChip ? "Player" : "AI")} ({currentPlayerChip}) WINS! ---");
                        if (currentPlayerChip == playerChip) playerScore++; else aiScore++;
                    }
                    else if (IsBoardFull())
                    {
                        gameEnded = true;
                        DisplayGameStatus(); // Show final board
                        Console.WriteLine("\n--- DRAW! Board is full. ---");
                    }
                    else
                    {
                        // If game continues, switch player for the next turn
                        SwitchPlayer();
                    }
                }
            } // End of turn loop

            // Ask to play again
            Console.Write("\nPlay again? (y/n): ");
            playAgain = Console.ReadLine().Trim().ToLower() == "y";
        }

        Console.WriteLine("\nThanks for playing!");
    }

    /// <summary>
    /// Asks the player to choose 'X' or 'O'.
    /// </summary>
    static void ChoosePlayerChip()
    {
        Console.Write($"Choose your chip ({Chip1} or {Chip2}): ");
        char choice = ' ';
        while (choice != Chip1 && choice != Chip2)
        {
            string input = Console.ReadLine().ToUpper();
            if (input.Length == 1 && (input[0] == Chip1 || input[0] == Chip2))
            {
                choice = input[0];
            }
            else
            {
                Console.Write($"Invalid choice. Please enter {Chip1} or {Chip2}: ");
            }
        }
        playerChip = choice;
        aiChip = (playerChip == Chip1) ? Chip2 : Chip1; // AI gets the other chip
        Console.WriteLine($"You are '{playerChip}'. AI is '{aiChip}'.");
    }

    /// <summary>
    /// Sets up the board array and fills it with empty slots.
    /// </summary>
    static void InitializeBoard()
    {
        cursorPosition = Columns / 2; // Start cursor in middle

        // Create the columns and fill with empty slots
        for (int c = 0; c < Columns; c++)
        {
            board[c] = new char[Rows]; // Create the rows for this column
            for (int r = 0; r < Rows; r++)
            {
                board[c][r] = Empty; // Set each slot to empty
            }
        }
    }

    /// <summary>
    /// Clears the console and displays the board, score, and current turn info.
    /// </summary>
    static void DisplayGameStatus()
    {
        Console.Clear();
        Console.WriteLine($"Score: Player [{playerChip}]={playerScore} | AI [{aiChip}]={aiScore}");
        Console.WriteLine("-----------------------------");

        // Display cursor position ('V') above the board
        Console.Write(" ");
        for (int c = 0; c < Columns; c++)
        {
            Console.Write(c == cursorPosition ? " V " : "   ");
        }
        Console.WriteLine();

        // Display column numbers
        Console.Write(" ");
        for (int c = 0; c < Columns; c++)
        {
            Console.Write($" {c + 1} "); // Show 1 to 7
        }
        Console.WriteLine();

        // Display the board grid from top row down
        for (int r = Rows - 1; r >= 0; r--)
        {
            Console.Write("|"); // Left edge
            for (int c = 0; c < Columns; c++)
            {
                // Optional: Add colors here if desired later
                Console.Write($" {board[c][r]} "); // Show chip or empty slot
            }
            Console.WriteLine("|"); // Right edge
        }

        // Display bottom border
        Console.Write("+");
        for (int c = 0; c < Columns; c++) Console.Write("---");
        Console.WriteLine("+");

        Console.WriteLine($"\nTurn: {(isPlayerTurn ? "Player" : "AI")} ({currentPlayerChip})");
    }

    /// <summary>
    /// Handles player keyboard input for moving cursor and dropping chip.
    /// </summary>
    static void HandlePlayerInput()
    {
        Console.Write("Move (LEFT/RIGHT), Drop (SPACE), Quit (Q): ");
        ConsoleKeyInfo keyInfo = Console.ReadKey(true); // Read key without displaying it

        switch (keyInfo.Key)
        {
            case ConsoleKey.LeftArrow:
                if (cursorPosition > 0) cursorPosition--;
                break; // Redraw happens outside switch

            case ConsoleKey.RightArrow:
                if (cursorPosition < Columns - 1) cursorPosition++;
                break; // Redraw happens outside switch

            case ConsoleKey.Spacebar:
                if (!TryDropChip(cursorPosition, playerChip))
                {
                    Console.WriteLine("Column full! Try again.");
                    Thread.Sleep(1000); // Pause so player sees message
                }
                // Win/Draw check happens back in the main loop after this returns
                break; // Exit input handling for this turn

            case ConsoleKey.Q:
                Console.WriteLine("\nQuitting game.");
                gameEnded = true;
                break; // Exit input handling and mark game as ended
        }
        // Note: Only dropping the chip or quitting actually ends the player's *turn*.
        // Moving the cursor just redraws the screen via the main loop.
    }

    /// <summary>
    /// Determines the AI's move and tries to place its chip.
    /// </summary>
    static void HandleAITurn()
    {
        Console.WriteLine("AI is thinking...");
        Thread.Sleep(1000); // Simulate thinking

        int aiColumn = ChooseAIColumn();

        if (aiColumn != -1)
        {
            Console.WriteLine($"AI chooses column {aiColumn + 1}.");
            Thread.Sleep(500);
            TryDropChip(aiColumn, aiChip); // Assume AI won't pick a full column
        }
        else
        {
            // Should only happen if board is full, which should be caught by IsBoardFull()
            Console.WriteLine("AI Error: No valid column found?");
            gameEnded = true; // End game if AI fails
        }
         Thread.Sleep(1000); // Pause after AI move
    }

    /// <summary>
    /// AI Logic: Tries to find a winning move, then block player's win, then picks randomly.
    /// </summary>
    static int ChooseAIColumn()
    {
        // 1. Check if AI can win
        for (int c = 0; c < Columns; c++)
        {
            if (CanPlace(c))
            {
                int r = FindLowestRow(c);
                board[c][r] = aiChip; // Try placing AI chip
                if (CheckWin(aiChip))
                {
                    board[c][r] = Empty; // Undo test move
                    return c; // Found winning move
                }
                board[c][r] = Empty; // Undo test move
            }
        }

        // 2. Check if Player can win next turn and block
        for (int c = 0; c < Columns; c++)
        {
            if (CanPlace(c))
            {
                int r = FindLowestRow(c);
                board[c][r] = playerChip; // Try placing Player chip
                if (CheckWin(playerChip))
                {
                    board[c][r] = Empty; // Undo test move
                    return c; // Found blocking move
                }
                board[c][r] = Empty; // Undo test move
            }
        }

        // 3. Otherwise, pick a random valid column (prefer center)
        List<int> preferred = new List<int> { 3, 4, 2, 5, 1, 6, 0 }; // Center-out
        foreach (int c in preferred)
        {
             if (CanPlace(c)) return c;
        }

        // Fallback (shouldn't be needed if board isn't full)
        for (int c = 0; c < Columns; c++)
        {
            if (CanPlace(c)) return c;
        }

        return -1; // No valid move found
    }

    /// <summary>
    /// Finds the lowest available row index in a column. Returns -1 if full.
    /// </summary>
    static int FindLowestRow(int column)
    {
        for (int r = 0; r < Rows; r++)
        {
            if (board[column][r] == Empty)
            {
                return r; // Return the first empty row from bottom
            }
        }
        return -1; // Column is full
    }

    /// <summary>
    /// Checks if a chip can be placed in the given column.
    /// </summary>
    static bool CanPlace(int column)
    {
        // Is the column index valid AND is the top slot empty?
        return column >= 0 && column < Columns && board[column][Rows - 1] == Empty;
    }

    /// <summary>
    /// Tries to place a chip in the lowest empty slot of a column.
    /// Returns true if successful, false if column is full.
    /// </summary>
    static bool TryDropChip(int column, char chip)
    {
        int row = FindLowestRow(column);
        if (row != -1) // If a valid row was found
        {
            board[column][row] = chip; // Place the chip
            return true;
        }
        return false; // Column was full
    }

    /// <summary>
    /// Checks if the specified chip has won (4 in a row anywhere).
    /// </summary>
    static bool CheckWin(char chip)
    {
        // Check horizontal (--)
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c <= Columns - 4; c++)
            {
                if (board[c][r] == chip && board[c + 1][r] == chip && board[c + 2][r] == chip && board[c + 3][r] == chip) return true;
            }
        }

        // Check vertical (|)
        for (int c = 0; c < Columns; c++)
        {
            for (int r = 0; r <= Rows - 4; r++)
            {
                if (board[c][r] == chip && board[c][r + 1] == chip && board[c][r + 2] == chip && board[c][r + 3] == chip) return true;
            }
        }

        // Check diagonal (/)
        for (int c = 0; c <= Columns - 4; c++)
        {
            for (int r = 0; r <= Rows - 4; r++)
            {
                if (board[c][r] == chip && board[c + 1][r + 1] == chip && board[c + 2][r + 2] == chip && board[c + 3][r + 3] == chip) return true;
            }
        }

        // Check diagonal (\)
        for (int c = 0; c <= Columns - 4; c++)
        {
            for (int r = 3; r < Rows; r++) // Start from row 3 checking down-left
            {
                if (board[c][r] == chip && board[c + 1][r - 1] == chip && board[c + 2][r - 2] == chip && board[c + 3][r - 3] == chip) return true;
            }
        }

        return false; // No win found
    }

    /// <summary>
    /// Checks if the board has any empty slots left.
    /// </summary>
    static bool IsBoardFull()
    {
        // Check the top row of all columns. If any are empty, board is not full.
        for (int c = 0; c < Columns; c++)
        {
            if (board[c][Rows - 1] == Empty)
            {
                return false; // Found an empty slot
            }
        }
        return true; // No empty slots in the top row means board is full
    }

    /// <summary>
    /// Switches the current player and the turn flag.
    /// </summary>
    static void SwitchPlayer()
    {
        isPlayerTurn = !isPlayerTurn; // Flip the boolean (true becomes false, false becomes true)
        currentPlayerChip = isPlayerTurn ? playerChip : aiChip; // Set chip based on new turn owner
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
                (3, "R")//Final move gives red a diagonal win
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
                    if (chip == "R") Console.ForegroundColor = ConsoleColor.Red;
                    else if (chip == "Y") Console.ForegroundColor = ConsoleColor.Yellow;
                    else Console.ResetColor();

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