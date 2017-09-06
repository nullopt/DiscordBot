namespace DiscordBot.Commands
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Threading.Tasks;

	using DiscordBot.Managers;

	using DSharpPlus;
	using DSharpPlus.CommandsNext;
	using DSharpPlus.CommandsNext.Attributes;
	using DSharpPlus.Interactivity;

	[Description("Administrative commands.")]
	[RequirePermissions(Permissions.Administrator)]
	internal class AdminCommands
	{
		#region Public Methods and Operators

		[Command("approve")]
		[Hidden]
		public async Task Approve(CommandContext ctx)
		{
			await ctx.Message.DeleteAsync();
			var general = ctx.Guild.Channels.FirstOrDefault(x => x.Id == 344181143488167936);
			var user = Utility.GetUser(ctx, ctx.Message.MentionedUsers[0].Id);
			var roles = ctx.Guild.Roles.FirstOrDefault(x => x.Name == "Member");
			if (user == null) return;

			await user.ReplaceRolesAsync(new List<DiscordRole> { roles });

			general?.SendMessageAsync("",
				embed: new DiscordEmbed
					       {
						       Color = 0x09C100, Title = "🙋 User Joined - Give them a warm welcome!",
						       Fields = new List<DiscordEmbedField>
							                {
								                new DiscordEmbedField
									                { Inline = true, Name = "Username", Value = $"{user.Username}#{user.Discriminator}" },
								                new DiscordEmbedField { Inline = true, Name = "ID", Value = $"{user.Id}" }
							                }
					       });
		}

		[Command("ban")]
		[Description("Bans the user")]
		public async Task Ban(CommandContext ctx, [Description("User to be banned")] string user, string reason = null)
		{
			if (ctx.Message.MentionedUsers.Count != 0)
			{
				var discordUser = ctx.Message.MentionedUsers.FirstOrDefault();
				if (discordUser != null)
				{
					await ctx.Guild.BanMemberAsync(discordUser.Id, int.MaxValue, reason);
					await Utility.UserEmbed(ctx, discordUser, 0xFF0000, "⛔️ User Banned", reason);
				}
			}
			else
			{
				var discordUser = Utility.GetUser(ctx, user);
				if (discordUser != null)
				{
					await ctx.Guild.BanMemberAsync(discordUser, int.MaxValue, reason);
					await Utility.UserEmbed(ctx, discordUser, 0xFF0000, "⛔️ User Banned", reason);
				}
			}
		}

		[Command("chat")]
		[Hidden]
		public async Task Chat(CommandContext ctx)
		{
			var user = Utility.GetUser(ctx, ctx.Message.MentionedUsers[0].Id);
			var message = $"{user.Mention}, <@{ctx.Member.Id}> initiated this chat with you.";
			await Utility.CreatePrivateChannel(ctx, user, message);
		}

		[Command("embed")]
		[Hidden]
		public async Task Embed(CommandContext ctx)
		{
			await Utility.PrintEmbed(ctx);
		}

		[Command("endchat")]
		[Hidden]
		public async Task EndChat(CommandContext ctx)
		{
			await Utility.EndChannel(ctx, "Text");
		}

		[Command("endgroup")]
		[Hidden]
		public async Task EndGroup(CommandContext ctx)
		{
			var channelName = "group-" + ctx.Message.Content.Split(' ')[1];
			var existingChannel =
				ctx.Guild.Channels.FirstOrDefault(x => string.Equals(x.Name,
					channelName,
					StringComparison.InvariantCultureIgnoreCase));
			if (existingChannel != null)
			{
				await ctx.RespondAsync("Are you sure you want close the group chat? Yes/No");
				var interactivity = ctx.Client.GetInteractivityModule();
				var confirmation =
					await interactivity.WaitForMessageAsync(xm => xm.Author.Id == ctx.Member.Id, TimeSpan.FromSeconds(60));

				if (confirmation.Content.ToLower().Contains("yes")) await existingChannel.DeleteAsync($"Admin closed channel");
				else if (confirmation.Content.ToLower().Contains("no")) await ctx.RespondAsync("Session Resumed");
			}
			else
			{
				await ctx.RespondAsync($"Channel #{channelName} no found.");
			}
		}

		[Command("endvoice")]
		[Hidden]
		public async Task EndVoice(CommandContext ctx)
		{
			await Utility.EndChannel(ctx, "Voice");
		}

		[Command("groupchat")]
		[Hidden]
		public async Task Group(CommandContext ctx)
		{
			await ctx.Message.DeleteAsync();
			var users = ctx.Message.MentionedUsers.Select(mentionedUser => Utility.GetUser(ctx, mentionedUser.Id)).ToList();
			var channelName = "Group-" + new Random().Next(1000, 10000);
			var message = $"<@{ctx.Member.Id}> initiated a groupchat with you.";
			var existingChannel =
				ctx.Guild.Channels.FirstOrDefault(x => string.Equals(x.Name,
					channelName,
					StringComparison.InvariantCultureIgnoreCase));

			if (existingChannel != null)
			{
				var exist = ctx.RespondAsync($"<#{existingChannel.Id}> already exists.").Result;
				await Task.Delay(5000);
				await exist.DeleteAsync();
				return;
			}

			var channel = ctx.Guild.CreateChannelAsync(channelName, ChannelType.Text).Result;
			message += $"<#{channel.Id}> \n";
			var everyone = ctx.Guild.EveryoneRole;
			users.ForEach(async user =>
				{
					await channel.AddOverwriteAsync(everyone,
						Permissions.None,
						Permissions.ReadMessages | Permissions.ReadMessageHistory);
					await channel.AddOverwriteAsync(user,
						Permissions.ReadMessages | Permissions.ReadMessageHistory | Permissions.SendMessages,
						Permissions.None);
					message += $"<@{user.Id}> ";
					await Task.Delay(500);
				});
			await Task.Delay(1000);
			await channel.SendMessageAsync(message);
		}

		[Command("kick")]
		[Description("Kicks the user")]
		public async Task Kick(CommandContext ctx, string user, string reason = null)
		{
			if (ctx.Message.MentionedUsers.Count != 0)
			{
				var discordUser = Utility.GetUser(ctx, ctx.Message.MentionedUsers.FirstOrDefault()?.Username);
				if (discordUser != null)
				{
					await ctx.Guild.RemoveMemberAsync(discordUser, reason);
					await Utility.UserEmbed(ctx, discordUser, 0xFFD400, "🏈 User Kicked", reason);
				}
			}
			else
			{
				var discordUser = Utility.GetUser(ctx, user);
				if (discordUser != null)
				{
					await ctx.Guild.RemoveMemberAsync(discordUser, reason);
					await Utility.UserEmbed(ctx, discordUser, 0xFFD400, "🏈 User Kicked", reason);
				}
			}
		}

		[Command("prune")]
		[Description("Deletes X amount of messages")]
		public async Task Prune(CommandContext ctx, [Description("Messages to be deleted")] int args)
		{
			await ctx.Channel.DeleteMessagesAsync(await ctx.Channel.GetMessagesAsync(args < 100 && args > 0 ? args + 1 : 100));
		}

		[Command("restart")]
		[Description("Restarts the bot")]
		public async Task Restart(CommandContext ctx)
		{
			try
			{
				Process.Start("DiscordBot.exe");
				await ctx.RespondAsync("Restarted.");
			}
			catch (Exception e)
			{
				ctx.Client.DebugLogger.LogMessage(LogLevel.Error,
					"Restart Command",
					$"Unable to start the program: {e}",
					DateTime.Now);
			}
			Process.GetProcessById(Process.GetCurrentProcess().Id).CloseMainWindow();
			await Task.CompletedTask;
		}

		[Command("voice")]
		[Hidden]
		public async Task Voice(CommandContext ctx)
		{
			var user = Utility.GetUser(ctx, ctx.Message.MentionedUsers[0].Id);
			var message = $"<@{user.Id}>, {user.Nickname} has invited you to voice chat. Join voice{user.DisplayName} to talk.";
			await Utility.CreateVoiceChannel(ctx, user, message);
		}

		#endregion
	}
}