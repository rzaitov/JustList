using System;

using UIKit;
using Foundation;

using Common;
using ListerKit;

namespace Lister
{
	[Register("NewDocumentController")]
	public class NewDocumentController : UIViewController, IUITextFieldDelegate
	{
		[Outlet("grayButton")]
		UIButton GrayButton { get; set; }

		[Outlet("blueButton")]
		UIButton BlueButton { get; set; }

		[Outlet("greenButton")]
		UIButton GreenButton { get; set; }

		[Outlet("yellowButton")]
		UIButton YellowButton { get; set; }

		[Outlet("orangeButton")]
		UIButton OrangeButton { get; set; }

		[Outlet("redButton")]
		UIButton RedButton { get; set; }

		[Outlet("saveButton")]
		UIBarButtonItem SaveButton { get; set; }

		[Outlet("toolbar")]
		UIToolbar Toolbar { get; set; }

		[Outlet("titleLabel")]
		UILabel TitleLabel { get; set; }

		[Outlet("NameInput")]
		UITextField NameInput { get; set; }

		UIButton selectedButton;

		ListColor selectedColor;
		string selectedTitle;

		readonly ListService listService;

		public DocumentsViewController MasterController { get; set; }

		public NewDocumentController (IntPtr handle)
			: base (handle)
		{
			listService = ServiceLocator.ListService;
		}

		#region UITextFieldDelegate

		[Export ("textFieldDidEndEditing:")]
		public void EditingEnded (UITextField textField)
		{
			var isValidName = IsNameValid(textField.Text);
			if (isValidName) {
				SaveButton.Enabled = true;
				selectedTitle = textField.Text;
			}
		}

		bool IsNameValid(string listName)
		{
			return listService.IsNameValid(listName);
		}

		[Export ("textFieldShouldReturn:")]
		public bool ShouldReturn (UITextField textField)
		{
			textField.ResignFirstResponder();
			return true;
		}

		#endregion

		#region IBActions

		[Export("pickColor:")]
		public void PickColor(UIButton sender)
		{
			NameInput.ResignFirstResponder();

			// Use the button's tag to determine the color.
			selectedColor = (ListColor)(int)sender.Tag;

			// Clear out the previously selected button's border.
			if(selectedButton != null)
				selectedButton.Layer.BorderWidth = 0;

			sender.Layer.BorderWidth = 5f;
			sender.Layer.BorderColor = UIColor.LightGray.CGColor;
			selectedButton = sender;

			TitleLabel.TextColor = AppColors.ColorFrom(selectedColor);
			Toolbar.TintColor = AppColors.ColorFrom(selectedColor);
		}

		[Export("saveAction:")]
		public void SaveAction(UIButton sender)
		{
			var list = new List {
				Name = selectedTitle,
				Color = selectedColor
			};
			listService.AddNewList (list);

			DismissViewController (true, null);
		}

		[Export("cancelAction:")]
		public void CancelAction(NSObject sender) {
			DismissViewController(true, null);
		}

		#endregion
	}
}

