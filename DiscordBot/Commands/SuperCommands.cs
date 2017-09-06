namespace DiscordBot.Commands
{
	using System.IO;
	using System.Threading.Tasks;

	using DSharpPlus;
	using DSharpPlus.CommandsNext;
	using DSharpPlus.CommandsNext.Attributes;

	[Description("Super Secret commands.")]
	[RequirePermissions(Permissions.Administrator)]
	internal class SuperCommands
	{
		#region Public Methods and Operators

		[Command("check")]
		[Description(
			"<:monksmega:346792440746999808> This is a super secret admin command, use it in <#347409218715910155> <:monksmega:346792440746999808>")]
		public async Task Check(CommandContext ctx)
		{
			if (ctx.Channel.Id != 347409218715910155)
			{
				await ctx.RespondAsync(
					"<:monksmega:346792440746999808> This is a super secret admin command, use it in <#347409218715910155> <:monksmega:346792440746999808>");
				return;
			}
			await ctx.RespondAsync("**./newfiles/ contains:**");
			foreach (var file in Directory.GetFiles("./newfiles/")) await ctx.RespondAsync($"{file}");
			await ctx.RespondAsync("**./oldfiles/ contains:**");
			foreach (var file in Directory.GetFiles("./oldfiles/")) await ctx.RespondAsync($"{file}");
		}

		#endregion
	}
}