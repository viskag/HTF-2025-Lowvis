using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;

namespace HTF
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var driver = new FirefoxDriver();

            try
            {
                driver.Navigate().GoToUrl(GameSolver.BaseUrl);
                Console.WriteLine($"Title: {driver.Title}");

                GameSolver.SolveCompleteGame(driver);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                Console.ReadLine();
            }
        }
    }

    public static class GameSolver
    {
        public const string BaseUrl = "https://hackthefuture.bignited.be/";

        public static void SolveCompleteGame(IWebDriver driver)
        {
            SolveHomeSection(driver);
            SolveOfficeSection(driver);
            SolveDockingBaySection(driver);
            SolveSubmarineSection(driver);
            SolveCaveSection(driver);
            SolveBossBattle(driver);
        }

        private static void SolveHomeSection(IWebDriver driver)
        {
            Thread.Sleep(2000);
            LocalStorageManager.ClearToken(driver);
            GameNavigator.WaitAndClick(driver, By.CssSelector("button.center-button"));
            LocalStorageManager.SetPlayerData(driver);
        }

        private static void SolveOfficeSection(IWebDriver driver)
        {
            GameNavigator.NavigateToUrl(driver, $"{BaseUrl}office");
            Thread.Sleep(1000);
            GameNavigator.WaitAndClick(driver, By.Id("letters"));
            Thread.Sleep(1000);
            GameNavigator.WaitAndClick(driver, By.CssSelector(".close"));
            Thread.Sleep(8000);
            GameNavigator.WaitAndClick(driver, By.CssSelector("button#crystal"));
            Thread.Sleep(1000);
            GameNavigator.WaitAndClick(driver, By.Id("image-crystal"));
        }

        private static void SolveDockingBaySection(IWebDriver driver)
        {
            GameNavigator.NavigateToUrl(driver, $"{BaseUrl}docking-bay");
            PuzzleSolvers.SolveDockingBay(driver);
        }

        private static void SolveSubmarineSection(IWebDriver driver)
        {
            PuzzleSolvers.SolveSubmarine(driver);
        }

        private static void SolveCaveSection(IWebDriver driver)
        {
            Thread.Sleep(13000);
            var exitSquare = driver.FindElement(By.CssSelector("div.square"));
            var actions = new Actions(driver);
            actions.DoubleClick(exitSquare).Perform();
            Console.WriteLine("EXIT DARKNESS");

            Thread.Sleep(1000);
            PuzzleSolvers.SolveEscape(driver);
            PuzzleSolvers.SpellAtlantisJS(driver);
            PuzzleSolvers.TheLongPress(driver);
        }

        private static void SolveBossBattle(IWebDriver driver)
        {
            //GameNavigator.NavigateToUrl(driver, $"{BaseUrl}boss");
            PuzzleSolvers.BeatBoss(driver);
        }
    }
}
