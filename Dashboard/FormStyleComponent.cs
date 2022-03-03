using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Media;

namespace Dashboard
{
    public partial class FormStyleComponent : Component
    {
        #region --Свойства--

        public Form Form { get; set; }

        private FStyle formStyle = FStyle.None;

        public FStyle FormStyle
        {
            get => formStyle;
            set
            {
                formStyle = value;
                Sign();
            }
        }
        public enum FStyle // Набор стилей
        {
            None
        }

        #endregion

        #region --Переменные--

        private readonly Color HeaderColor = Color.FromArgb(24, 30, 54);
        private readonly int HeaderHeight = 30;
        private readonly StringFormat SF = new();
        private readonly Font Font = new("Corbel", 15F, FontStyle.Regular);
        private Size IconSize = new(30, 30);
        private bool MousePressed = false;
        private Point clickPosition; // начальная позиция курсора в момент клика
        private Point moveStartPosition; // начальная позиция формы в момент клика
        private Rectangle rectBtnClose = new();
        private readonly Pen BluePen = new(Color.FromArgb(0, 126, 249)) { Width = 1.55F };
        private bool btnCloseHovered = false;
        private bool btnRollUpHovered = false;
        private bool isOnClosebtn = false;
        private bool isOnRollUpbtn = false;
        private Rectangle rectRollUp = new();
        private bool IconHovered = false; // Наведен ли курсор на иконку
        private Rectangle rectIcon = new();

        #endregion
        public FormStyleComponent()
        {
            InitializeComponent();
        }

        public FormStyleComponent(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        private void Sign()
        {
            if (Form != null)
            {
                Form.Load += Form_Load;
            }
        }

        private void Apply()
        {
            SF.Alignment = StringAlignment.Center;
            SF.LineAlignment = StringAlignment.Center;

            Form.FormBorderStyle = FormBorderStyle.None;
            SetDoubleBuffered(Form);
            OffsetControls();
            Form.Paint += Form_Paint;

            Form.MouseDown += Form_MouseDown;
            Form.MouseUp += Form_MouseUp;
            Form.MouseMove += Form_MouseMove;
            Form.MouseLeave += Form_MouseLeave;
        }

        private void OffsetControls()
        {
            Form.Height += HeaderHeight;

            foreach (Control ctrl in Form.Controls)
            {
                ctrl.Location = new Point(ctrl.Location.X, ctrl.Location.Y + HeaderHeight);
                ctrl.Refresh();
            }
        }

        #region --Form Ivents--
        private void Form_Load(object sender, EventArgs e) // чтобы стиль применялся при загрузке формы
        {
            Apply();
        }

        private void Form_Paint(object sender, PaintEventArgs e)
        {
            DrawStyle(e.Graphics);
        }

        private void Form_MouseLeave(object sender, EventArgs e)
        {
            btnCloseHovered = false;
            Form.Invalidate();
        }

        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (rectBtnClose.Contains(e.Location) || rectRollUp.Contains(e.Location))
                MousePressed = false;
            if (MousePressed && e.Button == MouseButtons.Left)
            {
                var frmOffset = new Size(Point.Subtract(Cursor.Position, new Size(clickPosition))); // перемещение окна
                Form.Location = Point.Add(moveStartPosition, frmOffset);
            }
            else
            {
                if (rectBtnClose.Contains(e.Location)) // проверка наведения мыши на крестик
                {
                    if (btnCloseHovered == false)
                    {
                        btnCloseHovered = true;
                        Form.Invalidate();
                    }
                }
                else
                {
                    if (btnCloseHovered == true)
                    {
                        btnCloseHovered = false;
                        Form.Invalidate();
                    }
                }

                if (rectRollUp.Contains(e.Location)) // проверка наведения мыши на кнопку свернуть
                {
                    if (btnRollUpHovered == false)
                    {
                        btnRollUpHovered = true;
                        Form.Invalidate();
                    }
                }
                else
                {
                    if (btnRollUpHovered == true)
                    {
                        btnRollUpHovered = false;
                        Form.Invalidate();
                    }
                }
            }

        }
        private void Form_MouseUp(object sender, MouseEventArgs e)
        {
            MousePressed = false;
            if (e.Button == MouseButtons.Left)
            {
                if (rectBtnClose.Contains(e.Location) && isOnClosebtn)
                {
                    isOnClosebtn = false;
                    Form.Close();

                }
                else if (rectRollUp.Contains(e.Location) && isOnRollUpbtn)
                {
                    isOnRollUpbtn = false;
                    Form.WindowState = FormWindowState.Minimized;
                }

            }

            if (e.Button == MouseButtons.Right)
            {
                if (rectIcon.Contains(e.Location) && IconHovered) // проверка наведения мыши на иконку
                {
                    IconHovered = false;
                    var Runduksound = new SoundPlayer(Properties.Resources.Runduk);
                    Runduksound.Play();
                    
                    MessageBox.Show("Ключ для рундука. Рундук - сундук для рун. Рундук");
                    
                }
            }
        }

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            isOnClosebtn = false;
            isOnRollUpbtn = false;
            IconHovered = false;
            if (e.Location.Y <= HeaderHeight)
            {
                MousePressed = true;
                clickPosition = Cursor.Position;
                moveStartPosition = Form.Location;
                if (e.Button == MouseButtons.Left && rectBtnClose.Contains(e.Location))
                    isOnClosebtn = true;
                else if (e.Button == MouseButtons.Left && rectRollUp.Contains(e.Location))
                    isOnRollUpbtn = true;
                else if (e.Button == MouseButtons.Right && rectIcon.Contains(e.Location))
                    IconHovered = true;
            }

        }

