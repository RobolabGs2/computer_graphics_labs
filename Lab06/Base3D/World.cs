using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab06.Base3D
{
    public class World
    {
        /// <summary>
        /// Сущности, находящиеся в мире, с которыми пользователь может повзаимодействовать
        /// </summary>
        public HashSet<Entity> entities = new HashSet<Entity>();
        /// <summary>
        /// выделенный сущности, должны быть из entities
        /// </summary>
        public HashSet<Entity> selected = new HashSet<Entity>();
        /// <summary>
        /// Сущности декоративные или технические, их нельзя выделить и пошевелить.
        /// Такие как стрелочки в гзмо, координатные оси, решётка
        /// </summary>
        public HashSet<Entity> control = new HashSet<Entity>();

    }
}
