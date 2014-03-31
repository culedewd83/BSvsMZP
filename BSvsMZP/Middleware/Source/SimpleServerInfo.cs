using System;

namespace Middleware
{
	public class SimpleServerInfo
	{
		public string Label { get; set; }
		public int Address { get; set; }
		public int Port { get; set; }
		public short Id { get; set; }
		public Common.EndPoint EndPoint { get; set; }
	}
}

