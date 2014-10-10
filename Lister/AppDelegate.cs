using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

using Common;
using ListerKit;

namespace Lister
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate, IUISplitViewControllerDelegate
	{
		public override UIWindow Window { get; set; }

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			Console.WriteLine (IntPtr.Size);
			Console.WriteLine ("FinishedLaunching");

			ServiceLocator.ListService = new MockListService ();

			return true;
		}
	}
}