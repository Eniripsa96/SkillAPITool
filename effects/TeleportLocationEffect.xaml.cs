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
    public partial class TeleportLocationEffect : IEffect {

        /// <summary>
        /// Constructor
        /// </summary>
        public TeleportLocationEffect() {
            InitializeComponent();

            worldBox.TextChanged += Filter.FilterString;
            xBox.TextChanged += Filter.FilterNInt;
            yBox.TextChanged += Filter.FilterNInt;
            zBox.TextChanged += Filter.FilterNInt;
            yawTextBox.TextChanged += Filter.FilterNInt;
            pitchTextBox.TextChanged += Filter.FilterNInt;
        }

        /// <summary>
        /// Constructor with initial target and group
        /// </summary>
        /// <param name="target">target</param>
        /// <param name="group">group</param>
        public TeleportLocationEffect(string target, string group) 
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
            return "TeleportLocation";
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
            list.Add(new Value("X", int.Parse(xBox.Text)));
            list.Add(new Value("Y", int.Parse(yBox.Text)));
            list.Add(new Value("Z", int.Parse(zBox.Text)));

            int yaw;
            if (yawBox.SelectedIndex == 0) yaw = -90;
            else if (yawBox.SelectedIndex == 1) yaw = -270;
            else if (yawBox.SelectedIndex == 2) yaw = 0;
            else if (yawBox.SelectedIndex == 3) yaw = -180;
            else if (yawBox.SelectedIndex == 4) yaw = -45;
            else if (yawBox.SelectedIndex == 5) yaw = -135;
            else if (yawBox.SelectedIndex == 6) yaw = -315;
            else if (yawBox.SelectedIndex == 7) yaw = -225;
            else yaw = int.Parse(yawTextBox.Text);
            list.Add(new Value("Yaw", yaw));

            int pitch;
            if (pitchBox.SelectedIndex == 0) pitch = 0;
            else if (pitchBox.SelectedIndex == 1) pitch = -90;
            else if (pitchBox.SelectedIndex == 2) pitch = 90;
            else if (pitchBox.SelectedIndex == 3) pitch = -45;
            else if (pitchBox.SelectedIndex == 4) pitch = 45;
            else pitch = int.Parse(pitchTextBox.Text);
            list.Add(new Value("Pitch", pitch));
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
            if (value.Key.Equals("X")) xBox.Text = value.Number.ToString();
            else if (value.Key.Equals("Y")) yBox.Text = value.Number.ToString();
            else if (value.Key.Equals("Z")) zBox.Text = value.Number.ToString();
            else if (value.Key.Equals("Yaw")) {
                yawTextBox.Text = value.Number.ToString();
                if (value.Number == -90) yawBox.SelectedIndex = 0;
                else if (value.Number == -270) yawBox.SelectedIndex = 1;
                else if (value.Number == 0) yawBox.SelectedIndex = 2;
                else if (value.Number == -180) yawBox.SelectedIndex = 3;
                else if (value.Number == -45) yawBox.SelectedIndex = 4;
                else if (value.Number == -135) yawBox.SelectedIndex = 5;
                else if (value.Number == -315) yawBox.SelectedIndex = 6;
                else if (value.Number == -225) yawBox.SelectedIndex = 7;
                else yawBox.SelectedIndex = 8;
            }
            else if (value.Key.Equals("Pitch")) {
                pitchTextBox.Text = value.Number.ToString();
                if (value.Number == 0) pitchBox.SelectedIndex = 0;
                else if (value.Number == -90) pitchBox.SelectedIndex = 1;
                else if (value.Number == 90) pitchBox.SelectedIndex = 2;
                else if (value.Number == -45) pitchBox.SelectedIndex = 3;
                else if (value.Number == 45) pitchBox.SelectedIndex = 4;
                else pitchBox.SelectedIndex = 5;
            }
        }

        /// <summary>
        /// Gets the strings for the skill
        /// </summary>
        /// <param name="list">list to add to</param>
        public void GetStrings(List<StringValue> list) {
            if (worldBox.Text.Length > 0) {
                list.Add(new StringValue("World", worldBox.Text));
            }
        }

        /// <summary>
        /// Applies a loaded string
        /// </summary>
        /// <param name="value">value to apply</param>
        public void ApplyString(StringValue value) {
            if (value.Key.Equals("World")) {
                worldBox.Text = value.String;
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

        /// <summary>
        /// Controls the visibility of pitch options when the combo box changes
        /// </summary>
        /// <param name="sender">pitch box</param>
        /// <param name="e">event details</param>
        private void PitchChanged(object sender, RoutedEventArgs e) {
            if (pitchBox.SelectedIndex == 5) {
                pitchBox.Visibility = Visibility.Collapsed;
                pitchTextBox.Visibility = Visibility.Visible;
                pitchCancelButton.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Controls the visibility of yaw options when the combo box changes
        /// </summary>
        /// <param name="sender">yaw box</param>
        /// <param name="e">event details</param>
        private void YawChanged(object sender, RoutedEventArgs e) {
            if (yawBox.SelectedIndex == 8) {
                yawBox.Visibility = Visibility.Collapsed;
                yawTextBox.Visibility = Visibility.Visible;
                yawCancelButton.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Controls the visibility of pitch options when the button is pressed
        /// </summary>
        /// <param name="sender">pitch cancel button</param>
        /// <param name="e">event details</param>
        private void PitchCanceled(object sender, RoutedEventArgs e) {
            pitchBox.Visibility = Visibility.Visible;
            pitchTextBox.Visibility = Visibility.Collapsed;
            pitchCancelButton.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Controls the visibility of yaw options when the button is pressed
        /// </summary>
        /// <param name="sender">yaw cancel button</param>
        /// <param name="e">event details</param>
        private void YawCanceled(object sender, RoutedEventArgs e) {
            yawBox.Visibility = Visibility.Visible;
            yawTextBox.Visibility = Visibility.Collapsed;
            yawCancelButton.Visibility = Visibility.Collapsed;
        }
    }
}
