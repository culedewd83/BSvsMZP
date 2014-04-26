using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

namespace ExcuseAgent
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		MainWindowController mainWindowController;
		ExcuseAgent eAgent = new ExcuseAgent();

		public AppDelegate()
		{
		}

		public override void FinishedLaunching (NSObject notification)
		{
			eAgent.eAgent.GetProcessID();
			mainWindowController = new MainWindowController ();
			mainWindowController.eAgent = eAgent;
			mainWindowController.Window.MakeKeyAndOrderFront(this);
		}
	}
}

