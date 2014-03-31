// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;
using System.CodeDom.Compiler;

namespace BSvsMZP
{
	[Register ("SampleWindowController")]
	partial class SampleWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSButton btnPushMe { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnPushMe != null) {
				btnPushMe.Dispose ();
				btnPushMe = null;
			}
		}
	}

	[Register ("SampleWindow")]
	partial class SampleWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
