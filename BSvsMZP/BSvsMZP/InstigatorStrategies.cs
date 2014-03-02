using System;
using System.Collections.Generic;

namespace BSvsMZP
{
	public class InstigatorStrategies
	{
		MessageQueue msgQueue;
		Communicator comm;

		public InstigatorStrategies(Communicator comm, MessageQueue msgQueue)
		{
			this.msgQueue = msgQueue;
			this.comm = comm;
		}



		public void getExcuse (Envelope envelope, Action<Common.Excuse> callback) {
			System.Threading.Thread thread = new System.Threading.Thread(delegate(){
			
				string convKey = "" + envelope.message.ConversationId.ProcessId + "," + envelope.message.ConversationId.SeqNumber;
				msgQueue.convoInProgressQueue.Add(convKey, new List<Envelope>());
				Common.Excuse excuse = null;
				short messagesReceived = 0;

				Console.WriteLine(convKey);

				for (int i = 0; i < 3; ++i) {

					comm.sendEnvelope(envelope);

					for (int j = 0; j < 200; ++j) {

						int stopAt = msgQueue.convoInProgressQueue[convKey].Count;

						for (int k = 0; k < stopAt; ++k) {
							messagesReceived++;
							if (msgQueue.convoInProgressQueue[convKey][0].message.MessageTypeId() == Messages.Message.MESSAGE_CLASS_IDS.AckNak) {
								// Reply from remote source is an AckNak, this is a good thing, check it for an Excuse
								Messages.AckNak reply = msgQueue.convoInProgressQueue[convKey][0].message as Messages.AckNak;
								if (reply.Status == Messages.Reply.PossibleStatus.Success) {
									excuse = (reply.ObjResult as Common.Excuse);
								}
							} else {
								// Reply from remote source is an unexpected type, handle that here
								Console.WriteLine("Unexpected reply");
							}
							msgQueue.convoInProgressQueue[convKey].RemoveAt(0);
						}

						if(excuse != null) {
							break;
						}

						System.Threading.Thread.Sleep(5);
					}

					if (excuse != null) {
						break;
					}

					envelope.message.MessageNr.SeqNumber += messagesReceived;
				}

				msgQueue.convoInProgressQueue.Remove(convKey);

				if (excuse != null) {
					callback(excuse);
				}

			});

			thread.Start();
		}

	}
}

