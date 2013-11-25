using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkillAPITool {

    /// <summary>
    /// Attribute for a skill
    /// </summary>
    public class Attribute {

        private string key;
        private int initial;
        private int scale;

        /// <summary>
        /// Key of the attribute
        /// </summary>
        public string Key {
            get { return key; }
            set { key = value; }
        }

        /// <summary>
        /// Initial value of the attribute
        /// </summary>
        public int Initial {
            get { return initial; }
            set { initial = value; }
        }

        /// <summary>
        /// Bonus value per level
        /// </summary>
        public int Scale {
            get { return scale; }
            set { scale = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">attribute key</param>
        /// <param name="initial">initial value</param>
        /// <param name="scale">bonus value per level</param>
        public Attribute(string key, int initial, int scale) {
            this.key = key;
            this.initial = initial;
            this.scale = scale;
        }
    }
}
