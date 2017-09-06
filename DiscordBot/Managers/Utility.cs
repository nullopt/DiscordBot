namespace DiscordBot.Managers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using DSharpPlus;
	using DSharpPlus.CommandsNext;
	using DSharpPlus.Interactivity;

	internal class Utility
	{
		#region Public Methods and Operators

		public static async Task CreatePrivateChannel(CommandContext ctx, DiscordMember user, string message)
		{
			var existingChannel = ctx.Guild.Channels.FirstOrDefault(x => string.Equals(x.Name,
				"text" + user.DisplayName.Replace(" ", "-"),
				StringComparison.InvariantCultureIgnoreCase));
			if (existingChannel != null)
			{
				var exist = ctx.RespondAsync($"<#{existingChannel.Id}> already exists.").Result;
				await Task.Delay(5000);
				await exist.DeleteAsync();
				return;
			}
			var channel = ctx.Guild.CreateChannelAsync("text" + user.DisplayName.Replace(" ", "-"), ChannelType.Text).Result;
			var everyone = ctx.Guild.EveryoneRole;
			await channel.AddOverwriteAsync(everyone,
				Permissions.None,
				Permissions.ReadMessages | Permissions.ReadMessageHistory);
			await channel.AddOverwriteAsync(user,
				Permissions.ReadMessages | Permissions.ReadMessageHistory | Permissions.SendMessages,
				Permissions.None);
			await Task.Delay(250);
			await channel.SendMessageAsync(message);
		}

		public static async Task CreateVoiceChannel(CommandContext ctx, DiscordMember user, string message)
		{
			var existingChannel = ctx.Guild.Channels.FirstOrDefault(x => string.Equals(x.Name,
				"voice" + user.DisplayName.Replace(" ", "-"),
				StringComparison.InvariantCultureIgnoreCase));
			if (existingChannel != null)
			{
				var exist = ctx.RespondAsync($"Voice channel already exists.").Result;
				await Task.Delay(5000);
				await exist.DeleteAsync();
				return;
			}
			var channel = ctx.Guild.CreateChannelAsync("voice" + user.DisplayName.Replace(" ", "-"), ChannelType.Voice).Result;
			var everyone = ctx.Guild.EveryoneRole;
			await channel.AddOverwriteAsync(everyone, Permissions.None, Permissions.Connect | Permissions.Speak);
			await channel.AddOverwriteAsync(user,
				Permissions.Connect | Permissions.Speak | Permissions.UseVAD,
				Permissions.None);
			await Task.Delay(250);
			await channel.SendMessageAsync(message);
		}

		public static async Task EndChannel(CommandContext ctx, string type)
		{
			var user = GetUser(ctx, ctx.Message.MentionedUsers[0].Id);
			var channel = ctx.Guild.Channels.FirstOrDefault(x => string.Equals(x.Name,
				type + user.DisplayName.Replace(" ", "-"),
				StringComparison.InvariantCultureIgnoreCase));

			if (channel == null) return;

			await ctx.RespondAsync("Are you sure you want to end session? Yes/No");
			var interactivity = ctx.Client.GetInteractivityModule();
			var confirmation =
				await interactivity.WaitForMessageAsync(xm => xm.Author.Id == ctx.Member.Id, TimeSpan.FromSeconds(60));

			if (confirmation.Content.ToLower().Contains("yes")) await channel.DeleteAsync($"{type} chat closed.");
			else if (confirmation.Content.ToLower().Contains("no")) await ctx.RespondAsync("Session Resumed.");
		}

		public static DiscordMember GetUser(CommandContext ctx, string user)
		{
			return ctx.Guild.Members.FirstOrDefault(
				x => string.Equals(x.DisplayName, user, StringComparison.CurrentCultureIgnoreCase));
		}

		public static DiscordMember GetUser(CommandContext ctx, ulong id)
		{
			return ctx.Guild.GetMemberAsync(id).Result;
		}

		public static DiscordMember GetUser(MessageCreateEventArgs e, string user)
		{
			return e.Guild.Members.FirstOrDefault(x => string.Equals(x.DisplayName,
				user,
				StringComparison.CurrentCultureIgnoreCase));
		}

		public static DiscordMember GetUser(MessageCreateEventArgs e, ulong id)
		{
			return e.Guild.GetMemberAsync(id).Result;
		}

		public static async Task PrintEmbed(CommandContext ctx)
		{
			var embed = new DiscordEmbed
				            {
					            Color = 0x9B59B6, Title = "Welcome to Rembrandt's PUBG Tool.",
					            Author = new DiscordEmbedAuthor { IconUrl = ctx.Member.AvatarUrl, Name = ctx.Message.Author.Username },
					            Description = "To access this Discord Server you must first be authenticated by an Administrator. "
					                          + "This is to keep security at a maximum, for you and I.",
					            Fields = new List<DiscordEmbedField>
						                     {
							                     new DiscordEmbedField
								                     {
									                     Name = "I would like to be verified.",
									                     Value =
										                     "Please type: `;verify` in order to setup a private group chat with the administration team."
								                     },
							                     new DiscordEmbedField
								                     {
									                     Name = "I'm lost, get me out of here.",
									                     Value = "Please type: `;leave` to leave the server completely."
								                     }
						                     }
				            };

			await ctx.RespondAsync("", embed: embed);
		}

		public static async Task UserEmbed(CommandContext ctx, DiscordUser discordUser, int color, string title, string reason)
		{
			await ctx.RespondAsync("",
				embed: new DiscordEmbed
					       {
						       Color = color, Title = title,
						       Fields = new List<DiscordEmbedField>
							                {
								                new DiscordEmbedField
									                {
										                Inline = true, Name = "Username",
										                Value = $"{discordUser.Username}#{discordUser.Discriminator}"
									                },
								                new DiscordEmbedField { Inline = true, Name = "ID", Value = $"{discordUser.Id}" }
							                },
							   Description = reason
					       });
		}

		#endregion
	}
}