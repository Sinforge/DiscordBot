
using AngleSharp.Html.Parser;
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
            string response;
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept",
                    "text/html,application/xhtml+xml,application/xml");
                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "UTF-8");

                response  = await client.GetStringAsync(new Uri($"https://www.dotabuff.com/players/{dotabuffId}/heroes"));

            }

            HtmlParser parser = new HtmlParser();
            var parsedContent = await parser.ParseDocumentAsync(response);
            if (parsedContent.QuerySelector(".fa fa-lock") != null)
            {
                return listHeroes;
            }

            var tdElements = parsedContent.Elemen ("td a");    
            for (int i = 0; i < tdElements.Length;)
            {
                listHeroes.Add(tdElements[i].TextContent + "-" + tdElements[i + 2].TextContent 
                               + "-" + tdElements[i + 3].TextContent + "-" + tdElements[i + 4].TextContent + "-" 
                               + tdElements[i + 5].TextContent.Split(" ")[0] + 
                               "-" + tdElements[i + 6].TextContent.Substring(0, tdElements[i + 6].TextContent.Length - 2) + "\n");
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
