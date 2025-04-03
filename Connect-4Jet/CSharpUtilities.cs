namespace Connect4Template;

public class CsharpUtilitiesRBP
{
    public static void StartUtilDemo()
    {
        int currentPageNumber = 0;
        Console.Clear();
        
        //Display first page
        PrintCurrentPage(currentPageNumber, "\n C# Code Utilities!\n");
        
        Print("\n---Press any key to continue---\n\n");
        Console.ReadKey(); //Halt screen until user presses any key 
        
        //New Page - Emojis test
        Console.Clear();
        ++currentPageNumber;
        PrintCurrentPage(currentPageNumber, "\n Emojis in C#!\n\n");
        
        PrintMessageWithEmojis("Hello CSI!!");
        
        Print("\n---Press any key to continue---\n\n");
        Console.ReadKey(); //Halt screen until user presses any key
            
        //New Page - Emojis test
        Console.Clear();
        ++currentPageNumber;
        PrintCurrentPage(currentPageNumber, "\n A board with Emojis\n\n");
        
        DrawBoard("\u2764\ufe0f ", " \ud83d\ude1c"); //Chip 1: ❤️ Chip 2: ⭐
        
        Print("\n---Press any key to continue---\n\n");
        Console.ReadKey(); //Halt screen until user presses any key
        
        
        //New Page - HighFrameRate animation
        Console.Clear();
        ++currentPageNumber;
        PrintCurrentPage(currentPageNumber, "\n Animations in C#!\n\n");
        
        //TODO
        StartTextAnimation();
        
        Print("\n---Press any key to FINISH---\n\n");
        Console.ReadKey(); //Halt screen until user presses any key
       
       
       
    }

    /// <summary>
    /// A method that prints a message with some emojis.
    /// </summary>
    /// <param name="msg">Message to be printed on screen.</param>
    static void PrintMessageWithEmojis(string msg)
    {
        //Lots of emojis can be found at https://emojipedia.org/ or similar sites.
        //NOTE: Be attentive to what version of UTF encoding the emoji you choose supports. 
        
        //The console's output encoding need to be set as UTF8 for the
        //emojis to be displayed properly
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Write(" {0}, and some emojis! -> ", msg); 
        Console.WriteLine(" \u2764\ufe0f \ud83d\ude1c ");// ❤️ 😜
    }

    static void DrawBoard(string chip1, string chip2)
    {
        //The console's output encoding need to be set as UTF8 for the
        //emojis to be displayed properly
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        
        
        Console.WriteLine("    |     |     ");
        Console.WriteLine("    | {0} |     ", chip1);
        Console.WriteLine("____|_____|_____");
        Console.WriteLine("    |     |     ");
        Console.WriteLine("    |     |     ");
        Console.WriteLine("____|_____|_____");
        Console.WriteLine("    |     |     ");
        Console.WriteLine("    |     | {0} ", chip2);
        Console.WriteLine("    |     |     ");
    }

    /// <summary>
    /// Starts a custom text animation. Animation by Reynaldo Belfort, S.J.
    /// </summary>
    static void StartTextAnimation()
    {
        /*
         * ASCII Art Archive is one of the best sites out there when it comes to complex text artwork.
         * They offer many free-to-use features such as ASCII Draw Studio, Image to ASCII Art,
         *  Text to ASCII ARt, Webcam to ASCII Art and more.
         * They also have a library of artwork designed by users. Of course, make sure to always credit
         *  artworks taken from their library!!
         * 
         * Main website: https://www.asciiart.eu/
         * ASCII Draw Studio: https://www.asciiart.eu/ascii-draw-studio
         * Text to ASCII Art Generator: 
         */

        string[] textBar = new string[]
        {
            "===",
            "    ===",
            "        ===",
            "            ===",
            "               ===",
            "                    ===",
            "                        ===",
            "                            ===",
            "                                ===",
            "                                    ==="
        };

        //Begin animation
        int refreshRate = 10; //Control how smooth we want the text animation to look
        int cycles = 3; //1 cycle is a forward + backward animation.
        Console.ForegroundColor = ConsoleColor.Yellow; //Make animation pop with a color.

        for (int i = 0; i < cycles; i++)
        {
            //Forward animation
            for(int x = 0; x < textBar.Length; x++)
            {
                Print("\n" + textBar[x]);
                DelayAndClear(refreshRate); //Pausing and clearing
            }    
            //Backward animation
            for(int y = 9; y >= 0; y--)
            {
                Print(textBar[y]);
                DelayAndClear(refreshRate); //Pausing and clearing
            }
        }
        
        Console.ResetColor();
        
        // Console.OutputEncoding = System.Text.Encoding.Unicode;
        
        
        string csiLogo = @"  
              _____    _____  _____ 
             / ____|  / ____||_   _|
            | |      | (___    | |  
            | |       \___ \   | |  
            | |____ _ ____) | _| |_ 
             \_____(_)_____(_)_____|";
        string leonesLogo = @"
             _      ______ ____  _   _ ______  _____ _ _ _ 
            | |    |  ____/ __ \| \ | |  ____|/ ____| | | |
            | |    | |__ | |  | |  \| | |__  | (___ | | | |
            | |    |  __|| |  | | . ` |  __|  \___ \| | | |
            | |____| |___| |__| | |\  | |____ ____) |_|_|_|
            |______|______\____/|_| \_|______|_____/(_|_|_)";

        //Print CSI Logo several times, looping through different colors 
        refreshRate = 100;
        foreach (ConsoleColor color in Enum.GetValues(typeof(ConsoleColor)))
        {
            Console.ForegroundColor = color;
            Print(csiLogo); //CSI
            Print(leonesLogo); //LEONES!!
            DelayAndClear(refreshRate); //Slight pause
        }
        
        //Last frame
        Console.ResetColor();
        Print(csiLogo);
        Print(leonesLogo);
        Print("\n\n");

    }

    static void DelayAndClear(int ms)
    {
        Thread.Sleep(ms);
        Console.Clear();
    }

    static void PrintCurrentPage(int pageNumber, string pageTitle)
    {
        Print("\n\n");
        PrintLn("-------- Page [" + pageNumber +  "] --------");
        PrintLn(pageTitle);
    }
    static void Print(string msg)
    {
        Console.Write(msg);
    }
    static void PrintLn(string msg)
    {
        Console.WriteLine(msg);
    }
}