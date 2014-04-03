using System;
using Middleware;

namespace BrilliantStudentAgent
{
	public class BrilliantStudentAgent
	{
		public BrilliantStudent bsAgent = new BrilliantStudent();
		public Middleware.AgentInfo AgentInfo { get{ return  bsAgent.agentInfo; }}
		public int NumOfExcuses { get { return bsAgent.excuses.Count; }}
		public int NumOfTicks { get { return bsAgent.ticks.Count; }}
		public int NumOfTwine { get { return bsAgent.whiningTwines.Count; }}
		public int NumOfBombs { get { return bsAgent.bombs.Count; }}

		public BrilliantStudentAgent()
		{
		}

		public void SetGameServer(SimpleServerInfo server)
		{
			bsAgent.setRemoteEndPoint(server.EndPoint);
			bsAgent.agentInfo.gameID = server.Id;
			bsAgent.agentInfo.gameServerLabel = server.Label;
		}

		public void StartAgent()
		{
			bsAgent.startListening();
		}

		public void JoinGameServer(Action<string> errorCallback, Action timeoutCallback)
		{
			bsAgent.agentInfo.status = "Joining Game...";

			bsAgent.JoinGame(bsAgent.agentInfo.remoteServerEndPoint, (errorMsg) => {
				bsAgent.agentInfo.status = "Inactive";
				bsAgent.agentInfo.gameID = 0;
				bsAgent.stopListening();
				if (errorCallback != null) {
					errorCallback(errorMsg);
				}
			}, () => {
				bsAgent.agentInfo.status = "Inactive";
				bsAgent.agentInfo.gameID = 0;
				bsAgent.stopListening();
				if (timeoutCallback != null) {
					timeoutCallback();
				}
			});
		}
	}
}

