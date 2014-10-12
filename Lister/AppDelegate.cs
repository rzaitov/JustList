using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

using Common;
using ListerKit;
using SQLite;
using System.IO;

namespace Lister
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate, IUISplitViewControllerDelegate
	{
		const string DbFileName = "JustList.sqlite3";

		string DbFilePath {
			get {
				string[] dirs = NSSearchPath.GetDirectories (NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User);
				string documentsDir = dirs [0];

				var path = Path.Combine (documentsDir, DbFileName);
				return path;
			}
		}

		public override UIWindow Window { get; set; }

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			Console.WriteLine (IntPtr.Size);
			Console.WriteLine ("FinishedLaunching");

			InitListService ();

			return true;
		}

		void InitListService()
		{
//			ServiceLocator.ListService = new MockListService ();

			Console.WriteLine (DbFilePath);

			var service = new ListService (new SQLiteConnection(DbFilePath, true));

			var isFirstRun = !App.RunBefore;
			if (isFirstRun) {
				service.InitStorage ();
				App.SetRunBeforeToTrue ();
			}

			ServiceLocator.ListService = service;
		}

		/// <summary>
		/// Don't call this on production.
		/// This is convinient method for dropping application.
		/// </summary>
		void MakeAsFirstRun()
		{
			App.DropUserDefaults ();

			var fileManager = NSFileManager.DefaultManager;
			bool isDbExists = fileManager.FileExists (DbFileName);
			if (isDbExists) {
				NSError error;
				fileManager.Remove (DbFileName, out error);
			}
		}
	}
}