namespace DiscordBot.Managers
{
	using HtmlAgilityPack;

	internal class WebpageManager
	{
		#region Public Methods and Operators

		public static HtmlDocument ReadWebpage(string url)
		{
			var web = new HtmlWeb();
			var doc = web.Load(url);

			return doc;
		}

		#endregion
	}
}