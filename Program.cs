// See https://aka.ms/new-console-template for more information
var words = File.ReadAllLines("5LetterWords.txt");
var guess = "ADIEU";
List<WordleChar> letters = new List<WordleChar>();

for (var i = 65; i < 91; i++)
{
    letters.Add(new WordleChar()
    {
        Letter = (char)i,
        Position = null,
        Status = LetterStatus.Unknown,
        KnownAntipositions = new List<short?>()
    });
}

Console.WriteLine("First, always guess adieu");
while (true)
{
    for (short i = 0; i < guess.Length; i++)
    {
        var currentLetterInGuess = guess[i];
        Console.Write($"Was {currentLetterInGuess} Green, Black, or Yellow (g/b/y) : ");
        var newStatus = Console.ReadLine()?.Trim().First() ?? 'u';
        var letterInCollection = letters.First(x => x.Letter == currentLetterInGuess);
        letterInCollection.Status = LetterStatusExtensions.Parse(newStatus);
        if (letterInCollection.Status == LetterStatus.CorrectPosition)
        {
            letterInCollection.Position = i;
        }
        if (letterInCollection.Status == LetterStatus.WrongPosition)
        {
            letterInCollection.KnownAntipositions.Add(i);
        }
    }

    var guesses = GetSomeGuesses(letters, words);
    foreach (var g in guesses)
    {
        Console.WriteLine(g);
    }
    Console.Write("Enter a guess: ");
    guess = Console.ReadLine().Trim().ToUpper();
}


List<string> GetSomeGuesses(List<WordleChar> letters, string[] wordList)
{
    List<string> guesses = new List<string>();

    var knownPositions = letters.Where(y => y.Status == LetterStatus.CorrectPosition);
    var knownInvalidLetters = letters.Where(y => y.Status == LetterStatus.Unused);
    var unknownValidLetters = letters.Where(y => y.Status == LetterStatus.WrongPosition);

    guesses.AddRange(wordList.Where(x =>
    {
        x = x.ToUpper();
        if (x.Any(y => knownInvalidLetters.Any(t => t.Letter == y)))
        {
            return false;
        }

        foreach (var pos in knownPositions)
        {
            if (x[(int)pos.Position] != pos.Letter)
            {
                return false;
            }
        }

        foreach (var pos in unknownValidLetters)
        {
            if (x.Contains(pos.Letter))
            {
                foreach (var knownInvalidPos in pos.KnownAntipositions)
                {
                    if (x[(int)knownInvalidPos] == pos.Letter)
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }));

    return guesses;
}
