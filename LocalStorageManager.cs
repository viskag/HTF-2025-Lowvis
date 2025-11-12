using OpenQA.Selenium;

namespace HTF
{
    public static class LocalStorageManager
    {
        public static void ClearToken(IWebDriver driver)
        {
            var js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("localStorage.removeItem('uselessTokenIGuess');");
        }

        public static void SetPlayerData(IWebDriver driver)
        {
            var js = (IJavaScriptExecutor)driver;
            const string script = @"
                localStorage.setItem('life', '3');
                localStorage.setItem('player', '1');
                localStorage.setItem('playerAge', '1');
                localStorage.setItem('playerCountry', 'Belgium');
                localStorage.setItem('playerName', 'Bob');
                return true;
            ";
            js.ExecuteScript(script);
        }
    }
}
