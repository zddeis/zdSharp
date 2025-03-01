using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zds.Core.Expressions
{
    public class PropertyAccessExpression : IExpression
    {
        private readonly IExpression _object;
        private readonly string _property;

        public PropertyAccessExpression(IExpression obj, string property)
        {
            _object = obj;
            _property = property;
        }

        public object? Evaluate()
        {
            var obj = _object.Evaluate();

            if (obj is Window window)
            {
                return _property switch
                {
                    "FullScreen" => window.FullScreen,
                    "MaxRefreshRate" => (double)window.MaxRefreshRate,
                    "BackgroundColor" => window.BackgroundColor,
                    "Width" => (double)window.Width,
                    "Height" => (double)window.Height,
                    "Title" => window.GetForm().Text,
                    _ => throw new Exception($"Unknown property '{_property}' for Window object")
                };
            }
            else if (obj is Panel panel)
            {
                // Add panel properties if needed
                throw new Exception($"Unknown property '{_property}' for Panel object");
            }

            throw new Exception($"Cannot access property on non-object value");
        }
    }
}