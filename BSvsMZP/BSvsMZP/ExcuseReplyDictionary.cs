using System;
using System.Collections.Generic;

namespace BSvsMZP
{
	public class ReplyDictionary
	{
		public Dictionary<string, Action<Envelope>> Strategies;
		private ResponderStrategies strats;
		MessageQueue msgQueue;
		Communicator comm;
		AgentInfo agentInfo;

		public ReplyDictionary(Communicator comm, MessageQueue msgQueue, AgentInfo agentInfo)
		{
			this.msgQueue = msgQueue;
			this.comm = comm;
			this.agentInfo = agentInfo;
			strats = new ResponderStrategies (comm, msgQueue, agentInfo);

			Strategies = new Dictionary<string, Action<Envelope>>();


			Strategies.Add("Excuse", delegate(Envelope envelope){
				strats.sendExcuse(envelope);
			});


			Strategies.Add("WhiningTwine", delegate(Envelope envelope){
				strats.sendWhiningTwine(envelope);
			});


		}
	}
}

