using System;
using Middleware;

namespace WhineAgent
{
	public class WhineAgent
	{
		private WhineFactory wAgent = new WhineFactory();
		public Middleware.AgentInfo AgentInfo { get{ return  wAgent.agentInfo; }}
		public int NumOfTicks { get { return wAgent.ticks.Count; }}
		public int NumOfTwine { get { return wAgent.whiningTwines.Count; }}
		System.Threading.Thread agentBrainThread;

		public WhineAgent()
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
			wAgent.setRemoteEndPoint(server.EndPoint);
			wAgent.agentInfo.gameID = server.Id;
			wAgent.agentInfo.gameServerLabel = server.Label;
		}

		public void StartAgent()
		{
			wAgent.startListening();
		}

		public void JoinGameServer(Action<string> errorCallback, Action timeoutCallback)
		{
			wAgent.agentInfo.status = "Joining Game...";

			wAgent.JoinGame(wAgent.agentInfo.remoteServerEndPoint, (errorMsg) => {
				wAgent.agentInfo.status = "Inactive";
				wAgent.agentInfo.gameID = 0;
				wAgent.stopListening();
				if (errorCallback != null) {
					errorCallback(errorMsg);
				}
			}, () => {
				wAgent.agentInfo.status = "Inactive";
				wAgent.agentInfo.gameID = 0;
				wAgent.stopListening();
				if (timeoutCallback != null) {
					timeoutCallback();
				}
			});
		}

		private void UpdateAgent ()
		{
			if (wAgent.ticks.Count > 0) {
				createWhiningTwine();
			}
		}

		private void createWhiningTwine ()
		{
			Common.WhiningTwine twine = new Common.WhiningTwine ();
			twine.CreatorId = wAgent.agentInfo.processId;
			twine.Ticks.Add(wAgent.ticks.Dequeue());
			wAgent.whiningTwines.Enqueue(twine);
		}
	}
}

