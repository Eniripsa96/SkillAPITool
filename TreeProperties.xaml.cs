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
    /// Properties window for a class
    /// </summary>
    public partial class TreeProperties : UserControl, IUpdateable {

        private MainPage parent;
        public Tree tree;
        public List<SkillOption> skills = new List<SkillOption>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tree">tree to edit</param>
        public TreeProperties(MainPage parent, Tree tree) {
            InitializeComponent();

            this.parent = parent;
            this.tree = tree;

            // Filters
            maxLevelBox.TextChanged += Filter.FilterInt;
            professLevelBox.TextChanged += Filter.FilterInt;
            healthBaseBox.TextChanged += Filter.FilterDouble;
            healthBonusBox.TextChanged += Filter.FilterDouble;
            manaBaseBox.TextChanged += Filter.FilterDouble;
            manaBonusBox.TextChanged += Filter.FilterDouble;

            Apply();
        }

        /// <summary>
        /// Applies class data to the display
        /// </summary>
        public void Apply() {

            // Basic stats
            nameBox.Text = tree.name;
            prefixBox.Text = tree.prefix;
            inheritBox.SelectedIndex = tree.inherit ? 0 : 1;
            maxLevelBox.Text = tree.maxLevel.ToString();
            professLevelBox.Text = tree.professLevel.ToString();
            healthBaseBox.Text = tree.healthBase.ToString();
            healthBonusBox.Text = tree.healthScale.ToString();
            manaBaseBox.Text = tree.manaBase.ToString();
            manaBonusBox.Text = tree.manaBonus.ToString();

            // Profession options
            foreach (Tree t in GetParent().trees) {
                if (t == tree) continue;
                requiredClassBox.Items.Add(t.name);
            }

            // Selected profession
            if (tree.parent != null) {
                foreach (object obj in requiredClassBox.Items) {
                    if (obj is ComboBoxItem) continue;
                    else if (obj.ToString().ToLower().Equals(tree.parent.ToLower())) {
                        requiredClassBox.SelectedItem = obj;
                        break;
                    }
                }
            }

            // Skill options
            int y = 30;
            foreach (Skill skill in GetParent().skills) {
                SkillOption option = new SkillOption(tree, skill);
                option.Margin = new Thickness(6, y, 6, 0);
                skillGrid.Children.Add(option);
                this.skills.Add(option);
                y += 25;
                if (tree.skills.Contains(skill.name)) {
                    option.IsChecked = true;
                }
            }
        }

        /// <summary>
        /// Updates tree data
        /// </summary>
        public void Update() {
            if (nameBox.Text.Length > 0 && !GetParent().classList.Items.Contains(nameBox.Text)) tree.name = nameBox.Text;
            tree.prefix = prefixBox.Text;
            if (requiredClassBox.SelectedIndex == 0) tree.parent = "None";
            else tree.parent = requiredClassBox.SelectedItem.ToString();
            tree.inherit = inheritBox.SelectedIndex == 0;
            tree.maxLevel = int.Parse(maxLevelBox.Text);
            tree.professLevel = int.Parse(professLevelBox.Text);
            tree.healthBase = int.Parse(healthBaseBox.Text);
            tree.healthScale = int.Parse(healthBonusBox.Text);
            tree.manaBase = int.Parse(manaBaseBox.Text);
            tree.manaBonus = int.Parse(manaBonusBox.Text);

            tree.skills.Clear();
            foreach (SkillOption option in skills) {
                if (option.IsChecked) tree.skills.Add(option.getSkill);
            }
        }

        /// <summary>
        /// Does nothing when cleared
        /// </summary>
        public void Clear() { }

        /// <summary>
        /// Class name changed
        /// </summary>
        /// <param name="sender">name box</param>
        /// <param name="e">event details</param>
        private void NameChanged(object sender, TextChangedEventArgs e) {
            Filter.FilterWords(sender, e);
            MainPage page = GetParent();
            Update();
            int index = page.classList.SelectedIndex;
            page.classList.Items[page.classList.SelectedIndex] = tree.name;
            page.classList.UpdateLayout();
            page.classList.SelectedIndex = index;
        }

        /// <summary>
        /// Retrieves the main page
        /// </summary>
        /// <returns></returns>
        private MainPage GetParent() {
            return parent;
        }
    }
}
