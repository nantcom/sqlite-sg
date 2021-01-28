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

namespace CoreSharp.SQLite
{

    public class TableMappingColumn
    {
        PropertyInfo _prop;

        public string Name { get; private set; }

        public PropertyInfo PropertyInfo => _prop;

        public string PropertyName { get { return _prop.Name; } }

        public Type ColumnType { get; private set; }

        public string Collation { get; private set; }

        public bool IsAutoInc { get; private set; }
        public bool IsAutoGuid { get; private set; }

        public bool IsPK { get; private set; }

        public IEnumerable<IndexedAttribute> Indices { get; set; }

        public bool IsNullable { get; private set; }

        public int? MaxStringLength { get; private set; }

        public bool StoreAsText { get; private set; }

        public TableMappingColumn(PropertyInfo prop, CreateFlags createFlags = CreateFlags.None)
        {
            var colAttr = prop.CustomAttributes.FirstOrDefault(x => x.AttributeType == typeof(ColumnAttribute));

            _prop = prop;

            var ca = prop.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute;
            Name = ca == null ? prop.Name : ca.Name;

            //If this type is Nullable<T> then Nullable.GetUnderlyingType returns the T, otherwise it returns null, so get the actual type instead
            ColumnType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
            Collation = Orm.Collation(prop);

            IsPK = Orm.IsPK(prop) ||
                (((createFlags & CreateFlags.ImplicitPK) == CreateFlags.ImplicitPK) &&
                     string.Compare(prop.Name, Orm.ImplicitPkName, StringComparison.OrdinalIgnoreCase) == 0);

            var isAuto = Orm.IsAutoInc(prop) || (IsPK && ((createFlags & CreateFlags.AutoIncPK) == CreateFlags.AutoIncPK));
            IsAutoGuid = isAuto && ColumnType == typeof(Guid);
            IsAutoInc = isAuto && !IsAutoGuid;

            Indices = Orm.GetIndices(prop);
            if (!Indices.Any()
                && !IsPK
                && ((createFlags & CreateFlags.ImplicitIndex) == CreateFlags.ImplicitIndex)
                && Name.EndsWith(Orm.ImplicitIndexSuffix, StringComparison.OrdinalIgnoreCase)
                )
            {
                Indices = new IndexedAttribute[] { new IndexedAttribute() };
            }
            IsNullable = !(IsPK || Orm.IsMarkedNotNull(prop));
            MaxStringLength = Orm.MaxStringLength(prop);

            StoreAsText = prop.PropertyType.GetTypeInfo().CustomAttributes.Any(x => x.AttributeType == typeof(StoreAsTextAttribute));
        }

        public void SetValue(object obj, object val)
        {
            if (val != null && ColumnType.GetTypeInfo().IsEnum)
            {
                _prop.SetValue(obj, Enum.ToObject(ColumnType, val));
            }
            else
            {
                _prop.SetValue(obj, val, null);
            }
        }

        public object GetValue(object obj)
        {
            return _prop.GetValue(obj, null);
        }
    }
}
