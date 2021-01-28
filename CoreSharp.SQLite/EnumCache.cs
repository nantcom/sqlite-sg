using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace CoreSharp.SQLite
{

	class EnumCacheInfo
	{
		public EnumCacheInfo(Type type)
		{
			var typeInfo = type.GetTypeInfo();

			IsEnum = typeInfo.IsEnum;

			if (IsEnum)
			{
				StoreAsText = typeInfo.CustomAttributes.Any(x => x.AttributeType == typeof(StoreAsTextAttribute));

				if (StoreAsText)
				{
					EnumValues = new Dictionary<int, string>();
					foreach (object e in Enum.GetValues(type))
					{
						EnumValues[Convert.ToInt32(e)] = e.ToString();
					}
				}
			}
		}

		public bool IsEnum { get; private set; }

		public bool StoreAsText { get; private set; }

		public Dictionary<int, string> EnumValues { get; private set; }
	}

	static class EnumCache
	{
		static readonly Dictionary<Type, EnumCacheInfo> Cache = new Dictionary<Type, EnumCacheInfo>();

		public static EnumCacheInfo GetInfo<T>()
		{
			return GetInfo(typeof(T));
		}

		public static EnumCacheInfo GetInfo(Type type)
		{
			lock (Cache)
			{
				EnumCacheInfo info = null;
				if (!Cache.TryGetValue(type, out info))
				{
					info = new EnumCacheInfo(type);
					Cache[type] = info;
				}

				return info;
			}
		}
	}
}
