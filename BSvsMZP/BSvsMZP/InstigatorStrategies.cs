using System;

namespace BSvsMZP
{
	public class InstigatorStrategies
	{
		Object thisLock = new object();
		short SeqNum;
		short ProcessId;
		MessageQueue msgQueue;
		Communicator comm;


		public InstigatorStrategies(Communicator comm, MessageQueue msgQueue, short pId)
		{
			SeqNum = 1;
			ProcessId = pId;
			this.msgQueue = msgQueue;
			this.comm = comm;
		}







		public void getExcuse (Common.EndPoint endPoint, Common.Tick tick, Action<Common.Excuse> callback) {
			System.Threading.Thread thread = new System.Threading.Thread(delegate(){

				short sNum = 1;
				lock(thisLock) {
					sNum = SeqNum;
					SeqNum = (short)(((int)SeqNum % (int)short.MaxValue) + 1);
				}
					
				Messages.GetResource msg = new Messages.GetResource();
				msg.ConversationId.ProcessId = ProcessId;
				msg.ConversationId.SeqNumber = sNum;
				msg.MessageNr.ProcessId = ProcessId;
				msg.MessageNr.SeqNumber = 1;
				msg.EnablingTick = tick;

				string convKey = "" + msg.ConversationId.ProcessId + "," + msg.ConversationId.SeqNumber;
				msgQueue.convInProgressMessages.Add(convKey, new MessagesLists());

				for (int i = 0; i < 3; ++i) {
					Common.ByteList byteList = new Common.ByteList();
					msg.Encode(byteList);

					if (i == 0) {
						comm.sendMessage(byteList.ToBytes(), endPoint, msg.ConversationId);
					} else {
						comm.sendMessage(byteList.ToBytes(), msg.ConversationId);
					}

					Common.Excuse excuse = null;

					bool receivedReply = false;
					for (int j = 0; j < 200; ++j)
					{ 
						while (msgQueue.convInProgressMessages[convKey].AckNak.Count > 0) {
							// Yay! we got a reply!
							Messages.AckNak reply = msgQueue.convInProgressMessages[convKey].AckNak[0];
							msgQueue.convInProgressMessages[convKey].Reply.RemoveAt(0);
							if(reply.Status == Messages.Reply.PossibleStatus.Success) {
								excuse = (Common.Excuse)reply.ObjResult;
								receivedReply = true;
							}
						}

						if (receivedReply){
							break;
						}

						System.Threading.Thread.Sleep(5);
					}

					if (excuse != null) {
						msgQueue.convInProgressMessages.Remove(convKey);
						callback(excuse);
					}
				}

				msgQueue.convInProgressMessages.Remove(convKey);
			});

			thread.Start();
		}



	}
}

