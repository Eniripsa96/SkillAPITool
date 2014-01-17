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
using System.IO;
using System.Windows.Threading;
using System.Text;
using System.IO.IsolatedStorage;

namespace SkillAPITool {

    /// <summary>
    /// Main page of the tool
    /// </summary>
    public partial class MainPage : UserControl {

        private static readonly string
            ROOT = Application.Current.HasElevatedPermissions ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\SkillAPI\\" : "",
            SKILL_ROOT = ROOT + "skill\\",
            CLASS_ROOT = ROOT + "class\\",
            DEFAULT_SKILL_NAME = "Skill",
            DEFAULT_CLASS_NAME = "Class",
            SKILL_FILE = "skills.yml",
            CLASS_FILE = "classes.yml",
            SKILL_FILTER = "Skill File | *.yml",
            CLASS_FILTER = "Class File | *.yml";

        public static readonly List<Skill> skills = new List<Skill>();
        public static readonly List<Tree> trees = new List<Tree>();

        private ResourceDictionary effectDictionary = new ResourceDictionary();
        private SaveFileDialog dialog = new SaveFileDialog();
        private StringBuilder sb = new StringBuilder();

        /// <summary>
        /// Constructor
        /// </summary>
        public MainPage() {
            InitializeComponent();

            dialog.DefaultExt = "yml";

            if (Application.Current.HasElevatedPermissions) {
                directoryLabel.Text = ROOT.Replace("\\", "/");
                onlineGrid.Visibility = Visibility.Collapsed;
                downloadLabel.Visibility = Visibility.Collapsed;
                instructionLabel.Visibility = Visibility.Collapsed;
                saveButton.Visibility = Visibility.Visible;
            }
            else {
                directoryTitle.Visibility = Visibility.Collapsed;
                directoryLabel.Text = "";
            }

            // Register effect names
            effectDictionary.Add("attackmodifier", "AttackModifier");
            effectDictionary.Add("chance", "Chance");
            effectDictionary.Add("cleanse", "Cleanse");
            effectDictionary.Add("command", "Command");
            effectDictionary.Add("condition", "Condition");
            effectDictionary.Add("cooldown", "Cooldown");
            effectDictionary.Add("damagebonus", "DamageBonus");
            effectDictionary.Add("damage", "Damage");
            effectDictionary.Add("damagepercent", "DamagePercent");
            effectDictionary.Add("damagepercentreduction", "DamagePercentReduction");
            effectDictionary.Add("damagereduction", "DamageReduction");
            effectDictionary.Add("defensemodifier", "DefenseModifier");
            effectDictionary.Add("delay", "Delay");
            effectDictionary.Add("dot", "DOT");
            effectDictionary.Add("fire", "Fire");
            effectDictionary.Add("heal", "Heal");
            effectDictionary.Add("healpercent", "HealPercent");
            effectDictionary.Add("healthdamage", "HealthDamage");
            effectDictionary.Add("hot", "HealOverTime");
            effectDictionary.Add("launch", "Launch");
            effectDictionary.Add("mana", "Mana");
            effectDictionary.Add("manadamage", "ManaDamage");
            effectDictionary.Add("manapercent", "ManaPercent");
            effectDictionary.Add("particle", "Particle");
            effectDictionary.Add("particleprojectile", "ParticleProjectile");
            effectDictionary.Add("potion", "Potion");
            effectDictionary.Add("projectile", "Projectile");
            effectDictionary.Add("pull", "Pull");
            effectDictionary.Add("push", "Push");
            effectDictionary.Add("sound", "Sound");
            effectDictionary.Add("status", "Status");
            effectDictionary.Add("taunt", "Taunt");
            effectDictionary.Add("teleport", "Teleport");
            effectDictionary.Add("teleportlocation", "TeleportLocation");
            effectDictionary.Add("teleportTarget", "TeleportTarget");

            // Initial File I/O
            if (Application.Current.HasElevatedPermissions && !Directory.Exists(ROOT)) {
                Directory.CreateDirectory(ROOT);
                Directory.CreateDirectory(SKILL_ROOT);
                Directory.CreateDirectory(CLASS_ROOT);
            }
            Load();

            // Create default class and skill if none were loaded
            if (trees.Count == 0) CreateNewClass();
            if (skills.Count == 0) CreateNewSkill();
        }

