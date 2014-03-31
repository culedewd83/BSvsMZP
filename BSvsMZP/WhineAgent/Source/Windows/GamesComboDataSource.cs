using System;
using MonoMac.AppKit;
using System.Collections.Generic;
using MonoMac.Foundation;
using Middleware;

namespace WhineAgent
{
	public class GamesComboDataSource : NSComboBoxDataSource
	{
		List<string> games;

		public GamesComboDataSource(Dictionary<string, SimpleServerInfo> servers)
		{
			games = new List<string> ();
			foreach (KeyValuePair<string, SimpleServerInfo> pair in servers) {
				games.Add(pair.Key);
			}
			if (games.Count == 0) {
				games.Add("No Game Servers Found");
			}
		}

		public override string CompletedString (NSComboBox comboBox, string uncompletedString)
		{
			return games.Find (n => n.StartsWith (uncompletedString, StringComparison.InvariantCultureIgnoreCase));
		}

		public override int IndexOfItem (NSComboBox comboBox, string value)
		{
			return games.FindIndex (n => n.Equals (value, StringComparison.InvariantCultureIgnoreCase));
		}

		public override int ItemCount (NSComboBox comboBox)
		{
			return games.Count;
		}

		public override NSObject ObjectValueForItem (NSComboBox comboBox, int index)
		{
			return NSObject.FromObject (games [index]);
		}
	}
}

