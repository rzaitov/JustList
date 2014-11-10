using System;
using Foundation;

namespace JustList
{
	public static class LocalizationExtensions
	{
		public static string GetLocalization(this string key)
		{
			return NSBundle.MainBundle.LocalizedString (key, null);
		}
	}
}

