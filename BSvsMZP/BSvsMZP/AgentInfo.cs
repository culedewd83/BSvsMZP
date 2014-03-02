using System;

namespace BSvsMZP
{
	public class AgentInfo
	{
		static readonly AgentInfo _instance = new AgentInfo();
		private Object thisLock;
		short convoNum;
		public short processId { get; set; }
		public short gameID { get; set; }
		public string remoteServerAddress { get; set; }
		public int remoteServerPort { get; set; }


		public static AgentInfo Instance
		{
			get
			{
				return _instance;
			}
		}

		AgentInfo()
		{
			thisLock = new Object();
			convoNum = 1;
			processId = 123;
			gameID = 555;
		}


		public short getConvoNum() {
			lock (thisLock) {
				convoNum = (short)(((int)convoNum % (int)short.MaxValue) + 1);
				return convoNum;
			}
		}
	}
}

