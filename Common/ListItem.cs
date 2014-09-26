using System;

using Foundation;

namespace Common
{
	[Register("ListItem")]
	public class ListItem: ICloneable
	{
		const string ListItemEncodingTextKey = "text";
		const string ListItemEncodingCompletedKey = "completed";
		const string ListItemEncodingUUIDKey = "uuid";

		public string Text { get; set; }
		public bool IsComplete { get; set; }
		public Guid UID { get; private set; }

		public ListItem()
		{}

		public ListItem (string text, bool isComplete, Guid uid)
		{
			Text = text;
			IsComplete = isComplete;
			UID = uid;
		}

		public ListItem(string text)
			: this(text, false, Guid.NewGuid())
		{
		}

		#region ICloneable implementation

		public object Clone ()
		{
			return new ListItem (Text, IsComplete, UID);
		}

		#endregion
	}
}

