using System;

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

				msgQueue.convoInProgressQueue.Remove(convKey);

			});

			thread.Start();
		}


		public void sendExcuse (Envelope receivedEnvelope) {
			System.Threading.Thread thread = new System.Threading.Thread(delegate(){

				string convKey = "" + receivedEnvelope.message.ConversationId.ProcessId + "," + receivedEnvelope.message.ConversationId.SeqNumber;

				Messages.GetResource receivedMsg = receivedEnvelope.message as Messages.GetResource;
				Messages.AckNak replyMsg;

				if (agent.excuses.Count > 0) {
					Common.Excuse excuse = getExcuse(receivedMsg.EnablingTick);
					replyMsg = new Messages.AckNak(Messages.Reply.PossibleStatus.Success, excuse);
				} else {
					replyMsg = new Messages.AckNak(Messages.Reply.PossibleStatus.Failure, 1, "Not enough excuses");
				}

				replyMsg.ConversationId = Common.MessageNumber.Create();
				replyMsg.ConversationId.ProcessId = receivedMsg.ConversationId.ProcessId;
				replyMsg.ConversationId.SeqNumber = receivedMsg.ConversationId.SeqNumber;
				replyMsg.MessageNr.ProcessId = agentInfo.processId;
				replyMsg.MessageNr.SeqNumber = (short)(receivedMsg.MessageNr.SeqNumber + 1);
				Envelope replyEnv = new Envelope(replyMsg, receivedEnvelope.endPoint);
				comm.sendEnvelope(replyEnv);

				msgQueue.convoInProgressQueue.Remove(convKey);

			});

			thread.Start();
		}
		
		public void sendWhiningTwine (Envelope receivedEnvelope) {
			System.Threading.Thread thread = new System.Threading.Thread(delegate(){

				string convKey = "" + receivedEnvelope.message.ConversationId.ProcessId + "," + receivedEnvelope.message.ConversationId.SeqNumber;
				Messages.GetResource receivedMsg = receivedEnvelope.message as Messages.GetResource;
				Messages.AckNak replyMsg;

				if (agent.whiningTwines.Count > 0) {
					Common.WhiningTwine whiningTwine = getWhiningTwine(receivedMsg.EnablingTick);
					replyMsg = new Messages.AckNak(Messages.Reply.PossibleStatus.Success, whiningTwine);
				} else {
					replyMsg = new Messages.AckNak(Messages.Reply.PossibleStatus.Failure, 1, "Not enough twine");
				}
				replyMsg.ConversationId = Common.MessageNumber.Create();
				replyMsg.ConversationId.ProcessId = receivedMsg.ConversationId.ProcessId;
				replyMsg.ConversationId.SeqNumber = receivedMsg.ConversationId.SeqNumber;
				replyMsg.MessageNr.ProcessId = agentInfo.processId;
				replyMsg.MessageNr.SeqNumber = (short)(receivedMsg.MessageNr.SeqNumber + 1);
				Envelope replyEnv = new Envelope(replyMsg, receivedEnvelope.endPoint);
				comm.sendEnvelope(replyEnv);
				msgQueue.convoInProgressQueue.Remove(convKey);

			});

			thread.Start();
		}

		private Common.Excuse getExcuse (Common.Tick requestTick) {
			Common.Excuse excuse = agent.excuses.Dequeue();
			//excuse.CreatorId = agentInfo.processId;
			//excuse.Ticks.Add(new Common.Tick ());
			excuse.RequestTick = requestTick;
			return agent.excuses.Dequeue();
		}


		private Common.WhiningTwine getWhiningTwine (Common.Tick requestTick) {
			Common.WhiningTwine whiningTwine = agent.whiningTwines.Dequeue();
			//whiningTwine.CreatorId = agentInfo.processId;
			//whiningTwine.Ticks.Add(new Common.Tick ());
			whiningTwine.RequestTick = requestTick;
			return whiningTwine;
		}


	}
}