        /// <summary>
        /// Creates a new skill
        /// </summary>
        public void CreateNewSkill() {

            // Get the next available default name
            int id = 0;
            bool exists = false;
            do {
                exists = false;
                id++;
                foreach (Skill skill in skills) {
                    if (skill.name.ToLower().Equals(DEFAULT_SKILL_NAME.ToLower() + id)) {
                        exists = true;
                        break;
                    }
                }
            }
            while (exists);

            ClearMainGrid();
            Skill newSkill = new Skill(DEFAULT_SKILL_NAME + id);
            mainGrid.Children.Add(new SkillProperties(this, newSkill));
            skillList.Items.Add(newSkill.name);
            skills.Add(newSkill);
            skillList.SelectedIndex = skillList.Items.Count - 1;
        }

        /// <summary>
        /// Creates a new class
        /// </summary>
        public void CreateNewClass() {

            // Get the next available default name
            int id = 0;
            bool exists = false;
            do {
                exists = false;
                id++;
                foreach (Tree tree in trees) {
                    if (tree.name.ToLower().Equals(DEFAULT_CLASS_NAME.ToLower() + id)) {
                        exists = true;
                        break;
                    }
                }
            }
            while (exists);

            ClearMainGrid();
            Tree newTree = new Tree(DEFAULT_CLASS_NAME + id);
            mainGrid.Children.Add(new TreeProperties(this, newTree));
            classList.Items.Add(newTree.name);
            trees.Add(newTree);
            classList.SelectedIndex = classList.Items.Count - 1;
        }

        /// <summary>
        /// Saves all data for the tool into the files
        /// </summary>
        public void Save() {

            // Skills
            using (StreamWriter write = new StreamWriter(ROOT + SKILL_FILE)) {
                SaveSkills(write);
            }
            foreach (Skill skill in skills) {
                using (StreamWriter write = new StreamWriter(SKILL_ROOT + skill.name + ".yml")) {
                    write.Write(skill.data);
                }
            }

            // Classes
            using (StreamWriter write = new StreamWriter(ROOT + CLASS_FILE)) {
                SaveClasses(write);
            }
            foreach (Tree tree in trees) {
                using (StreamWriter write = new StreamWriter(CLASS_ROOT + tree.name + ".yml")) {
                    write.Write(tree.data);
                }
            }
        }

        private void SaveSkills(StreamWriter write) {
            if (mainGrid.Children.Count > 0) {
                (mainGrid.Children[0] as IUpdateable).Update();
            }
            sb.Clear();
            sb.Append("# If you manually modify this file,\n# set loaded to false\nloaded: false\n");
            foreach (Skill skill in skills) {
                skill.Update();
                sb.Append(skill.data);
            }
            write.Write(sb.ToString());
        }

        private void SaveClasses(StreamWriter write) {
            if (mainGrid.Children.Count > 0) {
                (mainGrid.Children[0] as IUpdateable).Update();
            }
            sb.Clear();
            sb.Append("# If you manually modify this file,\n# set loaded to false\nloaded: false\n");
            foreach (Tree tree in trees) {
                tree.Update();
                sb.Append(tree.data);
            }
            write.Write(sb.ToString());
        }

        /// <summary>
        /// Loads files from the project directory
        /// </summary>
        private void Load() {

            // Skills file
            if (File.Exists(ROOT + SKILL_FILE)) {

                using (StreamReader sreader = new StreamReader(ROOT + SKILL_FILE)) {
                    LoadSkill(sreader);
                }
            }

            // Class file
            if (File.Exists(ROOT + CLASS_FILE)) {

                using (StreamReader sreader = new StreamReader(ROOT + CLASS_FILE)) {
                    LoadClass(sreader);
                }
            }

            if (classList.Items.Count > 0) classList.SelectedIndex = 0;
            if (skillList.Items.Count > 0) skillList.SelectedIndex = 0;
        }

