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

        public PropertyExpression(IExpression obj, string property, IExpression value)
        {
            _object = obj;
            _property = property;
            _value = value;
        }

        public object? Evaluate()
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
                            throw new Exception("Width property must be a number");
                        break;
                    case "Height":
                        if (value is double height)
                            window.Height = (int)height;
                        else
                            throw new Exception("Height property must be a number");
                        break;
                    case "FullScreen":
                        if (value is bool fullScreen)
                            window.FullScreen = fullScreen;
                        else
                            throw new Exception("FullScreen property must be a boolean");
                        break;
                    case "MaxRefreshRate":
                        if (value is double refreshRate)
                            window.MaxRefreshRate = (int)refreshRate;
                        else
                            throw new Exception("MaxRefreshRate property must be a number");
                        break;
                    case "BackgroundColor":
                        if (value is string color)
                            window.BackgroundColor = color;
                        else
                            throw new Exception("BackgroundColor property must be a string");
                        break;
                    case "Title":
                        if (value is string title)
                            window.SetTitle(title);
                        else
                            throw new Exception("Title property must be a string");
                        break;
                    default:
                        throw new Exception($"Unknown property '{_property}' for Window object");
                }
                return value;
            }

            throw new Exception($"Cannot set property on non-object value");
        }
    }
}