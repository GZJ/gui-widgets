using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ToolTip
{
    public class ToolbarForm : Form
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out Point lpPoint);

        private const int HOTKEY_ID = 9000;
        private Label infoLabel;
        private uint exitKeyModifiers;
        private uint exitKeyCode;

        public ToolbarForm(
            string labelText,
            uint exitKeyModifiers,
            uint exitKeyCode,
            Color fgColor,
            Color bgColor,
            Size size,
            int fontSize,
            Point? location = null
        )
        {
            this.exitKeyModifiers = exitKeyModifiers;
            this.exitKeyCode = exitKeyCode;

            this.Size = size;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;

            if (location.HasValue)
            {
                this.Location = location.Value;
            }
            else
            {
                Point cursorPosition;
                GetCursorPos(out cursorPosition);
                this.Location = cursorPosition;
            }

            this.ShowInTaskbar = false;
            this.TopMost = true;

            this.SetStyle(ControlStyles.Selectable, false);

            infoLabel = new Label();
            infoLabel.Dock = DockStyle.Fill;
            infoLabel.TextAlign = ContentAlignment.MiddleCenter;
            infoLabel.Font = new Font("Arial", fontSize);
            infoLabel.Text = labelText;
            infoLabel.ForeColor = fgColor;

            this.BackColor = bgColor;

            this.Controls.Add(infoLabel);

            RegisterHotKey(this.Handle, HOTKEY_ID, exitKeyModifiers, exitKeyCode);

            this.MouseDown += new MouseEventHandler(Form_MouseDown);
            this.MouseMove += new MouseEventHandler(Form_MouseMove);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x80;
                return cp;
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0x0312 && m.WParam.ToInt32() == HOTKEY_ID)
            {
                this.Close();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            UnregisterHotKey(this.Handle, HOTKEY_ID);
            base.OnFormClosing(e);
        }

        private Point lastLocation;

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            lastLocation = e.Location;
        }

        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new Point(
                    (this.Location.X - lastLocation.X) + e.X,
                    (this.Location.Y - lastLocation.Y) + e.Y
                );

                this.Update();
            }
        }

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string label = "ToolTip";
            uint exitKeyModifiers = 0;
            uint exitKeyCode = 0x0D; // VK_RETURN
            Color fgColor = Color.White;
            Color bgColor = Color.FromArgb(64, 64, 64);
            int width = 200;
            int height = 30;
            int fontSize = 10;
            Point? location = null;

            for (int i = 0; i < args.Length; i += 2)
            {
                if (i + 1 < args.Length)
                {
                    switch (args[i].ToLower())
                    {
                        case "--label":
                            label = args[i + 1];
                            break;
                        case "--exit-key":
                            ParseExitKey(args[i + 1], out exitKeyModifiers, out exitKeyCode);
                            break;
                        case "--fg-color":
                            fgColor = ParseColor(args[i + 1]);
                            break;
                        case "--bg-color":
                            bgColor = ParseColor(args[i + 1]);
                            break;
                        case "--width":
                            width = int.Parse(args[i + 1]);
                            break;
                        case "--height":
                            height = int.Parse(args[i + 1]);
                            break;
                        case "--font-size":
                            fontSize = int.Parse(args[i + 1]);
                            break;
                        case "--x":
                            if (location == null)
                                location = new Point();
                            location = new Point(int.Parse(args[i + 1]), location.Value.Y);
                            break;
                        case "--y":
                            if (location == null)
                                location = new Point();
                            location = new Point(location.Value.X, int.Parse(args[i + 1]));
                            break;
                    }
                }
            }

            Application.Run(
                new ToolbarForm(
                    label,
                    exitKeyModifiers,
                    exitKeyCode,
                    fgColor,
                    bgColor,
                    new Size(width, height),
                    fontSize,
                    location
                )
            );
        }

        private static void ParseExitKey(string keyString, out uint modifiers, out uint keyCode)
        {
            modifiers = 0;
            keyCode = 0;
            string[] parts = keyString.ToLower().Split('+');

            foreach (string part in parts)
            {
                switch (part.Trim())
                {
                    case "ctrl":
                        modifiers |= 0x0002;
                        break;
                    case "alt":
                        modifiers |= 0x0001;
                        break;
                    case "shift":
                        modifiers |= 0x0004;
                        break;
                    default:
                        if (part.Length == 1 && char.IsLetter(part[0]))
                        {
                            keyCode = (uint)part.ToUpper()[0];
                        }
                        else { }
                        break;
                }
            }

            if (keyCode == 0)
            {
                throw new ArgumentException("Invalid exit key format");
            }
        }

        private static Color ParseColor(string colorString)
        {
            if (colorString.StartsWith("#"))
            {
                return ColorTranslator.FromHtml(colorString);
            }
            else
            {
                return Color.FromName(colorString);
            }
        }
    }
}