        private void LoadSkill(StreamReader read) {
            string data = "";

            // Clear skill data
            skills.Clear();
            skillList.Items.Clear();
            ClearMainGrid();

            // Initialize data
            Skill skill = null;
            string effect, target, group;
            effect = target = group = null;
            bool a, p, c, m, v, r, d, s;
            a = p = c = m = v = r = d = s = false;
            List<Attribute> attributes = new List<Attribute>();
            List<Value> values = new List<Value>();
            List<StringValue> strings = new List<StringValue>();
            Attribute currentAttribute = new Attribute("", 0, 0);

            // Read through the file
            while ((data = read.ReadLine()) != null) {

                // Ignore comments and loaded flag
                if (data.StartsWith("#") || data.StartsWith("loaded:") || data.Length == 0) {
                    continue;
                }

                // Sub-values
                if (data.StartsWith("    ")) {
                    data = data.Substring(4);

                    // Attributes
                    if (a) {
                        if (data.StartsWith("  base: ")) currentAttribute.Initial = double.Parse(data.Substring(8));
                        else if (data.StartsWith("  scale: ")) currentAttribute.Scale = double.Parse(data.Substring(9));
                        else if (!data.StartsWith(" ")) {
                            if (!currentAttribute.Key.Equals("")) {
                                attributes.Add(new Attribute(currentAttribute.Key, currentAttribute.Initial, currentAttribute.Scale));
                                currentAttribute.Initial = 0;
                                currentAttribute.Scale = 0;
                            }
                            currentAttribute.Key = data.Substring(0, data.IndexOf(":"));
                        }
                    }

                    // Values
                    else if (v) {
                        values.Add(new Value(data.Substring(0, data.IndexOf(":")), int.Parse(data.Substring(data.IndexOf(": ") + 2))));
                    }

                    // Strings
                    else if (s) {
                        strings.Add(new StringValue(data.Substring(0, data.IndexOf(":")), data.Substring(data.IndexOf(":") + 2)));
                    }

                    // Mechanics
                    else if (m || c || p) {
                        if (!data.StartsWith(" ")) effect = target = group = null;
                        else if (data.StartsWith("  effect: ")) effect = data.Substring(10);
                        else if (data.StartsWith("  target: ")) target = data.Substring(10);
                        else if (data.StartsWith("  group: ")) group = data.Substring(9);

                        if (effect != null && target != null && group != null) {
                            effect = effect.ToLower();
                            if (effectDictionary.Contains(effect)) {
                                effect = effectDictionary[effect].ToString();
                                IEffect eff = (IEffect)Activator.CreateInstance(Type.GetType("SkillAPITool." + effect + "Effect"), target, group);
                                string t2 = eff.GetTarget();
                                string g2 = eff.GetGroup();
                                if (m) skill.embedded.Add(eff);
                                else if (c) skill.active.Add(eff);
                                else if (p) skill.passive.Add(eff);
                            }
                        }
                    }
                    continue;
                }
                a = m = c = p = v = s = false;

                // New skill
                if (!data.StartsWith(" ")) {
                    if (skill != null) {
                        UpdateSkill(skill, attributes, values, strings);
                        attributes.Clear();
                        values.Clear();
                        strings.Clear();
                    }
                    if (mainGrid.Children.Count > 0) (mainGrid.Children[0] as IUpdateable).Apply();
                    CreateNewSkill();
                    skill = skills[skills.Count - 1];
                    skill.name = data.Substring(0, data.IndexOf(':'));
                    skillList.Items.RemoveAt(skills.Count - 1);
                    skillList.Items.Add(skill.name);
                }

                // Description and permissions
                else if (data.StartsWith("  - ")) {
                    if (d) {
                        string value = data.Substring(4);
                        if (value[0] == '\'') {
                            value = value.Substring(1, value.LastIndexOf('\'') - 1);
                        }
                        if (skill.description.Length > 0) skill.description += ' ';
                        skill.description += value;
                    }

                    // Permissions
                    else {
                        if (skill.permissions.Length > 0) skill.permissions += ",";
                        skill.permissions += data.Substring(0, 4);
                    }
                }

                // Value groups
                else if (data.StartsWith("  attributes:")) a = true;
                else if (data.StartsWith("  values:")) v = true;
                else if (data.StartsWith("  strings:")) s = true;
                else if (data.StartsWith("  active:")) c = true;
                else if (data.StartsWith("  passive:")) p = true;
                else if (data.StartsWith("  embed:")) m = true;
                else if (data.StartsWith("  permissions:")) d = !(r = true);
                else if (data.StartsWith("  description:")) d = !(r = false);
                else if (data.StartsWith("  type: ")) skill.type = data.Substring(8);
                else if (data.StartsWith("  indicator: ")) skill.indicator = data.Substring(13);
                else if (data.StartsWith("  item-req: ")) skill.itemReq = data.Substring(12);
                else if (data.StartsWith("  skill-req:")) skill.skillReq = data.Substring(13);
                else if (data.StartsWith("  skill-req-level: ")) skill.skillReqLevel = int.Parse(data.Substring(18));
                else if (data.StartsWith("  max-level: ")) skill.maxLevel = int.Parse(data.Substring(13));
                else if (data.StartsWith("  message: ")) skill.message = data.Substring(11).Replace("'", "");
                else if (data.StartsWith("  needs-permission: ")) skill.needsPermission = data.StartsWith("  needs-permission: t");
            }

            // Add the last attribute if there were any
            if (!currentAttribute.Key.Equals("")) {
                attributes.Add(new Attribute(currentAttribute.Key, currentAttribute.Initial, currentAttribute.Scale));
            }

            // Apply the loaded data
            if (skill != null) {
                UpdateSkill(skill, attributes, values, strings);
                if (skills.Count == 0) CreateNewSkill();
                else (mainGrid.Children[0] as IUpdateable).Apply();
                explorerTab.SelectedItem = skillTab;
                skillList.SelectedIndex = 0;
            }
        }

