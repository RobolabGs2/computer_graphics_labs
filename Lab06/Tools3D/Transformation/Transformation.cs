using Lab06.Base3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Point = Lab06.Base3D.Point;

namespace Lab06.Tools3D.Transformation
{
    class Transformation : IToolPage
    {
        public Bitmap Image => Properties.Resources.Move;
        Gizmo gizmo;

        public void Init(ToolTab tab, Context context)
        {
            gizmo = new Gizmo(context);
            foreach (var (icon, state) in new[]
            {
                (Properties.Resources.Move, Gizmo.State.Move),
                (Properties.Resources.Scale, Gizmo.State.Scale),
                (Properties.Resources.Rotate, Gizmo.State.Rotate),
            })
            {
                var button = tab.AddButton(icon);
                button.ButtonClick += b => gizmo.CurrentState = state;
                button.ButtonDisable += b => gizmo.CurrentState = Gizmo.State.Unset;
            }

            var surface = tab.AddButton(Properties.Resources.SolidOfRev);
            surface.ButtonClick += button =>
            {
                if (!(context.world.selected.FirstOrDefault() is Spline))
                {
                    MessageBox.Show($"Нужно выбрать один сплайн", "Окошко-всплывашка", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    surface.ButtonEnable = false;
                    return;
                }

                gizmo.CurrentState = Gizmo.State.Axes;
            };
            surface.ButtonDisable += button => gizmo.CurrentState = Gizmo.State.Unset;

            var onlyGizmo = new CheckBox
                {Text = "Change gizmo", ForeColor = Constants.textColore, Font = Constants.font, Dock = DockStyle.Top};
            onlyGizmo.CheckStateChanged += (sender, args) => { gizmo.OnlyGizmo = onlyGizmo.Checked; };
            gizmo.CurrentStateChanged += state =>
                onlyGizmo.Enabled = state != Gizmo.State.Unset && state != Gizmo.State.Axes;
            var resetGizmo = new Button()
            {
                Text = "Reset gizmo", ForeColor = Constants.textColore, Font = Constants.font, Dock = DockStyle.Top,
                AutoSize = true
            };
            resetGizmo.Click += (sender, args) =>
            {
                gizmo.RestartDirection();
                gizmo.CurrentState = gizmo.CurrentState;
            };

            // TODO: по-очереди перебирать точки
            var moveGizmoToPoint = new Button
            {
                Text = "Gizmo to first point", ForeColor = Constants.textColore, Font = Constants.font,
                Dock = DockStyle.Top,
                AutoSize = true
            };
            gizmo.CurrentStateChanged += state =>
                moveGizmoToPoint.Enabled = context.world.selected.Count == 1;
            moveGizmoToPoint.Click += (sender, args) =>
            {
                gizmo.RestartDirection(context.world.selected.First().Points().First());
                gizmo.CurrentState = gizmo.CurrentState;
                context.Redraw();
            };

            var xyzSettings = new TableLayoutPanel
            {
                ColumnCount = 3, RowCount = 3, Dock = DockStyle.Top, ForeColor = Constants.textColore,
                AutoSize = true,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single
            };
            var axes = new[] {("X", Constants.xColor), ("Y", Constants.yColor), ("Z", Constants.zColor)};
            xyzSettings.Controls.AddRange(axes.Select(s => (Control) new Label
                {Text = s.Item1, TextAlign = ContentAlignment.TopCenter, Font = Constants.font}).ToArray());
            xyzSettings.SizeChanged += (sender, args) =>
            {
                for (int i = 0; i < xyzSettings.Controls.Count; i++)
                    xyzSettings.Controls[i].Width = xyzSettings.ClientSize.Width / 3 - 8;
            };
            var xyzInput = axes.Select(s =>
            {
                var textBox = new TextBox {Text = "", Font = Constants.font, ForeColor = s.Item2};
                gizmo.CurrentStateChanged += state =>
                {
                    textBox.Visible = state != Gizmo.State.Axes;
                    switch (state)
                    {
                        case Gizmo.State.Unset:
                            textBox.Text = "-.-";
                            break;
                        case Gizmo.State.Move:
                        case Gizmo.State.Rotate:
                            textBox.Text = "0.0";
                            break;
                        case Gizmo.State.Scale:
                            textBox.Text = "1.0";
                            break;
                        case Gizmo.State.Axes:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(state), state, null);
                    }
                };
                return textBox;
            }).ToArray();
            var xyzCheckboxes = axes.Select(s =>
            {
                var checkBox = new RadioButton {CheckAlign = ContentAlignment.MiddleCenter, BackColor = s.Item2};
                gizmo.CurrentStateChanged += state => checkBox.Visible = state == Gizmo.State.Axes;
                return checkBox;
            }).ToArray();
            xyzSettings.Controls.AddRange(xyzInput);
            xyzSettings.Controls.AddRange(xyzCheckboxes);

            var surfaceCountTextBox = new TextBox {Text = "3", Dock = DockStyle.Right};
            var surfaceSettings = new Panel
            {
                Dock = DockStyle.Top, Height = 32,
                Controls =
                {
                    new Label
                    {
                        Font = Constants.font, ForeColor = Constants.textColore, Text = "Surface count: ",
                        AutoSize = true, Dock = DockStyle.Left
                    },
                    surfaceCountTextBox
                }
            };
            gizmo.CurrentStateChanged += state => surfaceSettings.Visible = state == Gizmo.State.Axes;
            var applyXYZ = new Button
            {
                Text = "Apply",
                ForeColor = Constants.textColore,
                Font = Constants.font,
                Dock = DockStyle.Top,
                AutoSize = true
            };
            gizmo.CurrentStateChanged += state =>
            {
                applyXYZ.Enabled = state != Gizmo.State.Unset;
                xyzSettings.Enabled = state != Gizmo.State.Unset;
            };
            applyXYZ.Click += (sender, args) =>
            {
                try
                {
                    if (gizmo.CurrentState == Gizmo.State.Axes)
                    {
                        if (!xyzCheckboxes.Select(button => button.Checked).Any(b => b))
                        {
                            throw new Exception("Не выбрана ось");
                        }
                        var count = int.Parse(surfaceCountTextBox.Text);
                        if (count < 3)
                        {
                            throw new Exception("Количество должно быть не меньше трёх");
                        }
                        var angel = 2 * Math.PI / count;
                        var spline = context.world.selected.First() as Spline;

                        var rotation = gizmo.RotateMatrix(
                                xyzCheckboxes[0].Checked ? angel : 0,
                                xyzCheckboxes[1].Checked ? angel : 0,
                                xyzCheckboxes[2].Checked ? angel : 0)
                            .movingMatrix;
                        var points = Enumerable.Range(0, count).Aggregate(new List<List<Point>>{spline.points},
                            (list, i) =>
                            {
                                list.Add(list[list.Count - 1].Select(p => p * rotation).ToList());
                                return list;
                            }).SelectMany((i) => i).ToList();
                        var result = new Polytope {points = points};
                        var pointsCount = spline.points.Count;
                        for (var i = 1; i < pointsCount; i++)
                        {
                            result.polygons.AddRange(Enumerable.Range(0, count).Select(j => new Polygon
                            (new int[]{
                                    j * pointsCount + i, ((j + 1) % count) * pointsCount + i,
                                    ((j + 1) % count) * pointsCount + i - 1, j * pointsCount + i - 1
                                }
                            )));
                        }
                        context.world.entities.Add(result);
                        context.world.selected.Clear();
                        context.world.selected.Add(result);
                        context.Redraw();
                        surface.ButtonEnable = false;
                        return;
                    }
                    gizmo.Transform(
                        double.Parse(xyzInput[0].Text, NumberStyles.Float, CultureInfo.InvariantCulture),
                        double.Parse(xyzInput[1].Text, NumberStyles.Float, CultureInfo.InvariantCulture),
                        double.Parse(xyzInput[2].Text, NumberStyles.Float, CultureInfo.InvariantCulture));
                    context.Redraw();
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Не удалось применить: {e.Message}", "Окошко-всплывашка", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            };

            tab.Settings.Controls.AddRange(new Control[]
                {applyXYZ, surfaceSettings, xyzSettings, moveGizmoToPoint, onlyGizmo, resetGizmo}.ToArray());

            tab.TabSelected += () => gizmo.RestartDirection();
            tab.TabDeselected += () => gizmo.CurrentState = Gizmo.State.Unset;
            gizmo.CurrentState = Gizmo.State.Unset;
        }
    }
}