using System;

using UIKit;
using Foundation;

using Common;
using ListerKit;
using NotificationCenter;
using System.Drawing;
using CoreGraphics;
using System.Collections.Generic;

namespace Lister
{
	[Register("ListViewController")]
	public class ListViewController : UITableViewController, IUITextFieldDelegate
	{
		const string EmptyViewControllerStoryboardIdentifier = "emptyViewController";

		// Notification User Info Keys
		public static readonly NSString ListDidUpdateColorUserInfoKey = new NSString("ListDidUpdateColorUserInfoKey");
		public static readonly NSString ListDidUpdateURLUserInfoKey = new NSString("ListDidUPdateURLUserInfoKey");

		// UITableViewCell Identifiers
		static readonly NSString ListItemCellIdentifier = new NSString("listItemCell");
		static readonly NSString ListColorCellIdentifier = new NSString("listColorCell");

		UIBarButtonItem[] listToolbarItems;

		List list;
		public List List {
			get {
				return list;
			}
			set {
				list = value;
				CreateTextArrtibutes ();
			}
		}
		ListVisualManager manager;
		IList<Item> items;
		readonly ListService listService;

		public DocumentsViewController MasterController { get; set; }

		UIStringAttributes textAttributes;
		UIStringAttributes TextAttributes {
			get {
				return textAttributes;
			}
			set {
				textAttributes = value;

				if(IsViewLoaded)
					UpdateInterfaceWithTextAttributes ();
			}
		}

		public ListViewController(IntPtr handle)
			: base (handle)
		{
			listService = ServiceLocator.ListService;

			string title = "Delete List";
			UIBarButtonItem deleteList = new UIBarButtonItem (title, UIBarButtonItemStyle.Plain, DeleteList);

			UIBarButtonItem flexibleSpace = new UIBarButtonItem (UIBarButtonSystemItem.FixedSpace);

			listToolbarItems = new UIBarButtonItem[] {
				flexibleSpace,
				deleteList,
				flexibleSpace
			};
		}

		#region View Life Cycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			UpdateInterfaceWithTextAttributes ();

			// Use the edit button item provided by the table view controller.
			NavigationItem.RightBarButtonItem = EditButtonItem;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			items = listService.FetchItems (List.Id);
			manager = new ListVisualManager (items);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			// Hide the toolbar so the list can't be edited.
			NavigationController.SetToolbarHidden (true, animated);
		}

		#endregion

		#region UIViewController Overrides

		public override void SetEditing (bool editing, bool animated)
		{
			base.SetEditing (editing, animated);

			// Prevent navigating back in edit mode.
			NavigationItem.SetHidesBackButton (editing, animated);

			// Reload the first row to switch from "Add Item" to "Change Color"
			NSIndexPath indexPath = NSIndexPath.FromRowSection (0, 0);
			TableView.ReloadRows (new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Automatic);

			// If moving out of edit mode, notify observers about the list color and trigger a save.
			if (!editing) {
				// Notify the document of a change.
				MasterController.UpdateDocumentColor (list.Id, list.Color);
			}

			NavigationController.SetToolbarHidden (!editing, animated);
			NavigationController.Toolbar.SetItems (listToolbarItems, animated);
		}

		#endregion

