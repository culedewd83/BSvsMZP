using System;

namespace Middleware
{
	public class AgentInfo
	{
		private Object thisLock = new Object();

		private short _convoNum;
		public short convoNum {
			get { lock (this.thisLock) {
					this._convoNum = (short)(((int)this._convoNum % (int)short.MaxValue) + 1);
					return this._convoNum; }}
			set { lock (this.thisLock) { this._convoNum = value; }}
		}

		private short _processId;
		public short processId {
			get { lock (this.thisLock) { return this._processId; }}
			set { lock (this.thisLock) { this._processId = value; }}
		}

		private short _gameID;
		public short gameID {
			get { lock (this.thisLock) { return this._gameID; }}
			set { lock (this.thisLock) { this._gameID = value; }}
		}

		private string _remoteServerAddress;
		public string remoteServerAddress {
			get { lock (this.thisLock) { return this._remoteServerAddress; }}
			set { lock (this.thisLock) { this._remoteServerAddress = value; }}
		}

		private int _remoteServerPort;
		public int remoteServerPort {
			get { lock (this.thisLock) { return this._remoteServerPort; }}
			set { lock (this.thisLock) { this._remoteServerPort = value; }}
		}

		private Common.EndPoint _remoteServerEndPoint;
		public Common.EndPoint remoteServerEndPoint {
			get { lock (this.thisLock) { return this._remoteServerEndPoint; }}
			set { lock (this.thisLock) { this._remoteServerEndPoint = value; }}
		}

		public string gameServerLabel { get; set; }

		public string status { get; set; }

		public Common.AgentInfo.PossibleAgentType agentType { get; set; }

		public Common.AgentInfo CommonAgentInfo { get; set; }

		public string gameStatus { get; set; }

		public AgentInfo()
		{
			convoNum = 0;
			processId = 0;
			gameID = 0;
			remoteServerAddress = "";
			remoteServerPort = 0;
			remoteServerEndPoint = new Common.EndPoint ();
			gameServerLabel = "No Game Server Yet";
			status = "Inactive";
			gameStatus = "Not Joined";

		}

		public Common.MessageNumber getNewConvoNum () {
			Common.MessageNumber msgNum = Common.MessageNumber.Create();
			msgNum.ProcessId = processId;
			msgNum.SeqNumber = convoNum;
			return msgNum;
		}
	}
}

