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
        public int Line { get; }

        public PropertyAccessExpression(IExpression obj, string property, int line = 0)
        {
            _object = obj;
            _property = property;
            Line = line;
        }

        public object? Evaluate()
        {
            try
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
                        _ => throw new RuntimeException($"Unknown property '{_property}' for Window object", Line)
                    };
                }
                else if (obj is Panel panel)
                {
                    // Add panel properties if needed
                    throw new RuntimeException($"Unknown property '{_property}' for Panel object", Line);
                }

                throw new RuntimeException($"Cannot access property on non-object value", Line);
            }
            catch (RuntimeException)
            {
                throw; // Re-throw runtime exceptions that already have line info
            }
            catch (Exception ex)
            {
                throw new RuntimeException(ex.Message, Line);
            }
        }
    }
}