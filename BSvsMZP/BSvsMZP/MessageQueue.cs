using System;
using System.Collections.Generic;

namespace BSvsMZP
{
	public class MessageQueue
	{
		public MessagesLists newConvMessages;
		public Dictionary<string, MessagesLists> convInProgressMessages; 


		public MessageQueue()
		{
			newConvMessages = new MessagesLists();
			convInProgressMessages = new Dictionary<string, MessagesLists>();
		}
	}
}

