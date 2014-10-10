using System;
using System.Linq;
using System.Collections.Generic;

using UIKit;
using Foundation;

namespace Common
{
	public class List
	{
		public Guid Id { get; set; }

		public ListColor Color { get; set; }
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