		#region UITableViewDataSource

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return list == null ? 0 : items.Count + 1;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			if (Editing && indexPath.Row == 0) {
				var colorCell = (ListColorCell)tableView.DequeueReusableCell (ListColorCellIdentifier, indexPath);

				colorCell.Configure ();
				colorCell.ViewController = this;

				return colorCell;
			} else {
				var itemCell = (ListItemCell)tableView.DequeueReusableCell (ListItemCellIdentifier, indexPath);
				ConfigureListItemCell (itemCell, indexPath.Row);

				return itemCell;
			}
		}

		void ConfigureListItemCell(ListItemCell itemCell, int row)
		{
			itemCell.TextField.Font = UIFont.PreferredBody;
			itemCell.TextField.WeakDelegate = this;

			if (row == 0) {
				// Configure an "Add Item" list item cell.
				itemCell.TextField.Placeholder = "Add Item";
				itemCell.CheckBox.Hidden = true;
			} else {
				Item item = items[row - 1];

				itemCell.Completed = item.IsComplete;
				itemCell.TextField.Text = item.Text;
			}
		}

		public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
		{
			// The initial row is reserved for adding new items so it can't be deleted or edited.
			return indexPath.Row != 0;
		}

		public override bool CanMoveRow (UITableView tableView, NSIndexPath indexPath)
		{
			// The initial row is reserved for adding new items so it can't be moved.
			return indexPath.Row != 0;
		}

		public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
		{
			if (editingStyle != UITableViewCellEditingStyle.Delete)
				return;

			Item item = items[indexPath.Row - 1];
			manager.RemoveItems (new Item[]{ item });

			TableView.DeleteRows (new NSIndexPath[]{ indexPath }, UITableViewRowAnimation.Automatic);
		}

		public override void MoveRow (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
		{
			Item item = items[sourceIndexPath.Row - 1];
			manager.MoveItem (item, destinationIndexPath.Row - 1);
		}

		#endregion

		#region UITableViewDelegate

		public override void WillBeginEditing (UITableView tableView, NSIndexPath indexPath)
		{
			// When the user swipes to show the delete confirmation, don't enter editing mode.
			// UITableViewController enters editing mode by default so we override without calling super.
		}

		public override void DidEndEditing (UITableView tableView, NSIndexPath indexPath)
		{
			// When the user swipes to hide the delete confirmation, no need to exit edit mode because we didn't enter it.
			// UITableViewController enters editing mode by default so we override without calling super.
		}

		public override NSIndexPath CustomizeMoveTarget (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath proposedIndexPath)
		{
			Item item = items[sourceIndexPath.Row - 1];

			int row;
			if (proposedIndexPath.Row == 0) {
				row = item.IsComplete ? manager.IndexOfFirstCompletedItem() + 1 : 1;
				return NSIndexPath.FromRowSection (row, 0);
			} else if (manager.CanMoveItem(item, proposedIndexPath.Row - 1, false)) {
				return proposedIndexPath;
			} else if (item.IsComplete) {
				row = manager.IndexOfFirstCompletedItem () + 1;
				return NSIndexPath.FromRowSection (row, 0);
			} else {
				row = manager.IndexOfFirstCompletedItem ();
				return NSIndexPath.FromRowSection (row, 0);
			}
		}

		#endregion

		#region UITextFieldDelegate

		[Export ("textFieldDidEndEditing:")]
		public void EditingEnded (UITextField textField)
		{
			NSIndexPath indexPath = IndexPathForView (textField);

			if (indexPath.Row > 0) {
				// Edit the item in place.
				Item item = items[indexPath.Row - 1];

				// If the contents of the text field at the end of editing is the same as it started, don't trigger an update.
				if (item.Text != textField.Text) {
					item.Text = textField.Text;
				}
			} else if (textField.Text.Length > 0) {
				// Adds the item to the top of the list.
				Item item = new Item (textField.Text);
				int insertedIndex = manager.InsertItem (item);

				// Update the edit row to show the check box.
				ListItemCell itemCell = (ListItemCell)TableView.CellAt (indexPath);
				itemCell.CheckBox.Hidden = false;

				// Insert a new add item row into the table view.
				TableView.BeginUpdates ();

				NSIndexPath targetIndexPath = NSIndexPath.FromRowSection (insertedIndex, 0);
				TableView.InsertRows (new NSIndexPath[] { targetIndexPath }, UITableViewRowAnimation.Automatic);

				TableView.EndUpdates ();
			}
		}

		[Export ("textFieldShouldReturn:")]
		public bool ShouldReturn (UITextField textField)
		{
			NSIndexPath indexPath = IndexPathForView(textField);

			// An item must have text to dismiss the keyboard.
			if (!string.IsNullOrEmpty(textField.Text) || indexPath.Row == 0) {
				textField.ResignFirstResponder ();
				return true;
			}

			return false;
		}

		#endregion

		public void OnListColorCellDidChangeSelectedColor (ListColor color)
		{
			list.Color = color;
			CreateTextArrtibutes ();

			NSIndexPath[] indexPaths = TableView.IndexPathsForVisibleRows;
			TableView.ReloadRows (indexPaths, UITableViewRowAnimation.None);
		}

		void CreateTextArrtibutes ()
		{
			TextAttributes = new UIStringAttributes {
				Font = UIFont.PreferredHeadline,
				ForegroundColor = AppColors.ColorFrom (list.Color)
			};
		}

		#region IBActions

		void DeleteList(object sender, EventArgs e)
		{
			// TODO: On iPad we should show empty controller with SplitViewController
//			var emptyViewController = (UIViewController)Storyboard.InstantiateViewController (EmptyViewControllerStoryboardIdentifier);
//			SplitViewController.ShowDetailViewController (emptyViewController, null);

			listService.DeleteList (list.Id);
			MasterController.ListViewControllerDidDeleteList (this);
			NavigationController.PopViewController (true);
		}

		[Export("checkBoxTapped:")]
		public void CheckBoxTapped(CheckBox sender)
		{
			NSIndexPath indexPath = IndexPathForView (sender);

			if (indexPath.Row >= 1 && indexPath.Row <= items.Count) {
				Item item = items[indexPath.Row - 1];
				ListOperationInfo info = manager.ToggleItem (item, -1);

				if (info.FromIndex == info.ToIndex) {
					TableView.ReloadRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Automatic);
				} else {
					// Animate the row up or down depending on whether it was complete/incomplete.
					NSIndexPath target = NSIndexPath.FromRowSection (info.ToIndex + 1, 0);

					TableView.BeginUpdates ();
					TableView.MoveRow (indexPath, target);
					TableView.EndUpdates ();
					TableView.ReloadRows (new NSIndexPath[] { target }, UITableViewRowAnimation.Automatic);
				}
			}
		}

		#endregion

		void OnListDocumentWasDeleted (object sender, EventArgs e)
		{
			InvokeOnMainThread (() => {
				DismissViewController(true, null);
			});
		}

		#region Convenience

		void UpdateInterfaceWithTextAttributes()
		{
			NavigationController.NavigationBar.TitleTextAttributes = TextAttributes;

			UIColor color = TextAttributes.ForegroundColor;
			NavigationController.NavigationBar.TintColor = color;
			NavigationController.Toolbar.TintColor = color;
			TableView.TintColor = color;
		}

		NSIndexPath IndexPathForView(UIView view)
		{
			CGPoint viewOrigin = view.Bounds.Location;
			CGPoint viewLocation = TableView.ConvertPointFromView (viewOrigin, view);

			return TableView.IndexPathForRowAtPoint (viewLocation);
		}

		#endregion

	}
}