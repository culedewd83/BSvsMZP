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


			Communicator comm = new Communicator ();
			comm.startListening();
			comm.changePort();
			comm.startListening();



			Common.EndPoint localEP = new Common.EndPoint ();
			localEP.Address = BitConverter.ToInt32(IPAddress.Parse("127.0.0.1").GetAddressBytes(), 0);
			localEP.Port = comm.getIncommingPort();

			MessageQueue msgQue = new MessageQueue ();
			Listener listener = new Listener(comm, msgQue);
			listener.startListening();


			JoinGame joinGame = new JoinGame();
			joinGame.ANumber = "A01537812";
			ByteList bytes = new ByteList();
			joinGame.Encode(bytes);



			comm.sendMessage(bytes.ToBytes(), localEP, joinGame.ConversationId);


			//System.Threading.Thread.Sleep(2000);



			for (int i = 0; i < 10; ++i) {

				comm.sendMessage(bytes.ToBytes(), joinGame.ConversationId);
			}

			System.Threading.Thread.Sleep(2000);

		}


	}
}

