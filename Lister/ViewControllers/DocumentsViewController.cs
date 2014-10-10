using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;

using UIKit;
using Foundation;

using Common;
using ListerKit;
using System.Linq;

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

		readonly IListService listService;

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
			TableView.ReloadData ();

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
			Action<ListViewController> ConfigureListViewController = controller => {
				controller.List = list;
				controller.MasterController = this;
			};

			ListViewController listViewController = (ListViewController)Storyboard.InstantiateViewController ("listViewController");
			ConfigureListViewController(listViewController);
			NavigationController.PushViewController (listViewController, true);
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
			List list = listCollection[indexPath.Row];
			SelectListWithListInfo (list);
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

		/// <summary>
		/// This make sence only for iPad
		/// </summary>
		/// <param name="listViewController">List view controller.</param>
		public void ListViewControllerDidDeleteList (ListViewController listViewController)
		{
			if (listViewController == null)
				throw new ArgumentNullException ("listViewController");

			TableView.DeselectRow (TableView.IndexPathForSelectedRow, false);
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

		public void UpdateDocumentColor(Guid listId, ListColor newColor)
		{
			int index = listCollection.FindIndex (l => l.Id == listId);
			if (index != -1)
				UpdateDocumentColor (index, newColor);
			else
				HandleChangeColorForNonExistentList (listId);
		}

		void UpdateDocumentColor(int index, ListColor newColor)
		{
			var list = listCollection [index];
			list.Color = newColor;

			listService.UpdateList (list);

			NSIndexPath indexPath = NSIndexPath.FromRowSection (index, 0);
			ListCell cell = (ListCell)TableView.CellAt (indexPath);
			cell.ListColorView.BackgroundColor = AppColors.ColorFrom (newColor);
		}

		void HandleChangeColorForNonExistentList(Guid listId)
		{
			string message = string.Format ("asked change color for non-existent list. List id = {0}", listId);
			// TODO: replace with alert
			throw new InvalidProgramException (message);
		}

		void HandleContentSizeCategoryDidChangeNotification(object sender, UIContentSizeCategoryChangedEventArgs arg)
		{
			View.SetNeedsLayout ();
		}

		#endregion
	}
}

