namespace Connect_4Jet;
using System;
using System.Text;
using System.Threading; // For pausing the game (Thread.Sleep)
using System.Collections.Generic; // For the AI's list of choices

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
        Console.OutputEncoding = Encoding.UTF8; // Helps display board characters correctly
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

    
}