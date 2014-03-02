using System;
using System.Collections.Generic;

namespace BSvsMZP
{
	public class MessageQueue
	{
		public Queue<Envelope> newConvoQueue;
		public Dictionary<string, Queue<Envelope>> convoInProgressQueue;

		public MessageQueue()
		{
			newConvoQueue = new Queue<Envelope>();
			convoInProgressQueue = new Dictionary<string, Queue<Envelope>>();
		}
	}
}

