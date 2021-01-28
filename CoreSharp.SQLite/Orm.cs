using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;

using Sqlite3DatabaseHandle = System.IntPtr;
using Sqlite3BackupHandle = System.IntPtr;
using Sqlite3Statement = System.IntPtr;

namespace CoreSharp.SQLite
{

	public static class Orm
	{
		public const int DefaultMaxStringLength = 140;
		public const string ImplicitPkName = "Id";
		public const string ImplicitIndexSuffix = "Id";

		public static Type GetType(object obj)
		{
			if (obj == null)
				return typeof(object);
			var rt = obj as IReflectableType;
			if (rt != null)
				return rt.GetTypeInfo().AsType();
			return obj.GetType();
		}

		public static string SqlDecl(TableMappingColumn p, bool storeDateTimeAsTicks, bool storeTimeSpanAsTicks)
		{
			string decl = "\"" + p.Name + "\" " + SqlType(p, storeDateTimeAsTicks, storeTimeSpanAsTicks) + " ";

			if (p.IsPK)
			{
				decl += "primary key ";
			}
			if (p.IsAutoInc)
			{
				decl += "autoincrement ";
			}
			if (!p.IsNullable)
			{
				decl += "not null ";
			}
			if (!string.IsNullOrEmpty(p.Collation))
			{
				decl += "collate " + p.Collation + " ";
			}

			return decl;
		}

		public static string SqlType(TableMappingColumn p, bool storeDateTimeAsTicks, bool storeTimeSpanAsTicks)
		{
			var clrType = p.ColumnType;
			if (clrType == typeof(Boolean) || clrType == typeof(Byte) || clrType == typeof(UInt16) || clrType == typeof(SByte) || clrType == typeof(Int16) || clrType == typeof(Int32) || clrType == typeof(UInt32) || clrType == typeof(Int64))
			{
				return "integer";
			}
			else if (clrType == typeof(Single) || clrType == typeof(Double) || clrType == typeof(Decimal))
			{
				return "float";
			}
			else if (clrType == typeof(String) || clrType == typeof(StringBuilder) || clrType == typeof(Uri) || clrType == typeof(UriBuilder))
			{
				int? len = p.MaxStringLength;

				if (len.HasValue)
					return "varchar(" + len.Value + ")";

				return "varchar";
			}
			else if (clrType == typeof(TimeSpan))
			{
				return storeTimeSpanAsTicks ? "bigint" : "time";
			}
			else if (clrType == typeof(DateTime))
			{
				return storeDateTimeAsTicks ? "bigint" : "datetime";
			}
			else if (clrType == typeof(DateTimeOffset))
			{
				return "bigint";
			}
			else if (clrType.GetTypeInfo().IsEnum)
			{
				if (p.StoreAsText)
					return "varchar";
				else
					return "integer";
			}
			else if (clrType == typeof(byte[]))
			{
				return "blob";
			}
			else if (clrType == typeof(Guid))
			{
				return "varchar(36)";
			}
			else
			{
				throw new NotSupportedException("Don't know about " + clrType);
			}
		}

		public static bool IsPK(MemberInfo p)
		{
			return p.CustomAttributes.Any(x => x.AttributeType == typeof(PrimaryKeyAttribute));
		}

		public static string Collation(MemberInfo p)
		{
			return string.Empty;
			//return (p.GetCustomAttribute<CollationAttribute>()?.Value) ?? "";
		}

		public static bool IsAutoInc(MemberInfo p)
		{
			return p.CustomAttributes.Any(x => x.AttributeType == typeof(AutoIncrementAttribute));
		}

		public static FieldInfo GetField(TypeInfo t, string name)
		{
			var f = t.GetDeclaredField(name);
			if (f != null)
				return f;
			return GetField(t.BaseType.GetTypeInfo(), name);
		}

		public static PropertyInfo GetProperty(TypeInfo t, string name)
		{
			var f = t.GetDeclaredProperty(name);
			if (f != null)
				return f;
			return GetProperty(t.BaseType.GetTypeInfo(), name);
		}

		public static object InflateAttribute(CustomAttributeData x)
		{
			var atype = x.AttributeType;
			var typeInfo = atype.GetTypeInfo();
			var r = Activator.CreateInstance(x.AttributeType);
			return r;
		}

		public static IEnumerable<IndexedAttribute> GetIndices(MemberInfo p)
		{
			return p.GetCustomAttributes<IndexedAttribute>();
		}

		public static int? MaxStringLength(PropertyInfo p)
		{
			return p.GetCustomAttribute<MaxLengthAttribute>()?.Value;
		}

		public static bool IsMarkedNotNull(MemberInfo p)
		{
			return p.CustomAttributes.Any(x => x.AttributeType == typeof(NotNullAttribute));
		}
	}

}
