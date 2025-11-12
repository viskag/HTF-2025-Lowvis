using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace HTF
{
    public static class GameNavigator
    {
        public static void NavigateToUrl(IWebDriver driver, string url)
        {
            driver.Navigate().GoToUrl(url);
        }

        public static void WaitAndClick(IWebDriver driver, By locator, int timeoutSeconds = 10)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
            var element = wait.Until(d => d.FindElement(locator));
            element.Click();
        }

        public static IWebElement WaitForElement(IWebDriver driver, By locator, int timeoutSeconds = 10)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
            return wait.Until(d => d.FindElement(locator));
        }

        public static void SafeClick(IWebDriver driver, By locator, int timeoutSeconds = 10)
        {
            try
            {
                WaitAndClick(driver, locator, timeoutSeconds);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
