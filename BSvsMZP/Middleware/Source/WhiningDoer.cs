using System;

namespace Middleware
{
	public class WhiningDoer
	{
		//private Communicator comm;
		private MessageQueue msgQueue;
		private System.Threading.Thread doerThread;
		private bool shouldListen;
		ReplyDictionary replyDictionary;
		//AgentInfo agentInfo;


		public WhiningDoer(Communicator comm, MessageQueue msgQueue, AgentInfo agentInfo, Agent agent)
		{
			//this.comm = comm;
			this.msgQueue = msgQueue;
			//this.agentInfo = agentInfo;
			shouldListen = false;
			replyDictionary = new ReplyDictionary(comm, msgQueue, agentInfo, agent);
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

					while (msgQueue.newConvoQueue.Count > 0) {
						messagesMoved++;
						Envelope currEnv;
						if (msgQueue.newConvoQueue.TryDequeue(out currEnv)) {
							if (currEnv.message.MessageTypeId().ToString().Equals(Messages.Message.MESSAGE_CLASS_IDS.GetResource.ToString())) {
								if ((currEnv.message as Messages.GetResource).GetResourceType == Messages.GetResource.PossibleResourceType.WhiningTwine){
									replyDictionary.Strategies["WhiningTwine"].Invoke(currEnv);
								}
							} else if (currEnv.message.MessageTypeId().ToString().Equals(Messages.Message.MESSAGE_CLASS_IDS.TickDelivery.ToString())) {
								replyDictionary.Strategies["Tick"].Invoke(currEnv);
							} else if (currEnv.message.MessageTypeId().ToString().Equals(Messages.Message.MESSAGE_CLASS_IDS.StartGame.ToString())) {
								replyDictionary.Strategies["StartGame"].Invoke(currEnv);
							} else if (currEnv.message.MessageTypeId().ToString().Equals(Messages.Message.MESSAGE_CLASS_IDS.EndGame.ToString())) {
								replyDictionary.Strategies["EndGame"].Invoke(currEnv);
							} else if (currEnv.message is Messages.AgentListReply) {
								replyDictionary.Strategies["UpdateStream"].Invoke(currEnv);
							} else if (currEnv.message is Messages.ChangeStrength) {
								replyDictionary.Strategies["ChangeStrength"].Invoke(currEnv);
							}
						}
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

