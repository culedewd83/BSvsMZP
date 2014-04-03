using NUnit.Framework;
using System;
using Middleware;
using System.Collections.Generic;

namespace UnitTesting
{
	[TestFixture()]
	public class Test
	{


		[Test()]
		public void JoinGameStrategy ()
		{

			// Setting up a test communicator to send a JoinGame, it will reply with an AckNak
			Middleware.TestCommunicator testComm = new TestCommunicator ();
			testComm.startListening();


			// Making an end point to the test comm
			Common.EndPoint ep = new Common.EndPoint ("127.0.0.1", testComm.getPort());


			// This is the new BrilliantStudentAgent which contains the ability to connect to a game server
			BrilliantStudentAgent.BrilliantStudentAgent bsAgent = new BrilliantStudentAgent.BrilliantStudentAgent ();


			// Setting the endpoint to the test comm
			SimpleServerInfo srvInfo = new SimpleServerInfo ();
			srvInfo.Address = ep.Address;
			srvInfo.Port = ep.Port;
			srvInfo.EndPoint = ep;
			srvInfo.Id = (short)1;
			srvInfo.Label = "Test";
			bsAgent.SetGameServer(srvInfo);
			bsAgent.StartAgent();


			// Before joining a game, the AgentInfo should be null
			Assert.True(bsAgent.AgentInfo.CommonAgentInfo == null);


			// Invoking the join game stategy, this sends a join game message to the test comm
			bsAgent.JoinGameServer(null, null);


			// Wating a little to allow the reply to be processed
			System.Threading.Thread.Sleep(2000);


			// The test comm should have replied with a sucess AckNak. The AckNak
			// should have been processed by the BrilliantStudentAgent and set its
			// own AgentInfo property the the one the server supplied. This will
			// indicate a sucessful join game.
			Assert.True(bsAgent.AgentInfo.CommonAgentInfo != null);


			// Stopping the test comm from listening for messages
			testComm.stopListening();
			System.Threading.Thread.Sleep(1000);


			// Creating a simple bool to be used for testing the timeout functionality
			// of the join game strategy
			bool hasTimedOut = false;


			// Invoking the join game strategy again, this time I've supplied a
			// callback for the timeout. If the join game stategy fails to join
			// due to timeout, hasTimedOut will be set to true. Hopefully this
			// happens because the test comm is no longer listening for messages.
			bsAgent.JoinGameServer(null, () => { hasTimedOut = true; });


			// Need to wait for the timeout to occur.
			System.Threading.Thread.Sleep(4000);


			// Testing whether the timeout happened.
			Assert.True(hasTimedOut);

		}
			
	}
}

