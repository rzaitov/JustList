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
					Color = ListColor.Red,
					Id = Guid.Parse("52bf9a8a-9b52-4fba-8cd0-22dbe62e28df")
				},
				new List {
					Name = "Second",
					Color = ListColor.Green,
					Id = Guid.Parse("c8fd7a85-98f5-40d8-8002-a6a298397ee2")
				}
			};
		}

		public IList<ListItem> FetchItems(Guid listId)
		{
			if (listId == Guid.Parse ("52bf9a8a-9b52-4fba-8cd0-22dbe62e28df")) {
				return new List<ListItem> {
					new ListItem {
						IsComplete = false,
						Text = "Apple"
					},
					new ListItem {
						IsComplete = true,
						Text = "Banana"
					}
				};
			} else if (listId == Guid.Parse ("c8fd7a85-98f5-40d8-8002-a6a298397ee2")) {
				return new List<ListItem> {
					new ListItem {
						IsComplete = false,
						Text = "Cycle"
					},
					new ListItem {
						IsComplete = true,
						Text = "Car"
					},
					new ListItem {
						IsComplete = true,
						Text = "Train"
					}
				};
			}

			return null;
		}
	}
}

