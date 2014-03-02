using System;
using System.Collections.Generic;

namespace BSvsMZP
{
	public class Listener
	{
		private Communicator comm;
		private MessageQueue msgQueue;
		private System.Threading.Thread listenerThread;
		private bool shouldListen;


		public Listener(Communicator comm, MessageQueue msgQueue)
		{
			this.comm = comm;
			this.msgQueue = msgQueue;
			shouldListen = false;
		}


		public void startListening() {
			shouldListen = true;
			listenForMessages();
		}


		public void stopListening () {
			shouldListen = false;
		}



		private void listenForMessages () {
			listenerThread = new System.Threading.Thread (delegate() {

				int messagesMoved = 0;
				while (shouldListen) {
					messagesMoved = 0;

					List<Envelope> receivedEnvelopes = comm.getEnvelopes();

					foreach (Envelope envelope in receivedEnvelopes) {
						messagesMoved++;
						string convKey = "" + envelope.message.ConversationId.ProcessId + "," + envelope.message.ConversationId.SeqNumber;
						if (msgQueue.convoInProgressQueue.ContainsKey(convKey)) {
							msgQueue.convoInProgressQueue[convKey].Add(envelope);
						} else {
							msgQueue.newConvoQueue.Add(envelope);
						}
					}

					if (messagesMoved == 0) {
						// Be nice, sleep for a awhile...
						System.Threading.Thread.Sleep(5);
					}
				}
			});

			listenerThread.Start();
		}

	}
}
