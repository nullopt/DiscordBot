namespace DiscordBot.Managers
{
	using System;
	using System.Threading.Tasks;

	using DSharpPlus;
	using DSharpPlus.CommandsNext;
	using DSharpPlus.CommandsNext.Exceptions;

	internal class CommandManager
	{
		#region Public Methods and Operators

		public async Task CommandError(CommandErrorEventArgs e)
		{
			e.Context.Client.DebugLogger.LogMessage(LogLevel.Debug,
				"nullopt",
				$"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message}",
				DateTime.Now);

			if (e.Exception is ChecksFailedException)
			{
				var emoji = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");

				var embed = new DiscordEmbed
					            {
						            Title = "Access denied",
						            Description = $"{emoji} You do not have the permissions required to execute this command.",
						            Color = 0xFF0000
					            };
				await e.Context.RespondAsync("", embed: embed);
			}
		}

		public Task CommandExecuted(CommandExecutedEventArgs e)
		{
			e.Context.Client.DebugLogger.LogMessage(LogLevel.Debug,
				"nullopt",
				$"{e.Context.User.Username} successfully executed '{e.Command.QualifiedName}'",
				DateTime.Now);

			return Task.CompletedTask;
		}

		#endregion
	}
}