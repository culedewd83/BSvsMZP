using System;

namespace Middleware
{
	public class WhineFactory : Agent
	{
		public WhiningDoer doer;


		public WhineFactory() : base()
		{
			doer = new WhiningDoer (comm, msgQueue, agentInfo, this);
			agentInfo.agentType = Common.AgentInfo.PossibleAgentType.WhiningSpinner;
		}


		public void startListening() {
			comm.startListening();
			listener.startListening();
			doer.startListening();
			shouldListen = true;
		}

		public void stopListening() {
			comm.stopListening();
			listener.stopListening();
			doer.stopListening();
			shouldListen = true;
		}

//		public void JoinGame(Common.EndPoint ep, Action<string> errorCallback, Action timeoutCallback) {
//			if (shouldListen) {
//				Envelope envelope = new Envelope (makeJoinGameMessage(), ep);
//				instigatorStrategies.JoinGame(envelope, (aInfo) => {
//					ReceiveAgentInfo(aInfo);
//				}, errorCallback, timeoutCallback);
//			}
//		}
//
//		private void ReceiveAgentInfo(Common.AgentInfo aInfo)
//		{
//			Console.WriteLine("AgentInfo received from game server");
//			agentInfo.processId = aInfo.Id;
//			agentInfo.status = "Active";
//			agentInfo.CommonAgentInfo = aInfo;
//			agentInfo.gameStatus = "Joined, Not Started";
//		}
	}
}

