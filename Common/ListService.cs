using System;
using System.Collections.Generic;

namespace Common
{
	public class ListService
	{
		public IList<List> FetchLists()
		{
			return new List<List> {
				new List {
					Name = "First",
					Color = ListColor.Red
				},
				new List {
					Name = "Second",
					Color = ListColor.Green
				}
			};
		}
	}
}

