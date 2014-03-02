using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using Messages;
using Common;
using System.Net;
using System.Collections;

namespace BSvsMZP
{
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController
	{

		#region Constructors

		// Called when created from unmanaged code
		public MainWindowController(IntPtr handle) : base(handle)
		{
			Initialize();
		}
		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public MainWindowController(NSCoder coder) : base(coder)
		{
			Initialize();
		}
		// Call to load from the XIB/NIB file
		public MainWindowController() : base("MainWindow")
		{
			Initialize();
		}
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new MainWindow Window {
			get {
				return (MainWindow)base.Window;
			}
		}


		public override void AwakeFromNib ()
		{
			base.AwakeFromNib();


			Communicator comm = new Communicator();
			comm.startListening();

			Communicator comm2 = new Communicator();
			comm2.startListening();

			AgentInfo agentInfo = AgentInfo.Instance;
			agentInfo.processId = 4445;

			Common.EndPoint localEP = new Common.EndPoint ();
			localEP.Address = BitConverter.ToInt32(IPAddress.Parse("127.0.0.1").GetAddressBytes(), 0);
			localEP.Port = comm.getPort();

			Common.EndPoint localEP2 = new Common.EndPoint ();
			localEP2.Address = BitConverter.ToInt32(IPAddress.Parse("127.0.0.1").GetAddressBytes(), 0);
			localEP2.Port = comm2.getPort();


			MessageQueue msgQue = new MessageQueue();
			Listener listener = new Listener(comm, msgQue);
			listener.startListening();

			MessageQueue msgQue2 = new MessageQueue();
			Listener listener2 = new Listener(comm2, msgQue2);
			listener2.startListening();



			ExcuseDoer doer = new ExcuseDoer (comm2, msgQue2);
			doer.startListening();


			InstigatorStrategies iStrats = new InstigatorStrategies (comm, msgQue);

			Messages.GetResource getExcuseMsg = new Messages.GetResource (agentInfo.gameID, GetResource.PossibleResourceType.Excuse, new Common.Tick ());

			getExcuseMsg.ConversationId = Common.MessageNumber.Create(); 

			getExcuseMsg.ConversationId.ProcessId = agentInfo.processId;
			getExcuseMsg.ConversationId.SeqNumber = agentInfo.getConvoNum();


			getExcuseMsg.MessageNr.ProcessId = agentInfo.processId;
			getExcuseMsg.MessageNr.SeqNumber = 1;

			Envelope getExcuseEnv = new Envelope (getExcuseMsg, localEP2);

			iStrats.getExcuse(getExcuseEnv, (excuse) => {
				receiveExcuse(excuse);
			});
				



			System.Threading.Thread.Sleep(5000);


		}



		public void receiveExcuse(Common.Excuse excuse) {
			Console.WriteLine("received an excuse");
		}

	}
}

