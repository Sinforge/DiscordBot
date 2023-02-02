
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace DiscordBot.Services
{
    public class DotaStatsService
    {
        public async void GetMostPlayedHeroes(ulong dotabuffId)
        {
            IWebDriver driver = new EdgeDriver();
            INavigation nav = driver.Navigate();
            nav.GoToUrl($"https://www.dotabuff.com/players/{dotabuffId}/heroes");

            var element = driver.FindElement(By.TagName("table"));
            Console.WriteLine(element.Text);
        }
    }

    public class MostPlayedHeroesEntity
    {
        public string Name { get; set; }
        public int Matches { get; set; }

        public float WinRate { get; set; }
        public float KdaRatio  { get; set; }
    }
}