        private void LoadClass(StreamReader read) {
            string data = "";

            // Clear class data
            trees.Clear();
            classList.Items.Clear();
            ClearMainGrid();

            // Initialize data
            Tree tree = null;
            bool skills = false;

            // Read through the file
            while ((data = read.ReadLine()) != null) {

                // Ignore comments and loaded flag
                if (data.StartsWith("#") || data.StartsWith("loaded:") || data.Length == 0) {
                    continue;
                }

                // New class
                if (!data.StartsWith(" ")) {
                    if (mainGrid.Children.Count > 0) (mainGrid.Children[0] as IUpdateable).Apply();
                    CreateNewClass();
                    tree = trees[trees.Count - 1];
                    tree.name = data.Substring(0, data.IndexOf(':'));
                    classList.Items.RemoveAt(trees.Count - 1);
                    classList.Items.Add(tree.name);
                }

                // Prefix
                else if (data.StartsWith("  prefix: '")) {
                    int index = data.IndexOf('\'') + 1;
                    tree.prefix = data.Substring(index, data.IndexOf('\'', index) - index);
                }
                else if (data.StartsWith("  prefix: ")) {
                    int index = data.IndexOf(":") + 2;
                    tree.prefix = data.Substring(index);
                }

                // Class data
                else if (data.StartsWith("  parent: ")) tree.parent = data.Substring(data.IndexOf(": ") + 2);
                else if (data.StartsWith("  profess-level: ")) tree.professLevel = int.Parse(data.Substring(data.IndexOf(": ") + 2));
                else if (data.StartsWith("  max-level: ")) tree.maxLevel = int.Parse(data.Substring(data.IndexOf(": ") + 2));
                else if (data.StartsWith("  health-base: ")) tree.healthBase = double.Parse(data.Substring(data.IndexOf(": ") + 2));
                else if (data.StartsWith("  health-scale: ")) tree.healthScale = double.Parse(data.Substring(data.IndexOf(": ") + 2));
                else if (data.StartsWith("  mana-base: ")) tree.manaBase = double.Parse(data.Substring(data.IndexOf(": ") + 2));
                else if (data.StartsWith("  mana-scale: ")) tree.manaBonus = double.Parse(data.Substring(data.IndexOf(": ") + 2));
                else if (data.StartsWith("  needs-permission: ")) tree.needsPermission = data.StartsWith("  needs-permission: t");
                else if (data.Equals("  inherit:")) tree.inherit = true;
                else if (data.Equals("  inherit: []")) tree.inherit = false;
                else if (data.Equals("  skills:")) skills = true;
                else if (data.StartsWith("  - ") && skills) tree.skills.Add(data.Substring(4));
            }

            // Apply loaded data
            if (trees.Count == 0) CreateNewClass();
            else (mainGrid.Children[0] as IUpdateable).Apply();
            explorerTab.SelectedItem = classTab;
        }

