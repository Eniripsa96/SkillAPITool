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
    public partial class ParticleEffect : IEffect {

        /// <summary>
        /// Constructor
        /// </summary>
        public ParticleEffect() {
            InitializeComponent();
            UpdateDataBox();

            amountBaseBox.TextChanged += Filter.FilterInt;
            amountBonusBox.TextChanged += Filter.FilterInt;
            radiusBaseBox.TextChanged += Filter.FilterInt;
            RadiusBonusBox.TextChanged += Filter.FilterInt;
        }

        /// <summary>
        /// Constructor with initial target and group
        /// </summary>
        /// <param name="target">target</param>
        /// <param name="group">group</param>
        public ParticleEffect(string target, string group)
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
        /// Updates the available values in the data box
        /// depending on the particle type
        /// </summary>
        private void UpdateDataBox() {
            if (dataBox == null) return;

            dataBox.Items.Clear();
            if (typeBox.SelectedIndex == 3) {
                dataBox.Items.Add("Regeneration");
                dataBox.Items.Add("Speed");
                dataBox.Items.Add("Fire Resistance");
                dataBox.Items.Add("Poison");
                dataBox.Items.Add("Instant Health");
                dataBox.Items.Add("Night Vision");
                dataBox.Items.Add("Weakness");
                dataBox.Items.Add("Strength");
                dataBox.Items.Add("Slowness");
                dataBox.Items.Add("Instant Damage");
                dataBox.Items.Add("Invisibility");
            }
            else if (typeBox.SelectedIndex == 4) {
                dataBox.Items.Add("Angry Villager");
                dataBox.Items.Add("Bubble");
                dataBox.Items.Add("Cloud");
                dataBox.Items.Add("Crit");
                dataBox.Items.Add("Death Suspend");
                dataBox.Items.Add("Drip Lava");
                dataBox.Items.Add("Drip Water");
                dataBox.Items.Add("Enchantment Table");
                dataBox.Items.Add("Explode");
                dataBox.Items.Add("Firework Spark");
                dataBox.Items.Add("Flame");
                dataBox.Items.Add("Footstep");
                dataBox.Items.Add("Happy Villager");
                dataBox.Items.Add("Heart");
                dataBox.Items.Add("Huge Explosion");
                dataBox.Items.Add("Icon Crack");
                dataBox.Items.Add("Instant Spell");
                dataBox.Items.Add("Large Explode");
                dataBox.Items.Add("Large Smoke");
                dataBox.Items.Add("Lava");
                dataBox.Items.Add("Magic Crit");
                dataBox.Items.Add("Mob Spell");
                dataBox.Items.Add("Mob Spell Ambient");
                dataBox.Items.Add("Note");
                dataBox.Items.Add("Portal");
                dataBox.Items.Add("Red Dust");
                dataBox.Items.Add("Slime");
                dataBox.Items.Add("Snowball Poof");
                dataBox.Items.Add("Snow Shovel");
                dataBox.Items.Add("Spell");
                dataBox.Items.Add("Splash");
                dataBox.Items.Add("Suspend");
                dataBox.Items.Add("Tile Crack");
                dataBox.Items.Add("Town Aura");
                dataBox.Items.Add("Witch Magic");
            }
            else {
                dataBox.Items.Add("None");
            }
            dataBox.SelectedIndex = 0;
            dataBox.UpdateLayout();
        }

        /// <summary>
        /// Retrieves the value of the data box 
        /// mapped to the correct values
        /// </summary>
        /// <returns>mapped data value</returns>
        public int getDataValue() {
            if (typeBox.SelectedIndex == 3) {
                int value = dataBox.SelectedIndex + 1;
                if (value > 6) value++;
                if (value > 10) value++;
                if (value > 12) value++;
                return value;
            }
            else if (typeBox.SelectedIndex == 4) {
                return dataBox.SelectedIndex;
            }
            else return 0;
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
            return "Particle";
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
            list.Add(new Value("Particle", typeBox.SelectedIndex));
            list.Add(new Value("Particle Area", areaBox.SelectedIndex));
            list.Add(new Value("Particle Radius Base", int.Parse(radiusBaseBox.Text)));
            list.Add(new Value("Particle Radius Bonus", int.Parse(RadiusBonusBox.Text)));
            list.Add(new Value("Particle Amount Base", int.Parse(amountBaseBox.Text)));
            list.Add(new Value("Particle Amount Bonus", int.Parse(amountBonusBox.Text)));
            list.Add(new Value("Particle Data", getDataValue()));
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
            if (value.Key.Equals("Particle Amount Base") && value.Number > 0) {
                amountBaseBox.Text = value.Number.ToString();
            }
            else if (value.Key.Equals("Particle Amount Bonus") && value.Number >= 0) {
                amountBonusBox.Text = value.Number.ToString();
            }
            else if (value.Key.Equals("Particle Radius Base") && value.Number > 0) {
                radiusBaseBox.Text = value.Number.ToString();
            }
            else if (value.Key.Equals("Particle Radius Bonus") && value.Number >= 0) {
                RadiusBonusBox.Text = value.Number.ToString();
            }
            else if (value.Key.Equals("Particle Area") && value.Number >= 0 && value.Number < 3) {
                areaBox.SelectedIndex = value.Number;
            }
            else if (value.Key.Equals("Particle") && value.Number >= 0 && value.Number < typeBox.Items.Count) {
                typeBox.SelectedIndex = value.Number;
            }
            else if (value.Key.Equals("Particle Data")) {
                int num = value.Number;

                if (typeBox.SelectedIndex == 3) {
                    if (num > 13) num--;
                    if (num > 11) num--;
                    if (num > 7) num--;
                    num--;
                }

                if (num >= 0 && num < dataBox.Items.Count) {
                    dataBox.SelectedIndex = num;
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
        /// Updates the available data values when the particle type changes
        /// </summary>
        /// <param name="sender">combo box</param>
        /// <param name="e">event details</param>
        private void TypeChanged(object sender, SelectionChangedEventArgs e) {
            UpdateDataBox();
        }

        /// <summary>
        /// Removes the linear target option
        /// </summary>
        public void RemoveLinear() {
            targetBox.Items.RemoveAt(1);
        }
    }
}
