using System;
using System.Collections.Generic;

namespace BSvsMZP
{
	public class ExcuseFactory : Agent
	{
		public ExcuseDoer doer;


		public ExcuseFactory() : base()
		{
			doer = new ExcuseDoer (comm, msgQueue, agentInfo);

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

