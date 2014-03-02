using System;

namespace BSvsMZP
{
	public class ResponderStrategies
	{

		MessageQueue msgQueue;
		Communicator comm;
		AgentInfo agentInfo;


		public ResponderStrategies(Communicator comm, MessageQueue msgQueue)
		{
			this.msgQueue = msgQueue;
			this.comm = comm;
			agentInfo = AgentInfo.Instance;
		}


		public void sendExcuse (Envelope receivedEnvelope) {
			System.Threading.Thread thread = new System.Threading.Thread(delegate(){

				string convKey = "" + receivedEnvelope.message.ConversationId.ProcessId + "," + receivedEnvelope.message.ConversationId.SeqNumber;
				Messages.GetResource receivedMsg = receivedEnvelope.message as Messages.GetResource;
				Common.Excuse excuse = getExcuse(receivedMsg.EnablingTick);
				Messages.AckNak replyMsg = new Messages.AckNak(Messages.Reply.PossibleStatus.Success, (excuse as Common.DistributableObject));
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
			Common.Excuse excuse = new Common.Excuse();
			excuse.CreatorId = agentInfo.processId;
			excuse.Ticks.Add(new Common.Tick ());
			excuse.RequestTick = requestTick;
			return excuse;
		}


	}
}

