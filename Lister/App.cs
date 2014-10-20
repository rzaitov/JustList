using System;

using Foundation;

namespace JustList
{
	public static class App
	{
		const string RunBeforeKey = "RunBefore";

		public static bool RunBefore {
			get {
				return false;

				NSUserDefaults defaults = NSUserDefaults.StandardUserDefaults;
				var runBefore = defaults.BoolForKey (RunBeforeKey);
				return runBefore;
			}
		}

		public static void SetRunBeforeToTrue()
		{
			NSUserDefaults.StandardUserDefaults.SetBool (true, RunBeforeKey);
		}

		public static void DropUserDefaults()
		{
			var domainName = NSBundle.MainBundle.BundleIdentifier;
			NSUserDefaults.StandardUserDefaults.RemovePersistentDomain (domainName);
		}
	}
}