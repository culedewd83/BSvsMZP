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
	}
}

