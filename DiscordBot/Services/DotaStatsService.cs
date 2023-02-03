
using Discord;
using Newtonsoft.Json.Linq;
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
            var task = Task.Run(async() => driver.FindElement(By.TagName("tbody")).Text.Split("\r\n"));

            string[] elements;
            if(task.Wait(TimeSpan.FromSeconds(10)))
            {
                Console.WriteLine("i get data");
                elements = task.Result;
            }
            else
            {
                driver.Close();
                driver.Quit();
                return listHeroes;
            }

            driver.Close();
            driver.Quit();
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

        public async Task<ulong> GetSteamId(string customURL)
        {
            string steamApiKey;
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            using (TextReader reader = File.OpenText(projectDirectory + "\\config.json"))
            {
                JObject json = JObject.Parse(reader.ReadToEnd());
                steamApiKey = json["SteamWebApiKey"].ToString();
            }
            using (HttpClient client = new HttpClient())
            {
                string response = await client.GetStringAsync($"http://api.steampowered.com/ISteamUser/ResolveVanityURL/v0001/?key={steamApiKey}&vanityurl={customURL}");
                JObject json = JObject.Parse(response);
                if (int.Parse(json["response"]["success"].ToString()) == 1)
                {
                    Console.WriteLine("Successful request");
                    return ulong.Parse(json["response"]["steamid"].ToString());
                }
                else
                {
                    Console.WriteLine("Bad request");
                    return 0;
                }
            }
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
