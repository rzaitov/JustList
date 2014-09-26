using System;
using System.Linq;
using System.Collections.Generic;

using UIKit;
using Foundation;

namespace Common
{
	[Register("List")]
	public class List : ICloneable
	{
		const string ListEncodingItemsKey = "items";
		const string ListEncodingColorKey = "color";

		public Guid Id { get; set; }

		List<ListItem> items;
		public ListColor Color { get; set; }
		public string Name { get; set; }

		public bool IsEmpty {
			get {
				return items.Count == 0;
			}
		}

		public List ()
		{
			items = new List<ListItem> ();
		}

		public List(IEnumerable<ListItem> items, ListColor color)
			: this()
		{
			this.items.AddRange (items);
			Color = color;
		}

		public ListItem[] CopyAllItems()
		{
			return items.ToArray ();
		}

		#region ICloneable implementation

		public object Clone ()
		{
			return new List (items, Color);
		}

		#endregion
	}
}