        #endregion

        private void DrawStyle(Graphics graph)
        {
            graph.SmoothingMode = SmoothingMode.HighQuality;

            var rectHeader = new Rectangle(0, 0, Form.Width - 1, HeaderHeight);
            rectIcon = new Rectangle(rectHeader.Height - IconSize.Width, rectHeader.Height - IconSize.Height, IconSize.Width, IconSize.Height);

            // размер кнопки закрытия программы
            rectBtnClose = new Rectangle(rectHeader.Width - rectHeader.Height - 15, rectHeader.Y, rectHeader.Height + 15, rectHeader.Height);

            var rectCrosshair = new Rectangle(
                rectBtnClose.X + rectBtnClose.Width / 2 - 5,
                rectBtnClose.Height / 2 - 5,
                10, 10);

            // размер кнопки "свернуть окно"
            rectRollUp = new Rectangle(rectHeader.Width - rectHeader.Height - 61, rectHeader.Y, rectHeader.Height + 15, rectHeader.Height);

            // Шапка
            graph.DrawRectangle(new Pen(HeaderColor), rectHeader);
            graph.FillRectangle(new SolidBrush(HeaderColor), rectHeader);

            // Заголовок
            graph.DrawString("RSA Dashboard", Font, new SolidBrush(Color.FromArgb(0, 126, 249)), rectHeader, SF);

            // Иконка
            graph.DrawImage(Form.Icon.ToBitmap(), rectIcon);

            // Кнопка X
            graph.DrawRectangle(new Pen(btnCloseHovered ? Color.FromArgb(154, 6, 4) : HeaderColor), rectBtnClose);
            graph.FillRectangle(new SolidBrush(btnCloseHovered ? Color.FromArgb(154, 6, 4) : HeaderColor), rectBtnClose);
            DrawCrosshair(graph, rectCrosshair, BluePen);

            // Свернуть окно
            graph.DrawRectangle(new Pen(btnRollUpHovered ? Color.FromArgb(34, 40, 84) : HeaderColor), rectRollUp);
            graph.FillRectangle(new SolidBrush(btnRollUpHovered ? Color.FromArgb(34, 40, 84) : HeaderColor), rectRollUp);
            DrawLine(graph, rectRollUp, BluePen);
        }

        private void DrawLine(Graphics graph, Rectangle rect, Pen pen)
        {
            graph.DrawLine(pen, rect.X + rect.Width / 2 - 8, rect.Y + rect.Height / 2, rect.X + rect.Width - 14, rect.Y + rect.Height / 2);
        }

        private void DrawCrosshair(Graphics graph, Rectangle rect, Pen pen)
        {
            graph.DrawLine(pen, rect.X, rect.Y, rect.X + rect.Width, rect.Y + rect.Height);
            graph.DrawLine(pen, rect.X + rect.Width, rect.Y, rect.X, rect.Y + rect.Height);
        }
        public static void SetDoubleBuffered(Control c) // чтобы не было мерцания при наведении на кнопку
        {
            if (SystemInformation.TerminalServerSession)
                return;

            System.Reflection.PropertyInfo pDoubleBuffered =
                  typeof(Control).GetProperty(
                        "DoubleBuffered",
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance);

            pDoubleBuffered.SetValue(c, true, null);
        }
    }
}
