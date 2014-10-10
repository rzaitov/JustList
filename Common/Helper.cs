using System;
using System.Collections.Generic;

namespace Common
{
	public static class Helper
	{
		public static int FindIndex<T>(this IList<T> collection, Predicate<T> predicate)
		{
			int result = -1;
			for (int i = 0; i < collection.Count; i++) {
				if (predicate(collection[i])) {
					result = i;
					break;
				}
			}

			return result;
		}
	}
}

