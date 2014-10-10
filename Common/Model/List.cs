using System;
using System.Linq;
using System.Collections.Generic;

using UIKit;
using Foundation;

using SQLite;

namespace Common
{
	[Table("List")]
	public class List
	{
		[PrimaryKey, NotNull, Column("id")]
		public Guid Id { get; set; }

		[Column("color"), NotNull]
		public ListColor Color { get; set; }

		[Column("name"), NotNull, MaxLength(100)]
		public string Name { get; set; }

		public List ()
		{
			Id = Guid.NewGuid ();
		}

		public List(ListColor color)
		{
			Color = color;
		}
	}
}

