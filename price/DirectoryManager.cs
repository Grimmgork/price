using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using price.PriceProviders;

namespace price
{
	static class DirectoryManager
	{
		private static string _appdir;
		public static string AppDir
		{
			get
			{
				if (_appdir == null)
					_appdir = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
				return _appdir;
			}
		}

		public static T LoadSpecificObject<T>() where T : class
		{
			string filepath = AppDir + "\\" + typeof(T).GetCustomAttribute<SpecificFilePathAttribute>().relativeFilePath;
			if (!File.Exists(filepath))
				return null;

			return JsonSerializer.Deserialize<T>(File.ReadAllText(filepath));
		}

		public static void SaveSpecificObject(object obj)
		{
			string filepath = AppDir + "\\" + obj.GetType().GetCustomAttribute<SpecificFilePathAttribute>().relativeFilePath;
			File.WriteAllText(filepath, JsonSerializer.Serialize(obj));
		}

		public class SpecificFilePathAttribute : System.Attribute
		{
			public string relativeFilePath { get; private set; }

			public SpecificFilePathAttribute(string relativeFilePath)
			{
				this.relativeFilePath = relativeFilePath;
			}
		}
	}
}
