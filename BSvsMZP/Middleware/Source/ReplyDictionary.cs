﻿using System;
using System.Collections.Generic;

namespace Middleware
{
	public class ReplyDictionary
	{
		public Dictionary<string, Action<Envelope>> Strategies;
		private ResponderStrategies strats;
		MessageQueue msgQueue;
		Communicator comm;
		AgentInfo agentInfo;

		public ReplyDictionary(Communicator comm, MessageQueue msgQueue, AgentInfo agentInfo, Agent agent)
		{
			this.msgQueue = msgQueue;
			this.comm = comm;
			this.agentInfo = agentInfo;
			strats = new ResponderStrategies (comm, msgQueue, agentInfo, agent);

			Strategies = new Dictionary<string, Action<Envelope>>();


			Strategies.Add("Excuse", delegate(Envelope envelope){
				strats.sendExcuse(envelope);
			});


			Strategies.Add("WhiningTwine", delegate(Envelope envelope){
				strats.sendWhiningTwine(envelope);
			});


			Strategies.Add("Tick", delegate(Envelope envelope){
				strats.ReceiveTick(envelope);
			});

		}
	}
}
