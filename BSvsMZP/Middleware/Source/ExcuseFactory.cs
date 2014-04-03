using System;
using System.Collections.Generic;

namespace Middleware
{
	public class ExcuseFactory : Agent
	{
		public ExcuseDoer doer;


		public ExcuseFactory() : base()
		{
			doer = new ExcuseDoer (comm, msgQueue, agentInfo, this);
			agentInfo.agentType = Common.AgentInfo.PossibleAgentType.ExcuseGenerator;
		}


		public void startListening() {
			shouldListen = true;
			comm.startListening();
			listener.startListening();
			doer.startListening();
		}

		public void stopListening() {
			shouldListen = false;
			comm.stopListening();
			listener.stopListening();
			doer.stopListening();
		}

		public void JoinGame(Common.EndPoint ep, Action<string> errorCallback, Action timeoutCallback) {
			if (shouldListen) {
				Envelope envelope = new Envelope (makeJoinGameMessage(), ep);
				instigatorStrategies.JoinGame(envelope, (aInfo) => {
					ReceiveAgentInfo(aInfo);
				}, errorCallback, timeoutCallback);
			}
		}

		private void ReceiveAgentInfo(Common.AgentInfo aInfo)
		{
			Console.WriteLine("AgentInfo received from game server");
			agentInfo.processId = aInfo.Id;
			agentInfo.status = "Active";
			agentInfo.CommonAgentInfo = aInfo;
			agentInfo.gameStatus = "Joined, Not Started";
		}


	}
}

