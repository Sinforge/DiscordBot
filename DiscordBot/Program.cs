using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.Net.Providers.WS4Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace DiscordBot
{
    public class Program
    {
        DiscordSocketClient client;
            CommandService command;
            IServiceProvider service;
            static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

            public async Task RunBotAsync()
            {
                client = new DiscordSocketClient(new DiscordSocketConfig
                {
                    WebSocketProvider = WS4NetProvider.Instance,
                    GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
                });
                command = new CommandService();
                //Add Singletons
                service = new ServiceCollection()
                    .AddSingleton(client)
                    .AddSingleton(command)
                    .BuildServiceProvider();


                //Add logs
                client.Log += Log;

                //Add command handler
                client.MessageReceived += MessageReceived;


                //Connect Command.cs file
                await command.AddModulesAsync(Assembly.GetEntryAssembly(), service);

                string token;


                string workingDirectory = Environment.CurrentDirectory;
                string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
                using (TextReader reader = File.OpenText(projectDirectory + "\\config.json"))
                {
                    JObject json = JObject.Parse(reader.ReadToEnd());
                    Console.WriteLine(json.ToString());
                    Console.WriteLine(json["Token"].ToString());
                    token = json["Token"].ToString();

                }
                await client.LoginAsync(TokenType.Bot, token);
                await client.StartAsync();
                await Task.Delay(-1);
            }


            //Method for handling commands
            private async Task MessageReceived(SocketMessage message)
            {
                var _message = message as SocketUserMessage;
                if (_message is null || message.Author.IsBot) return;
                int argPos = 0;
                if (_message.HasStringPrefix("$$", ref argPos) || _message.HasMentionPrefix(client.CurrentUser, ref argPos))
                {
                    var txt = new SocketCommandContext(client, _message);
                    var result = await command.ExecuteAsync(txt, argPos, service);
                    if (!result.IsSuccess)
                        Console.WriteLine(result.ErrorReason);
                }
            }

            private Task Log(LogMessage msg)
            {
                Console.WriteLine(msg.ToString());

                return Task.CompletedTask;
            }
    }
}