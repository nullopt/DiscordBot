namespace DiscordBot.Commands
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;

	using DiscordBot.Managers;

	using DSharpPlus;
	using DSharpPlus.CommandsNext;
	using DSharpPlus.CommandsNext.Attributes;

	using Newtonsoft.Json;

	[Description("Member commands.")]
	[RequirePermissions(Permissions.AddReactions)]
	internal class MemberCommands
	{
		#region Public Methods and Operators

		[Command("lookup")]
		[Description("Lookup PUBG account")]
		public async Task LookUp(CommandContext ctx, [Description("Player username to lookup")] string args)
		{
			if (args == null)
			{
				await ctx.RespondAsync($"Example: `;lookup {ctx.Member.DisplayName}`");
				return;
			}
			await ctx.RespondAsync($"http://www.pubg.me/{args}");
		}

		//[Command("revert")]
		//[Description("Sends old files to user's DM's")]
		//public async Task Revert(CommandContext ctx,
		//                         [Description("What file(s) do you want to revert? [all | cheat | loader | settings]")]
		//                         string args = null)
		//{
		//	if (ctx.Channel.Id != 347430400424935426)
		//	{
		//		await ctx.RespondAsync($"<@{ctx.Member.Id}> <#347430400424935426>");
		//		return;
		//	}

		//	var cheat = Directory.GetFiles(OldFiles).FirstOrDefault(x => x.Contains("cheat.rar"));
		//	var loader = Directory.GetFiles(OldFiles).FirstOrDefault(x => x.Contains("loader.rar"));
		//	var settings = Directory.GetFiles(OldFiles).FirstOrDefault(x => x.Contains("settings.cfg"));

		//	ctx.Client.DebugLogger.LogMessage(LogLevel.Info,
		//		"Revert",
		//		"\n" + $"Discord: {cheat} | Loader: {loader} | Settings: {settings}",
		//		DateTime.Now);

		//	switch (args)
		//	{
		//		case "all":
		//			await ctx.RespondAsync("Attempting to send...");
		//			if (cheat != null)
		//			{
		//				File.Move(cheat, OldFiles + ctx.Member.Id + "cheat.rar");
		//				await ctx.Member.SendFileAsync(OldFiles + ctx.Member.Id + "cheat.rar", "**OLD CHEAT [RENAME YOUR FILES]:**");
		//			}
		//			if (loader != null)
		//			{
		//				File.Move(loader, OldFiles + ctx.Member.Id + "loader.rar");
		//				await ctx.Member.SendFileAsync(OldFiles + ctx.Member.Id + "loader.rar", "**OLD CHEAT [RENAME YOUR FILES]:**");
		//			}
		//			if (settings != null) await ctx.Member.SendFileAsync(settings, "**OLD SETTINGS:**");

		//			await ctx.RespondAsync($"<@{ctx.Member.Id}> **File(s) sent via DM\'s**");
		//			break;
		//		case "cheat":
		//			await ctx.RespondAsync("Attempting to send...");
		//			if (cheat != null)
		//			{
		//				File.Move(cheat, OldFiles + ctx.Member.Id + "cheat.rar");
		//				await ctx.Member.SendFileAsync(OldFiles + ctx.Member.Id + "cheat.rar", "**OLD CHEAT [RENAME YOUR FILES]:**");
		//				await ctx.RespondAsync($"<@{ctx.Member.Id}> **File(s) sent via DM\'s**");
		//			}
		//			else
		//			{
		//				await ctx.RespondAsync($"<@{ctx.Member.Id}> **cheat.rar doesn't exist/hasn't been updated.**");
		//			}
		//			break;
		//		case "loader":
		//			await ctx.RespondAsync("Attempting to send...");
		//			if (loader != null)
		//			{
		//				File.Move(loader, OldFiles + ctx.Member.Id + "loader.rar");
		//				await ctx.Member.SendFileAsync(OldFiles + ctx.Member.Id + "loader.rar", "**OLD CHEAT [RENAME YOUR FILES]:**");
		//				await ctx.RespondAsync($"<@{ctx.Member.Id}> **File(s) sent via DM\'s**");
		//			}
		//			else
		//			{
		//				await ctx.RespondAsync($"<@{ctx.Member.Id}> **loader.rar doesn't exist/hasn't been updated.**");
		//			}
		//			break;
		//		case "settings":
		//			await ctx.RespondAsync("Attempting to send...");
		//			if (settings != null)
		//			{
		//				await ctx.Member.SendFileAsync(settings, "**OLD SETTINGS:**");
		//				await ctx.RespondAsync($"<@{ctx.Member.Id}> **File(s) sent via DM\'s**");
		//			}
		//			else
		//			{
		//				await ctx.RespondAsync($"<@{ctx.Member.Id}> **settings.cfg doesn't exist/hasn't been updated.**");
		//			}
		//			break;
		//		default:
		//			await ctx.RespondAsync($"<@{ctx.Member.Id}>\n\n" + "`;revert all | cheat | loader | settings`\n"
		//			                       + "Example: `;revert cheat`\n" + $"Output: `{ctx.Message.Author.Id}cheat.rar`");
		//			break;
		//	}
		//}

		//[Command("update")]
		//[Description("Sends updated files to user's DM's")]
		//public async Task Update(CommandContext ctx,
		//                         [Description("What file(s) do you want to update? [all | cheat | loader | settings]")]
		//                         string args = null)
		//{
		//	if (ctx.Channel.Id != 347430400424935426)
		//	{
		//		await ctx.RespondAsync($"<@{ctx.Member.Id}> <#347430400424935426>");
		//		return;
		//	}

		//	var cheat = Directory.GetFiles(NewFiles).FirstOrDefault(x => x.Contains("cheat.rar"));
		//	var loader = Directory.GetFiles(NewFiles).FirstOrDefault(x => x.Contains("loader.rar"));
		//	var settings = Directory.GetFiles(NewFiles).FirstOrDefault(x => x.Contains("settings.cfg"));

		//	ctx.Client.DebugLogger.LogMessage(LogLevel.Debug,
		//		"nullopt",
		//		"\n" + $"Discord: {cheat} | Loader: {loader} | Settings: {settings}",
		//		DateTime.Now);

		//	switch (args)
		//	{
		//		case "all":
		//			await ctx.RespondAsync("Attempting to send...");
		//			if (cheat != null)
		//			{
		//				File.Move(cheat, NewFiles + ctx.Member.Id + "cheat.rar");
		//				await ctx.Member.SendFileAsync(NewFiles + ctx.Member.Id + "cheat.rar", "**UPDATED CHEAT [RENAME YOUR FILES]:**");
		//			}
		//			if (loader != null)
		//			{
		//				File.Move(loader, NewFiles + ctx.Member.Id + "loader.rar");
		//				await ctx.Member.SendFileAsync(NewFiles + ctx.Member.Id + "loader.rar", "**UPDATED CHEAT [RENAME YOUR FILES]:**");
		//			}
		//			if (settings != null) await ctx.Member.SendFileAsync(settings, "**UPDATED SETTINGS:**");

		//			await ctx.RespondAsync($"<@{ctx.Member.Id}> **File(s) sent via DM\'s**");
		//			break;
		//		case "cheat":
		//			await ctx.RespondAsync("Attempting to send...");
		//			if (cheat != null)
		//			{
		//				File.Move(cheat, NewFiles + ctx.Member.Id + "cheat.rar");
		//				await ctx.Member.SendFileAsync(NewFiles + ctx.Member.Id + "cheat.rar", "**UPDATED CHEAT [RENAME YOUR FILES]:**");
		//				await ctx.RespondAsync($"<@{ctx.Member.Id}> **File(s) sent via DM\'s**");
		//			}
		//			else
		//			{
		//				await ctx.RespondAsync($"<@{ctx.Member.Id}> **cheat.rar doesn't exist/hasn't been updated.**");
		//			}
		//			break;
		//		case "loader":
		//			await ctx.RespondAsync("Attempting to send...");
		//			if (loader != null)
		//			{
		//				File.Move(loader, NewFiles + ctx.Member.Id + "loader.rar");
		//				await ctx.Member.SendFileAsync(NewFiles + ctx.Member.Id + "loader.rar", "**UPDATED CHEAT [RENAME YOUR FILES]:**");
		//				await ctx.RespondAsync($"<@{ctx.Member.Id}> **File(s) sent via DM\'s**");
		//			}
		//			else
		//			{
		//				await ctx.RespondAsync($"<@{ctx.Member.Id}> **loader.rar doesn't exist/hasn't been updated.**");
		//			}
		//			break;
		//		case "settings":
		//			await ctx.RespondAsync("Attempting to send...");
		//			if (settings != null)
		//			{
		//				await ctx.Member.SendFileAsync(settings, "**UPDATED SETTINGS:**");
		//				await ctx.RespondAsync($"<@{ctx.Member.Id}> **File(s) sent via DM\'s**");
		//			}
		//			else
		//			{
		//				await ctx.RespondAsync($"<@{ctx.Member.Id}> **settings.cfg doesn't exist/hasn't been updated.**");
		//			}
		//			break;
		//		default:
		//			await ctx.RespondAsync($"<@{ctx.Member.Id}>\n\n" + "`;update all | cheat | loader | settings`\n"
		//			                       + "Example: `;update cheat`\n" + $"Output: `{ctx.Message.Author.Id}cheat.rar`");
		//			break;
		//	}
		//}

		[Command("revert")]
		public async Task Revert(CommandContext ctx, string args = null)
		{
			await ctx.Message.DeleteAsync();
			if (ctx.Channel.Id != 347430400424935426)
			{
				await ctx.RespondAsync($"<@{ctx.Member.Id}> <#347430400424935426>");
				return;
			}
			await ctx.Member.SendMessageAsync(GetLink(ctx, args));
			await ctx.RespondAsync($"<@{ctx.Member.Id}> **File(s) sent via DM\'s**");
		}

		[Command("update")]
		public async Task Update(CommandContext ctx, string args = null)
		{
			await ctx.Message.DeleteAsync();
			if (ctx.Channel.Id != 347430400424935426)
			{
				await ctx.RespondAsync($"<@{ctx.Member.Id}> <#347430400424935426>");
				return;
			}
			await ctx.Member.SendMessageAsync(GetLink(ctx, args));
			await ctx.RespondAsync($"<@{ctx.Member.Id}> **File(s) sent via DM\'s**");
		}

		#endregion

		#region Methods

		private static string GetLink(CommandContext ctx, string args)
		{
			var command = ctx.Message.Content.Split(' ')[0].Substring(1);
			var json = File.ReadAllText(@"Config\urls.json");
			var urls = JsonConvert.DeserializeObject<List<Url>>(json);

			if (args == null)
				return $"<@{ctx.Member.Id}>\n\n" + "`;update all | cheat | loader | settings`\n" + "Example: `;update cheat`\n"
				       + $"Output: `{ctx.Message.Author.Id}cheat.rar`";

			var message = $"**{(command == "update" ? "Latest" : "Old")} {args}:**\n";

			switch (command)
			{
				case "update":
					if (string.Equals(args, "all"))
					{
						message = $"**Latest Cheat:**\n";
						message +=
							$"{NewLink("cheat", urls)}\n**Latest Loader:**\n{NewLink("loader", urls)}\n**Latest Settings:**\n{NewLink("settings", urls)}";
						break;
					}
					message += NewLink(args, urls);
					break;
				case "revert":
					if (string.Equals(args, "all"))
					{
						message = $"**Old Cheat:**\n";
						message +=
							$"{OldLink("cheat", urls)}\n**Old Loader:**\n{OldLink("loader", urls)}\n**Old Settings:**\n{OldLink("settings", urls)}";
						break;
					}
					message += OldLink(args, urls);
					break;
				default: return "Unable to find URL - message nullopt for details.";
			}
			return message;
		}

		private static string NewLink(string args, IEnumerable<Url> urls)
		{
			return urls.First(x => x.Name == $"new{args}").Link;
		}

		private static string OldLink(string args, IEnumerable<Url> urls)
		{
			return urls.First(x => x.Name == $"old{args}").Link;
		}

		#endregion

		// private const string OldFiles = "./oldfiles/";

		// private const string NewFiles = "./newfiles/";
	}
}