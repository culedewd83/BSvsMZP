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
			testComm.stopListening();


			//Common.EndPoint ep = new Common.EndPoint(
			//int testCommPort = testComm.getPort();



			BrilliantStudentAgent.BrilliantStudentAgent bsAgent = new BrilliantStudentAgent.BrilliantStudentAgent ();
			bsAgent.bsAgent.
		}
			
	}
}

