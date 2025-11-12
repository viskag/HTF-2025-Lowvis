Het bovenste project gaat door heel de game maar het is ook mogelijk om doormiddel van de urls door de game te gaan als de variabelen in de local storage correct worden ingesteld zoals in onderstaande code:

```csharp
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System.Text;

namespace HackTheFuture
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string baseUrl = "https://hackthefuture.bignited.be";
            FirefoxDriver driver = new FirefoxDriver();

            try
            {
                driver.Navigate().GoToUrl($"{baseUrl}/home");
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

                IWebElement startButton = wait.Until(d =>
                {
                    try
                    {
                        IWebElement el = d.FindElement(By.CssSelector("button.center-button"));
                        return (el.Displayed && el.Enabled) ? el : null;
                    }
                    catch (NoSuchElementException)
                    {
                        return null;
                    }
                }); 
                startButton.Click();

                System.Threading.Thread.Sleep(2000);

                Dictionary<string, string> items = new Dictionary<string, string>
                {
                    ["life"] = "3",
                    ["player"] = "1",
                    ["playerAge"] = "20",
                    ["playerCountry"] = "Belgium",
                    ["playerName"] = "Bob",
                    ["uselessTokenIGuess"] = "c4R+dnuFeoByhqByfo55jHuFfG+Gin5+cYWGcX4=",
                    ["boss"] = "o8O/tA==",
                    ["crashedSubmarine"] = "pMattLa5rQ==",
                    ["crystal"] = "true",
                    ["defeatedMonster"] = "pbmypq/IrreOw7q0wrm7",
                    ["escapedCrash"] = "psevor65rZaztb+p",
                    ["escapedTunnel"] = "psevor65rae2wrqmug==",
                    ["submarine"] = "pca7sb65rQ=="
                };

                SetLocalStorageItems(driver, items);

                await Task.Delay(5000);
                driver.Navigate().GoToUrl($"{baseUrl}/char-select");
                await Task.Delay(2000);
                driver.Navigate().GoToUrl($"{baseUrl}/office");
                await Task.Delay(2000);
                driver.Navigate().GoToUrl($"{baseUrl}/docking-bay");
                await Task.Delay(2000);
                driver.Navigate().GoToUrl($"{baseUrl}/submarine");
                await Task.Delay(2000);
                driver.Navigate().GoToUrl($"{baseUrl}/crash");
                await Task.Delay(2000);
                driver.Navigate().GoToUrl($"{baseUrl}/escape");
                await Task.Delay(2000);
                driver.Navigate().GoToUrl($"{baseUrl}/cave");
                await Task.Delay(2000);
                driver.Navigate().GoToUrl($"{baseUrl}/boss");
                await Task.Delay(2000);
                driver.Navigate().GoToUrl($"{baseUrl}/atlantis");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            Console.ReadLine();
        }

        static void SetLocalStorageItems(IWebDriver driver, IDictionary<string, string> items)
        {
            if (items == null || items.Count == 0) return;

            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, string> kv in items)
            {
                string keyEscaped = EscapeJsSingleQuoted(kv.Key);
                string valueEscaped = EscapeJsSingleQuoted(kv.Value);
                sb.AppendLine($"localStorage.setItem('{keyEscaped}', '{valueEscaped}');");
            }

            sb.AppendLine("return true;");

            string script = sb.ToString();

            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript(script);

            System.Threading.Thread.Sleep(200);
        }

        static string EscapeJsSingleQuoted(string input)
        {
            if (input == null) return "";
            return input.Replace("\\", "\\\\").Replace("'", "\\'");
        }
    }
}
