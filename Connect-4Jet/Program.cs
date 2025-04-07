namespace Connect_4Jet;

/*  CSI Connect 4 2025
 * Below you will find a template for creating your Connect 4 game.
 *
 * TO-DO comments:
 * Notice the "TO-DO:" (without the '-') code comments, so that you may keep track of the code that is left to
 * be written. To see the list, search on the web how to see code's TODO the list on Rider.
 * Make sure to remove the TO-DO (without the '-') word when the code implementation in
 * that section of code has been completed
 *
 * The **DEBUGGER** is a powerful tool for troubleshooting your code, especially for logical errors. USE IT while you can!
 *
 * Make sure to personalize this Connect 4 game app.
 *
 * Happy coding!!!
 *
 * Template provided by prof. Reynaldo Belfort, S.J.
 * */


class Program
{
    //Define values that never change using 'const'
     const char _EMPTY = '_';
     const char _CHIP1 = 'X';
     const char _CHIP2 = 'O';
     const int _SIZEX = 7;
     const int _SIZEY = 6;
     
     
     //Define " global" variables, so that we can access them anywhere within
     //the 'Program' class
     static char[][] _board = new char[_SIZEX][];
     static int _boardCursorPos = 0; // x coordinate corresponds to _boardCursor[0]
                                             // y coordinate corresponds to _boardCursor[1]
     static char _currentPlayerChip = _EMPTY;
     static bool _gameEnded = false;
     
     
     static void Main(string[] args)
    {
        //Initial set up
        InitializeGame();
        _currentPlayerChip = _CHIP1;
        _gameEnded = false;

        //TODO any code work before game begins. You may want to print some sort of welcome message

        PrintLn("");
        PrintLn("   >> Let's Play Connect 4! <<"); //TODO modify/delete this line as you see fit

        //Start game
        PlayGame();
        
    }
     
     /// <summary>
     /// TODO: our documentation here
     /// </summary>
    static void InitializeGame() 
    {
        //Set board cursor to left side
        _boardCursorPos = 0;
        
        //----------------Initializing BOARD ------------------
        //Initialize every row according to the board _SIZE
        for (int i = 0; i < _SIZEX; i++)
        {
            _board[i] = new char[_SIZEY];
        }
         
        //Assign the default _EMPTY value
        for (int i = 0; i < _SIZEX; i++) {
            for (int j = 0; j < _SIZEY; j++) {
                _board[i][j] = _EMPTY;
            }
        }
    }
    
    /// <summary>
    /// TODO: our documentation here
    /// </summary>
    static void PlayGame()
    {

        ConsoleKeyInfo lastKeyPressed = new ConsoleKeyInfo();
        
        //Will run once and loop forever, until user chooses to exit, or some game condition leads to a closing
        do {
            
            //Clear console
            Console.Clear();

            //Print the current state of the game
            PrintBoard();
            
            //Request input from current player
            
                //TODO: For demonstration and debug purposes. Please remove this line once project is done.
                PrintLn("DEBUG - Last key pressed: " + lastKeyPressed.Key);
            
            //TODO: MODIFY CODE HERE
            
            PrintLn("\n Press arrow keys to position cursor (LEFT, RIGHT) \n SPACE to place chip or Q to quit: ");
            ConsoleKeyInfo userInput = Console.ReadKey();
            
            //Move cursor on board
            lastKeyPressed = userInput; //Store last move on record
            MoveCursor(userInput);

            //Check game status (winner? draw?)

            //TODO: YOUR CODE HERE. Modify the 'gameEnded' var here.


        } while (!_gameEnded);
    }
    
    /// <summary>
    /// TODO: our documentation here
    /// </summary>
    static void PrintBoard() {

        //The console's output encoding need to be set as UTF8 for the
        //emojis to be displayed properly
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("Nothing here :( \ud83d\ude2d \ud83d\ude2d");
        
    }

    /// <summary>
    /// TODO: our documentation here
    /// </summary>
    /// <param name="userInput">TODO: our documentation here </param>
    static void MoveCursor(ConsoleKeyInfo userInput)
    {
        //TODO: Sample cursor system. The team may adapt this system to whatever they wish
        
        if (userInput.Key == ConsoleKey.LeftArrow)
        {
            //Check if we can move cursor to left.
            if (_boardCursorPos - 1 >= 0)
            {
                _boardCursorPos--; //We can. Thus, move it to left.
            }
        }
        else if (userInput.Key == ConsoleKey.RightArrow)
        {
            //Check if we can move cursor to left.
            if (_boardCursorPos + 1 < _SIZEX)
            {
                _boardCursorPos++; //We can. Thus, move it to right.
            }
        }
        else if (userInput.Key == ConsoleKey.Spacebar)
        {
            //TODO Set chip on board
           
            
            //Change player
            
                
        }
        else if (userInput.Key == ConsoleKey.Q)
        {
            //TODO: YOUR CODE HERE. Application finishes??
            
        }
        else
        {
            //Then user has pressed another key... 
            //TODO: should we do something here? Up for the team to decide
        }
    }
    
    // Drops a piece into the selected column with animation
    static void DropPieceAnimated(int col)
    {
        
        //TODO Add code here
    }

    /// <summary>
    /// TODO: our documentation here
    /// </summary>
    /// <returns>TODO: our documentation here</returns>
    static bool IsBoardFull() {

        //TODO: YOUR CODE HERE
        return false;

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
    /// <param name="msg">TODO: our documentation here</param>
    static void Print(string msg)
    {
        Console.Write(msg);
    }
    /// <summary>
    /// TODO: our documentation here
    /// </summary>
    /// <param name="msg">TODO: our documentation here</param>
    static void PrintLn(string msg)
    {
        Console.WriteLine(msg);
    }

}