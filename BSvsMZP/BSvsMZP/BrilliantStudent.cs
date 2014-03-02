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
	}
}

