using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace zds.Core
{
    public class Window
    {
        private Form _form;
        private int _refreshRate = 60;
        private bool _fullScreen = false;
        private string _backgroundColor = "White";
        private Dictionary<string, Function> _keyDownHandlers = new Dictionary<string, Function>();
        private Dictionary<string, Function> _keyUpHandlers = new Dictionary<string, Function>();

        public Window(int width, int height)
        {
            _form = new Form
            {
                Width = width,
                Height = height,
                Text = "ZD# Window",
                StartPosition = FormStartPosition.CenterScreen,
                BackColor = Color.White
            };

            // Add key event handlers
            _form.KeyPreview = true;
            _form.KeyDown += Form_KeyDown;
            _form.KeyUp += Form_KeyUp;
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            string keyName = e.KeyCode.ToString();
            if (_keyDownHandlers.TryGetValue(keyName, out Function handler))
            {
                try
                {
                    // Call the handler with the key name as argument
                    handler.Call(Program.CurrentInterpreter, new List<object?> { keyName });
                }
                catch (Exception ex)
                {
                    Log.Error($"Error in key down handler: {ex.Message}");
                }
            }
        }

        private void Form_KeyUp(object sender, KeyEventArgs e)
        {
            string keyName = e.KeyCode.ToString();
            if (_keyUpHandlers.TryGetValue(keyName, out Function handler))
            {
                try
                {
                    // Call the handler with the key name as argument
                    handler.Call(Program.CurrentInterpreter, new List<object?> { keyName });
                }
                catch (Exception ex)
                {
                    Log.Error($"Error in key up handler: {ex.Message}");
                }
            }
        }

        public bool FullScreen
        {
            get => _fullScreen;
            set
            {
                _fullScreen = value;
                if (_fullScreen)
                {
                    _form.FormBorderStyle = FormBorderStyle.None;
                    _form.WindowState = FormWindowState.Maximized;
                }
                else
                {
                    _form.FormBorderStyle = FormBorderStyle.Sizable;
                    _form.WindowState = FormWindowState.Normal;
                }
            }
        }

        public int Width
        {
            get => _form.Width;
            set => _form.Width = value;
        }

        public int Height
        {
            get => _form.Height;
            set => _form.Height = value;
        }

        public int MaxRefreshRate
        {
            get => _refreshRate;
            set => _refreshRate = value;
        }

        public string BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                try
                {
                    _form.BackColor = Color.FromName(value);
                }
                catch
                {
                    // If color name is invalid, default to white
                    _form.BackColor = Color.White;
                }
            }
        }

        public void Show()
        {
            _form.Show();
        }

        public void Close()
        {
            _form.Close();
        }

        public void SetTitle(string title)
        {
            _form.Text = title;
        }

        public void AddLabel(string text, int x, int y)
        {
            var label = new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true
            };
            _form.Controls.Add(label);
        }

        public void AddButton(string text, int x, int y, Action onClick)
        {
            var button = new Button
            {
                Text = text,
                Location = new Point(x, y)
            };
            button.Click += (sender, e) => onClick();
            _form.Controls.Add(button);
        }

        public void OnKeyDown(string key, Function handler)
        {
            _keyDownHandlers[key] = handler;
        }

        public void OnKeyUp(string key, Function handler)
        {
            _keyUpHandlers[key] = handler;
        }

        public Form GetForm()
        {
            return _form;
        }
    }

    public class Panel
    {
        private PictureBox _pictureBox;
        private Bitmap _bitmap;
        private Graphics _graphics;

        public Panel(Window window, int width, int height)
        {
            _pictureBox = new PictureBox
            {
                Width = width,
                Height = height,
                Location = new Point(0, 0),
                BackColor = Color.Transparent
            };

            _bitmap = new Bitmap(width, height);
            _pictureBox.Image = _bitmap;
            _graphics = Graphics.FromImage(_bitmap);

            window.GetForm().Controls.Add(_pictureBox);
            _pictureBox.Dock = DockStyle.Fill;
        }

        public void Clear()
        {
            _graphics.Clear(Color.Transparent);
        }

        public void FillRectangle(string colorName, double x, double y, double width, double height)
        {
            try
            {
                Color color = Color.FromName(colorName);
                _graphics.FillRectangle(new SolidBrush(color), (float)x, (float)y, (float)width, (float)height);
            }
            catch
            {
                // If color name is invalid, default to black
                _graphics.FillRectangle(new SolidBrush(Color.Black), (float)x, (float)y, (float)width, (float)height);
            }
        }

        public void DrawRectangle(string colorName, double x, double y, double width, double height, double thickness = 1)
        {
            try
            {
                Color color = Color.FromName(colorName);
                _graphics.DrawRectangle(new Pen(color, (float)thickness), (float)x, (float)y, (float)width, (float)height);
            }
            catch
            {
                // If color name is invalid, default to black
                _graphics.DrawRectangle(new Pen(Color.Black, (float)thickness), (float)x, (float)y, (float)width, (float)height);
            }
        }

        public void FillCircle(string colorName, double x, double y, double radius)
        {
            try
            {
                Color color = Color.FromName(colorName);
                _graphics.FillEllipse(new SolidBrush(color), (float)(x - radius), (float)(y - radius), (float)(radius * 2), (float)(radius * 2));
            }
            catch
            {
                // If color name is invalid, default to black
                _graphics.FillEllipse(new SolidBrush(Color.Black), (float)(x - radius), (float)(y - radius), (float)(radius * 2), (float)(radius * 2));
            }
        }

        public void DrawCircle(string colorName, double x, double y, double radius, double thickness = 1)
        {
            try
            {
                Color color = Color.FromName(colorName);
                _graphics.DrawEllipse(new Pen(color, (float)thickness), (float)(x - radius), (float)(y - radius), (float)(radius * 2), (float)(radius * 2));
            }
            catch
            {
                // If color name is invalid, default to black
                _graphics.DrawEllipse(new Pen(Color.Black, (float)thickness), (float)(x - radius), (float)(y - radius), (float)(radius * 2), (float)(radius * 2));
            }
        }

        public void DrawLine(string colorName, double x1, double y1, double x2, double y2, double thickness = 1)
        {
            try
            {
                Color color = Color.FromName(colorName);
                _graphics.DrawLine(new Pen(color, (float)thickness), (float)x1, (float)y1, (float)x2, (float)y2);
            }
            catch
            {
                // If color name is invalid, default to black
                _graphics.DrawLine(new Pen(Color.Black, (float)thickness), (float)x1, (float)y1, (float)x2, (float)y2);
            }
        }

        public void DrawText(string text, string colorName, double x, double y, double fontSize = 12)
        {
            try
            {
                Color color = Color.FromName(colorName);
                Font font = new Font("Arial", (float)fontSize);
                _graphics.DrawString(text, font, new SolidBrush(color), (float)x, (float)y);
            }
            catch
            {
                // If color name is invalid, default to black
                Font font = new Font("Arial", (float)fontSize);
                _graphics.DrawString(text, font, new SolidBrush(Color.Black), (float)x, (float)y);
            }
        }

        public void Update()
        {
            _pictureBox.Invalidate();
        }
    }
}