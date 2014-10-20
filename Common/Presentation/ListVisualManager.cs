using System;
using System.Collections.Generic;
using Foundation;
using System.Linq;

namespace Common
{
	public class ListVisualManager
	{
		readonly IList<Item> items;

		public ListVisualManager (IList<Item> items)
		{
			this.items = items;
		}

		public int IndexOfItem(Item item)
		{
			return items.IndexOf (item);
		}

		/// Use this function to ensure that all inserted items are complete.
		/// All inserted items must be incomplete when inserted.
		public bool CanInsertIncompleteItems(IEnumerable<Item> incompleteItems, int index)
		{
			bool anyCompleteItem = incompleteItems.Any (item => item.IsComplete);

			if (anyCompleteItem)
				return false;

			return index <= IndexOfFirstCompletedItem ();
		}

		public void InsertItem(Item item, int index)
		{
			items.Insert (index, item);
		}

		public int InsertItem(Item item)
		{
			if (item.IsComplete) {
				items.Add (item);
				return items.Count - 1;
			} else {
				items.Insert (0, item);
				return 0;
			}
		}

		public bool CanMoveItem(Item item, int index, bool inclusive)
		{
			int fromIndex = items.IndexOf (item);

			if (fromIndex == -1)
				return false;

			if (item.IsComplete)
				return index <= items.Count && index >= IndexOfFirstCompletedItem ();
			else if (inclusive)
				return index >= 0 && index <= IndexOfFirstCompletedItem ();
			else
				return index >= 0 && index < IndexOfFirstCompletedItem ();
		}

		public ListOperationInfo MoveItem(Item item, int toIndex)
		{
			int fromIndex = items.IndexOf (item);

			if (fromIndex == -1)
				throw new InvalidProgramException ("Moving an item that isn't in the list is undefined.");

			items.RemoveAt (fromIndex);

			int normalizedToIndex = toIndex;

			if (fromIndex < toIndex)
				normalizedToIndex--;

			items.Insert (normalizedToIndex, item);

			var moveInfo = new ListOperationInfo {
				FromIndex = fromIndex,
				ToIndex = normalizedToIndex
			};

			return moveInfo;
		}

		public void RemoveItems(IEnumerable<Item> items)
		{
			foreach (var item in items)
				this.items.Remove (item);
		}

		// Toggles an item's completion state and moves the item to the appropriate index.
		// The normalized from/to indexes are returned in the ListOperationInfo struct.
		public ListOperationInfo ToggleItem(Item item, int preferredDestinationIndex)
		{
			int fromIndex = items.IndexOf (item);

			if (fromIndex == -1)
				throw new InvalidProgramException ("Toggling an item that isn't in the list is undefined.");

			items.RemoveAt (fromIndex);

			item.IsComplete = !item.IsComplete;

			int toIndex = preferredDestinationIndex;

			if (toIndex == -1)
				toIndex = item.IsComplete ? items.Count : IndexOfFirstCompletedItem();

			items.Insert (toIndex, item);

			var toggleInfo = new ListOperationInfo {
				FromIndex = fromIndex,
				ToIndex = toIndex
			};

			return toggleInfo;
		}

		// Set all of the items to be a specific completion state.
		public void UpdateAllItemsToCompletionState(bool completeStatus)
		{
			foreach (Item item in items)
				item.IsComplete = completeStatus;
		}

		public int IndexOfFirstCompletedItem()
		{
			int index = -1;
			for (int i = 0; i < items.Count; i++) {
				index = items [i].IsComplete ? i : -1;
				if (index != -1)
					break;
			}
			return index == -1 ? items.Count : index;
		}

	}
}

