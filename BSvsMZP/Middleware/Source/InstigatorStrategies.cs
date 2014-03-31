using System;
using System.Collections.Generic;

namespace Middleware
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
				msgQueue.convoInProgressQueue.Add(convKey, new Queue<Envelope>());
				Common.Excuse excuse = null;
				short messagesReceived = 0;

				Console.WriteLine(convKey);

				for (int i = 0; i < 3; ++i) {

					comm.sendEnvelope(envelope);

					for (int j = 0; j < 200; ++j) {

						while (msgQueue.convoInProgressQueue[convKey].Count > 0) {
							messagesReceived++;
							Envelope currEnv = msgQueue.convoInProgressQueue[convKey].Dequeue();

							if (currEnv.message.MessageTypeId() == Messages.Message.MESSAGE_CLASS_IDS.AckNak) {
								// Reply from remote source is an AckNak, this is a good thing, check it for an Excuse
								Messages.AckNak reply = currEnv.message as Messages.AckNak;
								if (reply.Status == Messages.Reply.PossibleStatus.Success) {
									excuse = (reply.ObjResult as Common.Excuse);
								} else {
									Console.WriteLine("Remote responded with a failure to give excuse");
								}
							} else {
								// Reply from remote source is an unexpected type, handle that here
								Console.WriteLine("Unexpected reply");
							}
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


		public void getWhiningTwine (Envelope envelope, Action<Common.WhiningTwine> callback) {
			System.Threading.Thread thread = new System.Threading.Thread(delegate(){

				string convKey = "" + envelope.message.ConversationId.ProcessId + "," + envelope.message.ConversationId.SeqNumber;
				msgQueue.convoInProgressQueue.Add(convKey, new Queue<Envelope>());
				Common.WhiningTwine whine = null;
				short messagesReceived = 0;

				Console.WriteLine(convKey);

				for (int i = 0; i < 3; ++i) {

					comm.sendEnvelope(envelope);

					for (int j = 0; j < 200; ++j) {

						while (msgQueue.convoInProgressQueue[convKey].Count > 0) {
							messagesReceived++;
							Envelope currEnv = msgQueue.convoInProgressQueue[convKey].Dequeue();

							if (currEnv.message.MessageTypeId() == Messages.Message.MESSAGE_CLASS_IDS.AckNak) {
								// Reply from remote source is an AckNak, this is a good thing, check it for an Excuse
								Messages.AckNak reply = currEnv.message as Messages.AckNak;
								if (reply.Status == Messages.Reply.PossibleStatus.Success) {
									whine = (reply.ObjResult as Common.WhiningTwine);
								} else {
									Console.WriteLine("Remote responded with a failure to give WhiningTwine");
								}
							} else {
								// Reply from remote source is an unexpected type, handle that here
								Console.WriteLine("Unexpected reply");
							}
						}
							
						if(whine != null) {
							break;
						}

						System.Threading.Thread.Sleep(5);
					}

					if (whine != null) {
						break;
					}

					envelope.message.MessageNr.SeqNumber += messagesReceived;
				}

				msgQueue.convoInProgressQueue.Remove(convKey);

				if (whine != null) {
					callback(whine);
				}

			});

			thread.Start();
		}


		public void JoinGame (Envelope envelope, Action<Common.AgentInfo> callback, Action<string> errorCallback, Action timeoutCallback) {

			System.Threading.Thread thread = new System.Threading.Thread(delegate(){

				string convKey = "" + envelope.message.ConversationId.ProcessId + "," + envelope.message.ConversationId.SeqNumber;
				msgQueue.convoInProgressQueue.Add(convKey, new Queue<Envelope>());
				Common.AgentInfo agentInfo = null;
				bool error = false;
				string errorMsg = "";
				short messagesReceived = 0;

				Console.WriteLine(convKey);

				for (int i = 0; i < 3; ++i) {

					comm.sendEnvelope(envelope);

					for (int j = 0; j < 200; ++j) {

						while (msgQueue.convoInProgressQueue[convKey].Count > 0) {
							messagesReceived++;
							Envelope currEnv = msgQueue.convoInProgressQueue[convKey].Dequeue();

							if (currEnv.message.MessageTypeId() == Messages.Message.MESSAGE_CLASS_IDS.AckNak) {
								// Reply from remote source is an AckNak, this is a good thing, check it for an Excuse
								Messages.AckNak reply = currEnv.message as Messages.AckNak;
								if (reply.Status == Messages.Reply.PossibleStatus.Success) {
									agentInfo = (reply.ObjResult as Common.AgentInfo);
								} else {
									Console.WriteLine("Remote responded with a failure to join game");
									Console.WriteLine(reply.Message);
									errorMsg = reply.Message;
									error = true;
									break;
								}
							} else {
								// Reply from remote source is an unexpected type, handle that here
								Console.WriteLine("Unexpected reply");
							}
						}



						if(agentInfo != null || error) {
							break;
						}

						System.Threading.Thread.Sleep(5);
					}

					if (agentInfo != null || error) {
						break;
					}

					envelope.message.MessageNr.SeqNumber += messagesReceived;
				}

				msgQueue.convoInProgressQueue.Remove(convKey);

				if (error) {
					if(errorCallback != null) {
						errorCallback(errorMsg);
					}
				} else if (agentInfo != null) {
					callback(agentInfo);
				} else {
					if(timeoutCallback != null) {
						timeoutCallback();
					}
				}

			});

			thread.Start();


		}
	}
}

