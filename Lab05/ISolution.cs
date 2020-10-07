using System.Drawing;
using System.Windows.Forms;

namespace Lab05
{
    public interface ISolution
    {
        string Name { get; }
        Control[] Controls { get; }
        Size Size { set; }
    }

    public abstract class AbstractSolution : ISolution
    {
        public string Name { get; }
        public abstract Control[] Controls { get; }
        public abstract Size Size { set; }

        protected AbstractSolution(string name)
        {
            Name = name;
        }
    }

    public class StubSolution : AbstractSolution
    {
        public StubSolution(string name) : base(name)
        {
            Controls = new Control[]
            {
                new Label {Text = name, Anchor = AnchorStyles.Left | AnchorStyles.Top, AutoSize = true},
                new Label {Text = name, Anchor = AnchorStyles.Right | AnchorStyles.Bottom, AutoSize = true},
                new Label {Text = @"Заглушка", Anchor = AnchorStyles.Top, AutoSize = true},
            };
        }

        public override Control[] Controls { get; }
        public override Size Size { set {}  }
    }
}