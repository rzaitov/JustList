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

		public static Item Create(Guid listId, string text)
		{
			if (text == null)
				throw new ArgumentNullException ("text");

			text = text.Trim ();

			return new Item {
				Id = Guid.NewGuid(),
				ListId = listId,
				Text = text,
			};
		}
	}
}

