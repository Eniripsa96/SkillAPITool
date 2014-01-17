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
    public partial class CooldownOption : UserControl {

        public int SkillIndex {
            get { return skillBox.SelectedIndex; }
        }

        public string Skill {
            get { return skillBox.SelectedItem.ToString(); }
            set { skillBox.SelectedItem = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tree">parent tree</param>
        /// <param name="skill">skill option</param>
        public CooldownOption() {
            InitializeComponent();
            foreach (Skill skill in MainPage.skills) {
                skillBox.Items.Add(skill.name);
            }
            skillBox.SelectedIndex = 0;
            valueBox.TextChanged += Filter.FilterDouble;
        }

        /// <summary>
        /// Changed the modification of the option
        /// </summary>
        /// <param name="sender">modification box</param>
        /// <param name="e">event details</param>
        private void ModificationChanged(object sender, SelectionChangedEventArgs e) {
            if (valueBox == null) return;
            valueBox.IsEnabled = modificationBox.SelectedIndex > 1;
        }

        /// <summary>
        /// Retrieves the modification data for the option
        /// </summary>
        /// <returns>modification data string</returns>
        public string GetModification() {
            if (modificationBox.SelectedIndex == 1) return "s";
            else if (modificationBox.SelectedIndex == 2) return "a" + double.Parse(valueBox.Text);
            else if (modificationBox.SelectedIndex == 3) return "a-" + double.Parse(valueBox.Text);
            else if (modificationBox.SelectedIndex == 4) return "p" + double.Parse(valueBox.Text);
            else if (modificationBox.SelectedIndex == 5) return "p-" + double.Parse(valueBox.Text);
            else return "r";
        }

        /// <summary>
        /// Sets the modification data for the option
        /// </summary>
        /// <param name="modification">modification data</param>
        public void SetModification(string modification) {
            try {
                if (modification.Equals("s")) modificationBox.SelectedIndex = 1;
                else if (modification.StartsWith("a")) {
                    double value = double.Parse(modification.Substring(1));
                    modificationBox.SelectedIndex = 2 + (value < 0 ? 1 : 0);
                    if (value < 0) value = -value;
                    valueBox.Text = value.ToString();
                }
                else if (modification.StartsWith("p")) {
                    double value = double.Parse(modification.Substring(1));
                    modificationBox.SelectedIndex = 4 + (value < 0 ? 1 : 0);
                    if (value < 0) value = -value;
                    valueBox.Text = value.ToString();
                }
            }
            catch (Exception) { }
        }
    }
}
