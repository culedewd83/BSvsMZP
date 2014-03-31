using System;
using System.Collections.Generic;

namespace Middleware
{
	public class BrilliantStudent : Agent
	{
		public StudentDoer doer;

		public BrilliantStudent() : base()
		{
			doer = new StudentDoer (comm, msgQueue, agentInfo, this);
			agentInfo.agentType = Common.AgentInfo.PossibleAgentType.BrilliantStudent;
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
			shouldListen = false;
		}

		public void getExcuse(Common.EndPoint ep) {
			if (shouldListen) {
				//Envelope envelope = new Envelope(makeGetExcuseMessage(ticks.Dequeue()), ep);
				Envelope envelope = new Envelope (makeGetExcuseMessage(new Common.Tick ()), ep);
				instigatorStrategies.getExcuse(envelope, (excuse) => {
					receiveExcuse(excuse);
				});
			}
		}

		public void receiveExcuse(Common.Excuse excuse) {
			excuses.Enqueue(excuse);
		}

		public void getWhiningTwine(Common.EndPoint ep) {
			if (shouldListen) {
				//Envelope envelope = new Envelope(makeGetExcuseMessage(ticks.Dequeue()), ep);
				Envelope envelope = new Envelope (makeGetWhiningTwineMessage(new Common.Tick ()), ep);
				instigatorStrategies.getWhiningTwine(envelope, (whine) => {
					receiveWhiningTwine(whine);
				});
			}
		}

		public void receiveWhiningTwine(Common.WhiningTwine whine) {
			whiningTwines.Enqueue(whine);
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

