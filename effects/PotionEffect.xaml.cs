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
    /// Damage effect
    /// </summary>
    public partial class PotionEffect : IEffect {

        private ComboBox[] typeBoxes;

        /// <summary>
        /// Constructor
        /// </summary>
        public PotionEffect() {
            InitializeComponent();
            typeBoxes = new ComboBox[] { typeBox1, typeBox2, typeBox3, typeBox4, typeBox5, typeBox6 };
            tierBaseBox.TextChanged += Filter.FilterInt;
            tierBonusBox.TextChanged += Filter.FilterInt;
            durationBaseBox.TextChanged += Filter.FilterDouble;
            durationBonusBox.TextChanged += Filter.FilterDouble;
        }

        /// <summary>
        /// Constructor with initial target and group
        /// </summary>
        /// <param name="target">target</param>
        /// <param name="group">group</param>
        public PotionEffect(string target, string group)
            : this() 
        {
            foreach (object obj in targetBox.Items) {
                string s = (obj as ComboBoxItem).Content.ToString();
                if (s.ToLower().Equals(target.ToLower())) {
                    targetBox.SelectedItem = obj;
                    break;
                }
            }
            foreach (object obj in groupBox.Items) {
                string s = (obj as ComboBoxItem).Content.ToString();
                if (s.ToLower().Equals(group.ToLower())) {
                    groupBox.SelectedItem = obj;
                    break;
                }
            }
        }

        /// <summary>
        /// Removes the effect
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">event details</param>
        private void Removed(object sender, RoutedEventArgs e) {
            GetParent().RemoveEffect(this);
        }

        /// <summary>
        /// Gets SkillProperties control
        /// </summary>
        /// <returns></returns>
        private SkillProperties GetParent() {
            return (SkillProperties)(((((Parent as FrameworkElement).Parent as FrameworkElement).Parent as FrameworkElement).Parent as FrameworkElement).Parent);
        }

        /// <summary>
        /// Returns the key for the effect
        /// </summary>
        /// <returns>key</returns>
        public string GetKey() {
            return "Potion";
        }

        /// <summary>
        /// Returns the target of the group
        /// </summary>
        /// <returns>target type key</returns>
        public string GetTarget() {
            return (targetBox.SelectedItem as ComboBoxItem).Content.ToString();
        }

        /// <summary>
        /// Returns the group the skill is targeting
        /// </summary>
        /// <returns>group name</returns>
        public string GetGroup() {
            return (groupBox.SelectedItem as ComboBoxItem).Content.ToString();
        }

        /// <summary>
        /// Gets the attributes for the skill
        /// </summary>
        /// <returns>attribute list</returns>
        public void GetAttributes(List<Attribute> list) {
            list.Add(new Attribute("Tier", int.Parse(tierBaseBox.Text), int.Parse(tierBonusBox.Text)));
            list.Add(new Attribute("Duration", double.Parse(durationBaseBox.Text), double.Parse(durationBonusBox.Text)));
        }

        /// <summary>
        /// Gets the values for the skill
        /// </summary>
        /// <returns>value list</returns>
        public void GetValues(List<Value> list) {
            int type = 1;
            int m = 1;
            for (int i = 0; i < typeBoxes.Length; i++) {
                int value = typeBoxes[i].SelectedIndex;
                if (value > 5 || (value == 5 && i == 0)) value += 2;
                type += m * value;
                if (value > 0 || i == 0) m *= 32;
            }
            list.Add(new Value("Type", type));
        }

        /// <summary>
        /// Applies a loaded attribute
        /// </summary>
        /// <param name="attribute">attribute</param>
        public void ApplyAttribute(Attribute attribute) {
            if (attribute.Key.EndsWith("Tier")) {
                tierBaseBox.Text = attribute.Initial.ToString();
                tierBonusBox.Text = attribute.Scale.ToString();
            }
            else if (attribute.Key.EndsWith("Duration")) {
                durationBaseBox.Text = attribute.Initial.ToString();
                durationBonusBox.Text = attribute.Scale.ToString();
            }
        }

        /// <summary>
        /// Applies a loaded value
        /// </summary>
        /// <param name="value">value</param>
        public void ApplyValue(Value value) {
            if (value.Key.Equals("Type")) {
                int num = value.Number;
                int box = 0;
                while (num > 0) {
                    int index = (num - 1) % 32;
                    num /= 32;
                    if (index > 5) index -= 2;
                    if (box > 0) index++;
                    if (typeBoxes[box].Items.Count > index) {
                        typeBoxes[box].SelectedIndex = index;
                        box++;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the strings for the skill
        /// </summary>
        /// <param name="list">list to add to</param>
        public void GetStrings(List<StringValue> list) { }

        /// <summary>
        /// Applies a loaded string
        /// </summary>
        /// <param name="value">value to apply</param>
        public void ApplyString(StringValue value) { }

        /// <summary>
        /// Sets visibility when target changes
        /// </summary>
        /// <param name="sender">target box</param>
        /// <param name="e">event details</param>
        private void TargetChanged(object sender, SelectionChangedEventArgs e) {
            if (Parent != null) {
                GetParent().SetVisibility();
            }
        }

        /// <summary>
        /// Removes the linear target option
        /// </summary>
        public void RemoveLinear() {
            targetBox.Items.RemoveAt(1);
        }
    }
}
