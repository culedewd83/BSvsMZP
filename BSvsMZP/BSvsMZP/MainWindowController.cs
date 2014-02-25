using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using Messages;
using Common;

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


			JoinGame joinGame = new JoinGame();
			joinGame.ANumber = "A01537812";
			var type = joinGame.GetType();

			var instance = Activator.CreateInstance(type);


			Console.WriteLine("Type is: ");


			ByteList bytes = new ByteList();
			joinGame.Encode(bytes);

			JoinGame testGame = new JoinGame ();
			testGame.Decode(bytes);


			Console.WriteLine(joinGame.ANumber);
			Console.WriteLine(testGame.ANumber);


			Communicator comm = new Communicator (12355);
			comm.startListening();


			udpExample udp = new udpExample ();
			//udp.receiveMessage(12355);
			udp.sendMessage("127.0.0.1", 12355, "hi");

		}


	}
}

