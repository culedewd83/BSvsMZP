using System;
using Middleware;

namespace ExcuseAgent
{
	public class ExcuseAgent
	{
		private ExcuseFactory eAgent = new ExcuseFactory();
		public Middleware.AgentInfo AgentInfo { get{ return  eAgent.agentInfo; }}
		public int NumOfExcuses { get { return eAgent.excuses.Count; }}
		public int NumOfTicks { get { return eAgent.ticks.Count; }}

		public ExcuseAgent()
		{
		}

		public void SetGameServer(SimpleServerInfo server)
		{
			eAgent.setRemoteEndPoint(server.EndPoint);
			eAgent.agentInfo.gameID = server.Id;
			eAgent.agentInfo.gameServerLabel = server.Label;
		}

		public void StartAgent()
		{
			eAgent.startListening();
		}

		public void JoinGameServer(Action<string> errorCallback, Action timeoutCallback)
		{
			eAgent.agentInfo.status = "Joining Game...";

			eAgent.JoinGame(eAgent.agentInfo.remoteServerEndPoint, (errorMsg) => {
				eAgent.agentInfo.status = "Inactive";
				eAgent.agentInfo.gameID = 0;
				eAgent.stopListening();
				if (errorCallback != null) {
					errorCallback(errorMsg);
				}
			}, () => {
				eAgent.agentInfo.status = "Inactive";
				eAgent.agentInfo.gameID = 0;
				eAgent.stopListening();
				if (timeoutCallback != null) {
					timeoutCallback();
				}
			});
		}
	}
}

