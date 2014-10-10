using System;

using Foundation;

using SQLite;

namespace Common
{
	[Table("Item")]
	public class Item
	{
		[Column("id"), PrimaryKey]
		public Guid Id { get; set; }

		[Column("list_id"), NotNull]
		public Guid ListId { get; set; }

		[Column("text"), MaxLength(100)]
		public string Text { get; set; }

		[Column("complete"), NotNull]
		public bool IsComplete { get; set; }


		public Item()
		{

		}

		public Item (string text, bool isComplete, Guid uid)
		{
			Text = text;
			IsComplete = isComplete;
			Id = uid;
		}

		public Item(string text)
			: this(text, false, Guid.NewGuid())
		{
		}
	}
}

