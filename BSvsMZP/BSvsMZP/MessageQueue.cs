using System;
using System.Collections.Generic;

namespace BSvsMZP
{
	public class MessageQueue
	{
		public List<Envelope> newConvoQueue;
		public Dictionary<string, List<Envelope>> convoInProgressQueue;

		public MessageQueue()
		{
			newConvoQueue = new List<Envelope>();
			convoInProgressQueue = new Dictionary<string, List<Envelope>>();
		}
	}
}

