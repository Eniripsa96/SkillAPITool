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
    public partial class CooldownEffect : IEffect {

        private static readonly char[] SPLITTER = new char[] { ';' };

        private List<CooldownOption> options = new List<CooldownOption>();

        /// <summary>
        /// Constructor
        /// </summary>
        public CooldownEffect() {
            InitializeComponent();
            AddOption();
        }

        /// <summary>
        /// Constructor with initial target and group
        /// </summary>
        /// <param name="target">target</param>
        /// <param name="group">group</param>
        public CooldownEffect(string target, string group) 
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

        private void Update() {
            Height = 300 + 30 * options.Count;
            int y = 175;
            foreach (CooldownOption option in options) {
                option.Margin = new Thickness(0, y, 0, 0);
                y += 30;
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
            return "Cooldown";
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
        public void GetValues(List<Value> list) { }

        /// <summary>
        /// Applies a loaded attribute
        /// </summary>
        /// <param name="attribute">attribute</param>
        public void ApplyAttribute(Attribute attribute) { }

        /// <summary>
        /// Applies a loaded value
        /// </summary>
        /// <param name="value">value</param>
        public void ApplyValue(Value value) { }

        /// <summary>
        /// Gets the strings for the skill
        /// </summary>
        /// <param name="list">list to add to</param>
        public void GetStrings(List<StringValue> list) {
            string s = "";
            bool first = true;
            foreach (CooldownOption option in options) {
                if (first) first = false;
                else s += ";";
                s += option.Skill + ";" + option.GetModification();
            }
            list.Add(new StringValue("Cooldowns", s));
        }

        /// <summary>
        /// Applies a loaded string
        /// </summary>
        /// <param name="value">value to apply</param>
        public void ApplyString(StringValue value) {
            if (value.Key.Equals("Cooldowns")) {
                foreach (CooldownOption o in options) {
                    mainGrid.Children.Remove(o);
                }
                options.Clear();
                string[] pieces = value.String.Split(SPLITTER);
                if (pieces == null) return;
                for (int i = 0; i < pieces.Length / 2; i++) {
                    CooldownOption option = AddOption();
                    option.Skill = pieces[i * 2];
                    option.SetModification(pieces[i * 2 + 1]);
                }
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
        /// Removes the linear target option
        /// </summary>
        public void RemoveLinear() {
            targetBox.Items.RemoveAt(1);
        }

        private CooldownOption AddOption() {
            CooldownOption option = new CooldownOption();
            options.Add(option);
            mainGrid.Children.Add(option);
            Update();
            return option;
        }

        /// <summary>
        /// Adds a new option to the window
        /// </summary>
        /// <param name="sender">add button</param>
        /// <param name="e">event details</param>
        private void AddClicked(object sender, RoutedEventArgs e) {
            AddOption();
        }
    }
}
