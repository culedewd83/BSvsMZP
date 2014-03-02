using System;
using System.Collections.Generic;

namespace BSvsMZP
{
	public class ExcuseReplyDictionary
	{
		public Dictionary<string, Action<Envelope>> Strategies;
		private ResponderStrategies strats;
		MessageQueue msgQueue;
		Communicator comm;

		public ExcuseReplyDictionary(Communicator comm, MessageQueue msgQueue)
		{
			this.msgQueue = msgQueue;
			this.comm = comm;
			strats = new ResponderStrategies (comm, msgQueue);

			Strategies = new Dictionary<string, Action<Envelope>>();


			Strategies.Add("Excuse", delegate(Envelope envelope){
				strats.sendExcuse(envelope);
			});


		}
	}
}

