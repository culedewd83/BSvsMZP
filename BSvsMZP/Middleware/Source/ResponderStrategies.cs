using System;
using Messages;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Middleware
{
	public class ResponderStrategies
	{

		MessageQueue msgQueue;
		Communicator comm;
		AgentInfo agentInfo;
		Middleware.Agent agent;


		public ResponderStrategies(Communicator comm, MessageQueue msgQueue, AgentInfo agentInfo, Middleware.Agent agent)
		{
			this.msgQueue = msgQueue;
			this.comm = comm;
			this.agentInfo = agentInfo;
			this.agent = agent;
		}

		public void ReceiveTick (Envelope receivedEnvelope) {
			System.Threading.Thread thread = new System.Threading.Thread(delegate(){

				string convKey = "" + receivedEnvelope.message.ConversationId.ProcessId + "," + receivedEnvelope.message.ConversationId.SeqNumber;

				agent.ticks.Enqueue(((Messages.TickDelivery)receivedEnvelope.message).CurrentTick);

				ConcurrentQueue<Envelope> disposed;
				msgQueue.convoInProgressQueue.TryRemove(convKey, out disposed);
			});

			thread.Start();
		}

		public void ReceiveAgentUpdateStream (Envelope receivedEnvelope) {
			System.Threading.Thread thread = new System.Threading.Thread(delegate(){
				string convKey = "" + receivedEnvelope.message.ConversationId.ProcessId + "," + receivedEnvelope.message.ConversationId.SeqNumber;
				foreach (Common.AgentInfo remoteAgent in ((Messages.AgentListReply)receivedEnvelope.message).Agents) {
					if (remoteAgent.AgentType == Common.AgentInfo.PossibleAgentType.BrilliantStudent) {
						if (agent.brillantAgents.ContainsKey(remoteAgent.Id)) {
							agent.brillantAgents[remoteAgent.Id].agent = remoteAgent;
						} else {
							agent.brillantAgents.TryAdd(remoteAgent.Id, new RemoteAgent(remoteAgent));
						}
					} else if (remoteAgent.AgentType == Common.AgentInfo.PossibleAgentType.ExcuseGenerator) {
						if (agent.excuseAgents.ContainsKey(remoteAgent.Id)) {
							agent.excuseAgents[remoteAgent.Id].agent = remoteAgent;
						} else {
							agent.excuseAgents.TryAdd(remoteAgent.Id, new RemoteAgent(remoteAgent));
						}
					} else if (remoteAgent.AgentType == Common.AgentInfo.PossibleAgentType.WhiningSpinner) {
						if (agent.whiningAgents.ContainsKey(remoteAgent.Id)) {
							agent.whiningAgents[remoteAgent.Id].agent = remoteAgent;
						} else {
							agent.whiningAgents.TryAdd(remoteAgent.Id, new RemoteAgent(remoteAgent));
						}
					} else if (remoteAgent.AgentType == Common.AgentInfo.PossibleAgentType.ZombieProfessor) {
						if (agent.zombieAgents.ContainsKey(remoteAgent.Id)) {
							agent.zombieAgents[remoteAgent.Id].agent = remoteAgent;
						} else {
							agent.zombieAgents.TryAdd(remoteAgent.Id, new RemoteAgent(remoteAgent));
						}
					}
				}
				ConcurrentQueue<Envelope> disposed;
				msgQueue.convoInProgressQueue.TryRemove(convKey, out disposed);
			});
			thread.Start();
		}

		public void ReceiveChangeStrength (Envelope receivedEnvelope) {
			System.Threading.Thread thread = new System.Threading.Thread(delegate(){
				string convKey = "" + receivedEnvelope.message.ConversationId.ProcessId + "," + receivedEnvelope.message.ConversationId.SeqNumber;
				Messages.AckNak replyMsg;

				try {
					agent.agentInfo.CommonAgentInfo.Strength += ((Messages.ChangeStrength)receivedEnvelope.message).DeltaValue;
					replyMsg = new AckNak(Reply.PossibleStatus.Success, agent.agentInfo.CommonAgentInfo);
				} catch {
					replyMsg = new AckNak(Reply.PossibleStatus.Failure);
				}

				replyMsg.ConversationId = receivedEnvelope.message.ConversationId;
				Envelope env = new Envelope(replyMsg, receivedEnvelope.endPoint);
				comm.sendEnvelope(env);

				ConcurrentQueue<Envelope> disposed;
				msgQueue.convoInProgressQueue.TryRemove(convKey, out disposed);

			});

			thread.Start();
		}

		public void ReceiveEndGame (Envelope receivedEnvelope) {
			System.Threading.Thread thread = new System.Threading.Thread(delegate(){

				string convKey = "" + receivedEnvelope.message.ConversationId.ProcessId + "," + receivedEnvelope.message.ConversationId.SeqNumber;

				agent.GameHasEnded();

				ConcurrentQueue<Envelope> disposed;
				msgQueue.convoInProgressQueue.TryRemove(convKey, out disposed);

			});

			thread.Start();
		}

		public void ReceiveStartGame (Envelope receivedEnvelope) {
			System.Threading.Thread thread = new System.Threading.Thread(delegate(){
				string convKey = "" + receivedEnvelope.message.ConversationId.ProcessId + "," + receivedEnvelope.message.ConversationId.SeqNumber;
				msgQueue.convoInProgressQueue.TryAdd(convKey, new ConcurrentQueue <Envelope>());
				ReadyReply readyReply = new ReadyReply(Reply.PossibleStatus.Success, "Good to go!");
				readyReply.ConversationId = receivedEnvelope.message.ConversationId;
				Envelope readyEnvelope = new Envelope(readyReply, receivedEnvelope.endPoint);
				SendReply (readyEnvelope, (msg) => { StartGameAckNakHandler(msg); });

			});

			thread.Start();
		}

		private void StartGameAckNakHandler (Message msg)
		{
			agent.GameHasStarted();
		}
			

		public void sendExcuse (Envelope receivedEnvelope) {
			System.Threading.Thread thread = new System.Threading.Thread(delegate(){

				string convKey = "" + receivedEnvelope.message.ConversationId.ProcessId + "," + receivedEnvelope.message.ConversationId.SeqNumber;

				Messages.GetResource receivedMsg = receivedEnvelope.message as Messages.GetResource;
				Messages.ResourceReply replyMsg;


				if (agent.excuses.Count > 0) {
					Common.Excuse excuse = getExcuse(receivedMsg.EnablingTick);
					replyMsg = new Messages.ResourceReply(Messages.Reply.PossibleStatus.Success, excuse, "Here you go dawg");
				} else {
					replyMsg = new Messages.ResourceReply(Messages.Reply.PossibleStatus.Failure, null, "Not enough excuses");
				}

				replyMsg.ConversationId = receivedMsg.ConversationId;
				Envelope replyEnv = new Envelope(replyMsg, receivedEnvelope.endPoint);
				comm.sendEnvelope(replyEnv);
				ConcurrentQueue<Envelope> disposed;
				msgQueue.convoInProgressQueue.TryRemove(convKey, out disposed);
			});
			thread.Start();
		}
		
		public void sendWhiningTwine (Envelope receivedEnvelope) {
			System.Threading.Thread thread = new System.Threading.Thread(delegate(){

				string convKey = "" + receivedEnvelope.message.ConversationId.ProcessId + "," + receivedEnvelope.message.ConversationId.SeqNumber;
				Messages.GetResource receivedMsg = receivedEnvelope.message as Messages.GetResource;
				Messages.ResourceReply replyMsg;


				if (agent.whiningTwines.Count > 0) {
					Common.WhiningTwine whiningTwine = getWhiningTwine(receivedMsg.EnablingTick);
					replyMsg = new Messages.ResourceReply(Messages.Reply.PossibleStatus.Success, whiningTwine, "Here you go dawg");
				} else {
					replyMsg = new Messages.ResourceReply(Messages.Reply.PossibleStatus.Failure, null, "Not enough twine");
				}

				replyMsg.ConversationId = receivedMsg.ConversationId;
				Envelope replyEnv = new Envelope(replyMsg, receivedEnvelope.endPoint);
				comm.sendEnvelope(replyEnv);

				ConcurrentQueue<Envelope> disposed;
				msgQueue.convoInProgressQueue.TryRemove(convKey, out disposed);

			});

			thread.Start();
		}

		private Common.Excuse getExcuse (Common.Tick requestTick) {
			Common.Excuse excuse = agent.excuses.Dequeue();
			excuse.RequestTick = requestTick;
			return excuse;
		}


		private Common.WhiningTwine getWhiningTwine (Common.Tick requestTick) {
			Common.WhiningTwine whiningTwine = agent.whiningTwines.Dequeue();
			whiningTwine.RequestTick = requestTick;
			return whiningTwine;
		}



		private void SendReply (Envelope envelope, Action<Message> callback)
		{
			System.Threading.Thread thread = new System.Threading.Thread (delegate() {
				string convKey = "" + envelope.message.ConversationId.ProcessId + "," + envelope.message.ConversationId.SeqNumber;
				ConcurrentQueue<Envelope> disposed;
				for (int i = 0; i < 3; ++i) {
					comm.sendEnvelope(envelope);
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

	}
}

