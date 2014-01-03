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
    public partial class DefenseModifierEffect : IEffect, IEmbedable {

        /// <summary>
        /// Constructor
        /// </summary>
        public DefenseModifierEffect() {
            InitializeComponent();
            durationBaseBox.TextChanged += Filter.FilterDouble;
            durationBonusBox.TextChanged += Filter.FilterDouble;
            attackBaseBox.TextChanged += Filter.FilterInt;
            attackBonusBox.TextChanged += Filter.FilterInt;
            chanceBaseBox.TextChanged += Filter.FilterDouble;
            chanceBonusBox.TextChanged += Filter.FilterDouble;
        }

        /// <summary>
        /// Constructor with initial target and group
        /// </summary>
        /// <param name="target">target</param>
        /// <param name="group">group</param>
        public DefenseModifierEffect(string target, string group) 
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
            return "DefenseModifier";
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
            list.Add(new Attribute("Modifier Duration", double.Parse(durationBaseBox.Text), double.Parse(durationBonusBox.Text)));
            list.Add(new Attribute("Attacks", int.Parse(attackBaseBox.Text), int.Parse(attackBonusBox.Text)));
            if (double.Parse(chanceBaseBox.Text) < 100) {
                list.Add(new Attribute("Modifier Chance", double.Parse(chanceBaseBox.Text), double.Parse(chanceBonusBox.Text)));
            }
        }

        /// <summary>
        /// Gets the values for the skill
        /// </summary>
        /// <returns>value list</returns>
        public void GetValues(List<Value> list) { 
            list.Add(new Value("Attack Type", typeBox.SelectedIndex));
        }

        /// <summary>
        /// Applies a loaded attribute
        /// </summary>
        /// <param name="attribute">attribute</param>
        public void ApplyAttribute(Attribute attribute) {
            if (attribute.Key.EndsWith("Modifier Duration")) {
                durationBaseBox.Text = attribute.Initial.ToString();
                durationBonusBox.Text = attribute.Scale.ToString();
            }
            else if (attribute.Key.EndsWith("Attacks")) {
                attackBaseBox.Text = attribute.Initial.ToString();
                attackBonusBox.Text = attribute.Scale.ToString();
            }
            else if (attribute.Key.EndsWith("Modifier Chance")) {
                chanceBaseBox.Text = attribute.Initial.ToString();
                chanceBonusBox.Text = attribute.Scale.ToString();
            }
        }

        /// <summary>
        /// Applies a loaded value
        /// </summary>
        /// <param name="value">value</param>
        public void ApplyValue(Value value) {
            if (value.Key.Equals("Attack Type")) {
                typeBox.SelectedIndex = value.Number;
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
    }
}
