using System.Collections.Generic;
using Microsoft.Win32;

namespace BackgroundSpotlight
{
	public class RegLookupParse
	{
		public struct KeyValue
		{
			public string Name { get; set; }
			public string Value { get; set; }
		}

		public struct SubKey
		{
			public string KeyPath { get; set; }
			public RegLookupParse SubKeyObj { get; set; }
		}


		public List<KeyValue> KeyValues { get; } = new List<KeyValue>();
		public List<SubKey> SubKeys { get; } = new List<SubKey>();
		public bool Error { get; set; }


		// keyPath (path to key), dive (recursively go through SubKeys)
		public RegLookupParse(string keyPath, bool dive = true)
		{
			var localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
			localKey = localKey.OpenSubKey(keyPath, false);

			// Check for empty (non-exsistent) key
			if (localKey == null)
			{
				Error = true;
				return;
			}

			// Process any Sub Keys
			foreach (var subKeyName in localKey.GetSubKeyNames())
			{
				var subKeyPath = keyPath + @"\" + subKeyName;

				// Check if it's needed to go through the subkeys
				if (dive)
				{
					SubKeys.Add(new SubKey
					{
						KeyPath = subKeyPath,
						SubKeyObj = new RegLookupParse(subKeyPath)
					});
				}
				else
				{
					SubKeys.Add(new SubKey
					{
						KeyPath = subKeyPath
					});
				}
			}

			// Process any Values
			foreach (var valueName in localKey.GetValueNames())
			{
				var valueForName = localKey.GetValue(valueName).ToString();
				KeyValues.Add(new KeyValue { Name = valueName, Value = valueForName });
			}
		}
	}
}
