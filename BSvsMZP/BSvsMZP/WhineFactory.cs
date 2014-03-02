using System;

namespace BSvsMZP
{
	public class WhineFactory : Agent
	{
		public WhiningDoer doer;


		public WhineFactory() : base()
		{
			doer = new WhiningDoer (comm, msgQueue, agentInfo);

		}


		public void startListening() {
			comm.startListening();
			listener.startListening();
			doer.startListening();
		}

		public void stopListening() {
			comm.stopListening();
			listener.stopListening();
			doer.stopListening();
		}
	}
}

