using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zds.Core.Expressions
{
    public class MethodCallExpression : IExpression
    {
        private readonly IExpression _object;
        private readonly string _method;
        private readonly List<IExpression> _arguments;

        public MethodCallExpression(IExpression obj, string method, List<IExpression> arguments)
        {
            _object = obj;
            _method = method;
            _arguments = arguments;
        }

        public object? Evaluate()
        {
            var obj = _object.Evaluate();
            var args = _arguments.Select(arg => arg.Evaluate()).ToList();

            if (obj is Window window)
            {
                if (_method == "Close")
                {
                    window.Close();
                    return null;
                }
                else if (_method == "Show")
                {
                    window.Show();
                    return null;
                }
                else if (_method == "SetTitle")
                {
                    if (args.Count < 1)
                        throw new Exception("SetTitle requires a string argument");
                    if (args[0] is not string title)
                        throw new Exception("SetTitle requires a string argument");
                    window.SetTitle(title);
                    return null;
                }
                else if (_method == "AddLabel")
                {
                    if (args.Count < 3)
                        throw new Exception("AddLabel requires text, x, and y arguments");
                    if (args[0] is not string text)
                        throw new Exception("First argument to AddLabel must be a string");
                    if (args[1] is not double x)
                        throw new Exception("Second argument to AddLabel must be a number");
                    if (args[2] is not double y)
                        throw new Exception("Third argument to AddLabel must be a number");
                    window.AddLabel(text, (int)x, (int)y);
                    return null;
                }
                else
                {
                    throw new Exception($"Unknown method '{_method}' for Window object");
                }
            }
            else if (obj is Panel panel)
            {
                if (_method == "Clear")
                {
                    panel.Clear();
                    return null;
                }
                else if (_method == "Update")
                {
                    panel.Update();
                    return null;
                }
                else if (_method == "FillRectangle")
                {
                    if (args.Count < 5)
                        throw new Exception("FillRectangle requires color, x, y, width, and height arguments");
                    if (args[0] is not string color)
                        throw new Exception("First argument to FillRectangle must be a string (color)");
                    if (args[1] is not double x)
                        throw new Exception("Second argument to FillRectangle must be a number (x)");
                    if (args[2] is not double y)
                        throw new Exception("Third argument to FillRectangle must be a number (y)");
                    if (args[3] is not double width)
                        throw new Exception("Fourth argument to FillRectangle must be a number (width)");
                    if (args[4] is not double height)
                        throw new Exception("Fifth argument to FillRectangle must be a number (height)");
                    panel.FillRectangle(color, x, y, width, height);
                    return null;
                }
                else if (_method == "DrawRectangle")
                {
                    if (args.Count < 5)
                        throw new Exception("DrawRectangle requires color, x, y, width, and height arguments");
                    if (args[0] is not string color)
                        throw new Exception("First argument to DrawRectangle must be a string (color)");
                    if (args[1] is not double x)
                        throw new Exception("Second argument to DrawRectangle must be a number (x)");
                    if (args[2] is not double y)
                        throw new Exception("Third argument to DrawRectangle must be a number (y)");
                    if (args[3] is not double width)
                        throw new Exception("Fourth argument to DrawRectangle must be a number (width)");
                    if (args[4] is not double height)
                        throw new Exception("Fifth argument to DrawRectangle must be a number (height)");
                    double thickness = args.Count > 5 && args[5] is double t ? t : 1;
                    panel.DrawRectangle(color, x, y, width, height, thickness);
                    return null;
                }
                else if (_method == "FillCircle")
                {
                    if (args.Count < 4)
                        throw new Exception("FillCircle requires color, x, y, and radius arguments");
                    if (args[0] is not string color)
                        throw new Exception("First argument to FillCircle must be a string (color)");
                    if (args[1] is not double x)
                        throw new Exception("Second argument to FillCircle must be a number (x)");
                    if (args[2] is not double y)
                        throw new Exception("Third argument to FillCircle must be a number (y)");
                    if (args[3] is not double radius)
                        throw new Exception("Fourth argument to FillCircle must be a number (radius)");
                    panel.FillCircle(color, x, y, radius);
                    return null;
                }
                else if (_method == "DrawCircle")
                {
                    if (args.Count < 4)
                        throw new Exception("DrawCircle requires color, x, y, and radius arguments");
                    if (args[0] is not string color)
                        throw new Exception("First argument to DrawCircle must be a string (color)");
                    if (args[1] is not double x)
                        throw new Exception("Second argument to DrawCircle must be a number (x)");
                    if (args[2] is not double y)
                        throw new Exception("Third argument to DrawCircle must be a number (y)");
                    if (args[3] is not double radius)
                        throw new Exception("Fourth argument to DrawCircle must be a number (radius)");
                    double thickness = args.Count > 4 && args[4] is double t ? t : 1;
                    panel.DrawCircle(color, x, y, radius, thickness);
                    return null;
                }
                else if (_method == "DrawLine")
                {
                    if (args.Count < 5)
                        throw new Exception("DrawLine requires color, x1, y1, x2, and y2 arguments");
                    if (args[0] is not string color)
                        throw new Exception("First argument to DrawLine must be a string (color)");
                    if (args[1] is not double x1)
                        throw new Exception("Second argument to DrawLine must be a number (x1)");
                    if (args[2] is not double y1)
                        throw new Exception("Third argument to DrawLine must be a number (y1)");
                    if (args[3] is not double x2)
                        throw new Exception("Fourth argument to DrawLine must be a number (x2)");
                    if (args[4] is not double y2)
                        throw new Exception("Fifth argument to DrawLine must be a number (y2)");
                    double thickness = args.Count > 5 && args[5] is double t ? t : 1;
                    panel.DrawLine(color, x1, y1, x2, y2, thickness);
                    return null;
                }
                else if (_method == "DrawText")
                {
                    if (args.Count < 4)
                        throw new Exception("DrawText requires text, color, x, and y arguments");
                    if (args[0] is not string text)
                        throw new Exception("First argument to DrawText must be a string (text)");
                    if (args[1] is not string color)
                        throw new Exception("Second argument to DrawText must be a string (color)");
                    if (args[2] is not double x)
                        throw new Exception("Third argument to DrawText must be a number (x)");
                    if (args[3] is not double y)
                        throw new Exception("Fourth argument to DrawText must be a number (y)");
                    double fontSize = args.Count > 4 && args[4] is double fs ? fs : 12;
                    panel.DrawText(text, color, x, y, fontSize);
                    return null;
                }
                else
                {
                    throw new Exception($"Unknown method '{_method}' for Panel object");
                }
            }
            else if (obj is System.Windows.Forms.Timer timer)
            {
                if (_method == "Stop")
                {
                    timer.Stop();
                    return null;
                }
                else if (_method == "Start")
                {
                    timer.Start();
                    return null;
                }
                else if (_method == "Dispose")
                {
                    timer.Dispose();
                    return null;
                }
                else
                {
                    throw new Exception($"Unknown method '{_method}' for Timer object");
                }
            }

            throw new Exception($"Cannot call method '{_method}' on non-object value");
        }
    }
}
