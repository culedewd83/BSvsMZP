using System;
using System.Collections.Generic;

namespace BSvsMZP
{
	public class InstigatorStrategies
	{
		//Object thisLock = new object();
		//short SeqNum;
		//short ProcessId;
		MessageQueue msgQueue;
		Communicator comm;
		AgentInfo agentInfo = AgentInfo.Instance;

		public InstigatorStrategies(Communicator comm, MessageQueue msgQueue)
		{
			//SeqNum = 1;
			//ProcessId = pId;
			this.msgQueue = msgQueue;
			this.comm = comm;
		}



		public void getExcuse (Envelope envelope, Action<Common.Excuse> callback) {
			System.Threading.Thread thread = new System.Threading.Thread(delegate(){
			
				string convKey = "" + envelope.message.ConversationId.ProcessId + "," + envelope.message.ConversationId.SeqNumber;
				msgQueue.convoInProgressQueue.Add(convKey, new List<Envelope>());
				Common.Excuse excuse = null;
				short messagesReceived = 0;

				for (int i = 0; i < 3; ++i) {

					comm.sendEnvelope(envelope);

					for (int j = 0; j < 200; ++j) {
						for (int k = 0; k < msgQueue.convoInProgressQueue[convKey].Count; ++k) {
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


		/*
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
		*/


	}
}

