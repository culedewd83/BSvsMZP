using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Middleware
{
	public class MessageQueue
	{
		public ConcurrentQueue<Envelope> newConvoQueue;
		public ConcurrentDictionary<string, ConcurrentQueue<Envelope>> convoInProgressQueue;

		public MessageQueue()
		{
			newConvoQueue = new ConcurrentQueue<Envelope>();
			convoInProgressQueue = new ConcurrentDictionary<string, ConcurrentQueue<Envelope>>();
		}
	}
}

