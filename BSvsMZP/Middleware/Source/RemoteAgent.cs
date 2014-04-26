using System;
using Common;

namespace Middleware
{
	public class RemoteAgent
	{
		public Common.AgentInfo agent { get; set; }
		public int successfulReplies { get; set; }
		public bool currentlyCommunicatingWith { get; set; }

		public RemoteAgent(Common.AgentInfo agent)
		{
			this.agent = agent;
			successfulReplies = 0;
			currentlyCommunicatingWith = false;
		}

		public RemoteAgent(Common.AgentInfo agent, int replies, bool ccw)
		{
			this.agent = agent;
			successfulReplies = replies;
			currentlyCommunicatingWith = ccw;
		}
	}
}

