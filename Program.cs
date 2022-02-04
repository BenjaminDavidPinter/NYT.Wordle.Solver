using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;


new DriverManager().SetUpDriver(new FirefoxConfig());
var _webDriver = new FirefoxDriver();

_webDriver.Navigate().GoToUrl($"https://www.devangthakkar.com/wordle_archive/?{new Random().Next(1, 229)}");

// See https://aka.ms/new-console-template for more information
var words = File.ReadAllLines("5LetterWords.txt");
var guess = "AADIEU";
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
WebDriverWait wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(10));
IWebElement firstResult = wait.Until(e => e.FindElement(By.XPath("/html/body/div[4]/div/div/div/button")));

firstResult.Click();
int divCounter = 1;
bool done = false;
while (!done)
{
    _webDriver.FindElement(By.XPath("*")).SendKeys($"{guess}" + Keys.Enter);
    guess = String.Join("",guess.Skip(1).Take(5));
    done = true;
    ///html/body/div[1]/div/div/div[2]/div/span[5]
    for (short i = 0; i < guess.Length; i++)
    {
        var letterInCollection = letters.First(x => x.Letter == guess.ToUpper()[i]);
        var ele = _webDriver.FindElement(By.XPath($"/html/body/div[1]/div/div/div[2]/div/span[{divCounter}]"));
        Console.WriteLine(divCounter);
        Console.WriteLine(ele.GetAttribute("class"));
        if(ele.GetAttribute("class").Contains("green")) {
            letterInCollection.Status = LetterStatus.CorrectPosition;
        } else if(ele.GetAttribute("class").Contains("yellow")) {
            done = false;
            letterInCollection.Status = LetterStatus.WrongPosition;
        } else if(ele.GetAttribute("class").Contains("gray")) {
            done = false;
            letterInCollection.Status = LetterStatus.Unused;
        }
        divCounter = divCounter + 1;

        if (letterInCollection.Status == LetterStatus.CorrectPosition)
        {
            letterInCollection.Position = i;
        }
        if (letterInCollection.Status == LetterStatus.WrongPosition)
        {
            letterInCollection.KnownAntipositions.Add(i);
        }
    }

    var guesses = WordAnalyser.GetSomeGuesses(letters, words);
    foreach (var g in guesses)
    {
        Console.WriteLine($"{g} - {WordAnalyser.ScoreWord(g)}");
    }
    foreach(var ltr in letters){
        Console.WriteLine($"{ltr.Letter} - {ltr.Status}");
    }
    guess = "A"+guesses.Last();
}



