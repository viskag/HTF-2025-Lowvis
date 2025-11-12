using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Diagnostics;

namespace HTF
{
    public static class PuzzleSolvers
    {
        public static void SolveDockingBay(IWebDriver driver)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

            var binaryValues = new List<int>();
            for (int i = 0; i < 5; i++)
            {
                var valueElement = wait.Until(d => d.FindElement(By.Id($"randomValue-{i}")));
                binaryValues.Add(int.Parse(valueElement.Text.Trim()));
            }
            for (int i = 0; i < 5; i++)
            {
                var switchElement = wait.Until(d => d.FindElement(By.Id($"switch-{i}")));
                var currentClass = switchElement.GetAttribute("class");

                if (binaryValues[i] == 1 && !currentClass.Contains("up"))
                {
                    switchElement.Click();
                    wait.Until(d => d.FindElement(By.Id($"switch-{i}")).GetAttribute("class").Contains("up"));
                }
                else if (binaryValues[i] == 0 && !currentClass.Contains("down"))
                {
                    switchElement.Click();
                    switchElement.Click();
                    wait.Until(d => d.FindElement(By.Id($"switch-{i}")).GetAttribute("class").Contains("down"));
                }
                Thread.Sleep(500);
            }

            var dropButton = wait.Until(d =>
            {
                var button = d.FindElement(By.Id("button"));
                return button.Enabled ? button : null;
            });
            dropButton.Click();
            var submarine = wait.Until(d => d.FindElement(By.Id("submarine")));
            submarine.Click();
        }

        public static void SolveSubmarine(IWebDriver driver)
        {
            Thread.Sleep(500);
            var arrowImage = driver.FindElement(By.CssSelector("#instruction img.arrow"));

            while (arrowImage.Displayed)
            {
                var arrowSrc = arrowImage.GetAttribute("src");

                var body = driver.FindElement(By.TagName("body"));
                if (arrowSrc.Contains("up.png")) body.SendKeys(Keys.ArrowUp);
                else if (arrowSrc.Contains("down.png")) body.SendKeys(Keys.ArrowDown);
                else if (arrowSrc.Contains("left.png")) body.SendKeys(Keys.ArrowLeft);
                else if (arrowSrc.Contains("right.png")) body.SendKeys(Keys.ArrowRight);
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

        public static void SolveEscape(IWebDriver driver)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(d => d.FindElements(By.CssSelector(".container .square")).Count > 0);

                var squares = driver.FindElements(By.CssSelector(".container .square")).ToList();
                if (squares.Count == 0)
                {
                    return;
                }

                var actions = new Actions(driver);
                IWebElement activeSquare = null;

                foreach (var sq in squares)
                {
                    try
                    {
                        actions.MoveToElement(sq).Perform();
                        Thread.Sleep(250);

                        var cls = sq.GetAttribute("class") ?? "";
                        if (cls.Contains("active"))
                        {
                            activeSquare = sq;
                            break;
                        }

                        var found = driver.FindElements(By.CssSelector(".square.active"));
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

                if (activeSquare == null)
                {
                    var rnd = new Random();
                    var sw = Stopwatch.StartNew();
                    while (sw.Elapsed < TimeSpan.FromSeconds(8) && activeSquare == null)
                    {
                        var candidate = squares[rnd.Next(squares.Count)];
                        try
                        {
                            actions.MoveToElement(candidate).Perform();
                            Thread.Sleep(200);
                            var found = driver.FindElements(By.CssSelector(".square.active"));
                            if (found.Count > 0)
                            {
                                activeSquare = found.First();
                                break;
                            }
                        }
                        catch { }
                    }
                }

                if (activeSquare == null)
                {
                    return;
                }

                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        activeSquare.Click();
                        Thread.Sleep(200);
                    }
                    catch (StaleElementReferenceException)
                    {
                        var id = activeSquare.GetAttribute("id");
                        if (!string.IsNullOrEmpty(id))
                        {
                            activeSquare = driver.FindElement(By.Id(id));
                            activeSquare.Click();
                            Thread.Sleep(200);
                        }
                        else
                        {
                            var found = driver.FindElements(By.CssSelector(".square.active")).FirstOrDefault();
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

            }
            catch (Exception ex)
            {
                Console.WriteLine("SolveEscape error: " + ex.Message);
            }
        }

        public static void SpellAtlantisJS(IWebDriver driver)
        {
            Thread.Sleep(1000);
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                const string targetWord = "ATLANTIS";

                foreach (char c in targetWord)
                {
                    string letter = c.ToString();

                    var targetSlot = wait.Until(d => d.FindElement(By.XPath(
                        $"//div[@id='word-target']/div[@data-letter='{letter}' and not(.//div[contains(@class,'draggable-cube')])]"
                    )));

                    var sourceCube = wait.Until(d => d.FindElement(By.XPath(
                        $"//div[@id='draggable-cubes-container']//div[@data-letter='{letter}']"
                    )));

                    const string dragDropScript = @"
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
                    Thread.Sleep(300);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("SpellAtlantisJS error: " + ex.Message);
            }
        }

        public static void TheLongPress(IWebDriver driver)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                var actions = new Actions(driver);

                var crystal = wait.Until(d => d.FindElement(By.Id("crystal")));
                crystal.Click();
                Thread.Sleep(500);

                var outsideButton = wait.Until(d => d.FindElement(By.CssSelector("button.crystal-outside")));
                outsideButton.Click();
                Thread.Sleep(1000);

                var insideButton = wait.Until(d => d.FindElement(By.CssSelector("button.crystal-inside")));
                actions.ClickAndHold(insideButton).Perform();
                Thread.Sleep(8000);
                actions.Release(insideButton).Perform();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in TheLongPress: " + ex.Message);
            }
        }

        public static void BeatBoss(IWebDriver driver)
        {
            Thread.Sleep(3000);

            var body = driver.FindElement(By.TagName("body"));
            body.SendKeys(Keys.Space);
            Thread.Sleep(1000);

            while (true)
            {
                try
                {
                    for (int i = 0; i < 18; i++)
                    {
                        for (int j = 0; j < 1; j++)
                        {
                            body.SendKeys(Keys.Space);
                            Thread.Sleep(10);
                        }
                        body.SendKeys(Keys.ArrowLeft);
                        Thread.Sleep(10);
                    }

                    for (int i = 0; i < 18; i++)
                    {
                        for (int j = 0; j < 1; j++)
                        {
                            body.SendKeys(Keys.Space);
                            Thread.Sleep(10);
                        }
                        body.SendKeys(Keys.ArrowRight);
                        Thread.Sleep(10);
                    }

                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    break;
                }
            }
        }
    }
}
