﻿using System;
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
    public partial class ProjectileEffect : IEffect, IEmbedable {

        /// <summary>
        /// Constructor
        /// </summary>
        public ProjectileEffect() {
            InitializeComponent();
            speedBaseBox.TextChanged += Filter.FilterDouble;
            speedBonusBox.TextChanged += Filter.FilterDouble;
            angleBaseBox.TextChanged += Filter.FilterInt;
            angleBonusBox.TextChanged += Filter.FilterInt;
            amountBaseBox.TextChanged += Filter.FilterInt;
            amountBonusBox.TextChanged += Filter.FilterInt;
            usedBox.TextChanged += Filter.FilterInt;
        }

        /// <summary>
        /// Constructor with initial target and group
        /// </summary>
        /// <param name="target">target</param>
        /// <param name="group">group</param>
        public ProjectileEffect(string target, string group)
            : this() { }

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
            return "Projectile";
        }

        /// <summary>
        /// Returns the target of the group
        /// </summary>
        /// <returns>target type key</returns>
        public string GetTarget() {
            return "Self";
        }

        /// <summary>
        /// Returns the group the skill is targeting
        /// </summary>
        /// <returns>group name</returns>
        public string GetGroup() {
            return "Ally";
        }

        /// <summary>
        /// Gets the attributes for the skill
        /// </summary>
        /// <returns>attribute list</returns>
        public void GetAttributes(List<Attribute> list) {
            list.Add(new Attribute("Speed", double.Parse(speedBaseBox.Text), double.Parse(speedBonusBox.Text)));
            list.Add(new Attribute("Angle", int.Parse(angleBaseBox.Text), int.Parse(angleBonusBox.Text)));
            list.Add(new Attribute("Quantity", int.Parse(amountBaseBox.Text), int.Parse(amountBonusBox.Text)));
        }

        /// <summary>
        /// Gets the values for the skill
        /// </summary>
        /// <returns>value list</returns>
        public void GetValues(List<Value> list) {
            if (typeBox.SelectedIndex != typeBox.Items.Count - 1) list.Add(new Value("Projectile", typeBox.SelectedIndex));
            else list.Add(new Value("Projectile", typeBox.Items.Count));
            list.Add(new Value("Spread", spreadBox.SelectedIndex));
            list.Add(new Value("Use Arrow", int.Parse(usedBox.Text)));
        }

        /// <summary>
        /// Applies a loaded attribute
        /// </summary>
        /// <param name="attribute">attribute</param>
        public void ApplyAttribute(Attribute attribute) {
            if (attribute.Key.EndsWith("Speed")) {
                speedBaseBox.Text = attribute.Initial.ToString();
                speedBonusBox.Text = attribute.Scale.ToString();
            }
            else if (attribute.Key.EndsWith("Angle")) {
                angleBaseBox.Text = ((int)attribute.Initial).ToString();
                angleBonusBox.Text = ((int)attribute.Scale).ToString();
            }
            else if (attribute.Key.EndsWith("Quantity")) {
                amountBaseBox.Text = ((int)attribute.Initial).ToString();
                amountBonusBox.Text = ((int)attribute.Scale).ToString();
            }
        }

        /// <summary>
        /// Applies a loaded value
        /// </summary>
        /// <param name="value">value</param>
        public void ApplyValue(Value value) {
            if (value.Key.Equals("Projectile")) {
                if (typeBox.Items.Count > value.Number - 1 && value.Number >= 0) {
                    if (value.Number == typeBox.Items.Count) typeBox.SelectedIndex = typeBox.Items.Count - 1;
                    else typeBox.SelectedIndex = value.Number;
                }
            } 
            if (value.Key.Equals("Spread")) {
                if (typeBox.Items.Count > value.Number && value.Number >= 0) {
                    spreadBox.SelectedIndex = value.Number;
                }
            }
            if (value.Key.Equals("Use Arrow")) {
                usedBox.Text = value.Number.ToString();
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
        /// Removes the linear target option
        /// </summary>
        public void RemoveLinear() { }
    }
}
