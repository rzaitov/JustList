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
			throw new NotImplementedException ();
		}

		public void AddNewList (List newList)
		{
			throw new NotImplementedException ();
		}

		public void UpdateList (List list)
		{
			throw new NotImplementedException ();
		}

		public void DeleteList (Guid listId)
		{
			throw new NotImplementedException ();
		}

		#endregion
	}
}

