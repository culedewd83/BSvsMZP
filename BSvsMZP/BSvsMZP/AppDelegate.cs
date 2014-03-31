using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

namespace BSvsMZP
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		MainWindowController mainWindowController;
		public SampleWindowController sample;

		public AppDelegate()
		{
		}

		public override void FinishedLaunching (NSObject notification)
		{
			mainWindowController = new MainWindowController ();
			//mainWindowController.ShowWindow(this);
			mainWindowController.Window.MakeKeyAndOrderFront(this);
			//sample = new SampleWindowController ();
			//sample.Window.MakeKeyAndOrderFront(this);
		}
	}
}

