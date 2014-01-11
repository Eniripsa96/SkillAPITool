using System;
using System.Collections.Generic;
using System.Text;

namespace SkillAPITool {

    /// <summary>
    /// Class data
    /// </summary>
    public class Tree {

        public List<String> skills = new List<string>();
        public string name;
        public string prefix;
        public string parent;
        public bool inherit;
        public int maxLevel = 40;
        public int professLevel = 40;
        public int interval = 1;
        public int offset = 1;
        public double healthBase = 20;
        public double healthScale = 0;
        public double manaBase = 20;
        public double manaBonus = 0;

        private StringBuilder sb = new StringBuilder();
        public string data;

        /// <summary>
        /// Whether or not the tree has a parent to inherit from
        /// </summary>
        public bool CanInherit {
            get { return !parent.Equals("None"); }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">class name</param>
        public Tree(string name) {
            this.name = name;
            this.prefix = name;
        }

        /// <summary>
        /// Updates the config data for the class
        /// </summary>
        public void Update() {

            sb.Clear();

            // Basic values
            sb.Append(name);
            sb.Append(":\n");
            sb.Append("  prefix: '");
            sb.Append(prefix);
            sb.Append("'\n");
            sb.Append("  max-level: ");
            sb.Append(maxLevel);
            sb.Append("\n");
            sb.Append("  health-base: ");
            sb.Append(healthBase);
            sb.Append("\n");
            sb.Append("  health-scale: ");
            sb.Append(healthScale);
            sb.Append("\n");
            sb.Append("  mana-base: ");
            sb.Append(manaBase);
            sb.Append("\n");
            sb.Append("  mana-scale: ");
            sb.Append(manaBonus);
            sb.Append("\n");

            // Inheritance
            if (CanInherit) {
                sb.Append("  profess-level: ");
                sb.Append(professLevel);
                sb.Append("\n");
                sb.Append("  parent: ");
                sb.Append(parent);
                sb.Append("\n");
                if (inherit) {
                    sb.Append("  inherit:\n");
                    sb.Append("  - ");
                    sb.Append(parent);
                    sb.Append("\n");
                }
            }
            else {
                sb.Append("  profess-level: ");
                sb.Append(professLevel);
                sb.Append("\n");
            }

            // Click combos
            if (offset != 1) {
                sb.Append("  offset: ");
                sb.Append(offset);
                sb.Append("\n");
            }
            if (interval != 1) {
                sb.Append("  interval: ");
                sb.Append(interval);
                sb.Append("\n");
            }

            // Skills
            if (skills.Count == 0) {
                sb.Append("  skills: []\n");
            }
            else {
                sb.Append("  skills:\n");
                foreach (string skill in skills) {
                    sb.Append("  - ");
                    sb.Append(skill);
                    sb.Append("\n");
                }
            }

            data = sb.ToString();
        }
    }
}
