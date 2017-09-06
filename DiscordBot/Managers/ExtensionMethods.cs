namespace DiscordBot.Managers
{
	using HtmlAgilityPack;

	public static class ExtensionMethods
	{
		#region Public Methods and Operators

		public static string GetNodeText(this HtmlNode node, string xPath)
		{
			var childNode = node.SelectNodes(xPath);
			return childNode != null ? childNode[0].InnerText : "";
		}

		#endregion
	}
}