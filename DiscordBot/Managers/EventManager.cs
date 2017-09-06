namespace DiscordBot.Managers
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;

	using DSharpPlus;

	internal class EventManager
	{
		#region Fields

		private readonly string prefix;

		#endregion

		#region Constructors and Destructors

		public EventManager(string prefix)
		{
			this.prefix = prefix;
		}

		#endregion

		#region Public Methods and Operators

		public Task ClientError(ClientErrorEventArgs e)
		{
			e.Client.DebugLogger.LogMessage(LogLevel.Error,
				"nullopt",
				$"Exception occured: {e.Exception.GetType()}: {e.Exception.Message}",
				DateTime.Now);
			return Task.CompletedTask;
		}

		public Task GuildMemberAdd(GuildMemberAddEventArgs e)
		{
			var user = e.Member;
			var unverified = e.Guild.Roles.FirstOrDefault(x => x.Name == "unverified");

			user.GrantRoleAsync(unverified);

			return Task.CompletedTask;
		}

		public Task OnMessage(MessageCreateEventArgs e)
		{
			if (e.Channel.IsPrivate) return Task.CompletedTask;
			if (e.Author.IsBot) return Task.CompletedTask;

			if (e.Channel.Id == 353587697048223747) LandingPageChecks(e);

			if (e.Channel.Id == 347409218715910155)
				if (e.Message.Attachments.Count >= 1)
					foreach (var attachment in e.Message.Attachments) DownloadManager.DownloadFile(attachment, e);

			if (e.Message.Content.Contains("commands"))
				e.Message.RespondAsync("Commands:\n" + "**Members:**\n" + "[All channels]\n" + $"`{this.prefix}lookup`\n"
				                       + "\'[<#347430400424935426>]\n" + $"`{this.prefix}update`\n" + $"`{this.prefix}revert`\n"
				                       + "**Admins:**\n" + $"`{this.prefix}prune`\n" + "**Super Secret Commands:**\n"
				                       + $"`{this.prefix}check`");

			if (e.Message.Content.Contains("javascript")) e.Message.RespondAsync("JavaShit*");
			if (e.Message.Content.ToLower().Equals("knock knock")) e.Message.RespondAsync("Who\'s there?");

			return Task.CompletedTask;
		}

		public Task Ready(ReadyEventArgs e)
		{
			e.Client.DebugLogger.LogMessage(LogLevel.Info, "nullopt", "Client is ready to process events.", DateTime.Now);
			e.Client.UpdateStatusAsync(new Game { Name = "with Tsuhgi", StreamType = GameStreamType.NoStream, Url = "" });
			return Task.CompletedTask;
		}

		public Task SocketError(SocketErrorEventArgs e)
		{
			e.Client.DebugLogger.LogMessage(LogLevel.Error,
				"nullopt",
				$"Exception occured: {e.Exception.GetType()}: {e.Exception.Message}",
				DateTime.Now);
			return Task.CompletedTask;
		}

		#endregion

		#region Methods

		private static void LandingPageChecks(MessageCreateEventArgs e)
		{
			var user = Utility.GetUser(e, e.Author.Id);
			var role = e.Guild.Roles.FirstOrDefault(
				x => string.Equals(x.Name, "Admin", StringComparison.InvariantCultureIgnoreCase));
			if (user.Roles.Contains(role) || e.Author.Id == 347405668065607680) return;

//			if (!e.Message.Content.ToLower().Contains(";verify") 
//				&& !e.Message.Content.ToLower().Contains(";leave")
//				&& !e.Message.Content.ToLower().Contains("get me out of here")
//				&& !e.Message.Content.ToLower().Contains("let me stay"))
//			{
//				var response = e.Message.RespondAsync($"<@{e.Author.Id}> type `;verify` to get your account verified by an Admin.")
//					.Result;
//				Task.Delay(5000).ContinueWith(task => response.DeleteAsync());
//			}
			e.Message.DeleteAsync();
		}

		#endregion
	}
}