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
		public long totalMessagesMoved;


		public Listener(Communicator comm, MessageQueue msgQueue)
		{
			this.comm = comm;
			this.msgQueue = msgQueue;
			shouldListen = false;
			totalMessagesMoved = 0;
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
						totalMessagesMoved++;
						string convKey = "" + envelope.message.ConversationId.ProcessId + "," + envelope.message.ConversationId.SeqNumber;
						if (msgQueue.convoInProgressQueue.ContainsKey(convKey)) {
							msgQueue.convoInProgressQueue[convKey].Enqueue(envelope);
						} else {
							msgQueue.newConvoQueue.Enqueue(envelope);
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
