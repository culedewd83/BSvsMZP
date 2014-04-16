using System;
using System.Collections.Generic;

namespace Middleware
{
	public class GameServers
	{
		public static Dictionary<string, SimpleServerInfo> GetAvailableServers ()
		{
			//RegistrarAlt.RegistrarAlt registrar = new RegistrarAlt.RegistrarAlt ("http://bsvszp.azurewebsites.net/RegistrarAlt.asmx");
			RegistrarAlt.RegistrarAlt registrar = new RegistrarAlt.RegistrarAlt ("http://cs5200web.serv.usu.edu/RegistrarAlt.asmx");
			RegistrarAlt.GameInfo[] gamesAvail = registrar.GetGames(RegistrarAlt.GameStatus.AVAILABLE);
			Dictionary<string, SimpleServerInfo> games = new Dictionary<string, SimpleServerInfo> ();
			foreach (RegistrarAlt.GameInfo server in gamesAvail) {
				SimpleServerInfo serverInfo = new SimpleServerInfo ();
				serverInfo.Label = server.Label;
				serverInfo.Address = server.CommunicationEndPoint.Address;
				serverInfo.Port = server.CommunicationEndPoint.Port;
				serverInfo.Id = server.Id;
				serverInfo.EndPoint = new Common.EndPoint (server.CommunicationEndPoint.Address, server.CommunicationEndPoint.Port);
				if (!games.ContainsKey(server.Label)) {
					games.Add(server.Label, serverInfo);
				}
			}

			return games;
		}

		public static short GetProcessID ()
		{
			RegistrarAlt.RegistrarAlt registrar = new RegistrarAlt.RegistrarAlt ("http://cs5200web.serv.usu.edu/RegistrarAlt.asmx");
			return registrar.GetProcessId();
		}

	}
}

