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

		ExcuseFactory eFactory;
		bool eFactoryShouldListen = false;

		BrilliantStudent student;
		bool studentShouldListen = false;



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


			eFactory = new ExcuseFactory ();
			excuseRemoteAddress.StringValue = eFactory.agentInfo.remoteServerAddress;
			excuseRemotePort.StringValue = "" + eFactory.agentInfo.remoteServerPort;
			excuseListenPortlbl.StringValue = "Listening on port: " + eFactory.comm.getPort();
			excuseAddressButton.Activated += (object sender, EventArgs e) => {
				int oldPort = eFactory.agentInfo.remoteServerPort;
				int newPort = 0;
				int.TryParse(excuseRemotePort.StringValue, out newPort);
				eFactory.setRemoteEndPoint(excuseRemoteAddress.StringValue, newPort);
			};
			excusePortButton.Activated += (object sender, EventArgs e) => {
				int oldPort = eFactory.agentInfo.remoteServerPort;
				int newPort = 0;
				int.TryParse(excuseRemotePort.StringValue, out newPort);
				eFactory.setRemoteEndPoint(excuseRemoteAddress.StringValue, newPort);
			};
			excuseListenButton.Activated += (object sender, EventArgs e) => {
				if(!eFactoryShouldListen) {
					eFactory.startListening();
					excuseListenButton.Title = "Stop Listening";
					eFactoryShouldListen = true;
				} else {
					eFactory.stopListening();
					excuseListenButton.Title = "Start Listening";
					eFactoryShouldListen = false;
				}
			};





			student = new BrilliantStudent ();
			studentRemoteAddress.StringValue = student.agentInfo.remoteServerAddress;
			studentRemotePort.StringValue = "" + student.agentInfo.remoteServerPort;
			studentListenPortlbl.StringValue = "Listening on port: " + student.comm.getPort();
			studentRemoteAddressButton.Activated += (object sender, EventArgs e) => {
				int oldPort = student.agentInfo.remoteServerPort;
				int newPort = 0;
				int.TryParse(studentRemotePort.StringValue, out newPort);
				student.setRemoteEndPoint(studentRemoteAddress.StringValue, newPort);
			};
			studentRemotePortButton.Activated += (object sender, EventArgs e) => {
				int oldPort = student.agentInfo.remoteServerPort;
				int newPort = 0;
				int.TryParse(studentRemotePort.StringValue, out newPort);
				student.setRemoteEndPoint(studentRemoteAddress.StringValue, newPort);

			};
			studentListenButton.Activated += (object sender, EventArgs e) => {
				if(!studentShouldListen) {
					student.startListening();
					studentListenButton.Title = "Stop Listening";
					studentShouldListen = true;
				} else {
					student.stopListening();
					studentListenButton.Title = "Start Listening";
					studentShouldListen = false;
				}
			};
			studentSendMessageButton.Activated += (object sender, EventArgs e) => {
				student.getExcuse(student.agentInfo.remoteServerEndPoint);
				Console.WriteLine("sent to port: " + student.agentInfo.remoteServerPort);
			};


			System.Threading.Thread updateWindowThread = new System.Threading.Thread(delegate(){
				while(true) {
					updateMainWindow();
					System.Threading.Thread.Sleep(100);
				}
			});

			updateWindowThread.Start();
		}



		public void receiveExcuse(Common.Excuse excuse) {
			Console.WriteLine("received an excuse");
		}


		public void updateMainWindow() {
			InvokeOnMainThread(() => {
				excuseMessagesMovedlbl.StringValue = "Messages moved: " + eFactory.MessagesMovedToQueue;
				studentMessagesMovedlbl.StringValue = "Messages moved: " + student.MessagesMovedToQueue;
			});
		}


	}
}

