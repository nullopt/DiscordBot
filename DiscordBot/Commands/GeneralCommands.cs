namespace DiscordBot.Commands
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;

	using DiscordBot.Managers;

	using DSharpPlus;
	using DSharpPlus.CommandsNext;
	using DSharpPlus.CommandsNext.Attributes;
	using DSharpPlus.Interactivity;

	[Description("General Commands")]
	internal class GeneralCommands
	{
		#region Public Methods and Operators

		[Command("end")]
		[Hidden]
		public async Task End(CommandContext ctx)
		{
			var channel = ctx.Guild.Channels.FirstOrDefault(x => string.Equals(x.Name,
				"Text" + ctx.Member.DisplayName.Replace(" ", "-"),
				StringComparison.InvariantCultureIgnoreCase));

			if (channel == null) return;

			await ctx.RespondAsync("Are you sure you want to end session? Yes/No");
			var interactivity = ctx.Client.GetInteractivityModule();
			var confirmation =
				await interactivity.WaitForMessageAsync(xm => xm.Author.Id == ctx.Member.Id, TimeSpan.FromSeconds(60));

			if (confirmation.Content.ToLower().Contains("yes")) await channel.DeleteAsync("User ended.");
			else if (confirmation.Content.ToLower().Contains("no"))
			{
				var message = ctx.RespondAsync("Session Resumed.").Result;
				await Task.Delay(5000).ContinueWith(task => message.DeleteAsync());
			}
		}

		[Command("verify")]
		[Hidden]
		public async Task Verify(CommandContext ctx)
		{
			if (ctx.Channel.Id != 353587697048223747) return;
			var user = Utility.GetUser(ctx, ctx.User.Id);
			var message = $"{user.Mention} - Use this channel to privately message the staff team. `;end` to end session.\n"
			              + $"<@179677325678346242> <@102870112314335232>";
			await Utility.CreatePrivateChannel(ctx, user, message);
		}

		[Command("leave")]
		[Hidden]
		public async Task Leave(CommandContext ctx)
		{
			if (ctx.Channel.Id != 353587697048223747) return;
			var user = ctx.User;

			await ctx.RespondAsync("Please type `GET ME OUT OF HERE` to leave the server or `LET ME STAY` to stay.");
			var interactivity = ctx.Client.GetInteractivityModule();
			var confirmation =
				await interactivity.WaitForMessageAsync(xm => xm.Author.Id == ctx.Member.Id, TimeSpan.FromSeconds(60));

			if (confirmation.Content.ToLower().Contains("get me out of here")) await ctx.Guild.RemoveMemberAsync((DiscordMember)user);
			else if (confirmation.Content.ToLower().Contains("let me stay"))
			{
				var message = ctx.RespondAsync("Fine. You can stay.").Result;
				await Task.Delay(5000).ContinueWith(task => message.DeleteAsync());
			}
		}

		#endregion
	}
}