        /// <summary>
        /// Updates a skill with the list of attributes and values
        /// </summary>
        /// <param name="skill">skill to update</param>
        /// <param name="attributes">attributes to update with</param>
        /// <param name="values">values to update with</param>
        private void UpdateSkill(Skill skill, List<Attribute> attributes, List<Value> values, List<StringValue> strings) {
            AttributeSet activeSet = new AttributeSet("", skill.active);
            AttributeSet passiveSet = new AttributeSet("Passive ", skill.passive);
            AttributeSet embedSet = new AttributeSet("Embed ", skill.embedded);

            // Update the attributes
            foreach (Attribute att in attributes) {

                // Detail attributes
                if (att.Key.Equals("Level")) skill.level = att;
                else if (att.Key.Equals("Cost")) skill.cost = att;
                else if (att.Key.Equals("Mana")) skill.mana = att;
                else if (att.Key.Equals("Cooldown")) skill.cooldown = att;
                else if (att.Key.Equals("Range")) skill.range = att;
                else if (att.Key.Equals("Radius")) skill.radius = att;

                // Effect attributes
                else {
                    AttributeSet set;
                    List<IEffect> list;
                    string key;

                    // Passive attributes
                    if (att.Key.StartsWith("Passive ")) {
                        key = att.Key.Substring(8);
                        set = passiveSet;
                        list = skill.passive;
                    }

                    // Embedded attributes
                    else if (att.Key.StartsWith("Embed ")) {
                        key = att.Key.Substring(6);
                        set = embedSet;
                        list = skill.embedded;
                    }

                    // Active attributes
                    else {
                        key = att.Key;
                        set = activeSet;
                        list = skill.active;
                    }

                    // Target name
                    string targetName = "";
                    if (key.Contains(" ")) targetName = key.Substring(0, key.IndexOf(" "));

                    // Cycle through each effect to find the correct one
                    foreach (IEffect eff in list) {
                        string prevKey = att.Key;
                        if (set.IsAliased(eff)) {
                            if (targetName.Length == 0) continue;
                            if (eff.GetTarget().ToLower().Equals(targetName.ToLower())) {
                                att.Key = att.Key.Substring(att.Key.IndexOf(" ") + 1);
                                eff.ApplyAttribute(att);
                                break;
                            }
                        }
                        else eff.ApplyAttribute(att);
                    }
                }
            }

            // Apply values
            foreach (Value value in values) {
                foreach (IEffect eff in skill) {
                    eff.ApplyValue(value);
                }
            }

            // Apply strings
            foreach (StringValue value in strings) {
                foreach (IEffect eff in skill) {
                    eff.ApplyString(value);
                }
            }
        }

        /// <summary>
        /// Creates a new object depending on whether a skill or class is selected
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">event details</param>
        private void CreateNew(object sender, RoutedEventArgs e) {
            if (mainGrid.Children[0] is SkillProperties) {
                CreateNewSkill();
            }
            else CreateNewClass();
        }

        private void delete(object sender, RoutedEventArgs e) {
            if (mainGrid.Children[0] is SkillProperties) {
                deleteSkill();
            }
            else deleteClass();
        }

        private void deleteSkill() {
            if (skills.Count > 1) {
                ClearMainGrid();
                int index = skillList.SelectedIndex;
                if (Application.Current.HasElevatedPermissions) {
                    File.Delete(SKILL_ROOT + skills[index].name + ".yml");
                }
                skills.RemoveAt(skillList.SelectedIndex);
                skillList.Items.RemoveAt(skillList.SelectedIndex);
                if (skillList.Items.Count == index) index--;
                skillList.SelectedIndex = index;
                mainGrid.Children.Add(new SkillProperties(this, skills[index]));
            }
        }

