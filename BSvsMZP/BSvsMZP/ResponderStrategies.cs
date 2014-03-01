using System;

namespace BSvsMZP
{
	public class ResponderStrategies
	{

		MessageQueue msgQueue;
		Communicator comm;


		public ResponderStrategies(Communicator comm, MessageQueue msgQueue)
		{
			this.msgQueue = msgQueue;
			this.comm = comm;
		}

		/*
		public void giveExcuse (Messages.GetResource msg) {
			System.Threading.Thread thread = new System.Threading.Thread(delegate(){

				Common.Excuse excuse = new Common.Excuse();
				Common.DistributableObject distObj = new Common.DistributableObject();
				Messages.AckNak ackNak = new Messages.AckNak(Messages.Reply.PossibleStatus.Success, 


			});

			thread.Start();
		}
		*/


	}
}

