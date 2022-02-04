public static class WordAnalyser
{
    private static List<float> _letterProbability  = new() { 
        43.31f, //a
        10.56f, //b
        23.13f, //c
        17.25f, //d
        56.88f, //e
        9.24f,  //f
        12.59f, //g
        15.31f, //h
        38.45f, //i
        1.00f,  //j
        5.61f,  //k
        27.98f, //l
        15.36f, //m
        33.92f, //n
        36.51f, //o
        16.14f, //p
        1.00f,  //q
        38.64f, //r
        29.23f, //s
        35.43f, //t
        18.51f, //u
        5.13f,  //v
        6.57f,  //w
        1.48f,  //x
        9.06f,  //y
        1.39f    //z
    };
    public static IEnumerable<string> OrderByProbability(List<string> words)
    {
        /*
        E	11.1607% 56.88	    M	3.0129%	15.36
        A	8.4966%	 43.31	    H	3.0034%	15.31
        R	7.5809%	 38.64	    G	2.4705%	12.59
        I	7.5448%	 38.45	    B	2.0720%	10.56
        O	7.1635%	 36.51	    F	1.8121%	9.24
        T	6.9509%	 35.43	    Y	1.7779%	9.06
        N	6.6544%	 33.92	    W	1.2899%	6.57
        S	5.7351%	 29.23	    K	1.1016%	5.61
        L	5.4893%	 27.98	    V	1.0074%	5.13
        C	4.5388%	 23.13	    X	0.2902%	1.48
        U	3.6308%	 18.51	    Z	0.2722%	1.39
        D	3.3844%	 17.25	    J	0.1965%	1.00
        P	3.1671%	 16.14	    Q	0.1962%	(1)
        */

        return words.OrderBy(x => ScoreWord(x));
    }

    public static float ScoreWord(string word){
        return word.ToUpper().Distinct().Select(y => _letterProbability[(int)y - 65]).Sum();
    }

    public static IEnumerable<string> GetSomeGuesses(List<WordleChar> letters, string[] wordList)
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

        return WordAnalyser.OrderByProbability(guesses);
    }
}