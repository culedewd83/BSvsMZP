using System;
using System.Net;
using System.Collections.Generic;
using MonoMac.AppKit;
using MonoMac.Foundation;

namespace Middleware
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
		public Queue<Common.WhiningTwine> whiningTwines;
		public Queue<Common.Tick> ticks;
		public Queue<Common.Bomb> bombs;
		public bool shouldListen;

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
			whiningTwines = new Queue<Common.WhiningTwine> ();
			bombs = new Queue<Common.Bomb> ();
			shouldListen = false;
		}

		public void setRemoteEndPoint (string address, int port) {
			agentInfo.remoteServerAddress = address;
			agentInfo.remoteServerPort = port;
			Common.EndPoint ep = new Common.EndPoint ();
			ep.Address = BitConverter.ToInt32(IPAddress.Parse(address).GetAddressBytes(), 0);
			ep.Port = port;
			agentInfo.remoteServerEndPoint = ep;
		}
			
		public void setRemoteEndPoint (Common.EndPoint ep) {
			agentInfo.remoteServerAddress = "" + ep.Address;
			agentInfo.remoteServerPort = ep.Port;
			agentInfo.remoteServerEndPoint = ep;
		}

		public Messages.GetResource makeGetExcuseMessage (Common.Tick tick) {
			Messages.GetResource msg = makeGetResourceMessage(Messages.GetResource.PossibleResourceType.Excuse, tick);
			return msg;
		}

		public Messages.GetResource makeGetWhiningTwineMessage (Common.Tick tick) {
			Messages.GetResource msg = makeGetResourceMessage(Messages.GetResource.PossibleResourceType.WhiningTwine, tick);
			return msg;
		}

		private Messages.GetResource makeGetResourceMessage(Messages.GetResource.PossibleResourceType type, Common.Tick tick) {
			Messages.GetResource msg = new Messages.GetResource(agentInfo.gameID, type, tick);
			//msg.ConversationId = agentInfo.getNewConvoNum();
			//msg.MessageNr = msg.ConversationId;
			return msg;
		}

		public Messages.JoinGame makeJoinGameMessage ()
		{
			Common.AgentInfo aInfo = new Common.AgentInfo ();
			aInfo.AgentType = agentInfo.agentType;
			aInfo.ANumber = "A12345678";
			aInfo.FirstName = "John";
			aInfo.LastName = "Smith";
			aInfo.Id = Common.MessageNumber.LocalProcessId;
			Messages.JoinGame msg = new Messages.JoinGame (agentInfo.gameID, aInfo);
			//msg.ConversationId = agentInfo.getNewConvoNum();
			//msg.MessageNr = msg.ConversationId;
			return msg;
		}

		public void GetProcessID ()
		{
			Common.MessageNumber.LocalProcessId = GameServers.GetProcessID();
			agentInfo.processId = Common.MessageNumber.LocalProcessId;
		}

	}
}

