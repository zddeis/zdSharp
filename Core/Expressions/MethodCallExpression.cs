using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using zds.Core.Exceptions;

namespace zds.Core.Expressions
{
    public class MethodCallExpression : IExpression
    {
        private readonly IExpression _object;
        private readonly string _method;
        private readonly List<IExpression> _arguments;
        public int Line { get; }

        public MethodCallExpression(IExpression obj, string method, List<IExpression> arguments, int line = 0)
        {
            _object = obj;
            _method = method;
            _arguments = arguments;
            Line = line;
        }

        public object? Evaluate()
        {
            try
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
                            throw new RuntimeException("SetTitle requires a string argument", Line);
                        if (args[0] is not string title)
                            throw new RuntimeException("SetTitle requires a string argument", Line);
                        window.SetTitle(title);
                        return null;
                    }
                    else if (_method == "AddLabel")
                    {
                        if (args.Count < 3)
                            throw new RuntimeException("AddLabel requires text, x, and y arguments", Line);
                        if (args[0] is not string text)
                            throw new RuntimeException("First argument to AddLabel must be a string", Line);
                        if (args[1] is not double x)
                            throw new RuntimeException("Second argument to AddLabel must be a number", Line);
                        if (args[2] is not double y)
                            throw new RuntimeException("Third argument to AddLabel must be a number", Line);
                        window.AddLabel(text, (int)x, (int)y);
                        return null;
                    }
                    else
                    {
                        throw new RuntimeException($"Unknown method '{_method}' for Window object", Line);
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
                            throw new RuntimeException("FillRectangle requires color, x, y, width, and height arguments", Line);
                        if (args[0] is not string color)
                            throw new RuntimeException("First argument to FillRectangle must be a string (color)", Line);
                        if (args[1] is not double x)
                            throw new RuntimeException("Second argument to FillRectangle must be a number (x)", Line);
                        if (args[2] is not double y)
                            throw new RuntimeException("Third argument to FillRectangle must be a number (y)", Line);
                        if (args[3] is not double width)
                            throw new RuntimeException("Fourth argument to FillRectangle must be a number (width)", Line);
                        if (args[4] is not double height)
                            throw new RuntimeException("Fifth argument to FillRectangle must be a number (height)", Line);
                        panel.FillRectangle(color, x, y, width, height);
                        return null;
                    }
                    else if (_method == "DrawRectangle")
                    {
                        if (args.Count < 5)
                            throw new RuntimeException("DrawRectangle requires color, x, y, width, and height arguments", Line);
                        if (args[0] is not string color)
                            throw new RuntimeException("First argument to DrawRectangle must be a string (color)", Line);
                        if (args[1] is not double x)
                            throw new RuntimeException("Second argument to DrawRectangle must be a number (x)", Line);
                        if (args[2] is not double y)
                            throw new RuntimeException("Third argument to DrawRectangle must be a number (y)", Line);
                        if (args[3] is not double width)
                            throw new RuntimeException("Fourth argument to DrawRectangle must be a number (width)", Line);
                        if (args[4] is not double height)
                            throw new RuntimeException("Fifth argument to DrawRectangle must be a number (height)", Line);
                        double thickness = args.Count > 5 && args[5] is double t ? t : 1;
                        panel.DrawRectangle(color, x, y, width, height, thickness);
                        return null;
                    }
                    else if (_method == "FillCircle")
                    {
                        if (args.Count < 4)
                            throw new RuntimeException("FillCircle requires color, x, y, and radius arguments", Line);
                        if (args[0] is not string color)
                            throw new RuntimeException("First argument to FillCircle must be a string (color)", Line);
                        if (args[1] is not double x)
                            throw new RuntimeException("Second argument to FillCircle must be a number (x)", Line);
                        if (args[2] is not double y)
                            throw new RuntimeException("Third argument to FillCircle must be a number (y)", Line);
                        if (args[3] is not double radius)
                            throw new RuntimeException("Fourth argument to FillCircle must be a number (radius)", Line);
                        panel.FillCircle(color, x, y, radius);
                        return null;
                    }
                    else if (_method == "DrawCircle")
                    {
                        if (args.Count < 4)
                            throw new RuntimeException("DrawCircle requires color, x, y, and radius arguments", Line);
                        if (args[0] is not string color)
                            throw new RuntimeException("First argument to DrawCircle must be a string (color)", Line);
                        if (args[1] is not double x)
                            throw new RuntimeException("Second argument to DrawCircle must be a number (x)", Line);
                        if (args[2] is not double y)
                            throw new RuntimeException("Third argument to DrawCircle must be a number (y)", Line);
                        if (args[3] is not double radius)
                            throw new RuntimeException("Fourth argument to DrawCircle must be a number (radius)", Line);
                        double thickness = args.Count > 4 && args[4] is double t ? t : 1;
                        panel.DrawCircle(color, x, y, radius, thickness);
                        return null;
                    }
                    else if (_method == "DrawLine")
                    {
                        if (args.Count < 5)
                            throw new RuntimeException("DrawLine requires color, x1, y1, x2, and y2 arguments", Line);
                        if (args[0] is not string color)
                            throw new RuntimeException("First argument to DrawLine must be a string (color)", Line);
                        if (args[1] is not double x1)
                            throw new RuntimeException("Second argument to DrawLine must be a number (x1)", Line);
                        if (args[2] is not double y1)
                            throw new RuntimeException("Third argument to DrawLine must be a number (y1)", Line);
                        if (args[3] is not double x2)
                            throw new RuntimeException("Fourth argument to DrawLine must be a number (x2)", Line);
                        if (args[4] is not double y2)
                            throw new RuntimeException("Fifth argument to DrawLine must be a number (y2)", Line);
                        double thickness = args.Count > 5 && args[5] is double t ? t : 1;
                        panel.DrawLine(color, x1, y1, x2, y2, thickness);
                        return null;
                    }
                    else if (_method == "DrawText")
                    {
                        if (args.Count < 4)
                            throw new RuntimeException("DrawText requires text, color, x, and y arguments", Line);
                        if (args[0] is not string text)
                            throw new RuntimeException("First argument to DrawText must be a string (text)", Line);
                        if (args[1] is not string color)
                            throw new RuntimeException("Second argument to DrawText must be a string (color)", Line);
                        if (args[2] is not double x)
                            throw new RuntimeException("Third argument to DrawText must be a number (x)", Line);
                        if (args[3] is not double y)
                            throw new RuntimeException("Fourth argument to DrawText must be a number (y)", Line);
                        double fontSize = args.Count > 4 && args[4] is double fs ? fs : 12;
                        panel.DrawText(text, color, x, y, fontSize);
                        return null;
                    }
                    else
                    {
                        throw new RuntimeException($"Unknown method '{_method}' for Panel object", Line);
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
                        throw new RuntimeException($"Unknown method '{_method}' for Timer object", Line);
                    }
                }

                throw new RuntimeException($"Cannot call method '{_method}' on non-object value", Line);
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