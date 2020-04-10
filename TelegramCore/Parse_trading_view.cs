using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace TelegramCore
{
    class Parse_trading_view
    {
        static string url = "https://s.tradingview.com/widgetembed/?frameElementId=tradingview_cad3c&symbol=BYBIT%3ABTCUSD&interval=5&symboledit=1&saveimage=1&toolbarbg=f1f3f6&studies=RSI%40tv-basicstudies&theme=Light&style=1&timezone=Etc%2FUTC&studies_overrides=%7B%7D&overrides=%7B%7D&enabled_features=%5B%5D&disabled_features=%5B%5D&locale=ru&utm_source=&utm_medium=widget_new&utm_campaign=chart&utm_term=BYBIT%3ABTCUSD";
        static string class_name = "valueValue-3kA0oJs5";

        IWebDriver driver;

        public Parse_trading_view()
        {

            var chromeOptions = new ChromeOptions();

            chromeOptions.AddArguments("headless");//запуск браузера без ui
      
            driver = new ChromeDriver(@"C:\ChromeDriver\", chromeOptions);

            driver.Url = @url;

            System.Threading.Thread.Sleep(3000);

        }
        public string GetValueRSI()//получить значение с индикатора RSI
        {
            return driver.FindElements(By.XPath(".//div[@class='" + class_name + "']"))[9].Text;
        }
        public string GetValueAverage()//получить значение курса
        {
           
            return driver.FindElements(By.XPath(".//div[@class='" + class_name + "']"))[3].Text;
        }
        public void ReloadPage()//перезагрузить страницу
        {
            driver.Url = @url;
            System.Threading.Thread.Sleep(300);
        }
    }
}
