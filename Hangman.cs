using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Linq;

static class MainClass {
  public static void Main (string[] args) {
    //Console.WriteLine ("Hello World");
    var result = GetFileViaHttp(@"https://raw.githubusercontent.com/dwyl/english-words/master/words_alpha.txt");
    string str = Encoding.UTF8.GetString(result);
    string[] strArr = str.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

    Console.Clear();

    bool gameOver = false;
    char[,] hangman = 
    {
      {'|', '‾', '‾', '|', ' '},
      {'|', ' ', ' ', ' ', ' '}, //  o            [1, 3]
      {'|', ' ', ' ', ' ', ' '}, // / | \\ [2, 2] [2, 3] [2, 4]
      {'|', ' ', ' ', ' ', ' '}, // / \\   [3, 2]        [3, 4]
      {'|', ' ', ' ', ' ', ' '},
    };

    int mistakes = 0;

    var rand = new Random();
    string randWord = strArr[rand.Next(0, strArr.Length)].ToLower();
    //Console.WriteLine(randWord);

    var badletters = new List<char>();
    var goodletters = new List<char>();
    
    char[] goodGuesses = new char[randWord.Length];
    for (int i = 0; i < goodGuesses.Length; i++) goodGuesses[i] = ' ';
    
    try {
    while (!gameOver) {
      string curGuess = new string(goodGuesses);
      printHangMan(hangman);

      if (randWord.Trim().CompareTo(curGuess.Trim()) == 0) {
        gameOver = true;
        Console.WriteLine("You won!!! The word was {0}", randWord);
        printWord(randWord);
        printNotInWord(badletters);
        printInWord(goodletters);
        Console.WriteLine("Your guess so far: {0}", curGuess);
        break;
      }
      
      if (mistakes == 6) {
        gameOver = true;
        Console.WriteLine("Game over! The word was {0}", randWord);
        printWord(randWord);
        printNotInWord(badletters);
        printInWord(goodletters);
        Console.WriteLine("Your guess so far: {0}", curGuess);
        break;
      }


      Console.WriteLine();
      printWord(curGuess);
      printNotInWord(badletters);
      printInWord(goodletters);
      Console.WriteLine("Your guess so far: {0}", curGuess);

      Console.Write("\n\nEnter a letter: ");
      string inLet = (Console.ReadLine()[0]).ToString().ToLower();

      //Console.WriteLine("You entered {0}!", inLet);

      int[] allInd = AllIndexesOf(randWord, inLet).Cast<int>().ToArray();

      try {
        //Console.WriteLine("First index: {0}, allInd[0]);
        int firstInd = allInd[0];
        foreach (int ind in allInd) goodGuesses[ind] = inLet[0];
        if (!goodletters.Contains(inLet[0])) goodletters.Add(inLet[0]);
      } catch (IndexOutOfRangeException) {
        //Console.WriteLine("Letter not found");
        mistakes++;
        if (!badletters.Contains(inLet[0])) badletters.Add(inLet[0]);
      }

      if (mistakes == 1) hangman[1,3] = 'o';
      if (mistakes == 2) hangman[2,3] = '|';
      if (mistakes == 3) hangman[2,2] = '/';
      if (mistakes == 4) hangman[2,4] = '\\';
      if (mistakes == 5) hangman[3,2] = '/';
      if (mistakes == 6) hangman[3,4] = '\\';
      Console.Clear();
    }
    } catch (IndexOutOfRangeException) { Console.WriteLine("Error. Exiting..."); };
  }

  static void printNotInWord(List<char> letters) {
    Console.Write("\n\nBad letters: ");
    foreach (char x in letters) Console.Write(x + " ");
    Console.WriteLine();
  }
  
  static void printInWord(List<char> letters) {
    Console.Write("Good letters: ");
    foreach (char x in letters) Console.Write(x + " ");
    Console.WriteLine();
  }

  static void printHangMan(char[,] hangman) {    
    for (int r = 0; r < 5; r++) {
      for (int c = 0; c < 5; c++) {
        Console.Write(hangman[r, c]);
      }
      Console.WriteLine();
    }
  }

  static void printWord(string word) {
    foreach (char l in word.ToCharArray()) Console.Write(l + "\t");
    Console.WriteLine();
    for (int i = 0; i < word.Length - 1; i++) Console.Write('‾' + "\t");
  }

  public static byte[] GetFileViaHttp(string url)
  {
    using (WebClient client = new WebClient())
    {
        return client.DownloadData(url);
    } 
  }

  public static IEnumerable<int> AllIndexesOf(this string str, string searchstring)
  {
    int minIndex = str.IndexOf(searchstring);
    while (minIndex != -1)
    {
        yield return minIndex;
        minIndex = str.IndexOf(searchstring, minIndex + searchstring.Length);
    }
  }
}