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
	}
}

