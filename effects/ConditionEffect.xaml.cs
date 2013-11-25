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
    public partial class ConditionEffect : IEffect, IEmbedable {

        /// <summary>
        /// Constructor
        /// </summary>
        public ConditionEffect() {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor with initial target and group
        /// </summary>
        /// <param name="target">target</param>
        /// <param name="group">group</param>
        public ConditionEffect(string target, string group) 
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
            return "Condition";
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
        public void GetAttributes(List<Attribute> list) { }

        /// <summary>
        /// Gets the values for the skill
        /// </summary>
        /// <returns>value list</returns>
        public void GetValues(List<Value> list) {
            int total = condition1.SelectedIndex;
            if (operator1.SelectedIndex > 0) {
                total += 256 * operator1.SelectedIndex + 4096 * condition2.SelectedIndex;
                if (operator2.SelectedIndex > 0) {
                    total += 4096 * 256 * operator2.SelectedIndex + 4096 * 4096 * condition3.SelectedIndex;
                }
            }
            list.Add(new Value("Condition", total));
        }

        /// <summary>
        /// Applies a loaded attribute
        /// </summary>
        /// <param name="attribute">attribute</param>
        public void ApplyAttribute(Attribute attribute) { }

        /// <summary>
        /// Applies a loaded value
        /// </summary>
        /// <param name="value">value</param>
        public void ApplyValue(Value value) {
            if (value.Key.Equals("Condition")) {
                int n = value.Number;
                condition1.SelectedIndex = n % 256;
                n /= 256;
                operator1.SelectedIndex = n % 16;
                n /= 16;
                condition2.SelectedIndex = n % 256;
                n /= 256;
                operator2.SelectedIndex = n % 16;
                n /= 16;
                condition3.SelectedIndex = n;
            }
        }

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
        /// Updates whether or not options are enabled
        /// </summary>
        /// <param name="sender">operator box</param>
        /// <param name="e">event details</param>
        private void UpdateEnabled(object sender, SelectionChangedEventArgs e) {
            if (condition3 == null) return;
            if (operator1.SelectedIndex == 0) {
                condition2.IsEnabled = false;
                operator2.IsEnabled = false;
                condition3.IsEnabled = false;
            }
            else {
                condition2.IsEnabled = true;
                operator2.IsEnabled = true;
                condition3.IsEnabled = operator2.SelectedIndex > 0;
            }
        }
    }
}
