using System;
using System.Collections.Generic;

namespace Common
{
	public interface IListService
	{
		IList<List> FetchLists();

		/// <summary>
		/// This method returns List's items. If List exists with provided id at least empty collection will be returned.
		/// If no List with specified Id exists null will be returned
		/// </summary>
		IList<Item> FetchItems(Guid listId);

		bool IsNameValid(string listName);

		void AddNewList(List newList);

		void UpdateList(List list);

		void DeleteList(Guid listId);
	}
}

