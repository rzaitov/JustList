using System;
using System.Collections.Generic;

namespace Common
{
	public class ListService
	{
		readonly IList<List> list;
		readonly Dictionary<Guid, List<Item>> storage;

		public ListService()
		{
			list = new List<List> {
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

			storage = new Dictionary<Guid, List<Item>> { { Guid.Parse ("52bf9a8a-9b52-4fba-8cd0-22dbe62e28df"), new List<Item> {
						new Item {
							IsComplete = false,
							Text = "Apple"
						},
						new Item {
							IsComplete = true,
							Text = "Banana"
						}
					}
				}, { Guid.Parse ("c8fd7a85-98f5-40d8-8002-a6a298397ee2"), new List<Item> {
						new Item {
							IsComplete = false,
							Text = "Cycle"
						},
						new Item {
							IsComplete = true,
							Text = "Car"
						},
						new Item {
							IsComplete = true,
							Text = "Train"
						}
					}
				}
			};
		}

		public IList<List> FetchLists()
		{
			return list;
		}

		public IList<Item> FetchItems(Guid listId)
		{
			if (storage.ContainsKey (listId))
				return storage [listId];

			return null;
		}
	}
}

