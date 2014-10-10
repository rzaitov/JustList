using System;
using SQLite;
using System.Collections.Generic;

namespace Common
{
	public class ListService : IListService
	{
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

		public IList<Item> FetchItems (Guid listId)
		{
			var list = connection.Find<List> (listId);
			if (list == null)
				return null;

			List<Item> items = connection.Query<Item>(@"
SELECT *
FROM `Item`
INNER JOIN `List`
ON `Item`.`list_id` = `List`.`id`
");
			return items;
		}

		public bool IsNameValid (string listName)
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

		public void AddNewList (List newList)
		{
			if (newList == null)
				throw new ArgumentNullException ("list");

			AssertListId (newList.Id);

			bool isNameValid = IsNameValid (newList.Name);
			if (!isNameValid)
				throw new ArgumentException (string.Format ("list with name {0} already exists", newList.Name));

			connection.Insert (newList, typeof(List));
		}

		public void UpdateList (List list)
		{
			if (list == null)
				throw new ArgumentNullException ("list");

			var id = list.Id;
			AssertListId (id);

			int affectedRows = connection.Update (list, typeof(List));
			AssertListRowExists (affectedRows, id);
		}

		public void DeleteList (Guid id)
		{
			AssertListId (id);

			int affectedRows = connection.Delete<List> (id);
			AssertListRowExists (affectedRows, id);
		}

		void AssertListRowExists(int affectedRows, Guid id)
		{
			if(affectedRows == 0)
				throw new InvalidProgramException (string.Format ("List with id={0} doesn't exists", id));
		}

		void AssertListId(Guid id)
		{
			if (id == default(Guid))
				throw new ArgumentException (string.Format ("Invalid id={0} value for List item", id));
		}

		#endregion
	}
}

