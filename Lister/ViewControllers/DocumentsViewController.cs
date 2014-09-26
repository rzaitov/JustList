using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;

using UIKit;
using Foundation;

using Common;
using ListerKit;

namespace Lister
{
	[Register("DocumentsViewController")]
	public class DocumentsViewController : UITableViewController
	{
		// User defaults keys.
		const string StorageOptionUserDefaultsKey = "StorageOptionKey";
		const string StorageOptionUserDefaultsLocal = "StorageOptionLocal";
		const string StorageOptionUserDefaultsCloud = "StorageOptionCloud";

		// Segue identifiers.
		const string ListDocumentSegueIdentifier = "showListDocument";
		const string NewListDocumentSegueIdentifier = "newListDocument";
		readonly NSString ListDocumentCellIdentifier = new NSString("listDocumentCell");

		NSObject sizeChangedToken, updateToken;
		IList<List> listCollection;

		readonly ListService listService;

		public DocumentsViewController(IntPtr handle)
			: base(handle)
		{
			listService = ServiceLocator.ListService;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			SetNeedsStatusBarAppearanceUpdate ();

			SetupTextAttributes ();

			sizeChangedToken = UIApplication.Notifications.ObserveContentSizeCategoryChanged (HandleContentSizeCategoryDidChangeNotification);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			listCollection = listService.FetchLists ();

			SetupTextAttributes ();

			NavigationController.NavigationBar.TintColor = AppColors.ColorFrom(ListColor.Gray);
			NavigationController.Toolbar.TintColor = AppColors.ColorFrom(ListColor.Gray);
			TableView.TintColor = AppColors.ColorFrom(ListColor.Gray);
		}

		void SetupTextAttributes ()
		{
			NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes {
				Font = UIFont.PreferredHeadline,
				ForegroundColor = AppColors.ColorFrom (ListColor.Gray)
			};
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);

			sizeChangedToken.Dispose ();
			updateToken.Dispose ();
		}

		#region Setup

		void SelectListWithListInfo(List list)
		{
			UISplitViewController splitViewController = SplitViewController;

			Action<ListViewController> ConfigureListViewController = listViewController => {
//				listViewController.ConfigureWith(listInfo);
				listViewController.MasterController = this;
			};

			if (splitViewController.Collapsed) {
				ListViewController listViewController = (ListViewController)Storyboard.InstantiateViewController ("listViewController");
				ConfigureListViewController(listViewController);
				ShowViewController (listViewController, this);
			} else {
				UINavigationController navigationController = (UINavigationController)Storyboard.InstantiateViewController ("listViewNavigationController");
				ListViewController listViewController = (ListViewController)navigationController.TopViewController;
				ConfigureListViewController(listViewController);
				SplitViewController.ViewControllers = new UIViewController[] {
					SplitViewController.ViewControllers [0],
					new UIViewController ()
				};
				ShowDetailViewController (navigationController, this);
			}
		}

		#endregion

		#region UITableViewDataSource

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return listCollection.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			List list = listCollection [indexPath.Row];
			ListCell cell = (ListCell)tableView.DequeueReusableCell(ListDocumentCellIdentifier, indexPath);

			Configure (cell, list);
			return cell;
		}

		void Configure(ListCell cell, List list)
		{
			// Show an empty string as the text since it may need to load.
			cell.Label.Text = string.Empty;
			cell.Label.Font = UIFont.PreferredBody;
			cell.ListColorView.BackgroundColor = UIColor.Clear;
			cell.BackgroundColor = UIColor.Clear;

			cell.Label.Text = list.Name;
			cell.ListColorView.BackgroundColor = AppColors.ColorFrom (list.Color);
		}

		#endregion

		#region UITableViewDelegate

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			throw new NotImplementedException ();
//			ListInfo listInfo = list[indexPath.Row];
//			SelectListWithListInfo (listInfo);
		}

		public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
		{
			return false;
		}

		public override bool CanMoveRow (UITableView tableView, NSIndexPath indexPath)
		{
			return false;
		}

		#endregion

		public void ListViewControllerDidDeleteList (ListViewController listViewController)
		{
			if (listViewController == null)
				throw new ArgumentNullException ("listViewController");

			TableView.DeselectRow (TableView.IndexPathForSelectedRow, false);
			// DeleteListAtUrl (listViewController.DocumentURL);
		}

		#region UIStoryboardSegue Handling

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == NewListDocumentSegueIdentifier) {
				var newListController = (NewDocumentController)segue.DestinationViewController;
				newListController.MasterController = this;
			}
		}

		public void OnNewListInfo (List listInfo)
		{
			InsertListInfo(listInfo, index => {
				NSIndexPath indexPathForInsertedRow = NSIndexPath.FromRowSection(index, 0);
				TableView.InsertRows(new NSIndexPath[] {indexPathForInsertedRow }, UITableViewRowAnimation.Automatic);
			});
		}

		#endregion

		#region Convenience

		void DeleteListAtUrl(NSUrl url)
		{
//			RemoveListInfo (url, index => {
//				NSIndexPath indexPathForRemoval = NSIndexPath.FromRowSection(index, 0);
//				TableView.DeleteRows(new NSIndexPath[] { indexPathForRemoval }, UITableViewRowAnimation.Automatic);
//			});
		}

		#endregion

		#region List Management

		void InsertListInfo(List listInfo, Action<int> completionHandler)
		{
			throw new NotImplementedException ();

			ComparisionComparer<List> comparer = new ComparisionComparer<List>((left, right) => {
				return left.Name.CompareTo(right.Name);
			});
			// read more about return value http://msdn.microsoft.com/en-us/library/ftfdbfx6(v=vs.110).aspx
//			int index = list.BinarySearch(listInfo, comparer);
//			index = index >= 0 ? index : ~index;
//
//			list.Insert (index, listInfo);
//
//			if (completionHandler != null)
//				completionHandler(index);
		}

		#endregion

		#region Notifications

		public void UpdateDocumentColor(List list, ListColor newColor)
		{
			throw new NotImplementedException ();
			/*
			ListInfo listInfo = new ListInfo (list);

			int index = list.IndexOf(listInfo);
			if (index != -1) {
				listInfo = list[index];
				listInfo.Color = newColor;

				NSIndexPath indexPath = NSIndexPath.FromRowSection (index, 0);
				ListCell cell = (ListCell)TableView.CellAt (indexPath);
				cell.ListColorView.BackgroundColor = AppColors.ColorFrom (newColor);
			}
			*/
		}

		void HandleContentSizeCategoryDidChangeNotification(object sender, UIContentSizeCategoryChangedEventArgs arg)
		{
			View.SetNeedsLayout ();
		}

		#endregion
	}
}

