using System;

namespace Middleware
{
	public class Envelope
	{
		public Messages.Message message;
		public Common.EndPoint endPoint;

		public Envelope(Messages.Message msg, Common.EndPoint ep)
		{
			message = msg;
			endPoint = ep;
		}
	}
}

