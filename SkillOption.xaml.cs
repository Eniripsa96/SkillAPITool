using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SkillAPITool {

    /// <summary>
    /// A skill option for a class
    /// </summary>
    public partial class SkillOption : UserControl {

        /// <summary>
        /// Whether or not the skill is selected
        /// </summary>
        public bool IsChecked {
            get { return (bool)optionBox.IsChecked; }
        }

        /// <summary>
        /// The skill name for the option
        /// </summary>
        public string getSkill {
            get { return optionBox.Content.ToString(); }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tree">parent tree</param>
        /// <param name="skill">skill option</param>
        public SkillOption(Tree tree, Skill skill) {
            InitializeComponent();
            optionBox.Content = skill.name;
            optionBox.IsChecked = false;
            foreach (string s in tree.skills) {
                if (s.ToLower().Equals(skill.name)) {
                    optionBox.IsChecked = true;
                    break;
                }
            }
        }
    }
}
