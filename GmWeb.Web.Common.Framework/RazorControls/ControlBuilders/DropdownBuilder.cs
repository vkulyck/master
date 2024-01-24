using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GmWeb.Logic.Enums;
using GmWeb.Logic.Utility.Extensions;
using System.Linq.Expressions;
using System.Reflection;
using GmWeb.Logic.Data.Models.Shared;

namespace GmWeb.Web.Common.RazorControls.ControlBuilders
{
    public class DropdownBuilder<T> : ControlBuilder<DropdownBuilder<T>>
    {
        protected Func<T, string> CompiledDisplaySelector { get; private set; }
        private Expression<Func<T, string>> _displaySelector;
        public Expression<Func<T, string>> DisplaySelector
        {
            get => _displaySelector;
            set
            {
                if (_displaySelector != value)
                {
                    _displaySelector = value;
                    this.CompiledDisplaySelector = _displaySelector.Compile();
                }
            }
        }

        protected Func<T, string> CompiledValueSelector { get; private set; }
        private Expression<Func<T, string>> _valueSelector;
        public Expression<Func<T, string>> ValueSelector
        {
            get => _valueSelector;
            set
            {
                if(_valueSelector != value)
                {
                    _valueSelector = value;
                    this.CompiledValueSelector = _valueSelector.Compile();
                }
            }
        }
        public List<T> OptionList { get; protected set; }
        public DropdownOptionBuilder NullOption { get; protected set; }
        public DropdownBuilder() : base("select")
        {
            this.SetDefaultDisplay("Desecription", "Description", "Display", "Name");
            var pkCandidate = typeof(T).Name + "ID";
            this.SetDefaultValue(pkCandidate, "ID");
        }

        protected void SetDefaultDisplay(params string[] propertyNames) => SetDefaultSelector(this.Display, propertyNames);
        protected void SetDefaultValue(params string[] propertyNames) => SetDefaultSelector(this.Value, propertyNames);

        protected void SetDefaultSelector(Func<PropertyInfo,DropdownBuilder<T>> selectorAssignment, params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                var property = typeof(T).GetProperty(propertyName);
                if (property == null)
                    continue;
                if (!property.CanRead)
                    continue;
                selectorAssignment(property);
                return;
            }
            this.DisplaySelector = x => x.ToString();
        }

        protected Expression<Func<T,string>> BuildSelectorExpression(PropertyInfo property)
        {
            var param = Expression.Parameter(typeof(T));
            var body = Expression.PropertyOrField(param, property.Name);
            Expression<Func<T, string>> lambda;
            if (property.PropertyType == typeof(int))
            {
                var inner = Expression.Lambda< Func<T, int>>(body, param);
                lambda = AddStringConversion(inner);
            }
            else lambda = Expression.Lambda<Func<T, string>>(body, param);
            return lambda;
        }

        protected Expression<Func<TInput, string>> AddStringConversion<TInput, TOutput>(Expression<Func<TInput, TOutput>> expression)
        {
            // Add a ToString call around the body of the incoming expression
            var toString = typeof(TOutput).GetMethods().Where(x => x.Name == "ToString").Where(x => x.GetParameters().Length == 0).Single();
            var toStringValue = Expression.Call(expression.Body, toString);
            return Expression.Lambda<Func<TInput, string>>(toStringValue, expression.Parameters);
        }

        public DropdownBuilder<T> Display(Expression<Func<T,string>> Selector)
        {
            this.DisplaySelector = Selector;
            return this;
        }

        public DropdownBuilder<T> Display(string propertyName)
        {
            var property = typeof(T).GetProperty(propertyName);
            return this.Display(property);
        }

        protected DropdownBuilder<T> Display(PropertyInfo property)
        {
            this.DisplaySelector = this.BuildSelectorExpression(property);
            return this;
        }

        public DropdownBuilder<T> Value(Expression<Func<T, string>> Selector)
        {
            this.ValueSelector = Selector;
            return this;
        }

        public DropdownBuilder<T> Value(Expression<Func<T, int>> Selector)
        {
            this.ValueSelector = AddStringConversion(Selector);
            return this;
        }

        public DropdownBuilder<T> Value(string propertyName)
        {
            var property = typeof(T).GetProperty(propertyName);
            return this.Value(property);
        }

        protected DropdownBuilder<T> Value(PropertyInfo property)
        {
            this.ValueSelector = this.BuildSelectorExpression(property);
            return this;
        }

        public DropdownBuilder<T> Values(IEnumerable<T> values)
        {
            this.OptionList = values.ToList();
            foreach (var value in this.OptionList)
            {
                var option = this.CreateChild<DropdownOptionBuilder>();
                option.Value(this.CompiledValueSelector(value));
                option.Display(this.CompiledDisplaySelector(value));
            }
            return this;
        }

        public DropdownBuilder<T> EnableNullOption(string DisplayText = null)
        {
            this.NullOption = this.CreateChild<DropdownOptionBuilder>(index: 0);
            this.NullOption.Display(DisplayText).Value(string.Empty);
            return this;
        }
    }
}