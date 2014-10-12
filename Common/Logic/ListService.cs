using System;
using SQLite;
using System.Collections.Generic;

namespace Common
{
	public class ListService : IListService
	{
		const string List = "List";
		const string Item = "Item";

		readonly SQLiteConnection connection;

		public ListService (SQLiteConnection connection)
		{
			if (connection == null)
				throw new ArgumentNullException ();

			this.connection = connection;
		}

		/// <summary>
		/// You should call this only once
		/// </summary>
		public void InitStorage()
		{
			connection.CreateTable<List> ();
			connection.CreateTable<Item> ();
		}

		/// <summary>
		/// This for testing only. Don't use in production
		/// </summary>
		public void DropStorage()
		{
			connection.DropTable<List> ();
			connection.DropTable<Item> ();
		}

		#region IListService implementation

		public IList<List> FetchLists ()
		{
			List<List> list = connection.Query<List> (@"
SELECT *
FROM `List`");
			return list;
		}

		public bool IsListNameValid (string listName)
		{
			bool exists = connection.ExecuteScalar<bool>(@"
SELECT EXISTS (
    SELECT `id`
    FROM `List`
    WHERE `List`.`name` = ?
    LIMIT 1)
", listName);

			return !exists;
		}

		public void Add (List list)
		{
			if (list == null)
				throw new ArgumentNullException ("list");

			AssertCorrectId (list.Id, List);

			bool isNameValid = IsListNameValid (list.Name);
			if (!isNameValid)
				throw new ArgumentException (string.Format ("list with name {0} already exists", list.Name));

			connection.Insert (list, typeof(List));
		}

		public void Update (List list)
		{
			if (list == null)
				throw new ArgumentNullException ("list");

			var id = list.Id;
			AssertCorrectId (id, List);

			int affectedRows = connection.Update (list, typeof(List));
			AssertRowExists (List, affectedRows, id);
		}

		public void DeleteList (Guid id)
		{
			AssertCorrectId (id, List);

			int affectedRows = connection.Delete<List> (id);
			AssertRowExists (List, affectedRows, id);
		}

		void ThrowIfListNotExists(Guid listId)
		{
			AssertCorrectId (listId, List);

			var list = connection.Get<List> (listId);
			if (list == null)
				throw new ArgumentException (string.Format ("List {0} doesn't exist", listId));
		}

		void AssertRowExists(string entityName, int affectedRows, Guid id)
		{
			if(affectedRows == 0)
				throw new InvalidProgramException (string.Format ("{0} with id={1} doesn't exists", entityName, id));
		}

		#endregion

		#region Item

		public void Add (Item item)
		{
			AssertNotNull (item, "item");
			AssertCorrectId (item.Id, Item);

			var list = connection.Get<List> (item.ListId);
			if (list == null)
				throw new ArgumentException (string.Format ("List {0} doesn't exist for item {1}", item.ListId, item.Id));

			connection.Insert (item);
		}

		public void Update (Item item)
		{
			AssertNotNull (item, "item");
			AssertCorrectId (item.Id, Item);

			ThrowIfListNotExists (item.ListId);
			var affectedRows = connection.Update (item);
			AssertRowExists (Item, affectedRows, item.Id);
		}

		public void DeleteItem (Guid id)
		{
			AssertCorrectId (id, Item);

			var affectedRows = connection.Delete<Item> (id);
			AssertRowExists (Item, affectedRows, id);
		}

		public IList<Item> FetchItems (Guid listId)
		{
			var list = connection.Find<List> (listId);
			if (list == null)
				return null;

			List<Item> items = connection.Query<Item>(@"
SELECT *
FROM `Item`
WHERE `Item`.`list_id` = ?", listId);

			// We don't store items positions,
			// but we want to show incomplete items on the top
			// and completed on the buttom
			Order (items);
			return items;
		}

		static void Order(List<Item> items)
		{
			items.Sort ((i1, i2) => {
				if(i1.IsComplete ^ i2.IsComplete)
					return i1.IsComplete.CompareTo(i2.IsComplete);
				else
					return i1.Text.CompareTo(i2.Text);
			});
		}

		#endregion

		void AssertNotNull(object obj, string name)
		{
			if (obj == null)
				throw new ArgumentNullException (name);
		}

		void AssertCorrectId (Guid id, string tableName)
		{
			if (id == default(Guid))
				throw new ArgumentException (string.Format ("Invalid id={0} value for {1}", id, tableName));
		}
	}
}

