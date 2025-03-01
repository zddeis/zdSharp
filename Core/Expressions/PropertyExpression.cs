using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zds.Core.Expressions
{
    public class PropertyExpression : IExpression
    {
        private readonly IExpression _object;
        private readonly string _property;
        private readonly IExpression _value;
        public int Line { get; }

        public PropertyExpression(IExpression obj, string property, IExpression value, int line = 0)
        {
            _object = obj;
            _property = property;
            _value = value;
            Line = line;
        }

        public object? Evaluate()
        {
            try
            {
                var obj = _object.Evaluate();
                var value = _value.Evaluate();

                if (obj is Window window)
                {
                    switch (_property)
                    {
                        case "Width":
                            if (value is double width)
                                window.Width = (int)width;
                            else
                                throw new RuntimeException("Width property must be a number", Line);
                            break;
                        case "Height":
                            if (value is double height)
                                window.Height = (int)height;
                            else
                                throw new RuntimeException("Height property must be a number", Line);
                            break;
                        case "FullScreen":
                            if (value is bool fullScreen)
                                window.FullScreen = fullScreen;
                            else
                                throw new RuntimeException("FullScreen property must be a boolean", Line);
                            break;
                        case "MaxRefreshRate":
                            if (value is double refreshRate)
                                window.MaxRefreshRate = (int)refreshRate;
                            else
                                throw new RuntimeException("MaxRefreshRate property must be a number", Line);
                            break;
                        case "BackgroundColor":
                            if (value is string color)
                                window.BackgroundColor = color;
                            else
                                throw new RuntimeException("BackgroundColor property must be a string", Line);
                            break;
                        case "Title":
                            if (value is string title)
                                window.SetTitle(title);
                            else
                                throw new RuntimeException("Title property must be a string", Line);
                            break;
                        default:
                            throw new RuntimeException($"Unknown property '{_property}' for Window object", Line);
                    }
                    return value;
                }

                throw new RuntimeException($"Cannot set property on non-object value", Line);
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