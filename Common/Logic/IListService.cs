using System;
using System.Collections.Generic;

namespace Common
{
	public interface IListService
	{
		#region List

		IList<List> FetchLists();

		bool IsListNameValid(string name);

		void Add(List list);

		void Update(List list);

		void DeleteList(Guid id);

		#endregion

		#region Item

		/// <summary>
		/// This method returns List's items. If List exists with provided id at least empty collection will be returned.
		/// If no List with specified Id exists null will be returned
		/// </summary>
		IList<Item> FetchItems(Guid listId);

		void Add (Item item);

		void Update (Item item);

		void DeleteItem (Guid id);

		#endregion
	}
}

