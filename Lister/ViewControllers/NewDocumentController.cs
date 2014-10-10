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
		public void SaveAction(NSObject sender)
		{
			throw new NotImplementedException();
			/*
			ListInfo listInfo = new ListInfo(FileUrl);
			listInfo.Color = selectedColor;
			listInfo.Name = selectedTitle;

			listInfo.CreateAndSaveWithCompletionHandler (success => {
				if (success) {
					MasterController.OnNewListInfo(listInfo);
				} else {
					// In your app, you should handle this error gracefully.
					Console.WriteLine ("Unable to create new document at URL: {0}", FileUrl.AbsoluteString);
					throw new InvalidProgramException();
				}
				DismissViewController(true, null);
			});
			*/
		}

		[Export("cancelAction:")]
		public void CancelAction(NSObject sender) {
			DismissViewController(true, null);
		}

		#endregion
	}
}

