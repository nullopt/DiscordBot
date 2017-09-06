namespace DiscordBot.Managers
{
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using System.Threading.Tasks;

	using Newtonsoft.Json;

	internal class JsonManager
	{
		#region Public Methods and Operators

		public static async Task<string> ParseJsonAsync(string path)
		{
			var json = "";
			using (var fs = File.OpenRead(path))
			using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
			{
				json = await sr.ReadToEndAsync();
			}

			return json;
		}

		#endregion
	}

	public class ScoreDataJson
	{
		#region Public Properties

		[JsonProperty("scores")]
		public List<Score> Scores { get; set; }

		#endregion
	}

	public class Score
	{
		#region Constructors and Destructors

		public Score(string discordId,
		             int overallRank,
		             int winRank,
		             int killRank,
		             float headshotRatio,
		             int headshots,
		             float kda,
		             int kills,
		             int longestKill,
		             int mostKills,
		             int totalDamage,
		             float averageDamage,
		             float winRatio,
		             int wins)
		{
			this.DiscordId = discordId;
			this.OverallRank = overallRank;
			this.WinRank = winRank;
			this.KillRank = killRank;
			this.HeadshotRatio = headshotRatio;
			this.Headshots = headshots;
			this.Kda = kda;
			this.Kills = kills;
			this.LongestKill = longestKill;
			this.MostKills = mostKills;
			this.TotalDamage = totalDamage;
			this.AverageDamage = averageDamage;
			this.WinRatio = winRatio;
			this.Wins = wins;
		}

		#endregion

		#region Public Properties

		[JsonProperty("averagedamage")]
		public float AverageDamage { get; set; }

		[JsonProperty("discordid")]
		public string DiscordId { get; set; }

		[JsonProperty("headshotratio")]
		public float HeadshotRatio { get; set; }

		[JsonProperty("headshots")]
		public int Headshots { get; set; }

		[JsonProperty("kda")]
		public float Kda { get; set; }

		[JsonProperty("killrank")]
		public int KillRank { get; set; }

		[JsonProperty("kills")]
		public int Kills { get; set; }

		[JsonProperty("longestkill")]
		public int LongestKill { get; set; }

		[JsonProperty("mostkills")]
		public int MostKills { get; set; }

		[JsonProperty("overallrank")]
		public int OverallRank { get; set; }

		[JsonProperty("totaldamage")]
		public int TotalDamage { get; set; }

		[JsonProperty("winrank")]
		public int WinRank { get; set; }

		[JsonProperty("winratio")]
		public float WinRatio { get; set; }

		[JsonProperty("wins")]
		public int Wins { get; set; }

		#endregion
	}

	public class UserDataJson
	{
		#region Public Properties

		[JsonProperty("users")]
		public List<User> Users { get; set; }

		#endregion
	}

	public class UrlsJson
	{
		#region Public Properties

		[JsonProperty("urls")]
		public List<Url> Urls { get; set; }

		#endregion
	}

	public class Url
	{
		#region Public Properties

		[JsonProperty("link")]
		public string Link { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		#endregion
	}

	public class ConfigJson
	{
		#region Public Properties

		[JsonProperty("prefix")]
		public string Prefix { get; private set; }

		[JsonProperty("token")]
		public string Token { get; private set; }

		#endregion
	}

	public class User
	{
		#region Public Properties

		[JsonProperty("discordid")]
		public string DiscordId { get; set; }

		[JsonProperty("mode")]
		public string Mode { get; set; }

		[JsonProperty("pubgname")]
		public string PubgName { get; set; }

		[JsonProperty("region")]
		public string Region { get; set; }

		#endregion
	}
}