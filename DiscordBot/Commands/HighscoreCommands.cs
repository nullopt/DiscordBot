namespace DiscordBot.Commands
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;

	using DiscordBot.Managers;

	using DSharpPlus;
	using DSharpPlus.CommandsNext;
	using DSharpPlus.CommandsNext.Attributes;
	using DSharpPlus.Interactivity;

	using Newtonsoft.Json;

	[Group("scores")]
	[Description("Highscore Commands")]
	internal class HighscoreCommands
	{
		#region Constants

		private const string Bold = "**";

		private const string Bronze = "<:bronze:349759608912347157>";

		private const string Gold = "<:gold:349759578092863488>";

		private const string Silver = "<:silver:349759594479747072>";

		private const string Spacer = "<:spacer:349754197216067585>";

		#endregion

		#region Static Fields

		public static List<Score> SCORES = new List<Score>();

		public static IEnumerable<User> USERS;

		private static readonly DiscordEmbed Embed = new DiscordEmbed
			                                             {
				                                             Title = "**Leaderboard**", Color = 0x9B59B6,
				                                             Fields = new List<DiscordEmbedField>
					                                                      {
						                                                      new DiscordEmbedField
							                                                      { Inline = true, Name = "Users [Overall Rank]" },
						                                                      new DiscordEmbedField { Inline = true, Name = "First" },
						                                                      new DiscordEmbedField { Inline = true, Name = "Second" }
					                                                      }
			                                             };

		#endregion

		#region Public Methods and Operators

		public static void UpdateHighscores(CommandContext ctx)
		{
			foreach (var user in USERS)
			{
				var doc = WebpageManager.ReadWebpage(
					$"https://pubg.me/player/{user.PubgName}/{user.Mode}?season=2017-pre4&region={user.Region}");

				var overallRank =
					doc.DocumentNode.GetNodeText("//*[@id=\"body\"]/div/div[3]/div[1]/div[2]/div/div/div[1]/div[3]/div[1]");
				var winRank =
					doc.DocumentNode.GetNodeText("//*[@id=\"body\"]/div/div[3]/div[1]/div[2]/div/div/div[2]/div[3]/div[1]");
				var killRank =
					doc.DocumentNode.GetNodeText("//*[@id=\"body\"]/div/div[3]/div[1]/div[2]/div/div/div[3]/div[3]/div[1]");

				var headshotRatio =
					doc.DocumentNode.GetNodeText("//*[@id=\"body\"]/div/div[3]/div[1]/div[3]/div/div/div[3]/div[6]/div[1]");
				var kda = doc.DocumentNode.GetNodeText("//*[@id=\"body\"]/div/div[3]/div[1]/div[2]/div/div/div[3]/div[1]/div[1]");
				var mostKills =
					doc.DocumentNode.GetNodeText("//*[@id=\"body\"]/div/div[3]/div[1]/div[3]/div/div/div[3]/div[2]/div[1]");
				var winRatio =
					doc.DocumentNode.GetNodeText("//*[@id=\"body\"]/div/div[3]/div[1]/div[2]/div/div/div[2]/div[1]/div[1]");
				var wins = doc.DocumentNode.GetNodeText("//*[@id=\"body\"]/div/div[3]/div[1]/div[2]/div/div/div[2]/div[2]/div[1]");
				var longestKill =
					doc.DocumentNode.GetNodeText("//*[@id=\"body\"]/div/div[3]/div[1]/div[3]/div/div/div[3]/div[7]/div[1]");
				var totalDamage =
					doc.DocumentNode.GetNodeText("//*[@id=\"body\"]/div/div[3]/div[1]/div[3]/div/div/div[1]/div[9]/div[1]");
				var averageDamage =
					doc.DocumentNode.GetNodeText("//*[@id=\"body\"]/div/div[3]/div[1]/div[3]/div/div/div[3]/div[1]/div[1]");
				var kills = doc.DocumentNode.GetNodeText("//*[@id=\"body\"]/div/div[3]/div[1]/div[2]/div/div/div[3]/div[2]/div[1]");
				var headshots =
					doc.DocumentNode.GetNodeText("//*[@id=\"body\"]/div/div[3]/div[1]/div[3]/div/div/div[3]/div[5]/div[1]");

				var oRank = int.Parse(overallRank.Replace(",", string.Empty));
				var wRank = int.Parse(winRank.Replace(",", string.Empty));
				var kRank = int.Parse(killRank.Replace(",", string.Empty));
				overallRank = string.IsNullOrEmpty(overallRank) ? "0" : GetRank(oRank);
				winRank = string.IsNullOrEmpty(winRank) ? "0" : GetRank(wRank);
				killRank = string.IsNullOrEmpty(killRank) ? "0" : GetRank(kRank);
				Sanitize(ref headshotRatio,
					ref kda,
					ref mostKills,
					ref winRatio,
					ref wins,
					ref longestKill,
					ref totalDamage,
					ref averageDamage,
					ref kills,
					ref headshots);

				SCORES.Add(new Score(user.DiscordId,
					int.Parse(overallRank),
					int.Parse(winRank),
					int.Parse(killRank),
					float.Parse(headshotRatio.Replace("%", string.Empty)),
					int.Parse(headshots),
					float.Parse(kda),
					int.Parse(kills),
					int.Parse(longestKill.Replace(" m", string.Empty)),
					int.Parse(mostKills),
					int.Parse(totalDamage.Replace(",", string.Empty)),
					float.Parse(averageDamage),
					float.Parse(winRatio.Replace("%", string.Empty)),
					int.Parse(wins)));
			}
		}

		[Command("damage")]
		[Description("View the Damage high scores")]
		public async Task Damage(CommandContext ctx)
		{
			var loading = await Load(ctx);

			var position = 1;
			var damage = Embed.Fields.First(x => x.Name == "First");
			var averageDamage = Embed.Fields.First(x => x.Name == "Second");
			if (damage != null) damage.Name = "**Damage**";
			if (averageDamage != null) averageDamage.Name = "**Average Damage Per Match**";

			foreach (var score in SCORES.OrderByDescending(score => score.TotalDamage).Take(10))
			{
				var ranking = $"{(position == 1 ? Gold : "")}{(position == 2 ? Silver : "")}{(position == 3 ? Bronze : Spacer)}";
				var userName = Embed.Fields.FirstOrDefault(x => x.Name == "Users");
				if (userName != null)
					userName.Value +=
						$"{(position == 1 ? Bold : "")}{position} - {(await ctx.Guild.GetMemberAsync(ulong.Parse(score.DiscordId)))?.DisplayName} [<{score.OverallRank}]{(position == 1 ? Bold : "")} {ranking}\n";

				var highestDamage = SCORES.OrderByDescending(o => o.TotalDamage).FirstOrDefault()?.DiscordId == score.DiscordId;
				var highestAvgDamage = SCORES.OrderByDescending(o => o.AverageDamage).FirstOrDefault()?.DiscordId
				                       == score.DiscordId;

				if (damage != null)
					damage.Value +=
						$"{(highestDamage ? Bold : "")}{score.TotalDamage}{(highestDamage ? Bold : "")} {(highestDamage ? Gold : Spacer)}\n";

				if (averageDamage != null)
					averageDamage.Value +=
						$"{(highestAvgDamage ? Bold : "")}{score.AverageDamage}{(highestAvgDamage ? Bold : "")} {(highestAvgDamage ? Gold : Spacer)}\n";

				position++;
				await Task.Delay(100);
			}
			await loading.DeleteAsync();
			await ctx.RespondAsync("", embed: Embed);
		}

		[Command("headshots")]
		[Description("View the Headshots high scores")]
		public async Task Headshots(CommandContext ctx)
		{
			var loading = await Load(ctx);

			var position = 1;
			var headshots = Embed.Fields.First(x => x.Name == "First");
			var headshotPercentage = Embed.Fields.First(x => x.Name == "Second");
			if (headshots != null) headshots.Name = "**Headshots**";
			if (headshotPercentage != null) headshotPercentage.Name = "**Headshot Percentage**";

			foreach (var score in SCORES.OrderByDescending(score => score.Headshots).Take(10))
			{
				var ranking = $"{(position == 1 ? Gold : "")}{(position == 2 ? Silver : "")}{(position == 3 ? Bronze : Spacer)}";
				var userName = Embed.Fields.FirstOrDefault(x => x.Name == "Users");
				if (userName != null)
					userName.Value +=
						$"{(position == 1 ? Bold : "")}{position} - {(await ctx.Guild.GetMemberAsync(ulong.Parse(score.DiscordId))).DisplayName} [<{score.OverallRank}]{(position == 1 ? Bold : "")} {ranking}\n";

				var highestHs = SCORES.OrderByDescending(o => o.Headshots).FirstOrDefault()?.DiscordId == score.DiscordId;
				var highestPercent = SCORES.OrderByDescending(o => o.HeadshotRatio).FirstOrDefault()?.DiscordId == score.DiscordId;

				if (headshots != null)
					headshots.Value +=
						$"{(highestHs ? Bold : "")}{score.Headshots}{(highestHs ? Bold : "")} {(highestHs ? Gold : Spacer)}\n";

				if (headshotPercentage != null)
					headshotPercentage.Value +=
						$"{(highestPercent ? Bold : "")}{score.HeadshotRatio}{(highestPercent ? Bold : "")}% {(highestPercent ? Gold : Spacer)}\n";

				position++;
				await Task.Delay(100);
			}
			await loading.DeleteAsync();
			await ctx.RespondAsync("", embed: Embed);
		}

		[Command("kills")]
		[Description("View the Kills high scores")]
		public async Task Kills(CommandContext ctx)
		{
			var loading = await Load(ctx);

			var position = 1;
			var kills = Embed.Fields.First(x => x.Name == "First");
			var kda = Embed.Fields.First(x => x.Name == "Second");
			if (kda != null) kda.Name = "**KDA**";
			if (kills != null) kills.Name = "**Kills**";

			foreach (var score in SCORES.OrderByDescending(score => score.Kills).Take(10))
			{
				var ranking = $"{(position == 1 ? Gold : "")}{(position == 2 ? Silver : "")}{(position == 3 ? Bronze : Spacer)}";
				var userName = Embed.Fields.FirstOrDefault(x => x.Name == "Users");
				if (userName != null)
					userName.Value +=
						$"{(position == 1 ? Bold : "")}{position} - {(await ctx.Guild.GetMemberAsync(ulong.Parse(score.DiscordId))).DisplayName} [<{score.OverallRank}]{(position == 1 ? Bold : "")} {ranking}\n";

				var highestKills = SCORES.OrderByDescending(o => o.Kills).FirstOrDefault()?.DiscordId == score.DiscordId;
				var highestKda = SCORES.OrderByDescending(o => Math.Abs(o.Kda) <= 0 ? o.Kills : o.Kda).FirstOrDefault()?.DiscordId
				                 == score.DiscordId;

				if (kills != null)
					kills.Value +=
						$"{(highestKills ? Bold : "")}{score.Kills}{(highestKills ? Bold : "")} {(highestKills ? Gold : Spacer)}\n";

				if (kda != null)
					kda.Value +=
						$"{(highestKda ? Bold : "")}{(Math.Abs(score.Kda) <= 0 ? score.Kills : score.Kda)}{(highestKda ? Bold : "")} {(highestKda ? Gold : Spacer)}\n";

				position++;
				await Task.Delay(100);
			}
			await loading.DeleteAsync();
			await ctx.RespondAsync("", embed: Embed);
		}

		[Command("rank")]
		[Description("View the Ranking high scores")]
		public async Task Rank(CommandContext ctx)
		{
			var loading = await Load(ctx);

			var position = 1;
			var wRank = Embed.Fields.First(x => x.Name == "First");
			var kRank = Embed.Fields.First(x => x.Name == "Second");
			if (wRank != null) wRank.Name = "**Win Rank**";
			if (kRank != null) kRank.Name = "**Kill Rank**";

			foreach (var score in SCORES.OrderBy(score => score.OverallRank).Take(10))
			{
				var ranking = $"{(position == 1 ? Gold : "")}{(position == 2 ? Silver : "")}{(position == 3 ? Bronze : Spacer)}";
				var userName = Embed.Fields.FirstOrDefault(x => x.Name == "Users");
				if (userName != null)
					userName.Value +=
						$"{(position == 1 ? Bold : "")}{position} - {(await ctx.Guild.GetMemberAsync(ulong.Parse(score.DiscordId)))?.DisplayName} [<{score.OverallRank}]{(position == 1 ? Bold : "")} {ranking}\n";

				var highestWRank = SCORES.OrderBy(o => o.WinRank).FirstOrDefault()?.DiscordId == score.DiscordId;
				var highestKRank = SCORES.OrderBy(o => o.KillRank).FirstOrDefault()?.DiscordId == score.DiscordId;

				if (wRank != null)
					wRank.Value +=
						$"{(highestWRank ? Bold : "")}Top {score.WinRank}{(highestWRank ? Bold : "")} {(highestWRank ? Gold : Spacer)}\n";

				if (kRank != null)
					kRank.Value +=
						$"{(highestKRank ? Bold : "")}Top {score.KillRank}{(highestKRank ? Bold : "")} {(highestKRank ? Gold : Spacer)}\n";

				position++;
				await Task.Delay(100);
			}
			await loading.DeleteAsync();
			await ctx.RespondAsync("", embed: Embed);
		}

		[Command("verify")]
		[Description("Verify a PUBG account to your Discord account.")]
		public async Task Verify(CommandContext ctx,
		                         [Description("Account to add")] string account,
		                         [Description("Game Mode")] string mode,
		                         [Description("Region")] string region)
		{
			await ctx.Message.DeleteAsync();
			if (!SanitiseInput(ctx, mode, region)) return;
			var interactivity = ctx.Client.GetInteractivityModule();

			var json = File.ReadAllText(@"Config\users.json");
			var users = JsonConvert.DeserializeObject<List<User>>(json);

			if (users.Count(user => user.DiscordId == ctx.Member.Id.ToString()) != 0)
			{
				await ctx.RespondAsync("You have already verified an account. If you wish to remove it, message nullopt.");
				return;
			}

			users.Add(new User { DiscordId = ctx.Member.Id.ToString(), Mode = mode, PubgName = account, Region = region });
			await ctx.RespondAsync("Type CONFIRM to lock this PUBG account to your Discord.");
			var confirmation =
				await interactivity.WaitForMessageAsync(
					xm => xm.Content.ToLower().Contains("confirm") && xm.Author.Id == ctx.Member.Id,
					TimeSpan.FromSeconds(60));

			if (confirmation == null) await ctx.RespondAsync("Unable to verify account.");
			else
				try
				{
					json = JsonConvert.SerializeObject(users, Formatting.Indented);
					File.WriteAllText(@"Config\users.json", json);
					await ctx.RespondAsync("Account successfully added.");
				}
				catch (Exception e)
				{
					ctx.Client.DebugLogger.LogMessage(LogLevel.Error,
						"nullopt",
						$"Unable to write account to users.json - {e}",
						DateTime.Now);
				}
		}

		[Command("wins")]
		[Description("View the Kills high scores")]
		public async Task Wins(CommandContext ctx)
		{
			var loading = await Load(ctx);

			var position = 1;
			var wins = Embed.Fields.First(x => x.Name == "First");
			var winsRatio = Embed.Fields.First(x => x.Name == "Second");
			if (wins != null) wins.Name = "**Wins**";
			if (winsRatio != null) winsRatio.Name = "**Ratio**";

			foreach (var score in SCORES.OrderByDescending(score => score.Kills).Take(10))
			{
				var ranking = $"{(position == 1 ? Gold : "")}{(position == 2 ? Silver : "")}{(position == 3 ? Bronze : Spacer)}";
				var userName = Embed.Fields.FirstOrDefault(x => x.Name == "Users");
				if (userName != null)
					userName.Value +=
						$"{(position == 1 ? Bold : "")}{position} - {(await ctx.Guild.GetMemberAsync(ulong.Parse(score.DiscordId))).DisplayName} [<{score.OverallRank}]{(position == 1 ? Bold : "")} {ranking}\n";

				var highestWins = SCORES.OrderByDescending(o => o.Wins).FirstOrDefault()?.DiscordId == score.DiscordId;
				var highestRatio = SCORES.OrderByDescending(o => Math.Abs(o.WinRatio) <= 0 ? o.Wins : o.WinRatio).FirstOrDefault()
					                   ?.DiscordId == score.DiscordId;

				if (wins != null)
					wins.Value +=
						$"{(highestWins ? Bold : "")}{score.Wins}{(highestWins ? Bold : "")} {(highestWins ? Gold : Spacer)}\n";

				if (winsRatio != null)
					winsRatio.Value +=
						$"{(highestRatio ? Bold : "")}{(Math.Abs(score.Kda) <= 0 ? score.Wins : score.WinRatio)}{(highestRatio ? Bold : "")}% {(highestRatio ? Gold : Spacer)}\n";

				position++;
				await Task.Delay(100);
			}
			await loading.DeleteAsync();
			await ctx.RespondAsync("", embed: Embed);
		}

		#endregion

		#region Methods

		private static string GetRank(int oRank)
		{
			string rank;
			if (oRank == 0) rank = "0";
			else if (oRank <= 10) rank = "10";
			else if (oRank <= 50) rank = "50";
			else if (oRank <= 100) rank = "100";
			else if (oRank <= 250) rank = "250";
			else if (oRank <= 500) rank = "500";
			else if (oRank <= 1000) rank = "1000";
			else if (oRank <= 2500) rank = "2500";
			else if (oRank <= 5000) rank = "5000";
			else if (oRank <= 10000) rank = "10000";
			else if (oRank <= 50000) rank = "50000";
			else rank = "1000000";
			return rank;
		}

		private static async Task<DiscordMessage> Load(CommandContext ctx)
		{
			var loading = await ctx.RespondAsync("Loading leaderboard...");
			var load = await LoadUsers();
			Embed.Fields.Clear();
			Embed.Fields = new List<DiscordEmbedField>
				               {
					               new DiscordEmbedField { Inline = true, Name = "Users" },
					               new DiscordEmbedField { Inline = true, Name = "First" },
					               new DiscordEmbedField { Inline = true, Name = "Second" }
				               };
			SCORES.Clear();

			if (load.IsCompleted) UpdateHighscores(ctx);
			return loading;
		}

		private static async Task<Task> LoadUsers()
		{
			var usersJson = await JsonManager.ParseJsonAsync("Config/users.json");
			USERS = JsonConvert.DeserializeObject<IEnumerable<User>>(usersJson);
			return Task.CompletedTask;
		}

		private static bool SanitiseInput(CommandContext ctx, string mode, string region)
		{
			if (!mode.ToLower().Equals("squad") && !mode.ToLower().Equals("solo") && !mode.ToLower().Equals("duo"))
			{
				ctx.RespondAsync("Invalid game mode: solo | squad | duo");
				return false;
			}
			if (region.ToLower().Equals("na") || region.ToLower().Equals("eu") || region.ToLower().Equals("as")
			    || region.ToLower().Equals("oc") || region.ToLower().Equals("sa") || region.ToLower().Equals("sea")) return true;
			ctx.RespondAsync("Invalid region: na | eu | as | sa | sea | oc");
			return false;
		}

		private static void Sanitize(ref string headshotRatio,
		                             ref string kda,
		                             ref string mostKills,
		                             ref string winRatio,
		                             ref string wins,
		                             ref string longestKill,
		                             ref string totalDamage,
		                             ref string averageDamage,
		                             ref string kills,
		                             ref string headshots)
		{
			if (headshotRatio == string.Empty) headshotRatio = "0";
			if (headshots == string.Empty) headshots = "0";
			if (kda == string.Empty) kda = "0";
			if (kills == string.Empty) kills = "0";
			if (longestKill == string.Empty) longestKill = "0";
			if (mostKills == string.Empty) mostKills = "0";
			if (totalDamage == string.Empty) totalDamage = "0";
			if (averageDamage == string.Empty) averageDamage = "0";
			if (winRatio == string.Empty) winRatio = "0";
			if (wins == string.Empty) wins = "0";
		}

		#endregion
	}
}