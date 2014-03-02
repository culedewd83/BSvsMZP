using System;
using System.Collections.Generic;

namespace BSvsMZP
{
	public class BrilliantStudent : Agent
	{
		public StudentDoer doer;

		public BrilliantStudent() : base()
		{
			doer = new StudentDoer (comm, msgQueue, agentInfo);
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

		public void getExcuse(Common.EndPoint ep) {
			Envelope envelope = new Envelope(makeGetExcuseMessage(ticks.Dequeue()), ep);
			instigatorStrategies.getExcuse(envelope, (excuse) => {
				receiveExcuse(excuse);
			});
		}

		public void receiveExcuse(Common.Excuse excuse) {
			excuses.Enqueue(excuse);
		}
	}
}

