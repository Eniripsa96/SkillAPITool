using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkillAPITool {

    /// <summary>
    /// Value for a skill
    /// </summary>
    public class StringValue {

        private string key;
        private string value;

        /// <summary>
        /// Key of the attribute
        /// </summary>
        public string Key {
            get { return key; }
        }

        /// <summary>
        /// Value
        /// </summary>
        public string String {
            get { return value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">value key</param>
        /// <param name="value">value</param>
        public StringValue(string key, string value) {
            this.key = key;
            this.value = value;
        }
    }
}
