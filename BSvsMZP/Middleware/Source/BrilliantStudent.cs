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

	}
}

