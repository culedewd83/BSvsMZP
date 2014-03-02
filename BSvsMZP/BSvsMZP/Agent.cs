using System;
using System.Net;
using System.Collections.Generic;

namespace BSvsMZP
{
	public class Agent
	{
		public Communicator comm;
		public MessageQueue msgQueue;
		public Listener listener;
		public InstigatorStrategies instigatorStrategies;
		public AgentInfo agentInfo;
		public long MessagesMovedToQueue { get { return listener.totalMessagesMoved; }}
		public Queue<Common.Excuse> excuses;
		public Queue<Common.Tick> ticks;

		public Agent()
		{
			comm = new Communicator ();
			msgQueue = new MessageQueue();
			listener = new Listener (comm, msgQueue);
			instigatorStrategies = new InstigatorStrategies (comm, msgQueue);
			agentInfo = new AgentInfo ();
			agentInfo.remoteServerAddress = "127.0.0.1";
			agentInfo.remoteServerPort = comm.getPort();
			Common.EndPoint localEP = new Common.EndPoint ();
			localEP.Address = BitConverter.ToInt32(IPAddress.Parse("127.0.0.1").GetAddressBytes(), 0);
			localEP.Port = comm.getPort();
			agentInfo.remoteServerEndPoint = localEP;
			excuses = new Queue<Common.Excuse>();
			ticks = new Queue<Common.Tick>();

			// Making some dummy ticks for testing use
			for (int i = 0; i < 1000; ++i) {
				ticks.Enqueue(new Common.Tick ());
			}
		}

		public void setRemoteEndPoint (string address, int port) {
			agentInfo.remoteServerAddress = address;
			agentInfo.remoteServerPort = port;
			Common.EndPoint ep = new Common.EndPoint ();
			ep.Address = BitConverter.ToInt32(IPAddress.Parse(address).GetAddressBytes(), 0);
			ep.Port = port;
			agentInfo.remoteServerEndPoint = ep;
		}
			


		public Messages.GetResource makeGetExcuseMessage (Common.Tick tick) {
			Messages.GetResource msg = makeGetResourceMessage(Messages.GetResource.PossibleResourceType.Excuse, tick);
			return msg;
		}

		private Messages.GetResource makeGetResourceMessage(Messages.GetResource.PossibleResourceType type, Common.Tick tick) {
			Messages.GetResource msg = new Messages.GetResource(agentInfo.gameID, type, tick);
			msg.ConversationId = agentInfo.getNewConvoNum();
			msg.MessageNr = msg.ConversationId;
			return msg;
		}
	}
}

