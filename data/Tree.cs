﻿using System;
using System.Collections.Generic;

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
        public int healthBase = 20;
        public int healthScale = 0;
        public int manaBase = 20;
        public int manaBonus = 0;

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
    }
}