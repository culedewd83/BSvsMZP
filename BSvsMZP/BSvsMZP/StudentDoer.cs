using System;

namespace BSvsMZP
{
	public class StudentDoer
	{
		private Communicator comm;
		private MessageQueue msgQueue;
		private System.Threading.Thread doerThread;
		private bool shouldListen;
		ExcuseReplyDictionary replyDictionary;
		AgentInfo agentInfo;


		public StudentDoer(Communicator comm, MessageQueue msgQueue, AgentInfo agentInfo)
		{
			this.comm = comm;
			this.msgQueue = msgQueue;
			this.agentInfo = agentInfo;
			shouldListen = false;
			replyDictionary = new ExcuseReplyDictionary(comm, msgQueue, agentInfo);
		}


		public void startListening() {
			shouldListen = true;
			listenForMessages();
		}


		public void stopListening () {
			shouldListen = false;
		}



		private void listenForMessages () {
			doerThread = new System.Threading.Thread (delegate() {

				int messagesMoved = 0;
				while (shouldListen) {
					messagesMoved = 0;

					int stopAt = msgQueue.newConvoQueue.Count;

					for (int i = 0; i < stopAt; ++i) {
						messagesMoved++;


						if (msgQueue.newConvoQueue[0].message.MessageTypeId().ToString().Equals(Messages.Message.MESSAGE_CLASS_IDS.GetResource.ToString())) {
							// Does not reply to this type!!!
						}



						msgQueue.newConvoQueue.RemoveAt(0);
					}

					if (messagesMoved == 0) {
						// Be nice, sleep for a awhile...
						System.Threading.Thread.Sleep(5);
					}
				}
			});

			doerThread.Start();
		}
	}
}

