using System;
using Middleware;
using System.Collections.Generic;

namespace BrilliantStudentAgent
{
	public class BrilliantStudentAgent
	{
		public BrilliantStudent bsAgent = new BrilliantStudent();
		public Middleware.AgentInfo AgentInfo { get{ return  bsAgent.agentInfo; }}
		public int NumOfExcuses { get { return bsAgent.excuses.Count; }}
		public int NumOfTicks { get { return bsAgent.ticks.Count; }}
		public int NumOfTwine { get { return bsAgent.whiningTwines.Count; }}
		public int NumOfBombs { get { return bsAgent.bombs.Count; }}
		System.Threading.Thread agentBrainThread;
		public bool brainShouldLoop = true;

		public BrilliantStudentAgent()
		{
			agentBrainThread = new System.Threading.Thread(delegate(){
				while(brainShouldLoop) {
					UpdateAgent();
					System.Threading.Thread.Sleep(5);
				}
				Console.WriteLine("Agent brain has stopped");
			});
			agentBrainThread.Start();
		}

		private void UpdateAgent ()
		{
			if (bsAgent.gameHasEnded) {
				bsAgent.stopListening();
				brainShouldLoop = false;
				return;
			}

			//bsAgent.RandomMove();

			Common.FieldLocation closestZombieLocation = bsAgent.ClosestZombieLocation();
			if (closestZombieLocation != null) {
				if (DistanceTo(closestZombieLocation) <= bsAgent.gameConfig.BombTwinePerSquareOfDistance) {
					bsAgent.RandomMove();
					//bsAgent.TryToThrowBombAt(closestZombieLocation);
					//bsAgent.TryToMoveAwayFrom(closestZombieLocation);
				} else if (DistanceTo(closestZombieLocation) <= bsAgent.gameConfig.BombTwinePerSquareOfDistance * 1.5) {
					//bsAgent.TryToMoveCloserTo(closestZombieLocation);
					bsAgent.TryToThrowBombAt(closestZombieLocation);
				}
				//bsAgent.TryToThrowBombAt(closestZombieLocation);

			}
				
				bsAgent.TryToMakeBomb();
				bsAgent.TryToGetExcuse();
				bsAgent.TryToGetWhiningTwine();

		}



		public void SetGameServer(SimpleServerInfo server)
		{
			bsAgent.setRemoteEndPoint(server.EndPoint);
			bsAgent.agentInfo.gameID = server.Id;
			bsAgent.agentInfo.gameServerLabel = server.Label;
		}

		public void StartAgent()
		{
			bsAgent.startListening();
		}

		public void JoinGameServer(Action<string> errorCallback, Action timeoutCallback)
		{
			bsAgent.agentInfo.status = "Joining Game...";

			bsAgent.JoinGame(bsAgent.agentInfo.remoteServerEndPoint, (errorMsg) => {
				bsAgent.agentInfo.status = "Inactive";
				bsAgent.agentInfo.gameID = 0;
				bsAgent.stopListening();
				if (errorCallback != null) {
					errorCallback(errorMsg);
				}
			}, () => {
				bsAgent.agentInfo.status = "Inactive";
				bsAgent.agentInfo.gameID = 0;
				bsAgent.stopListening();
				if (timeoutCallback != null) {
					timeoutCallback();
				}
			});
		}

		public double DistanceTo (Common.FieldLocation location)
		{
			return Math.Sqrt(Math.Pow((bsAgent.agentInfo.CommonAgentInfo.Location.X - location.X),2) + Math.Pow((bsAgent.agentInfo.CommonAgentInfo.Location.Y - location.Y),2));

		}
	}
}

