using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Lab03
{
    public interface IDrawingTool
    {
        // Название будет на кнопке
        string Name { get; }
        // Глобальная установка цвета
        Color Color { set; }

        // Установка родительского контейнера
        void Init(Control container);
        //  Нажатие мышки по битмапу
        void MouseDown(int x, int y, FastBitmap bitmap);
        //  Отпускание мышки
        void MouseUp(int x, int y, FastBitmap bitmap);
        //  Шевеление мышки по битмапу
        void MouseMove(int x, int y, FastBitmap bitmap);
        //  Выбран инструмент
        void Start(FastBitmap bitmap);
    }
}
