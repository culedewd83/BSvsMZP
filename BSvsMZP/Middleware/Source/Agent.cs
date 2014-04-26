using System;
using System.Net;
using System.Collections.Generic;
using MonoMac.AppKit;
using MonoMac.Foundation;
using System.Collections.Concurrent;

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
		public bool gameHasStarted = false;
		public bool gameHasEnded = false;
		public Common.GameConfiguration gameConfig;
		public Common.AgentList remoteExcuseAgents;
		public Common.AgentList remoteWhiningAgents;
		public Common.AgentList remoteBrilliantAgents;
		public Common.AgentList remoteZombieAgents;
		public System.Threading.Thread agentListThread;
		public Dictionary<int, int> waitingForReply = new Dictionary<int, int>();
		public ConcurrentDictionary<short, RemoteAgent> excuseAgents = new ConcurrentDictionary<short, RemoteAgent>();
		public ConcurrentDictionary<short, RemoteAgent> whiningAgents = new ConcurrentDictionary<short, RemoteAgent>();
		public ConcurrentDictionary<short, RemoteAgent> brillantAgents = new ConcurrentDictionary<short, RemoteAgent>();
		public ConcurrentDictionary<short, RemoteAgent> zombieAgents = new ConcurrentDictionary<short, RemoteAgent>();


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
			return msg;
		}

		public void GetGameConfig()
		{
			Messages.GetResource msg = new Messages.GetResource (agentInfo.gameID, Messages.GetResource.PossibleResourceType.GameConfiguration);
			Envelope env = new Envelope (msg, agentInfo.remoteServerEndPoint);
			instigatorStrategies.GetGameConfig(env, (config) => {
				gameConfig = config;
			});
		}

		public void GetBrilliantAgentList()
		{
			Messages.GetResource msg = new Messages.GetResource (agentInfo.gameID, Messages.GetResource.PossibleResourceType.BrillianStudentList);
			Envelope env = new Envelope (msg, agentInfo.remoteServerEndPoint);
			instigatorStrategies.GetAgentList(env, (agentList) => {
				remoteBrilliantAgents = agentList;
			});
		}

		public void GetExcuseAgentList()
		{
			Messages.GetResource msg = new Messages.GetResource (agentInfo.gameID, Messages.GetResource.PossibleResourceType.ExcuseGeneratorList);
			Envelope env = new Envelope (msg, agentInfo.remoteServerEndPoint);
			instigatorStrategies.GetAgentList(env, (agentList) => {
				remoteExcuseAgents = agentList;
			});
		}

		public void GetWhiningAgentList()
		{
			Messages.GetResource msg = new Messages.GetResource (agentInfo.gameID, Messages.GetResource.PossibleResourceType.WhiningSpinnerList);
			Envelope env = new Envelope (msg, agentInfo.remoteServerEndPoint);
			instigatorStrategies.GetAgentList(env, (agentList) => {
				remoteWhiningAgents = agentList;
			});
		}

		public void GetZombieAgentList()
		{
			Messages.GetResource msg = new Messages.GetResource (agentInfo.gameID, Messages.GetResource.PossibleResourceType.ZombieProfessorList);
			Envelope env = new Envelope (msg, agentInfo.remoteServerEndPoint);
			instigatorStrategies.GetAgentList(env, (agentList) => {
				remoteZombieAgents = agentList;
			});
		}

		public List<Common.AgentInfo> CreateRandomAgentList (Common.AgentList lst)
		{
			List<Common.AgentInfo> agentList = new List<Common.AgentInfo> ();
			foreach (Common.AgentInfo agent in lst) {
				agentList.Add(agent);
			}
			List<Common.AgentInfo> rndList = new List<Common.AgentInfo> ();
			Random rnd = new Random ();
			while (agentList.Count > 0) {
				int index = rnd.Next(0, agentList.Count);
				rndList.Add(agentList[index]);
				agentList.RemoveAt(index);
			}
			return rndList;
		}

		public void StartUpdatingAgentLists ()
		{
			agentListThread = new System.Threading.Thread(delegate(){
				int counter = 0;
				while(!gameHasEnded) {
					counter = counter % 2;
					if(counter == 0) {
						GetExcuseAgentList();
						GetWhiningAgentList();
					}
					GetBrilliantAgentList();
					GetZombieAgentList();
					counter++;
					System.Threading.Thread.Sleep(5000);
				}
			});
			agentListThread.Start();
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
			return msg;
		}

		public void GetProcessID ()
		{
			Common.MessageNumber.LocalProcessId = GameServers.GetProcessID();
			agentInfo.processId = Common.MessageNumber.LocalProcessId;
		}


		public void GameHasStarted()
		{
			gameHasStarted = true;
			//StartUpdatingAgentLists();
			StartUpdateStream();
			agentInfo.gameStatus = "Game in progress";
		}

		public void GameHasEnded()
		{
			gameHasEnded = true;
			agentInfo.gameStatus = "Game has ended";
			agentInfo.status = "Inactive";
		}

		public void ReceiveAgentInfo(Common.AgentInfo aInfo)
		{
			Console.WriteLine("AgentInfo received from game server");
			agentInfo.status = "Active";
			agentInfo.CommonAgentInfo = aInfo;
			agentInfo.gameStatus = "Joined, Not Started";
			GetGameConfig();
		}

		public void JoinGame(Common.EndPoint ep, Action<string> errorCallback, Action timeoutCallback) {
			if (shouldListen) {
				Envelope envelope = new Envelope (makeJoinGameMessage(), ep);
				instigatorStrategies.JoinGame(envelope, (aInfo) => {
					ReceiveAgentInfo(aInfo);
				}, errorCallback, timeoutCallback);
			}
		}

		public void getExcuse(Common.EndPoint ep) {
			if (shouldListen && ticks.Count > 0) {
				Envelope envelope = new Envelope (makeGetExcuseMessage(ticks.Dequeue()), ep);
				instigatorStrategies.getExcuse(envelope, (excuse) => {
					excuses.Enqueue(excuse);
					waitingForReply.Remove(ep.Address);
				}, (tick) => {
					ticks.Enqueue(tick);
					waitingForReply.Remove(ep.Address);
				});
			}
		}

//		public void receiveExcuse(Common.Excuse excuse) {
//			excuses.Enqueue(excuse);
//		}

		public void getWhiningTwine(Common.EndPoint ep) {
			if (shouldListen && ticks.Count > 0) {
				Envelope envelope = new Envelope (makeGetWhiningTwineMessage(ticks.Dequeue()), ep);
				instigatorStrategies.getWhiningTwine(envelope, (twine) => {
					whiningTwines.Enqueue(twine);
					waitingForReply.Remove(ep.Address);
				}, (tick) => {
					ticks.Enqueue(tick);
					waitingForReply.Remove(ep.Address);
				});
			}
		}

//		public void receiveWhiningTwine(Common.WhiningTwine whine) {
//			whiningTwines.Enqueue(whine);
//		}


		public void StartUpdateStream ()
		{
			Envelope envelope = new Envelope (new Messages.StartUpdateStream (), agentInfo.remoteServerEndPoint);
			instigatorStrategies.StartUpdateStream(envelope, (didSucceed) => {
				if (didSucceed) {
					Console.WriteLine("Successfully started update stream");
				} else {
					Console.WriteLine("Failed to start update stream");
				}
			});
		}

	}
}

