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
	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSTextField remoteAddressTF { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton setAddressButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (remoteAddressTF != null) {
				remoteAddressTF.Dispose ();
				remoteAddressTF = null;
			}

			if (setAddressButton != null) {
				setAddressButton.Dispose ();
				setAddressButton = null;
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
