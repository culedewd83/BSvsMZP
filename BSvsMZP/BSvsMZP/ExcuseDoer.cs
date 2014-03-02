using System;
using System.Collections.Generic;

namespace BSvsMZP
{
	public class ExcuseDoer
	{
		private Communicator comm;
		private MessageQueue msgQueue;
		private System.Threading.Thread doerThread;
		private bool shouldListen;
		ExcuseReplyDictionary replyDictionary;


		public ExcuseDoer(Communicator comm, MessageQueue msgQueue)
		{
			this.comm = comm;
			this.msgQueue = msgQueue;
			shouldListen = false;
			replyDictionary = new ExcuseReplyDictionary(comm, msgQueue);
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



						//if (msgQueue.newConvoQueue[0].message. == Messages.Message.MESSAGE_CLASS_IDS.GetResource) {
							if ((msgQueue.newConvoQueue[0].message as Messages.GetResource).GetResourceType == Messages.GetResource.PossibleResourceType.Excuse){
							Console.WriteLine((msgQueue.newConvoQueue[0].message as Messages.GetResource).GetResourceType.ToString());
								if(replyDictionary.Strategies.ContainsKey((msgQueue.newConvoQueue[0].message as Messages.GetResource).GetResourceType.ToString())){
								replyDictionary.Strategies[(msgQueue.newConvoQueue[0].message as Messages.GetResource).GetResourceType.ToString()].Invoke(msgQueue.newConvoQueue[0]);
								Console.WriteLine("tried to reply");
								}
							}
						//}


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

