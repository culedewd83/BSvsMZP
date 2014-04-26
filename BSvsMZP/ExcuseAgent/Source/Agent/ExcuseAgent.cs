using System;
using Middleware;

namespace ExcuseAgent
{
	public class ExcuseAgent
	{
		public ExcuseFactory eAgent = new ExcuseFactory();
		public Middleware.AgentInfo AgentInfo { get{ return  eAgent.agentInfo; }}
		public int NumOfExcuses { get { return eAgent.excuses.Count; }}
		public int NumOfTicks { get { return eAgent.ticks.Count; }}
		System.Threading.Thread agentBrainThread;

		public ExcuseAgent()
		{
			agentBrainThread = new System.Threading.Thread(delegate(){
				while(true) {
					UpdateAgent();
					System.Threading.Thread.Sleep(5);
				}
			});
			agentBrainThread.Start();
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

		private void UpdateAgent ()
		{
			if (eAgent.gameHasEnded) {
				eAgent.stopListening();
			}

			if (eAgent.gameConfig != null) {
				if (eAgent.ticks.Count >= eAgent.gameConfig.NumberOfTicksRequiredToBuildAnExcuse) {
					createExcuse();
				}
			}
		}

		private void createExcuse ()
		{
			Common.Excuse excuse = new Common.Excuse ();
			excuse.CreatorId = eAgent.agentInfo.processId;
			for (int i = 0; i < eAgent.gameConfig.NumberOfTicksRequiredToBuildAnExcuse; ++i) {
				excuse.Ticks.Add(eAgent.ticks.Dequeue());
			}
			eAgent.excuses.Enqueue(excuse);
		}
	}
}

