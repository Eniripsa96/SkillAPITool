using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkillAPITool {

    /// <summary>
    /// Value for a skill
    /// </summary>
    public class Value {

        private string key;
        private int value;

        /// <summary>
        /// Key of the attribute
        /// </summary>
        public string Key {
            get { return key; }
        }

        /// <summary>
        /// Value
        /// </summary>
        public int Number {
            get { return value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">value key</param>
        /// <param name="value">value</param>
        public Value(string key, int value) {
            this.key = key;
            this.value = value;
        }
    }
}
