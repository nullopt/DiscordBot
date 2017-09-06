namespace DiscordBot.Managers
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	using DSharpPlus;

	using Newtonsoft.Json;

	internal class DownloadManager
	{
		#region Public Methods and Operators

		public static void DownloadFile(DiscordAttachment file, MessageCreateEventArgs e)
		{
			// const string NewFiles = "./newfiles/";
			// const string OldFiles = "./oldfiles/";
			var json = File.ReadAllText(@"Config\urls.json");
			var urls = JsonConvert.DeserializeObject<List<Url>>(json);
			var announcements = e.Guild.Channels.FirstOrDefault(x => x.Name == "announcements");

			switch (file.FileName)
			{
				case "settings.cfg":
					//					var settings = Directory.GetFiles(NewFiles).FirstOrDefault(x => x.Contains(file.FileName));
					//					var oldSettings = Directory.GetFiles(OldFiles).FirstOrDefault(x => x.Contains(file.FileName));
					//					if (settings != null)
					//					{
					//						if (File.Exists(oldSettings)) if (oldSettings != null) File.Delete(oldSettings);
					//						File.Move(settings, OldFiles + "settings.cfg");
					//						e.Channel.SendMessageAsync($"Backed up old version to: {OldFiles}settings.cfg");
					//					}
					//					using (var client = new WebClient())
					//					{
					//						client.DownloadFileCompleted += async (sender, args) =>
					//							{
					//								await e.Channel.SendMessageAsync($"New version downloaded to: {NewFiles}settings.cfg");
					//								var announcements = e.Guild.Channels.FirstOrDefault(x => x.Id == 344202765725073429);
					//								if (announcements != null)
					//									await announcements.SendMessageAsync(
					//										"@everyone **New settings update posted in <#347409218715910155>. Please use `;update settings` in <#347430400424935426> to get the lastest version.**");
					//							};
					//						client.DownloadFileAsync(new Uri(file.Url), NewFiles + file.FileName);
					//					}

					urls.First(setting => setting.Name == "oldsettings").Link =
						urls.First(setting => setting.Name == "newsettings").Link; // Backup old link

					e.Channel.SendMessageAsync(@"Backed up old version to: \Config\urls.json @ oldsettings");

					urls.First(x => x.Name == "newsettings").Link = file.Url;

					e.Channel.SendMessageAsync(@"New version downloaded to: \Config\urls.json @ newsettings");

					if (announcements != null)
						announcements.SendMessageAsync(
							"@everyone **New settings update posted in <#347409218715910155>. Please use `;update settings` in <#347430400424935426> to get the lastest version.**");

					break;
				case "cheat.rar":
					//					var discord = Directory.GetFiles(NewFiles).FirstOrDefault(x => x.Contains(file.FileName));
					//					var oldDiscord = Directory.GetFiles(OldFiles).FirstOrDefault(x => x.Contains(file.FileName));
					//					if (discord != null)
					//					{
					//						if (File.Exists(oldDiscord)) if (oldDiscord != null) File.Delete(oldDiscord);
					//						File.Move(discord, OldFiles + "cheat.rar");
					//						e.Channel.SendMessageAsync($"Backed up old version to: {OldFiles}cheat.rar");
					//					}
					//					using (var client = new WebClient())
					//					{
					//						client.DownloadFileCompleted += async (sender, args) =>
					//							{
					//								await e.Channel.SendMessageAsync($"New version downloaded to: {NewFiles}cheat.rar");
					//								if (announcements != null)
					//									await announcements.SendMessageAsync(
					//										"@everyone **New cheat update posted in <#347409218715910155>. Please use `;update cheat` in <#347430400424935426> to get the lastest version.**");
					//							};
					//						client.DownloadFileAsync(new Uri(file.Url), NewFiles + file.FileName);
					//					}

					urls.First(setting => setting.Name == "oldcheat").Link =
						urls.First(setting => setting.Name == "newcheat").Link; // Backup old link

					e.Channel.SendMessageAsync(@"Backed up old version to: \Config\urls.json @ oldcheat");

					urls.First(x => x.Name == "newcheat").Link = file.Url;

					e.Channel.SendMessageAsync(@"New version downloaded to: \Config\urls.json @ newcheat");

					if (announcements != null)
						announcements.SendMessageAsync(
							"@everyone **New settings update posted in <#347409218715910155>. Please use `;update cheat` in <#347430400424935426> to get the lastest version.**");
					break;
				case "loader.rar":
					//					var loader = Directory.GetFiles(NewFiles).FirstOrDefault(x => x.Contains(file.FileName));
					//					var oldLoader = Directory.GetFiles(OldFiles).FirstOrDefault(x => x.Contains(file.FileName));
					//					if (loader != null)
					//					{
					//						if (File.Exists(oldLoader)) if (oldLoader != null) File.Delete(oldLoader);
					//						File.Move(loader, OldFiles + "loader.rar");
					//						e.Channel.SendMessageAsync($"Backed up old version to: {OldFiles}loader.rar");
					//					}
					//					using (var client = new WebClient())
					//					{
					//						client.DownloadFileCompleted += async (sender, args) =>
					//							{
					//								await e.Channel.SendMessageAsync($"New version downloaded to: {NewFiles}loader.rar");
					//								if (announcements != null)
					//									await announcements.SendMessageAsync(
					//										"@everyone **New loader update posted in <#347409218715910155>. Please use `;update loader` in <#347430400424935426> to get the lastest version.**");
					//							};
					//						client.DownloadFileAsync(new Uri(file.Url), NewFiles + file.FileName);
					//					}

					urls.First(setting => setting.Name == "oldloader").Link =
						urls.First(setting => setting.Name == "newloader").Link; // Backup old link

					e.Channel.SendMessageAsync(@"Backed up old version to: \Config\urls.json @ oldloader");

					urls.First(x => x.Name == "newloader").Link = file.Url;

					e.Channel.SendMessageAsync(@"New version downloaded to: \Config\urls.json @ newloader");

					if (announcements != null)
						announcements.SendMessageAsync(
							"@everyone **New settings update posted in <#347409218715910155>. Please use `;update loader` in <#347430400424935426> to get the lastest version.**");
					break;
				default:
					e.Channel.SendMessageAsync(
						$"What are you trying to upload <@{e.Channel.GetMessageAsync(e.Channel.LastMessageId).Result.Author.Id}>?\n"
						+ "(cheat.rar | loader.rar | settings.cfg) only");
					return;
			}
			WriteJson(e.Message, json, @"Config\urls.json", urls);
		}

		#endregion

		#region Methods

		public static void WriteJson<T>(DiscordMessage msg, string json, string path, IReadOnlyCollection<T> list)
		{
			try
			{
				json = JsonConvert.SerializeObject(list, Formatting.Indented);
				File.WriteAllText(path, json);
				msg.RespondAsync("Json successfully written.");
			}
			catch (Exception ex)
			{
				Program.Client.DebugLogger.LogMessage(LogLevel.Error, "WriteJson", $"Unable to write to {path} - {ex}", DateTime.Now);
			}
		}

		#endregion
	}
}