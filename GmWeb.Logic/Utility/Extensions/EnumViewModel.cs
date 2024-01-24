using System;
using System.Collections.Generic;
using System.Linq;

namespace GmWeb.Logic.Utility.Extensions.Enums
{
    public record class EnumViewModel<T> where T : struct, Enum
    {
        public int ID { get; private set; }
        public T Value { get; private set; }

        public string Name { get; private set; }
        public string Display { get; private set; }
        public string ShortName { get; private set; }
        public string Description { get; private set; }
        public string GroupName { get; private set; }

        public bool Matches(string other)
        {
            if (this.ShortName == other)
                return true;
            if (this.Description == other)
                return true;
            return false;
        }

        public EnumViewModel(string name)
        {
            this.Value = this.Name.ToEnumValue<T>();
            this.setProperties();
        }

        public EnumViewModel(int id)
        {
            // .NET doesn't provide a standard way to convert a generic to/from an int, so we need this hack.
            this.Value = (T)(object)id;
            this.setProperties();
        }

        public EnumViewModel(T t)
        {
            this.Value = t;
            this.setProperties();
        }

        public TAttribute GetAttribute<TAttribute>()
            where TAttribute : Attribute
            => this.Value.GetAttribute<T, TAttribute>();
        public List<TAttribute> GetAttributes<TAttribute>()
            where TAttribute : Attribute
            => this.Value.GetAttributes<T, TAttribute>();

        /// <summary>
        /// Set the value of all properties based on this.Value
        /// </summary>
        private void setProperties()
        {
            this.Name = this.Value.ToString();
            this.Display = this.Value.GetDisplayName();
            this.ShortName = this.Value.GetShortName();
            this.Description = this.Value.GetEnumDescription();
            this.GroupName = this.Value.GetEnumGroupNamme();
            this.ID = this.Value.ToN();
        }

        public override string ToString() => $"{typeof(T).Name} [{this.ID}]: {this.Description}";
    }
}