        private void deleteClass() {
            if (trees.Count > 1) {
                ClearMainGrid();
                int index = classList.SelectedIndex;
                if (Application.Current.HasElevatedPermissions) {
                    File.Delete(CLASS_ROOT + trees[index].name + ".yml");
                }
                trees.RemoveAt(classList.SelectedIndex);
                classList.Items.RemoveAt(classList.SelectedIndex);
                if (classList.Items.Count == index) index--;
                classList.SelectedIndex = index;
                mainGrid.Children.Add(new TreeProperties(this, trees[index]));
            }
        }

        private int prevIndex = -1;

        /// <summary>
        /// Changes skills when one is selected
        /// </summary>
        /// <param name="sender">skill list</param>
        /// <param name="e">event details</param>
        private void SkillSelected(object sender, SelectionChangedEventArgs e) {
            bool cancel = prevIndex == -1;
            prevIndex = skillList.SelectedIndex;
            if (cancel) return;
            SelectSkill();
        }

        /// <summary>
        /// Changes the displayed class
        /// </summary>
        /// <param name="sender">class box</param>
        /// <param name="e">event details</param>
        private void ClassSelected(object sender, SelectionChangedEventArgs e) {
            bool cancel = prevIndex == -1;
            prevIndex = classList.SelectedIndex;
            if (cancel) return;
            SelectClass();
        }

        /// <summary>
        /// Updates the current data and clears the display
        /// </summary>
        private void ClearMainGrid() {
            if (mainGrid.Children.Count > 0) {
                (mainGrid.Children[0] as IUpdateable).Update();
                (mainGrid.Children[0] as IUpdateable).Clear();
            }
            mainGrid.Children.Clear();
        }

        /// <summary>
        /// Displays the data for the selected skill
        /// </summary>
        private void SelectSkill() {
            if (skillList.SelectedItem == null) return;
            ClearMainGrid();
            foreach (Skill skill in skills) {
                if (skill.name.Equals(skillList.SelectedItem.ToString())) {
                    SkillProperties prop = new SkillProperties(this, skill);
                    mainGrid.Children.Add(prop);
                    break;
                }
            }
        }

        /// <summary>
        /// Displays the data for the selected class
        /// </summary>
        private void SelectClass() {
            if (classList.SelectedItem == null) return;
            ClearMainGrid();
            foreach (Tree tree in trees) {
                if (tree.name.Equals(classList.SelectedItem.ToString())) {
                    mainGrid.Children.Add(new TreeProperties(this, tree));
                    break;
                }
            }
        }

        /// <summary>
        /// Change between skills and classes
        /// </summary>
        /// <param name="sender">tab</param>
        /// <param name="e">event details</param>
        private void TabChanged(object sender, SelectionChangedEventArgs e) {
            if (explorerTab == null) return;
            if (explorerTab.SelectedItem == skillTab) SelectSkill();
            else SelectClass();
        }

        /// <summary>
        /// Saves the changes to the data
        /// </summary>
        /// <param name="sender">save button</param>
        /// <param name="e">event details</param>
        private void saveButton_Click(object sender, RoutedEventArgs e) {
            Save();
        }

        private void saveSkillButton_Click(object sender, RoutedEventArgs e) {
            dialog.Filter = SKILL_FILTER;
            if (dialog.ShowDialog() == true) {
                using (StreamWriter write = new StreamWriter(dialog.OpenFile())) {
                    SaveSkills(write);
                }
            }
        }

        private void saveClassButton_Click(object sender, RoutedEventArgs e) {
            dialog.Filter = CLASS_FILTER;
            if (dialog.ShowDialog() == true) {
                using (StreamWriter write = new StreamWriter(dialog.OpenFile())) {
                    SaveClasses(write);
                }
            }
        }

        private void DropFile(object sender, DragEventArgs e) {
            IDataObject dataObject = e.Data as IDataObject;
            if (dataObject == null) return;
            FileInfo[] files = dataObject.GetData(DataFormats.FileDrop) as FileInfo[];
            if (files == null) return;

            foreach (FileInfo file in files) {
                if (file.Name.Equals("skills.yml")) {
                    using (StreamReader read = file.OpenText()) {
                        LoadSkill(read);
                    }
                }
                if (file.Name.Equals("classes.yml")) {
                    using (StreamReader read = file.OpenText()) {
                        LoadClass(read);
                    }
                }
            }
        }
    }
}
