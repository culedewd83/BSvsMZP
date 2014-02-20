using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

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

			udpExample udp = new udpExample ();
			udp.receiveMessage(12355);
			udp.sendMessage("127.0.0.1", 12355, "hi");



		}


	}
}

