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

		public List<short> GetRandomizeIDs (ConcurrentDictionary<short, RemoteAgent> dict)
		{
			List<short> agentIDs = new List<short> ();
			foreach (KeyValuePair<short, RemoteAgent> pair in dict) {
				agentIDs.Add(pair.Key);
			}

			List<short> rndList = new List<short> ();
			Random rnd = new Random ();
			while (agentIDs.Count > 0) {
				int index = rnd.Next(0, agentIDs.Count);
				rndList.Add(agentIDs[index]);
				agentIDs.RemoveAt(index);
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
			aInfo.ANumber = "A01537812";
			aInfo.FirstName = "Jesse";
			aInfo.LastName = "Rogers";
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

		public Common.FieldLocation ClosestZombieLocation ()
		{
			Common.FieldLocation location = null;
			double shortestDistance = double.PositiveInfinity;
			foreach (KeyValuePair<short, RemoteAgent> pair in zombieAgents) {
				Common.AgentInfo zombie = zombieAgents [pair.Key].agent;
				double distanceToThisZombie = Math.Sqrt(Math.Pow((agentInfo.CommonAgentInfo.Location.X - zombie.Location.X),2) + Math.Pow((agentInfo.CommonAgentInfo.Location.Y - zombie.Location.Y),2));
				if (distanceToThisZombie < shortestDistance) {
					shortestDistance = distanceToThisZombie;
					location = zombie.Location;
				}
			}
			return location;
		}

		public void TryToMoveAwayFrom(Common.FieldLocation location)
		{

		}

		public void TryToMoveCloserTo(Common.FieldLocation location)
		{
			short myX = agentInfo.CommonAgentInfo.Location.X;
			short myY = agentInfo.CommonAgentInfo.Location.Y;
			short toX, toY;

			if (location.Y == myY) {
				if (location.X < myX) {
					toX = (short)(myX - Math.Min(gameConfig.BrilliantStudentBaseSpeed, myX - location.X));
				} else {
					toX = (short)(myX + Math.Min(gameConfig.BrilliantStudentBaseSpeed, location.X - myX));
				}
				toY = myY;
			} else if (location.X == myX) {
				if (location.Y < myY) {
					toY = (short)(myY - Math.Min(gameConfig.BrilliantStudentBaseSpeed, myY - location.Y));
				} else {
					toY = (short)(myY + Math.Min(gameConfig.BrilliantStudentBaseSpeed, location.Y - myY));
				}
				toX = myX;
			} else if (location.Y < myY) {
				if (location.X < myX) {
					toX = (short)(myX - gameConfig.BrilliantStudentBaseSpeed/2);
					toY = (short)(myY - gameConfig.BrilliantStudentBaseSpeed/2);
					if (toX < 1) {
						toX = 1;
					}
					if (toY < 1) {
						toY = 1;
					}
				} else {
					toX = (short)(myX + gameConfig.BrilliantStudentBaseSpeed/2);
					toY = (short)(myY - gameConfig.BrilliantStudentBaseSpeed/2);
					if (toX > gameConfig.PlayingFieldWidth) {
						toX = gameConfig.PlayingFieldWidth;
					}
					if (toY < 1) {
						toY = 1;
					}
				}
			} else {
				if (location.X < myX) {
					toX = (short)(myX - gameConfig.BrilliantStudentBaseSpeed/2);
					toY = (short)(myY + gameConfig.BrilliantStudentBaseSpeed/2);
					if (toX < 1) {
						toX = 1;
					}
					if (toY > gameConfig.PlayingFieldHeight) {
						toY = gameConfig.PlayingFieldHeight;
					}
				} else {
					toX = (short)(myX + gameConfig.BrilliantStudentBaseSpeed/2);
					toY = (short)(myY + gameConfig.BrilliantStudentBaseSpeed/2);
					if (toX > gameConfig.PlayingFieldWidth) {
						toX = gameConfig.PlayingFieldWidth;
					}
					if (toY > gameConfig.PlayingFieldHeight) {
						toY = gameConfig.PlayingFieldHeight;
					}
				}
			}

			Console.WriteLine("X: " + toX);
			Console.WriteLine("Y: " + toY);

			Common.FieldLocation moveLocation = new Common.FieldLocation (toX, toY, true);
			TryToMoveTo(moveLocation);
		}

		public void TryToThrowBombAt(Common.FieldLocation location)
		{
			if (bombs.Count > 0 && ticks.Count > 0) {
				//Console.WriteLine("trying to throw bomb!!!!!!!!!!!");
				Messages.ThrowBomb bombMsg = new Messages.ThrowBomb (agentInfo.processId, bombs.Dequeue(), location, ticks.Dequeue());
				Envelope envelope = new Envelope (bombMsg, agentInfo.remoteServerEndPoint);
				instigatorStrategies.ThrowBomb(envelope, (updatedAgentInfo) => {
					UpdateRemoteAgents(updatedAgentInfo);
				});
			}
		}

		public void RandomMove ()
		{
			if (ticks.Count <= 0) {
				return;
			}
			int x = agentInfo.CommonAgentInfo.Location.X;
			int y = agentInfo.CommonAgentInfo.Location.Y;

			Random rand = new Random ();
			int choice = rand.Next(1, 4);
			switch (choice)
			{
			case 1:
				x++;
				break;

			case 2:
				x--;
				break;
			case 3:
				y++;
				break;
			case 4:
				y--;
				break;
			}
			if (y > 0 && x > 0 && y < gameConfig.PlayingFieldHeight && x < gameConfig.PlayingFieldWidth) {
				Common.FieldLocation loc = new Common.FieldLocation ((short)x, (short)y, true);
				TryToMoveTo(loc);
			}
		}


		public void TryToMoveTo(Common.FieldLocation location)
		{
			if (ticks.Count > 0) {
				Messages.Move moveMsg = new Messages.Move (agentInfo.processId, location, ticks.Dequeue());
				Envelope envelope = new Envelope (moveMsg, agentInfo.remoteServerEndPoint);
				instigatorStrategies.MoveAgent(envelope, (updatedAgentInfo) => {
					agentInfo.CommonAgentInfo = updatedAgentInfo;
				});
			}
		}

		public void TryToMakeBomb()
		{
			if (ticks.Count > 0 && excuses.Count > 0 && whiningTwines.Count > 0) {
				bombs.Enqueue(new Common.Bomb(agentInfo.processId,
					new List<Common.Excuse>(){excuses.Dequeue()},
					new List<Common.WhiningTwine>(){whiningTwines.Dequeue()},
					ticks.Dequeue()));
			}
		}

		public void TryToGetExcuse()
		{
			if (excuseAgents != null && excuseAgents.Count > 0 && ticks.Count > 0) {
				List<short> agentIDs = GetRandomizeIDs(excuseAgents);
				foreach (short agentID in agentIDs) {
					if (!excuseAgents[agentID].currentlyCommunicatingWith) {
						excuseAgents[agentID].currentlyCommunicatingWith = true;
						GetExcuse(excuseAgents[agentID].agent);
						break;
					}
				}
			}
		}

		public void GetExcuse(Common.AgentInfo remoteAgent) {
			if (shouldListen && ticks.Count > 0) {
				Envelope envelope = new Envelope (makeGetExcuseMessage(ticks.Dequeue()), remoteAgent.CommunicationEndPoint);
				instigatorStrategies.getExcuse(envelope, (excuse) => {
					excuses.Enqueue(excuse);
					excuseAgents[remoteAgent.Id].currentlyCommunicatingWith = false;
					excuseAgents[remoteAgent.Id].successfulReplies += 1;
				}, (tick) => {
					ticks.Enqueue(tick);
					excuseAgents[remoteAgent.Id].currentlyCommunicatingWith = false;
				});
			}
		}

		public void TryToGetWhiningTwine()
		{
			if (whiningAgents != null && whiningAgents.Count > 0 && ticks.Count > 0) {
				List<short> agentIDs = GetRandomizeIDs(whiningAgents);
				foreach (short agentID in agentIDs) {
					if (!whiningAgents[agentID].currentlyCommunicatingWith) {
						whiningAgents[agentID].currentlyCommunicatingWith = true;
						GetWhiningTwine(whiningAgents[agentID].agent);
						break;
					}
				}
			}
		}

		public void GetWhiningTwine(Common.AgentInfo remoteAgent) {
			if (shouldListen && ticks.Count > 0) {
				Envelope envelope = new Envelope (makeGetWhiningTwineMessage(ticks.Dequeue()), remoteAgent.CommunicationEndPoint);
				instigatorStrategies.getWhiningTwine(envelope, (twine) => {
					whiningTwines.Enqueue(twine);
					whiningAgents[remoteAgent.Id].currentlyCommunicatingWith = false;
					whiningAgents[remoteAgent.Id].successfulReplies += 1;
				}, (tick) => {
					ticks.Enqueue(tick);
					whiningAgents[remoteAgent.Id].currentlyCommunicatingWith = false;
				});
			}
		}

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

		public void UpdateRemoteAgents (Common.AgentList agents)
		{
			foreach (Common.AgentInfo remoteAgent in agents) {
				if (remoteAgent.AgentType == Common.AgentInfo.PossibleAgentType.BrilliantStudent) {
					if (brillantAgents.ContainsKey(remoteAgent.Id)) {
						brillantAgents [remoteAgent.Id].agent = remoteAgent;
					} else {
						brillantAgents.TryAdd(remoteAgent.Id, new RemoteAgent (remoteAgent));
					}
				} else if (remoteAgent.AgentType == Common.AgentInfo.PossibleAgentType.ExcuseGenerator) {
					if (excuseAgents.ContainsKey(remoteAgent.Id)) {
						excuseAgents [remoteAgent.Id].agent = remoteAgent;
					} else {
						excuseAgents.TryAdd(remoteAgent.Id, new RemoteAgent (remoteAgent));
					}
				} else if (remoteAgent.AgentType == Common.AgentInfo.PossibleAgentType.WhiningSpinner) {
					if (whiningAgents.ContainsKey(remoteAgent.Id)) {
						whiningAgents [remoteAgent.Id].agent = remoteAgent;
					} else {
						whiningAgents.TryAdd(remoteAgent.Id, new RemoteAgent (remoteAgent));
					}
				} else if (remoteAgent.AgentType == Common.AgentInfo.PossibleAgentType.ZombieProfessor) {
					if (zombieAgents.ContainsKey(remoteAgent.Id)) {
						zombieAgents [remoteAgent.Id].agent = remoteAgent;
					} else {
						zombieAgents.TryAdd(remoteAgent.Id, new RemoteAgent (remoteAgent));
					}
				}
			}
		}

	}
}

