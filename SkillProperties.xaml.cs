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
    /// Properties control for skills
    /// </summary>
    public partial class SkillProperties : UserControl, IUpdateable {

        private readonly MainPage parent;
        private readonly Skill skill;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="skill">skill to edit</param>
        public SkillProperties(MainPage parent, Skill skill) {
            InitializeComponent();

            nameBox.TextChanged += Filter.FilterString;
            descriptionBox.TextChanged += Filter.FilterString;
            messageBox.TextChanged += Filter.FilterString;
            permissionBox.TextChanged += Filter.FilterString;
            indicatorBox.TextChanged += Filter.FilterMaterial;
            indicatorBox.TextChanged += UpdateDataBox;
            maxLevelBox.TextChanged += Filter.FilterInt;
            itemReqBox.TextChanged += Filter.FilterMaterials;
            requiredLevelBox.TextChanged += Filter.FilterInt;
            levelBaseBox.TextChanged += Filter.FilterInt;
            levelBonusBox.TextChanged += Filter.FilterInt;
            costBaseBox.TextChanged += Filter.FilterInt;
            costBonusBox.TextChanged += Filter.FilterNInt;
            manaBaseBox.TextChanged += Filter.FilterInt;
            manaBonusBox.TextChanged += Filter.FilterNInt;
            cooldownBaseBox.TextChanged += Filter.FilterDouble;
            cooldownBonusBox.TextChanged += Filter.FilterNDouble;
            rangeBaseBox.TextChanged += Filter.FilterDouble;
            rangeBonusBox.TextChanged += Filter.FilterNDouble;
            radiusBaseBox.TextChanged += Filter.FilterDouble;
            radiusBonusBox.TextChanged += Filter.FilterNDouble;
            periodBaseBox.TextChanged += Filter.FilterDouble;
            periodBonusBox.TextChanged += Filter.FilterNDouble;

            this.parent = parent;
            this.skill = skill;
            Apply();
        }

        /// <summary>
        /// Applies skill data to the display
        /// </summary>
        public void Apply() {

            // Details
            nameBox.Text = skill.name;
            descriptionBox.Text = skill.description;
            messageBox.Text = skill.message;
            maxLevelBox.Text = skill.maxLevel.ToString();
            requiredLevelBox.Text = skill.skillReqLevel.ToString();
            indicatorBox.Text = skill.indicator;
            itemReqBox.Text = skill.itemReq;
            UpdateDataBox(null, null);
            ApplyData(skill.indicatorData);
            levelBaseBox.Text = skill.level.Initial.ToString();
            levelBonusBox.Text = skill.level.Scale.ToString();
            costBaseBox.Text = skill.cost.Initial.ToString();
            costBonusBox.Text = skill.cost.Scale.ToString();
            manaBaseBox.Text = skill.mana.Initial.ToString();
            manaBonusBox.Text = skill.mana.Scale.ToString();
            cooldownBaseBox.Text = skill.cooldown.Initial.ToString();
            cooldownBonusBox.Text = skill.cooldown.Scale.ToString();
            rangeBaseBox.Text = skill.range.Initial.ToString();
            rangeBonusBox.Text = skill.range.Scale.ToString();
            radiusBaseBox.Text = skill.radius.Initial.ToString();
            radiusBonusBox.Text = skill.radius.Scale.ToString();
            periodBaseBox.Text = skill.period.Initial.ToString();
            periodBonusBox.Text = skill.period.Scale.ToString();
            permissionBox.Text = skill.permissions;
            needPermBox.SelectedIndex = skill.needsPermission ? 1 : 0;

            // Required skill options
            foreach (Skill s in MainPage.skills) {
                if (s != skill) {
                    requiredSkillBox.Items.Add(s.name);
                }
            }

            // Skill type
            foreach (ComboBoxItem item in typeBox.Items) {
                if (item.Content.ToString().ToLower().Equals(skill.type.ToLower())) {
                    typeBox.SelectedItem = item;
                    break;
                }
            }

            // Required skill
            foreach (object item in requiredSkillBox.Items) {
                string value;
                if (item is ComboBoxItem) value = (item as ComboBoxItem).Content.ToString();
                else value = item.ToString();
                if (value.ToLower().Equals(skill.skillReq.ToLower())) {
                    requiredSkillBox.SelectedItem = item;
                    break;
                }
            }

            // Effects
            foreach (IEffect effect in skill.active) {
                activeGrid.Children.Add((UserControl)effect);
            }
            foreach (IEffect effect in skill.passive) {
                passiveGrid.Children.Add((UserControl)effect);
            }
            foreach (IEffect effect in skill.embedded) {
                embedGrid.Children.Add((UserControl)effect);
            }

            // Update the display
            SetVisibility();
            Rearrange();
        }

        /// <summary>
        /// Clears skill data
        /// </summary>
        public void Clear() {
            activeGrid.Children.Clear();
            passiveGrid.Children.Clear();
            embedGrid.Children.Clear();
        }

        /// <summary>
        /// Updates the visibility of options
        /// </summary>
        public void SetVisibility() {

            int y = 422;

            // Skill requirement
            if (requiredSkillBox.SelectedIndex == 0) {
                requiredLevelBox.Visibility = Visibility.Collapsed;
                requiredLevelLabel.Visibility = Visibility.Collapsed;
            }
            else {
                requiredLevelBox.Visibility = Visibility.Visible;
                requiredLevelLabel.Visibility = Visibility.Visible;
                y += 30;
            }

            // Range
            if (skill.RequiresRange) {
                rangeLabel.Visibility = Visibility.Visible;
                rangeBaseBox.Visibility = Visibility.Visible;
                rangeBonusBox.Visibility = Visibility.Visible;
                rangeLabel1.Visibility = Visibility.Visible;
                rangeLabel2.Visibility = Visibility.Visible;
                rangeLabel.Margin = new Thickness(rangeLabel.Margin.Left, y, 0, 0);
                rangeBaseBox.Margin = new Thickness(0, y, rangeBaseBox.Margin.Right, 0);
                rangeBonusBox.Margin = new Thickness(0, y, rangeBonusBox.Margin.Right, 0);
                rangeLabel1.Margin = new Thickness(0, y, rangeLabel1.Margin.Right, 0);
                rangeLabel2.Margin = new Thickness(0, y, rangeLabel2.Margin.Right, 0);
                y += 30;
            }
            else {
                rangeLabel.Visibility = Visibility.Collapsed;
                rangeBaseBox.Visibility = Visibility.Collapsed;
                rangeBonusBox.Visibility = Visibility.Collapsed;
                rangeLabel1.Visibility = Visibility.Collapsed;
                rangeLabel2.Visibility = Visibility.Collapsed;
            }

            // Radius
            if (skill.RequiresRadius) {
                radiusLabel.Visibility = Visibility.Visible;
                radiusBaseBox.Visibility = Visibility.Visible;
                radiusBonusBox.Visibility = Visibility.Visible;
                radiusLabel1.Visibility = Visibility.Visible;
                radiusLabel2.Visibility = Visibility.Visible;
                radiusLabel.Margin = new Thickness(rangeLabel.Margin.Left, y, 0, 0);
                radiusBaseBox.Margin = new Thickness(0, y, rangeBaseBox.Margin.Right, 0);
                radiusBonusBox.Margin = new Thickness(0, y, rangeBonusBox.Margin.Right, 0);
                radiusLabel1.Margin = new Thickness(0, y, rangeLabel1.Margin.Right, 0);
                radiusLabel2.Margin = new Thickness(0, y, rangeLabel2.Margin.Right, 0);
                y += 30;
            }
            else {
                radiusLabel.Visibility = Visibility.Collapsed;
                radiusBaseBox.Visibility = Visibility.Collapsed;
                radiusBonusBox.Visibility = Visibility.Collapsed;
                radiusLabel1.Visibility = Visibility.Collapsed;
                radiusLabel2.Visibility = Visibility.Collapsed;
            }

            // Passive period
            if (skill.passive.Count > 0) {
                periodLabel.Visibility = Visibility.Visible;
                periodLabel1.Visibility = Visibility.Visible;
                periodLabel2.Visibility = Visibility.Visible;
                periodBaseBox.Visibility = Visibility.Visible;
                periodBonusBox.Visibility = Visibility.Visible;
                periodLabel.Margin = new Thickness(periodLabel.Margin.Left, y, 0, 0);
                periodLabel1.Margin = new Thickness(0, y, periodLabel1.Margin.Right, 0);
                periodLabel2.Margin = new Thickness(0, y, periodLabel2.Margin.Right, 0);
                periodBaseBox.Margin = new Thickness(0, y, periodBaseBox.Margin.Right, 0);
                periodBonusBox.Margin = new Thickness(0, y, periodBonusBox.Margin.Right, 0);
                y += 30;
            }
            else {
                periodLabel.Visibility = Visibility.Collapsed;
                periodLabel1.Visibility = Visibility.Collapsed;
                periodLabel2.Visibility = Visibility.Collapsed;
                periodBaseBox.Visibility = Visibility.Collapsed;
                periodBonusBox.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Updates the required level box when the required skill is changed
        /// </summary>
        /// <param name="sender">combo box</param>
        /// <param name="e">event details</param>
        private void RequiredSkillChanged(object sender, SelectionChangedEventArgs e) {
            if (requiredLevelBox == null) return;
            SetVisibility();
        }

        /// <summary>
        /// Adds a new active effect
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">event details</param>
        private void AddActiveEffect(object sender, RoutedEventArgs e) {
            IEffect effect = GetEffect((activeEffectBox.SelectedItem as ComboBoxItem).Content.ToString());
            activeGrid.Children.Add((UserControl)effect);
            skill.active.Add(effect);
            Rearrange();
            SetVisibility();
        }

        /// <summary>
        /// Adds a new passive effect
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">event details</param>
        private void AddPassiveEffect(object sender, RoutedEventArgs e) {
            IEffect effect = GetEffect((passiveEffectBox.SelectedItem as ComboBoxItem).Content.ToString());
            passiveGrid.Children.Add((UserControl)effect);
            skill.passive.Add(effect);
            Rearrange();
            SetVisibility();
        }

        /// <summary>
        /// Adds a new embed effect
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">event details</param>
        private void AddEmbedEffect(object sender, RoutedEventArgs e) {
            IEffect effect = GetEffect((embedEffectBox.SelectedItem as ComboBoxItem).Content.ToString());
            effect.RemoveLinear();
            embedGrid.Children.Add((UserControl)effect);
            skill.embedded.Add(effect);
            Rearrange();
            SetVisibility();
        }

        /// <summary>
        /// Gets an effect control from a key
        /// </summary>
        /// <param name="key">effect key</param>
        /// <returns>effect control</returns>
        private IEffect GetEffect(string key) {
            return (IEffect)Activator.CreateInstance(Type.GetType("SkillAPITool." + key + "Effect"));
        }

        /// <summary>
        /// Updates the skill data
        /// </summary>
        public void Update() {
            if (MaterialDictionary.IsValid(indicatorBox.Text)) skill.indicator = indicatorBox.Text;
            skill.indicatorData = GetData();
            skill.maxLevel = int.Parse(maxLevelBox.Text);
            if (nameBox.Text.Length > 0 && !GetParent().skillList.Items.Contains(nameBox.Text)) skill.name = nameBox.Text;
            if (requiredSkillBox.SelectedIndex == 0) skill.skillReq = "";
            else skill.skillReq = requiredSkillBox.SelectedItem.ToString();
            skill.skillReqLevel = int.Parse(requiredLevelBox.Text);
            skill.type = (typeBox.SelectedItem as ComboBoxItem).Content.ToString();
            skill.description = descriptionBox.Text;
            skill.message = messageBox.Text;
            skill.itemReq = itemReqBox.Text;
            skill.level.Initial = int.Parse(levelBaseBox.Text);
            skill.level.Scale = int.Parse(levelBonusBox.Text);
            skill.cost.Initial = int.Parse(costBaseBox.Text);
            skill.cost.Scale = int.Parse(costBonusBox.Text);
            skill.mana.Initial = int.Parse(manaBaseBox.Text);
            skill.mana.Scale = int.Parse(manaBonusBox.Text);
            skill.cooldown.Initial = double.Parse(cooldownBaseBox.Text);
            skill.cooldown.Scale = double.Parse(cooldownBonusBox.Text);
            skill.range.Initial = double.Parse(rangeBaseBox.Text);
            skill.range.Scale = double.Parse(rangeBonusBox.Text);
            skill.radius.Initial = double.Parse(radiusBaseBox.Text);
            skill.radius.Scale = double.Parse(radiusBonusBox.Text);
            skill.period.Initial = double.Parse(periodBaseBox.Text);
            skill.period.Scale = double.Parse(periodBonusBox.Text);
            skill.permissions = permissionBox.Text;
            skill.needsPermission = needPermBox.SelectedIndex == 1;
        }

        /// <summary>
        /// Removes an effect
        /// </summary>
        /// <param name="effect">effect to remove</param>
        public void RemoveEffect(IEffect effect) {
            if (skill.active.Contains(effect)) {
                skill.active.Remove(effect);
                activeGrid.Children.Remove((UserControl)effect);
            }
            else if (skill.passive.Contains(effect)) {
                skill.passive.Remove(effect);
                passiveGrid.Children.Remove((UserControl)effect);
            }
            else {
                skill.embedded.Remove(effect);
                embedGrid.Children.Remove((UserControl)effect);
            }
            
            Rearrange();
            SetVisibility();
        }

        /// <summary>
        /// Rearranges the skill grids
        /// </summary>
        public void Rearrange() {
            RearrangeSkills(activeGrid);
            RearrangeSkills(passiveGrid);
            RearrangeSkills(embedGrid);
        }

        /// <summary>
        /// Rearranges the skill grid
        /// </summary>
        private void RearrangeSkills(Grid grid) {
            double y = 38;
            int index = 0;

            // Update each child control
            foreach (Control control in grid.Children) {
                index++;

                // active box and button don't change
                if (index <= 2) {
                    continue;
                }

                // Move the child object
                control.Margin = new Thickness(6, y, 6, 0);
                y += control.Height + 6;
            }
        }

        /// <summary>
        /// Skill name is changed
        /// </summary>
        /// <param name="sender">name box</param>
        /// <param name="e">event details</param>
        private void NameChanged(object sender, TextChangedEventArgs e) {
            Filter.FilterWords(sender, e);
            MainPage page = GetParent();
            Update();
            int index = page.skillList.SelectedIndex;
            page.skillList.Items[page.skillList.SelectedIndex] = skill.name;
            page.skillList.UpdateLayout();
            page.skillList.SelectedIndex = index;
        }

        /// <summary>
        /// Retrieves the main page
        /// </summary>
        /// <returns></returns>
        private MainPage GetParent() {
            return parent;
        }

        /// <summary>
        /// Updates the data box when the indicator changes
        /// </summary>
        private void UpdateDataBox(object sender, EventArgs e) {
            string indicator = indicatorBox.Text.ToUpper().Replace(" ", "_");
            dataBox.Items.Clear();
            if (indicator.Equals("WOOD") 
                || indicator.Equals("SAPLING") 
                || indicator.Equals("LOG") 
                || indicator.Equals("LEAVES") 
                || indicator.Equals("WOOD_STEP") 
                || indicator.Equals("WOOD_DOUBLE_STEP")) {
                dataBox.Items.Add("Oak");
                dataBox.Items.Add("Spruce");
                dataBox.Items.Add("Birch");
                dataBox.Items.Add("Jungle");
            }
            else if (indicator.Equals("SANDSTONE")) {
                dataBox.Items.Add("Normal");
                dataBox.Items.Add("Chiseled");
                dataBox.Items.Add("Smooth");
            }
            else if (indicator.Equals("DEAD_BUSH")) {
                dataBox.Items.Add("Dead Shrub");
                dataBox.Items.Add("Grass");
                dataBox.Items.Add("Fern");
            }
            else if (indicator.Equals("WOOL") || indicator.Equals("STAINED_CLAY") || indicator.Equals("CARPET")) {
                dataBox.Items.Add("White");
                dataBox.Items.Add("Orange");
                dataBox.Items.Add("Magenta");
                dataBox.Items.Add("Light Blue");
                dataBox.Items.Add("Yellow");
                dataBox.Items.Add("Lime");
                dataBox.Items.Add("Pink");
                dataBox.Items.Add("Gray");
                dataBox.Items.Add("Light Gray");
                dataBox.Items.Add("Cyan");
                dataBox.Items.Add("Purple");
                dataBox.Items.Add("Blue");
                dataBox.Items.Add("Brown");
                dataBox.Items.Add("Green");
                dataBox.Items.Add("Red");
                dataBox.Items.Add("Black");
            }
            else if (indicator.Equals("DOUBLE_STEP") || indicator.Equals("STEP")) {
                dataBox.Items.Add("Stone");
                dataBox.Items.Add("Sandstone");
                dataBox.Items.Add("Wood");
                dataBox.Items.Add("Cobblestone");
                dataBox.Items.Add("Brick");
                dataBox.Items.Add("Stone Brick");
                dataBox.Items.Add("Nether Brick");
                dataBox.Items.Add("Quartz");
            }
            else if (indicator.Equals("MONSTER_EGGS")) {
                dataBox.Items.Add("Stone");
                dataBox.Items.Add("Cobblestone");
                dataBox.Items.Add("Stone Brick");
            }
            else if (indicator.Equals("SMOOTH_BRICK")) {
                dataBox.Items.Add("Normal");
                dataBox.Items.Add("Mossy");
                dataBox.Items.Add("Cracked");
                dataBox.Items.Add("Chiseled");
            }
            else if (indicator.Equals("COBBLE_WALL")) {
                dataBox.Items.Add("Normal");
                dataBox.Items.Add("Mossy");
            }
            else if (indicator.Equals("QUARTZ_BLOCK")) {
                dataBox.Items.Add("Normal");
                dataBox.Items.Add("Chiseled");
                dataBox.Items.Add("Pillar");
            }
            else if (indicator.Equals("COAL")) {
                dataBox.Items.Add("Coal");
                dataBox.Items.Add("Charcoal");
            }
            else if (indicator.Equals("GOLDEN_APPLE")) {
                dataBox.Items.Add("Normal");
                dataBox.Items.Add("Enchanted");
            }
            else if (indicator.Equals("INK_SACK")) {
                dataBox.Items.Add("Ink Sack");
                dataBox.Items.Add("Rose Red");
                dataBox.Items.Add("Cactus Green");
                dataBox.Items.Add("Coco Beans");
                dataBox.Items.Add("Lapis Lazuli");
                dataBox.Items.Add("Purple Dye");
                dataBox.Items.Add("Cyan Dye");
                dataBox.Items.Add("Light Gray Dye");
                dataBox.Items.Add("Gray Dye");
                dataBox.Items.Add("Pink Dye");
                dataBox.Items.Add("Lime Dye");
                dataBox.Items.Add("Dandelion Yellow");
                dataBox.Items.Add("Light Blue Dye");
                dataBox.Items.Add("Magenta Dye");
                dataBox.Items.Add("Orange Dye");
                dataBox.Items.Add("Bone Meal");
            }
            else if (indicator.Equals("POTION")) {
                dataBox.Items.Add("Water");
                dataBox.Items.Add("Awkward");
                dataBox.Items.Add("Thick");
                dataBox.Items.Add("Mundane");
                dataBox.Items.Add("Regeneration");
                dataBox.Items.Add("Swiftness");
                dataBox.Items.Add("Fire Resistance");
                dataBox.Items.Add("Poison");
                dataBox.Items.Add("Healing");
                dataBox.Items.Add("Night Vision");
                dataBox.Items.Add("Weakness");
                dataBox.Items.Add("Strength");
                dataBox.Items.Add("Slowness");
                dataBox.Items.Add("Harming");
                dataBox.Items.Add("Water Breathing");
                dataBox.Items.Add("Invisibility");
                dataBox.Items.Add("Splash Regeneration");
                dataBox.Items.Add("Splash Swiftness");
                dataBox.Items.Add("Splash Fire Resistance");
                dataBox.Items.Add("Splash Poison");
                dataBox.Items.Add("Splash Healing");
                dataBox.Items.Add("Splash Night Vision");
                dataBox.Items.Add("Splash Weakness");
                dataBox.Items.Add("Splash Strength");
                dataBox.Items.Add("Splash Slowness");
                dataBox.Items.Add("Splash Harming");
                dataBox.Items.Add("Splash Water Breathing");
                dataBox.Items.Add("Splash Invisibility");
            }
            else if (indicator.Equals("MONSTER_EGG")) {
                dataBox.Items.Add("Creeper");
                dataBox.Items.Add("Skeleton");
                dataBox.Items.Add("Spider");
                dataBox.Items.Add("Zombie");
                dataBox.Items.Add("Slime");
                dataBox.Items.Add("Ghast");
                dataBox.Items.Add("Pigman");
                dataBox.Items.Add("Enderman");
                dataBox.Items.Add("Cave Spider");
                dataBox.Items.Add("Silverfish");
                dataBox.Items.Add("Blaze");
                dataBox.Items.Add("Magma Cube");
                dataBox.Items.Add("Bat");
                dataBox.Items.Add("Witch");
                dataBox.Items.Add("Pig");
                dataBox.Items.Add("Sheep");
                dataBox.Items.Add("Cow");
                dataBox.Items.Add("Chicken");
                dataBox.Items.Add("Squid");
                dataBox.Items.Add("Wolf");
                dataBox.Items.Add("Mooshroom");
                dataBox.Items.Add("Ocelot");
                dataBox.Items.Add("Horse");
                dataBox.Items.Add("Villager");
            }
            else if (indicator.Equals("SKULL") || indicator.Equals("SKULL_ITEM")) {
                dataBox.Items.Add("Skeleton");
                dataBox.Items.Add("Wither Skeleton");
                dataBox.Items.Add("Zombie");
                dataBox.Items.Add("Human");
                dataBox.Items.Add("Creeper");
            }
            else {
                dataBox.Items.Add(" ");
            }
            dataBox.SelectedIndex = 0;
        }

        private int GetData() {
            string indicator = indicatorBox.Text.ToUpper().Replace(" ", "_");
            int value = dataBox.SelectedIndex;
            if (indicator.Equals("POTION")) {
                if (value < 4) value *= 16;
                else if (value < 16) {
                    if (value > 12) value += 1;
                    if (value > 9) value += 1;
                    value += 8189;
                }
                else {
                    if (value > 24) value += 1;
                    if (value > 21) value += 1;
                    value += 16369;
                }
            }
            else if (indicator.Equals("MONSTER_EGG")) {
                if (value > 22) value += 19;
                if (value > 21) value++;
                if (value > 20) value++;
                if (value > 13) value += 23;
                if (value > 11) value += 2;
                if (value > 2) value++;
                value += 50;
            }
            return value;
        }

        private void ApplyData(int value) {
            string indicator = indicatorBox.Text.ToUpper().Replace(" ", "_");
            if (indicator.Equals("POTION")) {
                if (value < 100) value /= 16;
                else if (value < 10000) {
                    value = value - 8189;
                    if (value > 9) value -= 1;
                    if (value > 12) value -= 1;
                }
                else {
                    value = value - 16369;
                    if (value > 21) value -= 1;
                    if (value > 24) value -= 1;
                }
            }
            else if (indicator.Equals("MONSTER_EGG")) {
                value -= 50;
                if (value > 2) value--;
                if (value > 11) value -= 2;
                if (value > 13) value -= 23;
                if (value > 20) value--;
                if (value > 21) value--;
                if (value > 22) value = 23;
            }
            if (value > 0 && value < dataBox.Items.Count) {
                dataBox.SelectedIndex = value;
            }
        }
    }
}
