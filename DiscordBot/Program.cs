using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.Net;
using Discord.Net.Providers.WS4Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DiscordBot
{
    public class Program
    {
            private DiscordSocketClient client;
            private CommandService command;
            private IServiceProvider service;
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
                client.SlashCommandExecuted += SlashCommandHandler;
                 client.Ready += ClientReady;

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

            public async Task ClientReady()
            {
                var guild = client.GetGuild(client.Guilds.Last().Id);
                var guildCommand = new SlashCommandBuilder();

                guildCommand.WithName("list-roles");
                guildCommand.WithDescription("This is my first guild slash command")
                    .AddOption("user", ApplicationCommandOptionType.User, "The users whos roles you want to listed",
                        isRequired: true);


                try
                {
                    await guild.CreateApplicationCommandAsync(guildCommand.Build());
                }
                catch (ApplicationCommandException ex)
                {
                    var json = JsonConvert.SerializeObject(ex.Reason, Formatting.Indented);

                    // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
                    Console.WriteLine(json);
                }
            }

            private async Task SlashCommandHandler(SocketSlashCommand command)
            {
                switch (command.Data.Name)
                {
                case "list-roles":
                    await HandleListRoleCommand(command);
                    break;
                }
            }

            private async Task HandleListRoleCommand(SocketSlashCommand command)
            {
                var guildUser = (SocketGuildUser)command.Data.Options.First().Value;
                var roleList = string.Join(",\n", guildUser.Roles.Where(x => !x.IsEveryone).Select(x => x.Mention));

                var embedBuiler = new EmbedBuilder()
                    .WithAuthor(guildUser.ToString(), guildUser.GetAvatarUrl() ?? guildUser.GetDefaultAvatarUrl())
                    .WithTitle("Roles")
                    .WithDescription(roleList)
                    .WithColor(Color.Purple)
                    .WithCurrentTimestamp();

                // Now, Let's respond with the embed.
                await command.RespondAsync(embed: embedBuiler.Build(), ephemeral: true);
        }

            private Task Log(LogMessage msg)
            {
                Console.WriteLine(msg.ToString());

                return Task.CompletedTask;
            }
    }
}