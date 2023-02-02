
using Discord;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace DiscordBot.Services
{
    public class DotaStatsService
    {
        public async Task<List<string>> GetMostPlayedHeroes(ulong dotabuffId)
        {
            List<string> listHeroes = new List<string>();
            IWebDriver driver = new EdgeDriver();
            INavigation nav = driver.Navigate();
            nav.GoToUrl($"https://www.dotabuff.com/players/{dotabuffId}/heroes");
            var elements = await Task.Run(async () =>
            {
                return driver.FindElement(By.TagName("tbody")).Text.Split("\r\n");
            });
            driver.Close();
            driver.Quit();
            Console.WriteLine("Получил данные с дотабафа");
            Console.WriteLine(elements);
            for (int i = 0; i < elements.Length;)
            {
                listHeroes.Add(elements[i] + "-" + elements[i + 2] + "-" + elements[i + 3] + "-" + elements[i + 4] + "-" + elements[i + 5].Split(" ")[0] +
                "-" + elements[i + 6].Substring(0, elements[i + 6].Length - 2) + "\n");
                i += 7;
                if (listHeroes.Count >= 5)
                {
                    break;
                }
            }

            return listHeroes;


        }


    }

    public class MostPlayedHeroesEntity
    {
        public string Name { get; set; }
        public int Matches { get; set; }

        public string WinRate { get; set; }
        public float KdaRatio  { get; set; }
        public string PrimaryRole { get; set; }
        
        public string PrimaryLane { get; set; }

        public override string ToString()
        {
            return $"{this.Name}-{this.Matches}-{this.WinRate}-{this.KdaRatio}-{this.PrimaryRole}-{this.PrimaryLane}";
        }
    }
}
