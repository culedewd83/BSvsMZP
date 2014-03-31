using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using Middleware;

namespace WhineAgent
{
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController
	{

		public WhineAgent wAgent { get; set; }
		bool hasJoinedGame = false;
		Dictionary<string, SimpleServerInfo> servers;
		System.Threading.Thread updateWindowThread;

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

			FillGameServerComboBox();

			btnRefresh.Activated += (object sender, EventArgs e) => {
				RefeshButtonPressed();
			};

			btnJoinGame.Activated += (object sender, EventArgs e) => {
				JoinButtonPressed();
			};

			updateWindowThread = new System.Threading.Thread(delegate(){
				while(true) {
					UpdateWindowInformation();
					System.Threading.Thread.Sleep(100);
				}
			});
			updateWindowThread.Start();
		}

		private void FillGameServerComboBox()
		{
			servers = Middleware.GameServers.GetAvailableServers();

			gamesComboBox.UsesDataSource = true;
			gamesComboBox.Completes = true;
			gamesComboBox.RefusesFirstResponder = true;
			gamesComboBox.DataSource = new GamesComboDataSource (servers);
			gamesComboBox.SelectItem(0);

			if (servers.Count == 0) {
				btnJoinGame.Enabled = false;
			}
		}

		private void RefeshButtonPressed()
		{
			servers = Middleware.GameServers.GetAvailableServers();

			gamesComboBox.DataSource = new GamesComboDataSource (servers);
			gamesComboBox.ReloadData();
			gamesComboBox.SelectItem(0);

			if (servers.Count == 0) {
				btnJoinGame.Enabled = false;
			} else {
				btnJoinGame.Enabled = true;
			}

			Console.WriteLine("Servers Refreshed");
		}


		private void JoinButtonPressed()
		{
			wAgent.SetGameServer(servers [gamesComboBox.StringValue]);
			wAgent.StartAgent();
			wAgent.JoinGameServer((errorMsg) => { ErrorAlert(errorMsg); }, () => { TimeoutAlert(); });
			btnJoinGame.Enabled = false;
			btnRefresh.Enabled = false;
			gamesComboBox.Enabled = false;
		}

		private void UpdateWindowInformation()
		{
			InvokeOnMainThread(() => {
				lblAgentStatus.StringValue = wAgent.AgentInfo.status;
				lblAgentID.StringValue = "" + wAgent.AgentInfo.processId;
				lblGameStatus.StringValue = wAgent.AgentInfo.gameStatus;
				lblGameID.StringValue = "" + wAgent.AgentInfo.gameID;

				if (wAgent.AgentInfo.CommonAgentInfo != null) {
					lblAgentLocation.StringValue = "" + wAgent.AgentInfo.CommonAgentInfo.Location.X + ", " + wAgent.AgentInfo.CommonAgentInfo.Location.Y;
					lblAgentHealth.StringValue = "" + wAgent.AgentInfo.CommonAgentInfo.Points;
					lblAgentSpeed.StringValue = "" + wAgent.AgentInfo.CommonAgentInfo.Speed;
					lblAgentStrength.StringValue = "" + wAgent.AgentInfo.CommonAgentInfo.Strength;
					lblNumOfTicks.StringValue = "" + wAgent.NumOfTicks;
					lblNumOfTwine.StringValue = "" + wAgent.NumOfTwine;
				} else {
					lblAgentLocation.StringValue = "N/A";
					lblAgentHealth.StringValue = "N/A";
					lblAgentSpeed.StringValue = "N/A";
					lblAgentStrength.StringValue = "N/A";
					lblNumOfTicks.StringValue = "N/A";
					lblNumOfTwine.StringValue = "N/A";
				}

			});
		}

		private void TimeoutAlert ()
		{
			InvokeOnMainThread(() => {
				var alert = new NSAlert {
					MessageText = "Join game timed out =(",
					AlertStyle = NSAlertStyle.Informational
				};
				alert.AddButton("OK");
				var returnValue = alert.RunModal();

				RefeshButtonPressed();
				gamesComboBox.Enabled = true;
				btnRefresh.Enabled = true;

			});
		}

		private void ErrorAlert (string errorMsg)
		{
			InvokeOnMainThread(() => {
				var alert = new NSAlert {
					MessageText = "Failed to Join Game: " + errorMsg,
					AlertStyle = NSAlertStyle.Informational
				};
				alert.AddButton("OK");
				var returnValue = alert.RunModal();


				RefeshButtonPressed();
				gamesComboBox.Enabled = true;
				btnRefresh.Enabled = true;

			});
		}
	}
}

