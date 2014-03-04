using NUnit.Framework;
using System;
using BSvsMZP;
using System.Collections.Generic;

namespace UnitTesting
{
	[TestFixture()]
	public class Test
	{


		[Test()]
		public void Communicator ()
		{

			// This is the communicator that will be tested for receiving messages
			BSvsMZP.Communicator comm1 = new BSvsMZP.Communicator();

			// Comm should have allocated
			Assert.True(comm1 != null);

			// UDP Client in comm1 should have been allocated
			Assert.True(comm1.udpServer != null);

			// shouldListen should have been init to false
			Assert.True(comm1.shouldListen == false);

			// the message buffer in comm1 should have been allocated
			Assert.True(comm1.envelopeBuffer != null);

			// Getting the port number comm1 is listening on
			int port = comm1.getPort();

			// The port should be greater than 0;
			Assert.True(port > 0);

			// The port should be less than or equal to 65535
			Assert.True(port <= 65535);

			// This is the communicator that will send messages to the test communicator
			BSvsMZP.Communicator comm2 = new BSvsMZP.Communicator ();

			// Current number of messages in comm1's buffer should be zero
			Assert.True(comm1.envelopeBuffer.Count == 0);

			// Telling comm1 to start listening
			comm1.startListening();

			// comm1 shouldListen should now be true
			Assert.True(comm1.shouldListen);

			// comm1 listerthread should now be running
			Assert.True(comm1.listenerThread.IsAlive);

			// Making an endpoint
			Common.EndPoint ep = new Common.EndPoint ("127.0.0.1", port);

			// Making a message
			Messages.GetResource msg = new Messages.GetResource ();

			// Making an evelope
			BSvsMZP.Envelope env = new BSvsMZP.Envelope (msg, ep);

			// Sending the message from comm2 to comm1 once
			comm2.sendEnvelope(env);

			// Waiting a little to ensure the message has been processed
			System.Threading.Thread.Sleep(500);

			// comm1 should now have 1 message in its message buffer
			Assert.True(comm1.envelopeBuffer.Count == 1);

			// getting the buffer
			List<BSvsMZP.Envelope> envList = comm1.getEnvelopes();

			// The list should not be null
			Assert.True(envList != null);

			// The list should have one evelope
			Assert.True(envList.Count == 1);

			// comm1 should now have an empty buffer
			Assert.True(comm1.envelopeBuffer.Count == 0);

			// Now sending 100 messages from comm2 to comm1
			for (int i = 0; i < 100; ++i) {
				comm2.sendEnvelope(env);
			}

			// Waiting a little to let comm1 processes all the messages
			System.Threading.Thread.Sleep(3000);

			// comm1 should now have 100 messages in its message buffer
			Assert.True(comm1.envelopeBuffer.Count == 100);

			// getting the buffer
			envList = comm1.getEnvelopes();

			// The list should not be null
			Assert.True(envList != null);

			// The list should have 100 evelopes
			Assert.True(envList.Count == 100);

			// comm1 should now have an empty buffer
			Assert.True(comm1.envelopeBuffer.Count == 0);

			// stopping the comm1 listener thread
			comm1.stopListening();

			// comm1 shouldListen should now be false
			Assert.True(comm1.shouldListen == false);

			// Waiting a little bit to allow the thread to stop
			System.Threading.Thread.Sleep(1000);

			// comm1 thread should now have stopped
			Assert.True(comm1.listenerThread.IsAlive == false);

		}



		[Test()]
		public void Listener ()
		{
			// This is the communicator that will be tested for receiving messages
			BSvsMZP.Communicator comm1 = new BSvsMZP.Communicator();

			// Telling comm1 to start listening
			comm1.startListening();

			// Getting the port number comm1 is listening on
			int port = comm1.getPort();

			// This is the communicator that will send messages to the test communicator
			BSvsMZP.Communicator comm2 = new BSvsMZP.Communicator ();

			// This is the messagequeue that will be used during testing
			BSvsMZP.MessageQueue msgQueue = new BSvsMZP.MessageQueue ();

			// This is the listener that will be tested
			BSvsMZP.Listener listener = new BSvsMZP.Listener (comm1, msgQueue);

			// Listener should have allocated
			Assert.True(listener != null);

			// The comm in listener should have been set
			Assert.True(listener.comm != null);

			// The msgQueue in listener should have been set
			Assert.True(listener.msgQueue != null);

			// The listener thread should still be null
			Assert.True(listener.listenerThread == null);

			// Should listen should be false
			Assert.True(listener.shouldListen == false);

			// messages moved by listener should be zero
			Assert.True(listener.totalMessagesMoved == 0);

			// Telling Listener to start listening
			listener.startListening();

			// Listener shouldListen should now be true
			Assert.True(listener.shouldListen);

			// Listener listerthread should now be running
			Assert.True(listener.listenerThread.IsAlive);

			// Making an endpoint
			Common.EndPoint ep = new Common.EndPoint ("127.0.0.1", port);

			// Making a message
			Messages.GetResource msg = new Messages.GetResource ();

			// Making an evelope
			BSvsMZP.Envelope env = new BSvsMZP.Envelope (msg, ep);

			// Sending the message from comm2 to comm1 once
			comm2.sendEnvelope(env);

			// Waiting a little to ensure the message has been processed
			System.Threading.Thread.Sleep(500);

			// Listener should now have 1 message moved from comm to msgqueue
			Assert.True(listener.totalMessagesMoved == 1);

			// Now sending 100 messages from comm2 to comm1
			for (int i = 0; i < 100; ++i) {
				comm2.sendEnvelope(env);
			}

			// Waiting a little to let comm1 and listener processes all the messages
			System.Threading.Thread.Sleep(3000);

			// Listener should now have 101 message moved from comm to msgqueue
			Assert.True(listener.totalMessagesMoved == 101);

			// stopping the comm1 listener thread
			comm1.stopListening();

			// stopping the listener thread
			listener.stopListening();

			// listener shouldListen should now be false
			Assert.True(listener.shouldListen == false);

			// Waiting a little bit to allow the thread to stop
			System.Threading.Thread.Sleep(1000);

			// listener thread should now have stopped
			Assert.True(listener.listenerThread.IsAlive == false);

		}



