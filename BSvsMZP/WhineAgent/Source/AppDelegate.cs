using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

namespace WhineAgent
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		MainWindowController mainWindowController;
		WhineAgent wAgent = new WhineAgent();

		public AppDelegate()
		{
		}

		public override void FinishedLaunching (NSObject notification)
		{
			mainWindowController = new MainWindowController ();
			mainWindowController.wAgent = wAgent;
			mainWindowController.Window.MakeKeyAndOrderFront(this);
		}
	}
}

