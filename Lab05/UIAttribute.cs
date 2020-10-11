using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab05
{
    class UIAttribute : System.Attribute
    {
        public readonly string Name;

        public UIAttribute(string name)
        {
            Name = name;
        }

        public static Font Font = new Font("Consolas", 10);

        public static IEnumerable<(Label label, TextBox text, Action update)> GetControlsByProperties(object optimizer)
        {
            foreach (var property in optimizer.GetType().GetFields())
            {
                foreach (var attribute in property
                    .CustomAttributes
                    .Where(a => a.AttributeType == typeof(UIAttribute)))
                {
                    var name = attribute.ConstructorArguments[0].Value.ToString();
                    var label = new Label {Text = name, Font = Font,  TextAlign = ContentAlignment.MiddleCenter };
                    var edit = new TextBox {Font = Font, Text = property.GetValue(optimizer).ToString()};
                    edit.TextChanged += (sender, args) =>
                    {
                        var value = edit.Text.Trim();
                        if (value.Length == 0)
                            return;
                        try
                        {
                            property.SetValue(optimizer,
                                Convert.ChangeType(value, property.FieldType));
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show($"Не смогли распарсить '{name}': {e.Message}", "Беда",
                                MessageBoxButtons.OK);
                        }
                    };
                    yield return (label, edit, () => edit.Text = property.GetValue(optimizer).ToString());
                }
            }
        }
    }
}