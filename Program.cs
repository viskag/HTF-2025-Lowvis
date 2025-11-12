using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Diagnostics;

namespace HTF
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string url = "https://hackthefuture.bignited.be/";

            FirefoxDriver driver = new FirefoxDriver();

            try
            {
                driver.Navigate().GoToUrl(url);
                Console.WriteLine($"Title: {driver.Title}");
                SolveTheGame(driver);//opl
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured: " + ex.Message);
            }
            //finally
            //{
            //    driver.Quit();
            //}
            static void SolveTheGame(IWebDriver driver)
            {
                //wacht ff []
                Thread.Sleep(2000);

                //klik op de start+clear onnodig token [home]
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("localStorage.removeItem('uselessTokenIGuess');");
                Console.WriteLine("LOCALSTORAGE CLEANED");
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                IWebElement startButton = wait.Until(d => d.FindElement(By.CssSelector("button.center-button")));
                startButton.Click();
                Console.WriteLine("START BUTTON KLIK");

                //vul personage gegevens in en cheat voor life-points [office]
                IJavaScriptExecutor jsExec = (IJavaScriptExecutor)driver;
                string setLocalStorageScript = @"
                    localStorage.setItem('life', '3');
                    localStorage.setItem('player', '1');
                    localStorage.setItem('playerAge', '1');
                    localStorage.setItem('playerCountry', 'Belgium');
                    localStorage.setItem('playerName', 'Bob');
                    return true;
                ";
                object? result = jsExec.ExecuteScript(setLocalStorageScript);
                Console.WriteLine("LocalStorage toegevoegd: " + (result ?? "ok"));
                string officeUrl = "https://hackthefuture.bignited.be/office";
                driver.Navigate().GoToUrl(officeUrl);
                //klik op papier
                Thread.Sleep(1000);
                IWebElement letters = driver.FindElement(By.Id("letters"));
                letters.Click();
                Console.WriteLine("LETTERS KLIK");
                //sluit letterspopup
                Thread.Sleep(1000);
                IWebElement closeButton = driver.FindElement(By.CssSelector(".close"));
                closeButton.Click();
                Console.WriteLine("SLUIT LETTERS");
                //klik crystal
                Thread.Sleep(8000);
                //var crystal = driver.FindElement(By.Id("crystal"));
                //crystal.Click();
                IWebElement crystalButton = driver.FindElement(By.CssSelector("button#crystal"));
                crystalButton.Click();
                Console.WriteLine("CRYSTAL KLIK");
                //klik crystal in popup
                Thread.Sleep(1000);
                IWebElement crystalImage = driver.FindElement(By.Id("image-crystal"));
                crystalImage.Click();
                Console.WriteLine("CRYSTAL 2DE KLIK");

                //los de switchpuzeel op [docking-bay]
                string dockingBayUrl = "https://hackthefuture.bignited.be/docking-bay";
                driver.Navigate().GoToUrl(dockingBayUrl);
                SolveDockingBay(driver);

                //los de subnavigatiepuzzel op [submarine]
                SolveSubmarine(driver);

                //zoek naar uitgang in de grot [crash]
                Thread.Sleep(13000);
                IWebElement exitSquare = driver.FindElement(By.CssSelector("div.square"));
                Actions actions = new Actions(driver);
                actions.DoubleClick(exitSquare).Perform();
                Console.WriteLine("EXIT DARKNESS");

                //solve escape/cave [escape]
                SolveEscape(driver);

                //solve wordpuzzle en longpress [cave]
                Thread.Sleep(1000);
                SpellAtlantisJS(driver);
                TheLongPress(driver);

                //boss battle [boss]
                BeatBoss(driver);
            }
            //docking-bay helpermethode
            static void SolveDockingBay(IWebDriver driver)
            {
                Thread.Sleep(2000);
                // lezen
                List<int> binaryValues = new List<int>();
                for (int i = 0; i < 5; i++)
                {
                    IWebElement valueElement = driver.FindElement(By.Id($"randomValue-{i}"));
                    string valueText = valueElement.Text.Trim();
                    binaryValues.Add(int.Parse(valueText));
                    Console.WriteLine($"Position {i}: {valueText}");
                }
                Console.WriteLine($"Binary pattern: {string.Join(" ", binaryValues)}");
                // zetten
                for (int i = 0; i < 5; i++)
                {
                    IWebElement switchElement = driver.FindElement(By.Id($"switch-{i}"));
                    string? currentClass = switchElement.GetAttribute("class");
                    // 1: 1keer drukken
                    if (binaryValues[i] == 1 && !currentClass.Contains("up"))
                    {
                        switchElement.Click();
                        Console.WriteLine($"Set switch {i} to UP (1)");
                    }
                    // 0: 2keer drukken
                    else if (binaryValues[i] == 0 && !currentClass.Contains("down"))
                    {
                        switchElement.Click(); switchElement.Click();
                        Console.WriteLine($"Set switch {i} to DOWN (0)");
                    }
                    else
                    {
                        Console.WriteLine($"Switch {i} already in correct position");
                    }
                    Thread.Sleep(500);
                }
                // klik drop
                IWebElement dropButton = driver.FindElement(By.Id("button"));
                Console.WriteLine("BUTTON GEVONDEN!!!!");
                dropButton.Click();
                Thread.Sleep(500);
                driver.FindElement(By.Id("submarine")).Click();
            }
            static void SolveSubmarine(IWebDriver driver)
            {
                Thread.Sleep(500);
                IWebElement arrowImage = driver.FindElement(By.CssSelector("#instruction img.arrow"));
                while (arrowImage.Displayed)
                {
                    string? arrowSrc = arrowImage.GetAttribute("src");
                    Console.WriteLine($"Current arrow: {arrowSrc}");
                    IWebElement body = driver.FindElement(By.TagName("body"));
                    if (arrowSrc.Contains("up.png")) body.SendKeys(Keys.ArrowUp);
                    else if (arrowSrc.Contains("down.png")) body.SendKeys(Keys.ArrowDown);
                    else if (arrowSrc.Contains("left.png")) body.SendKeys(Keys.ArrowLeft);
                    else if (arrowSrc.Contains("right.png")) body.SendKeys(Keys.ArrowRight);
                    Console.WriteLine($"Pressed {arrowSrc.Split('/').Last().Replace(".png", "").ToUpper()} arrow");
                    Thread.Sleep(1000);
                    try
                    {
                        arrowImage = driver.FindElement(By.CssSelector("#instruction img.arrow"));
                    }
                    catch
                    {
                        break;
                    }
                }
                Thread.Sleep(500);
            }
            static void SolveEscape(IWebDriver driver)
            {
                try
                {
                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                    // wacht vooraleer er square komt
                    wait.Until(d => d.FindElements(By.CssSelector(".container .square")).Count > 0);
                    List<IWebElement> squares = driver.FindElements(By.CssSelector(".container .square")).ToList();
                    if (squares.Count == 0)
                    {
                        Console.WriteLine("No squares found in container.");
                        return;
                    }
                    Actions actions = new Actions(driver);
                    IWebElement activeSquare = null;
                    //checke elke ingang
                    foreach (IWebElement? sq in squares)
                    {
                        try
                        {
                            actions.MoveToElement(sq).Perform();
                            Thread.Sleep(250); // wachten om tijd te geven aan de UI
                            //check class vd hovered item
                            string cls = sq.GetAttribute("class") ?? "";
                            if (cls.Contains("active"))
                            {
                                activeSquare = sq;
                                break;
                            }
                            // check overal voor square.active
                            System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> found = driver.FindElements(By.CssSelector(".square.active"));
                            if (found.Count > 0)
                            {
                                activeSquare = found.First();
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Hover attempt failed: {ex.Message}");
                        }
                    }
                    //als niet gevonden probeer random hovers voor korte periode
                    if (activeSquare == null)
                    {
                        Random rnd = new Random();
                        Stopwatch sw = Stopwatch.StartNew();
                        while (sw.Elapsed < TimeSpan.FromSeconds(8) && activeSquare == null)
                        {
                            IWebElement candidate = squares[rnd.Next(squares.Count)];
                            try
                            {
                                actions.MoveToElement(candidate).Perform();
                                Thread.Sleep(200);
                                System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> found = driver.FindElements(By.CssSelector(".square.active"));
                                if (found.Count > 0)
                                {
                                    activeSquare = found.First();
                                    break;
                                }
                            }
                            catch { /* verder doen */ }
                        }
                    }
                    if (activeSquare == null)
                    {
                        Console.WriteLine("Active square not found after hovering attempts.");
                        return;
                    }
                    Console.WriteLine($"Active square found (id='{activeSquare.GetAttribute("id") ?? "unknown"}'). Clicking 3 times...");
                    // Click the active square three times
                    for (int i = 0; i < 3; i++)
                    {
                        try
                        {
                            activeSquare.Click();
                            Thread.Sleep(200);
                        }
                        catch (StaleElementReferenceException)
                        {
                            // element might have been re-rendered; re-locate by selector
                            string? id = activeSquare.GetAttribute("id");
                            if (!string.IsNullOrEmpty(id))
                            {
                                activeSquare = driver.FindElement(By.Id(id));
                                activeSquare.Click();
                                Thread.Sleep(200);
                            }
                            else
                            {
                                // fallback: find any .square.active and click it
                                IWebElement? found = driver.FindElements(By.CssSelector(".square.active")).FirstOrDefault();
                                if (found != null)
                                {
                                    activeSquare = found;
                                    activeSquare.Click();
                                    Thread.Sleep(200);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Click #{i + 1} failed: {ex.Message}");
                        }
                    }

                    Console.WriteLine("Finished clicking active square.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SolveCave error: " + ex.Message);
                }
            }

            static void SpellAtlantisJS(IWebDriver driver)
            {
                try
                {
                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                    string targetWord = "ATLANTIS";
                    foreach (char c in targetWord)
                    {
                        string letter = c.ToString();
                        // Find first empty target-slot for this letter
                        IWebElement targetSlot = wait.Until(d => d.FindElement(By.XPath(
                            $"//div[@id='word-target']/div[@data-letter='{letter}' and not(.//div[contains(@class,'draggable-cube')])]"
                        )));
                        // Find a matching cube still in the draggable container
                        IWebElement sourceCube = wait.Until(d => d.FindElement(By.XPath(
                            $"//div[@id='draggable-cubes-container']//div[@data-letter='{letter}']"
                        )));
                        // Use JS to simulate proper HTML5 drag-and-drop
                        string dragDropScript = @"
                function simulateDragDrop(sourceNode, destinationNode) {
                    var dataTransfer = new DataTransfer();
                    var dragStartEvent = new DragEvent('dragstart', { dataTransfer: dataTransfer });
                    sourceNode.dispatchEvent(dragStartEvent);

                    var dropEvent = new DragEvent('drop', { dataTransfer: dataTransfer });
                    destinationNode.dispatchEvent(dropEvent);

                    var dragEndEvent = new DragEvent('dragend', { dataTransfer: dataTransfer });
                    sourceNode.dispatchEvent(dragEndEvent);
                }
                simulateDragDrop(arguments[0], arguments[1]);
            ";
                        ((IJavaScriptExecutor)driver).ExecuteScript(dragDropScript, sourceCube, targetSlot);
                        Console.WriteLine($"Placed '{letter}' via JS drag-and-drop.");

                        Thread.Sleep(300); // small delay
                    }
                    Console.WriteLine("Finished spelling ATLANTIS with JS drag-and-drop.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SpellAtlantisJS error: " + ex.Message);
                }
            }
            static void TheLongPress(IWebDriver driver)
            {
                try
                {
                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                    Actions actions = new Actions(driver);
                    //klik crystal eerste keer
                    IWebElement crystal = wait.Until(d => d.FindElement(By.Id("crystal")));
                    crystal.Click();
                    Console.WriteLine("Crystal clicked normally.");
                    Thread.Sleep(500); // small delay
                    // druk ergens buitoen
                    IWebElement outsideButton = wait.Until(d => d.FindElement(By.CssSelector("button.crystal-outside")));
                    outsideButton.Click();
                    Console.WriteLine("Crystal outside button clicked.");
                    Thread.Sleep(1000); // small delay
                    // houd ingedrukt
                    IWebElement insideButton = wait.Until(d => d.FindElement(By.CssSelector("button.crystal-inside")));
                    actions.ClickAndHold(insideButton).Perform();
                    Console.WriteLine("Started long press on crystal inside...");
                    Thread.Sleep(8000); // 7sec
                    actions.Release(insideButton).Perform();
                    Console.WriteLine("Released long press on crystal inside.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in TheLongPress: " + ex.Message);
                }
            }
            static void BeatBoss(IWebDriver driver)
            {
                Thread.Sleep(3000);
                //druk voor starten
                IWebElement body = driver.FindElement(By.TagName("body"));
                body.SendKeys(Keys.Space);
                Console.WriteLine("BOSS BATTLE START");
                Thread.Sleep(1000);
                //beweeg rechts terwijl je schiet initieel
                Console.WriteLine("Moving RIGHT 9 times while shooting...");
                for (int i = 0; i < 9; i++)
                {
                    // Shoot multiple times
                    for (int j = 0; j < 3; j++)
                    {
                        body.SendKeys(Keys.Space);
                        Thread.Sleep(50);
                    }
                    // Move right
                    body.SendKeys(Keys.ArrowRight);
                    Thread.Sleep(10);
                }
                // Daarna:links en recths 18 keer terwijl je schiet
                Console.WriteLine("Starting left-right pattern...");
                while (true)
                {
                    try
                    {
                        Console.WriteLine("Moving LEFT 18 times...");
                        for (int i = 0; i < 18; i++)
                        {
                            // shiet
                            for (int j = 0; j < 3; j++)
                            {
                                body.SendKeys(Keys.Space);
                                Thread.Sleep(50);
                            }
                            // links
                            body.SendKeys(Keys.ArrowLeft);
                            Thread.Sleep(10);
                        }
                        Console.WriteLine("Moving RIGHT 18 times...");
                        for (int i = 0; i < 18; i++)
                        {
                            // shier
                            for (int j = 0; j < 3; j++)
                            {
                                body.SendKeys(Keys.Space);
                                Thread.Sleep(50);
                            }
                            // rechts
                            body.SendKeys(Keys.ArrowRight);
                            Thread.Sleep(10);
                        }
                        Thread.Sleep(500);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Boss battle error: {ex.Message}");
                        break;
                    }
                }
            }
        }
    }
}
