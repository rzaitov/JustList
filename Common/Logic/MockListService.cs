using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
	public class MockListService : IListService
	{
		readonly IList<List> listCollection;
		readonly Dictionary<Guid, List<Item>> storage;

		public MockListService()
		{
			listCollection = new List<List> {
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
			return listCollection;
		}

		/// <summary>
		/// This method returns List's items. If List exists with provided id at least empty collection will be returned.
		/// If no List with specified Id exists null will be returned
		/// </summary>
		public IList<Item> FetchItems(Guid listId)
		{
			if (storage.ContainsKey (listId))
				return storage [listId];

			return null;
		}

		public bool IsNameValid(string listName)
		{
			if (string.IsNullOrWhiteSpace (listName))
				return false;

			bool exists = listCollection.Any ( l => l.Name == listName);
			return !exists;
		}

		public void AddNewList(List newList)
		{
			if (newList == null)
				throw new ArgumentNullException ("list");

			bool isNameValid = IsNameValid (newList.Name);
			if (!isNameValid)
				throw new ArgumentException (string.Format ("list with name {0} already exists", newList.Name));

			listCollection.Add (newList);
			storage.Add (newList.Id, new List<Item> ());
		}

		public void UpdateList(List list)
		{
			if (list == null)
				throw new ArgumentNullException ("list");

			int index = listCollection.FindIndex (l => l.Id == list.Id);
			AssertListExists (index, list.Id);

			listCollection [index] = list;
		}

		public void DeleteList(Guid listId)
		{
			int index = listCollection.FindIndex (l => l.Id == listId);
			AssertListExists (index, listId);

			// Remove items relative to List
			storage.Remove (listId);
			listCollection.RemoveAt (index);
		}

		void AssertListExists(int index, Guid listId)
		{
			if (index == -1)
				throw new InvalidProgramException (string.Format ("List with Id={0} doesn't exists", listId));
		}
	}
}

