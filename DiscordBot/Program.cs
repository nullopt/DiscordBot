namespace DiscordBot
{
	using System.Threading.Tasks;

	using DiscordBot.Commands;
	using DiscordBot.Managers;

	using Dropbox.Api;

	using DSharpPlus;
	using DSharpPlus.CommandsNext;
	using DSharpPlus.Interactivity;

	using Newtonsoft.Json;

	internal class Program
	{
		#region Public Properties

		public static DiscordClient Client { get; set; }

		public CommandManager CommandManager { get; set; }

		public CommandsNextModule Commands { get; set; }

		public EventManager EventManager { get; set; }

		public InteractivityModule Interactivity { get; set; }

		#endregion

		#region Public Methods and Operators

		public async Task RunBot()
		{
			var json = JsonManager.ParseJsonAsync("Config/config.json").Result;
			var cfgjson = JsonConvert.DeserializeObject<ConfigJson>(json);
			var cfg = new DiscordConfig
				          {
					          Token = cfgjson.Token, TokenType = TokenType.Bot, AutoReconnect = true, LogLevel = LogLevel.Info,
					          UseInternalLogHandler = true
				          };

			var ccfg = new CommandsNextConfiguration
				           { StringPrefix = cfgjson.Prefix, EnableDms = true, EnableMentionPrefix = true };

			Client = new DiscordClient(cfg);
			this.Commands = Client.UseCommandsNext(ccfg);
			this.EventManager = new EventManager(cfgjson.Prefix);
			this.CommandManager = new CommandManager();

			Client.UseInteractivity();

			Client.Ready += this.EventManager.Ready;
			Client.ClientError += this.EventManager.ClientError;
			Client.MessageCreated += this.EventManager.OnMessage;
			Client.SocketError += this.EventManager.SocketError;

			Client.GuildMemberAdd += this.EventManager.GuildMemberAdd;

			this.Commands.CommandExecuted += this.CommandManager.CommandExecuted;
			this.Commands.CommandErrored += this.CommandManager.CommandError;

			this.Commands.RegisterCommands<CommandManager>();
			this.Commands.RegisterCommands<SuperCommands>();
			this.Commands.RegisterCommands<AdminCommands>();
			this.Commands.RegisterCommands<MemberCommands>();
			this.Commands.RegisterCommands<GeneralCommands>();
			this.Commands.RegisterCommands<HighscoreCommands>();

			await Client.ConnectAsync();
			await Task.Delay(-1);
		}

		#endregion

		#region Methods

		private static void Main()
		{
			new Program().RunBot().GetAwaiter().GetResult();
		}

		#endregion
	}
}