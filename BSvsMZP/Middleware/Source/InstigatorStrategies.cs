using System;
using System.Collections.Generic;
using Messages;
using System.Collections.Concurrent;

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


		public void SendResponseReply (Envelope envelope, Action<Message> callback, Action timeoutCallback)
		{
			System.Threading.Thread thread = new System.Threading.Thread (delegate() {
				string convKey = "" + envelope.message.ConversationId.ProcessId + "," + envelope.message.ConversationId.SeqNumber;
				msgQueue.convoInProgressQueue.TryAdd(convKey, new ConcurrentQueue<Envelope>());
				ConcurrentQueue<Envelope> disposed;
				for (int i = 0; i < 3; ++i) {
					comm.sendEnvelope(envelope);
					//Console.WriteLine("JoinGame message sent");
					for (int j = 0; j < 500; ++j) {
						if (msgQueue.convoInProgressQueue[convKey].Count > 0) {
							Envelope reply;
							if(msgQueue.convoInProgressQueue[convKey].TryDequeue(out reply)){
								msgQueue.convoInProgressQueue.TryRemove(convKey, out disposed);
								AckNak ackNak = new AckNak(Reply.PossibleStatus.Success);
								ackNak.ConversationId = envelope.message.ConversationId;
								comm.sendEnvelope(new Envelope(ackNak, reply.endPoint));
								callback(reply.message);
								return;
							}
						}
						System.Threading.Thread.Sleep(5);
					}
				}
				msgQueue.convoInProgressQueue.TryRemove(convKey, out disposed);
				timeoutCallback();
			});
			thread.Start();
		}



		public void JoinGame (Envelope envelope, Action<Common.AgentInfo> callback, Action<string> errorCallback, Action timeoutCallback)
		{
			SendResponseReply(envelope, (msg) => {
				JoinGameReplyHandler(msg, callback, errorCallback);
			}, timeoutCallback);
		}


		private void JoinGameReplyHandler (Message msg, Action<Common.AgentInfo> callback, Action<string> errorCallback)
		{
			Messages.AckNak reply;
			try {
				reply = msg as AckNak;
			} catch {
				errorCallback("Could not cast response to AckNak");
				return;
			}
			if (reply.Status == Reply.PossibleStatus.Failure) {
				errorCallback("AckNak contains a failure: " + reply.Message);
				return;
			}
			callback((Common.AgentInfo)reply.ObjResult);
		}


		public void SendReply (Envelope envelope, Action<Message> callback)
		{
			System.Threading.Thread thread = new System.Threading.Thread (delegate() {
				string convKey = "" + envelope.message.ConversationId.ProcessId + "," + envelope.message.ConversationId.SeqNumber;
				msgQueue.convoInProgressQueue.TryAdd(convKey, new ConcurrentQueue<Envelope>());
				ConcurrentQueue<Envelope> disposed;
				for (int i = 0; i < 3; ++i) {
					comm.sendEnvelope(envelope);
					//Console.WriteLine("message sent");
					for (int j = 0; j < 500; ++j) {
						if (msgQueue.convoInProgressQueue.ContainsKey(convKey) && msgQueue.convoInProgressQueue[convKey].Count > 0) {
							Envelope reply;
							if(msgQueue.convoInProgressQueue[convKey].TryDequeue(out reply)){
								msgQueue.convoInProgressQueue.TryRemove(convKey, out disposed);
								callback(reply.message);
								return;
							}
						}
						System.Threading.Thread.Sleep(5);
					}
				}
				msgQueue.convoInProgressQueue.TryRemove(convKey, out disposed);
			});
			thread.Start();
		}


		public void SendReplyRecover (Envelope envelope, Action<Message> callback, Action<Envelope> recovery)
		{
			System.Threading.Thread thread = new System.Threading.Thread (delegate() {
				string convKey = "" + envelope.message.ConversationId.ProcessId + "," + envelope.message.ConversationId.SeqNumber;
				msgQueue.convoInProgressQueue.TryAdd(convKey, new ConcurrentQueue<Envelope>());
				ConcurrentQueue<Envelope> disposed;
				for (int i = 0; i < 3; ++i) {
					comm.sendEnvelope(envelope);
					//Console.WriteLine("message sent");
					for (int j = 0; j < 500; ++j) {
						if (msgQueue.convoInProgressQueue.ContainsKey(convKey) && msgQueue.convoInProgressQueue[convKey].Count > 0) {
							Envelope reply;
							if(msgQueue.convoInProgressQueue[convKey].TryDequeue(out reply)){
								msgQueue.convoInProgressQueue.TryRemove(convKey, out disposed);
								callback(reply.message);
								return;
							}
						}
						System.Threading.Thread.Sleep(5);
					}
				}
				msgQueue.convoInProgressQueue.TryRemove(convKey, out disposed);
				recovery(envelope);
			});
			thread.Start();
		}



		public void GetAgentList (Envelope envelope, Action<Common.AgentList> callback)
		{
			SendReply(envelope, (msg) => {
				GetAgentListHandler(msg, callback);
			});
		}

		private void GetAgentListHandler (Message msg, Action<Common.AgentList> callback)
		{
			try {
				AgentListReply reply = msg as AgentListReply;
				if (reply.Status == Reply.PossibleStatus.Success) {
					callback((Common.AgentList)reply.Agents);
				}
			} catch { }
		}


		public void getExcuse (Envelope envelope, Action<Common.Excuse> callback, Action<Common.Tick> recovery)
		{
			SendReplyRecover(envelope, (msg) => {
				GetExcuseReplyHandler(msg, callback, envelope, recovery);
			}, (env) => {
				recovery(((Messages.GetResource)env.message).EnablingTick);
			});
		}

		private void GetExcuseReplyHandler (Message msg, Action<Common.Excuse> callback, Envelope envelope, Action<Common.Tick> recovery)
		{
			try {
				Messages.ResourceReply reply = msg as ResourceReply;
				if (reply.Status == Reply.PossibleStatus.Success) {
					Console.WriteLine("YAYAYAYAYAYAYAYAYAYAYAYAYAYAYYYYYYYYY!!!!");
					callback((Common.Excuse)reply.Resource);
				} else {
					recovery(((Messages.GetResource)envelope.message).EnablingTick);
				}
			} catch {
				Console.WriteLine("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
				recovery(((Messages.GetResource)envelope.message).EnablingTick);
			}

		}


		public void getWhiningTwine (Envelope envelope, Action<Common.WhiningTwine> callback, Action<Common.Tick> recovery)
		{
			SendReplyRecover(envelope, (msg) => {
				GetWhineReplyHandler(msg, callback, envelope, recovery);
			}, (env) => {
				recovery(((Messages.GetResource)env.message).EnablingTick);
			});
		}

		private void GetWhineReplyHandler (Message msg, Action<Common.WhiningTwine> callback, Envelope envelope, Action<Common.Tick> recovery)
		{
			try {
				Messages.ResourceReply reply = msg as ResourceReply;
				if (reply.Status == Reply.PossibleStatus.Success) {
					callback((Common.WhiningTwine)reply.Resource);
				} else {
					recovery(((Messages.GetResource)envelope.message).EnablingTick);
				}
			} catch {
				recovery(((Messages.GetResource)envelope.message).EnablingTick);
			}
		}

		public void StartUpdateStream (Envelope envelope, Action<bool> callback)
		{
			SendReply(envelope, (msg) => {
				StartUpdateStreamReplyHandler(msg, callback);
			});
		}

		private void StartUpdateStreamReplyHandler (Message msg, Action<bool> callback)
		{
			try {
				AckNak reply = msg as AckNak;
				if (reply.Status == Reply.PossibleStatus.Success) {
					callback(true);
				} else {
					callback(false);
				}
			} catch { }
		}

		public void GetGameConfig (Envelope envelope, Action<Common.GameConfiguration> callback)
		{
			SendReply(envelope, (msg) => {
				GetGameConfigReplyHandler(msg, callback);
			});
		}

		private void GetGameConfigReplyHandler (Message msg, Action<Common.GameConfiguration> callback)
		{
			try {
				ConfigurationReply reply = msg as ConfigurationReply;
				if (reply.Status == Reply.PossibleStatus.Success) {
					callback(reply.Configuration);
				}
			} catch { }
		}

//		public void getExcuse (Envelope envelope, Action<Common.Excuse> callback) {
//			System.Threading.Thread thread = new System.Threading.Thread(delegate(){
//			
//				string convKey = "" + envelope.message.ConversationId.ProcessId + "," + envelope.message.ConversationId.SeqNumber;
//				msgQueue.convoInProgressQueue.Add(convKey, new Queue<Envelope>());
//				Common.Excuse excuse = null;
//				short messagesReceived = 0;
//
//				Console.WriteLine(convKey);
//
//				for (int i = 0; i < 3; ++i) {
//
//					comm.sendEnvelope(envelope);
//
//					for (int j = 0; j < 200; ++j) {
//
//						while (msgQueue.convoInProgressQueue[convKey].Count > 0) {
//							messagesReceived++;
//							Envelope currEnv = msgQueue.convoInProgressQueue[convKey].Dequeue();
//
//							if (currEnv.message.MessageTypeId() == Messages.Message.MESSAGE_CLASS_IDS.AckNak) {
//								// Reply from remote source is an AckNak, this is a good thing, check it for an Excuse
//								Messages.AckNak reply = currEnv.message as Messages.AckNak;
//								if (reply.Status == Messages.Reply.PossibleStatus.Success) {
//									excuse = (reply.ObjResult as Common.Excuse);
//								} else {
//									Console.WriteLine("Remote responded with a failure to give excuse");
//								}
//							} else {
//								// Reply from remote source is an unexpected type, handle that here
//								Console.WriteLine("Unexpected reply");
//							}
//						}
//
//
//
//						if(excuse != null) {
//							break;
//						}
//
//						System.Threading.Thread.Sleep(5);
//					}
//
//					if (excuse != null) {
//						break;
//					}
//
//					envelope.message.MessageNr.SeqNumber += messagesReceived;
//				}
//
//				msgQueue.convoInProgressQueue.Remove(convKey);
//
//				if (excuse != null) {
//					callback(excuse);
//				}
//
//			});
//
//			thread.Start();
//
//
//		}


//		public void getWhiningTwine (Envelope envelope, Action<Common.WhiningTwine> callback) {
//			System.Threading.Thread thread = new System.Threading.Thread(delegate(){
//
//				string convKey = "" + envelope.message.ConversationId.ProcessId + "," + envelope.message.ConversationId.SeqNumber;
//				msgQueue.convoInProgressQueue.Add(convKey, new Queue<Envelope>());
//				Common.WhiningTwine whine = null;
//				short messagesReceived = 0;
//
//				Console.WriteLine(convKey);
//
//				for (int i = 0; i < 3; ++i) {
//
//					comm.sendEnvelope(envelope);
//
//					for (int j = 0; j < 200; ++j) {
//
//						while (msgQueue.convoInProgressQueue[convKey].Count > 0) {
//							messagesReceived++;
//							Envelope currEnv = msgQueue.convoInProgressQueue[convKey].Dequeue();
//
//							if (currEnv.message.MessageTypeId() == Messages.Message.MESSAGE_CLASS_IDS.AckNak) {
//								// Reply from remote source is an AckNak, this is a good thing, check it for an Excuse
//								Messages.AckNak reply = currEnv.message as Messages.AckNak;
//								if (reply.Status == Messages.Reply.PossibleStatus.Success) {
//									whine = (reply.ObjResult as Common.WhiningTwine);
//								} else {
//									Console.WriteLine("Remote responded with a failure to give WhiningTwine");
//								}
//							} else {
//								// Reply from remote source is an unexpected type, handle that here
//								Console.WriteLine("Unexpected reply");
//							}
//						}
//							
//						if(whine != null) {
//							break;
//						}
//
//						System.Threading.Thread.Sleep(5);
//					}
//
//					if (whine != null) {
//						break;
//					}
//
//					envelope.message.MessageNr.SeqNumber += messagesReceived;
//				}
//
//				msgQueue.convoInProgressQueue.Remove(convKey);
//
//				if (whine != null) {
//					callback(whine);
//				}
//
//			});
//
//			thread.Start();
//		}


//		public void JoinGame (Envelope envelope, Action<Common.AgentInfo> callback, Action<string> errorCallback, Action timeoutCallback) {
//
//			System.Threading.Thread thread = new System.Threading.Thread(delegate(){
//
//				string convKey = "" + envelope.message.ConversationId.ProcessId + "," + envelope.message.ConversationId.SeqNumber;
//				msgQueue.convoInProgressQueue.Add(convKey, new Queue<Envelope>());
//				Common.AgentInfo agentInfo = null;
//				bool error = false;
//				string errorMsg = "";
//				short messagesReceived = 0;
//
//				Console.WriteLine(convKey);
//
//				for (int i = 0; i < 3; ++i) {
//
//					comm.sendEnvelope(envelope);
//
//					for (int j = 0; j < 200; ++j) {
//
//						while (msgQueue.convoInProgressQueue[convKey].Count > 0) {
//							messagesReceived++;
//							Envelope currEnv = msgQueue.convoInProgressQueue[convKey].Dequeue();
//
//							if (currEnv.message.MessageTypeId() == Messages.Message.MESSAGE_CLASS_IDS.AckNak) {
//								// Reply from remote source is an AckNak, this is a good thing, check it for an Excuse
//								Messages.AckNak reply = currEnv.message as Messages.AckNak;
//								if (reply.Status == Messages.Reply.PossibleStatus.Success) {
//									agentInfo = (reply.ObjResult as Common.AgentInfo);
//								} else {
//									Console.WriteLine("Remote responded with a failure to join game");
//									Console.WriteLine(reply.Message);
//									errorMsg = reply.Message;
//									error = true;
//									break;
//								}
//							} else {
//								// Reply from remote source is an unexpected type, handle that here
//								Console.WriteLine("Unexpected reply");
//							}
//						}
//
//
//
//						if(agentInfo != null || error) {
//							break;
//						}
//
//						System.Threading.Thread.Sleep(5);
//					}
//
//					if (agentInfo != null || error) {
//						break;
//					}
//
//					envelope.message.MessageNr.SeqNumber += messagesReceived;
//				}
//
//				msgQueue.convoInProgressQueue.Remove(convKey);
//
//				if (error) {
//					if(errorCallback != null) {
//						errorCallback(errorMsg);
//					}
//				} else if (agentInfo != null) {
//					callback(agentInfo);
//				} else {
//					if(timeoutCallback != null) {
//						timeoutCallback();
//					}
//				}
//
//			});
//
//			thread.Start();
//
//
//		}
	}
}

