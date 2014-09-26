using System;

using Foundation;

namespace Common
{
	public class Item
	{
		public string Text { get; set; }
		public bool IsComplete { get; set; }
		public Guid UID { get; private set; }

		public Item()
		{

		}

		public Item (string text, bool isComplete, Guid uid)
		{
			Text = text;
			IsComplete = isComplete;
			UID = uid;
		}

		public Item(string text)
			: this(text, false, Guid.NewGuid())
		{
		}
	}
}

