using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using Middleware;

namespace BrilliantStudentAgent
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		MainWindowController mainWindowController;
		BrilliantStudentAgent bsAgent = new BrilliantStudentAgent();

		public AppDelegate()
		{
		}

		public override void FinishedLaunching (NSObject notification)
		{
			mainWindowController = new MainWindowController ();
			mainWindowController.bsAgent = bsAgent;
			mainWindowController.Window.MakeKeyAndOrderFront(this);
		}
	}
}