		[Test()]
		public void MessageQueque ()
		{

			// This is the messagequeue that will be tested during this unit test
			BSvsMZP.MessageQueue msgQueue = new BSvsMZP.MessageQueue ();

			// This is the communicator that will be needed to test the messageQueue
			BSvsMZP.Communicator comm1 = new BSvsMZP.Communicator();

			// Telling comm1 to start listening
			comm1.startListening();

			// Getting the port number comm1 is listening on
			int port = comm1.getPort();

			// This is the communicator that will send messages to the test communicator
			BSvsMZP.Communicator comm2 = new BSvsMZP.Communicator ();

			// This is the listener that will be tested
			BSvsMZP.Listener listener = new BSvsMZP.Listener (comm1, msgQueue);

			// Telling Listener to start listening
			listener.startListening();

			// Listener shouldListen should now be true
			Assert.True(listener.shouldListen);

			// Listener listerthread should now be running
			Assert.True(listener.listenerThread.IsAlive);

			// Making an endpoint
			Common.EndPoint ep = new Common.EndPoint ("127.0.0.1", port);

			// Making a message
			Messages.GetResource msg = new Messages.GetResource ();

			// Making an evelope
			BSvsMZP.Envelope env = new BSvsMZP.Envelope (msg, ep);

			// Sending the message from comm2 to comm1 once
			comm2.sendEnvelope(env);

			// Waiting a little to ensure the message has been processed
			System.Threading.Thread.Sleep(500);

			// The msgQueue should now have one message in queue
			Assert.True(msgQueue.newConvoQueue.Count == 1);

			// Creating a convoKey acording to the way I've implemented the keys for convos in progress
			string convoKey = "" + msg.ConversationId.ProcessId + "," + msg.ConversationId.SeqNumber;

			// Adding a convo key to msgQueue convo in progress
			msgQueue.convoInProgressQueue.Add(convoKey, new Queue<BSvsMZP.Envelope>());

			// msgQueue should now have one convo in its convo in progress queue
			Assert.True(msgQueue.convoInProgressQueue.Count == 1);

			// msgQueue should have zero messsges in the one convo
			Assert.True(msgQueue.convoInProgressQueue[convoKey].Count == 0);

			// Now sending 100 messages from comm2 to comm1
			for (int i = 0; i < 100; ++i) {
				comm2.sendEnvelope(env);
			}

			// Waiting a little to let comm1 and listener processes all the messages
			System.Threading.Thread.Sleep(3000);

			// msgQueue should still have one new messge
			Assert.True(msgQueue.newConvoQueue.Count == 1);

			// msgQueue should still have one convo in progress
			Assert.True(msgQueue.convoInProgressQueue.Count == 1);

			// msgQueue should have 100 messges in the one convo
			Assert.True(msgQueue.convoInProgressQueue[convoKey].Count == 100);

			// Now sending 100 new messages from comm2 to comm1
			for (int i = 0; i < 100; ++i) {
				msg = new Messages.GetResource ();
				env = new BSvsMZP.Envelope (msg, ep);
				comm2.sendEnvelope(env);
			}

			// Waiting a little to let comm1 and listener processes all the messages
			System.Threading.Thread.Sleep(3000);

			// msgQueue should still have 101 new messges
			Assert.True(msgQueue.newConvoQueue.Count == 101);

			// stopping the comm1 listener thread
			comm1.stopListening();

			// stopping the listener thread
			listener.stopListening();

		}
	}
}

