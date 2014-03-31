// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;
using System.CodeDom.Compiler;

namespace ExcuseAgent
{
	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSButton btnJoinGame { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnRefresh { get; set; }

		[Outlet]
		MonoMac.AppKit.NSComboBox gamesComboBox { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblAgentHealth { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblAgentID { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblAgentLocation { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblAgentSpeed { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblAgentStatus { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblAgentStrength { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblGameID { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblGameStatus { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblNumOfExcuses { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblNumOfTicks { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnJoinGame != null) {
				btnJoinGame.Dispose ();
				btnJoinGame = null;
			}

			if (btnRefresh != null) {
				btnRefresh.Dispose ();
				btnRefresh = null;
			}

			if (gamesComboBox != null) {
				gamesComboBox.Dispose ();
				gamesComboBox = null;
			}

			if (lblAgentHealth != null) {
				lblAgentHealth.Dispose ();
				lblAgentHealth = null;
			}

			if (lblAgentID != null) {
				lblAgentID.Dispose ();
				lblAgentID = null;
			}

			if (lblAgentLocation != null) {
				lblAgentLocation.Dispose ();
				lblAgentLocation = null;
			}

			if (lblAgentSpeed != null) {
				lblAgentSpeed.Dispose ();
				lblAgentSpeed = null;
			}

			if (lblAgentStatus != null) {
				lblAgentStatus.Dispose ();
				lblAgentStatus = null;
			}

			if (lblAgentStrength != null) {
				lblAgentStrength.Dispose ();
				lblAgentStrength = null;
			}

			if (lblGameID != null) {
				lblGameID.Dispose ();
				lblGameID = null;
			}

			if (lblGameStatus != null) {
				lblGameStatus.Dispose ();
				lblGameStatus = null;
			}

			if (lblNumOfExcuses != null) {
				lblNumOfExcuses.Dispose ();
				lblNumOfExcuses = null;
			}

			if (lblNumOfTicks != null) {
				lblNumOfTicks.Dispose ();
				lblNumOfTicks = null;
			}
		}
	}

	[Register ("MainWindow")]
	partial class MainWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
