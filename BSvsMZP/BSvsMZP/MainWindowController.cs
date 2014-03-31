using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using Messages;
using Common;
using System.Net;
using System.Collections;
using System.ServiceModel;

namespace BSvsMZP
{
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController
	{

		ExcuseFactory eFactory;
		bool eFactoryShouldListen = false;

		WhineFactory wFactory;
		bool wFactoryShouldListen = false;

		BrilliantStudent student;
		bool studentShouldListen = false;

		SampleWindowController sample;


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

			RegistrarAlt.RegistrarAlt registrar = new BSvsMZP.RegistrarAlt.RegistrarAlt ("http://bsvszp.azurewebsites.net/RegistrarAlt.asmx");
			//RegistrarAlt.RegistrarAlt registrar = new BSvsMZP.RegistrarAlt.RegistrarAlt ("http://cs5200web.serv.usu.edu/RegistrarAlt.asmx");
			var gamesAvail = registrar.GetGames(BSvsMZP.RegistrarAlt.GameStatus.AVAILABLE);
			//var gamesAvail = registrar.GetGamesAlt(BSvsMZP.RegistrarAlt.GameStatus.AVAILABLE);

			eFactory = new ExcuseFactory ();

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



			wFactory = new WhineFactory ();

			whineRemoteAddressButton.Activated += (object sender, EventArgs e) => {
				int oldPort = wFactory.agentInfo.remoteServerPort;
				int newPort = 0;
				int.TryParse(whineRemotePort.StringValue, out newPort);
				wFactory.setRemoteEndPoint(whineRemoteAddress.StringValue, newPort);
			};
			whineRemotePortButton.Activated += (object sender, EventArgs e) => {
				int oldPort = wFactory.agentInfo.remoteServerPort;
				int newPort = 0;
				int.TryParse(whineRemotePort.StringValue, out newPort);
				wFactory.setRemoteEndPoint(whineRemoteAddress.StringValue, newPort);

			};
			whineListenButton.Activated += (object sender, EventArgs e) => {
				if(!wFactoryShouldListen) {
					wFactory.startListening();
					whineListenButton.Title = "Stop Listening";
					wFactoryShouldListen = true;
				} else {
					wFactory.stopListening();
					whineListenButton.Title = "Start Listening";
					wFactoryShouldListen = false;
				}
			};





			student = new BrilliantStudent ();

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
				if(studentMessageBox.StringValue.Equals("GetExcuse")) {
					student.getExcuse(student.agentInfo.remoteServerEndPoint);
				} else if(studentMessageBox.StringValue.Equals("GetWhiningTwine")) {
					student.getWhiningTwine(student.agentInfo.remoteServerEndPoint);
				}
			};


			excuseRemoteAddress.StringValue = eFactory.agentInfo.remoteServerAddress;
			excuseRemotePort.StringValue = "" + eFactory.agentInfo.remoteServerPort;

			whineRemoteAddress.StringValue = wFactory.agentInfo.remoteServerAddress;
			whineRemotePort.StringValue = "" + wFactory.agentInfo.remoteServerPort;

			studentRemoteAddress.StringValue = student.agentInfo.remoteServerAddress;
			studentRemotePort.StringValue = "" + student.agentInfo.remoteServerPort;

			studentMessageBox.UsesDataSource = true;
			studentMessageBox.Completes = true;
			studentMessageBox.RefusesFirstResponder = true;
			studentMessageBox.DataSource = new StudentDataSource ();
			studentMessageBox.SelectItem(0);




			System.Threading.Thread updateWindowThread = new System.Threading.Thread(delegate(){
				while(true) {
					updateMainWindow();
					System.Threading.Thread.Sleep(100);
				}
			});

			updateWindowThread.Start();



			sample = new SampleWindowController ();
			//sample.ShowWindow(this);
			//sample.Window.OrderFront(this);
			//this.Window.CanHide = true;
			//this.Window.IsVisible = false;
		}



		public void receiveExcuse(Common.Excuse excuse) {
			Console.WriteLine("received an excuse");
		}


		public void updateMainWindow() {
			InvokeOnMainThread(() => {

				excuseListenPortlbl.StringValue = "Listening on port: " + eFactory.comm.getPort();
				whineListeningPortlbl.StringValue = "Listening on port: " + wFactory.comm.getPort();
				studentListenPortlbl.StringValue = "Listening on port: " + student.comm.getPort();

				excuseMessagesMovedlbl.StringValue = "Messages moved: " + eFactory.MessagesMovedToQueue;
				whineMessagesMovedlbl.StringValue = "Messages moved: " + wFactory.MessagesMovedToQueue;
				studentMessagesMovedlbl.StringValue = "Messages moved: " + student.MessagesMovedToQueue;
			});
		}



		class StudentDataSource : NSComboBoxDataSource
		{
			List<string> messages = new List<string> {
				"GetExcuse",
				"GetWhiningTwine"
			};


			public override string CompletedString (NSComboBox comboBox, string uncompletedString)
			{
				return messages.Find (n => n.StartsWith (uncompletedString, StringComparison.InvariantCultureIgnoreCase));
			}

			public override int IndexOfItem (NSComboBox comboBox, string value)
			{
				return messages.FindIndex (n => n.Equals (value, StringComparison.InvariantCultureIgnoreCase));
			}

			public override int ItemCount (NSComboBox comboBox)
			{
				return messages.Count;
			}

			public override NSObject ObjectValueForItem (NSComboBox comboBox, int index)
			{
				return NSObject.FromObject (messages [index]);
			}


		}

	}
}

