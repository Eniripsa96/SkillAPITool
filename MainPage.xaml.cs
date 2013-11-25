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

namespace SkillAPITool {

    /// <summary>
    /// Main page of the tool
    /// </summary>
    public partial class MainPage : UserControl {

        private static readonly string 
            DEFAULT_SKILL_NAME = "Skill",
            DEFAULT_CLASS_NAME = "Class",
            SKILL_FILE = "Skills.yml",
            CLASS_FILE = "Classes.yml";

        public readonly List<Skill> skills = new List<Skill>();
        public readonly List<Tree> trees = new List<Tree>();

        private ResourceDictionary effectDictionary = new ResourceDictionary();
        private DispatcherTimer timer;

        /// <summary>
        /// Constructor
        /// </summary>
        public MainPage() {
            InitializeComponent();
            configText.IsReadOnly = true;
            CreateNewClass();
            CreateNewSkill();

            // Set up timer
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Tick += TimerComplete;
            timer.Start();

            // Register effect names
            effectDictionary.Add("attackmodifier", "AttackModifier");
            effectDictionary.Add("chance", "Chance");
            effectDictionary.Add("cleanse", "Cleanse");
            effectDictionary.Add("condition", "Condition");
            effectDictionary.Add("damagebonus", "DamageBonus");
            effectDictionary.Add("damage", "Damage");
            effectDictionary.Add("damagepercent", "DamagePercent");
            effectDictionary.Add("damagepercentreduction", "DamagePercentReduction");
            effectDictionary.Add("damagereduction", "DamageReduction");
            effectDictionary.Add("delay", "Delay");
            effectDictionary.Add("dot", "DOT");
            effectDictionary.Add("fire", "Fire");
            effectDictionary.Add("heal", "Heal");
            effectDictionary.Add("hot", "HealOverTime");
            effectDictionary.Add("launch", "Launch");
            effectDictionary.Add("mana", "Mana");
            effectDictionary.Add("manadamage", "ManaDamage");
            effectDictionary.Add("particle", "Particle");
            effectDictionary.Add("potion", "Potion");
            effectDictionary.Add("projectile", "Projectile");
            effectDictionary.Add("pull", "Pull");
            effectDictionary.Add("push", "Push");
            effectDictionary.Add("sound", "Sound");
            effectDictionary.Add("status", "Status");
            effectDictionary.Add("taunt", "Taunt");
            effectDictionary.Add("teleport", "Teleport");
            effectDictionary.Add("teleportTarget", "TeleportTarget");
        }

        /// <summary>
        /// Updates the .yml output each timer tick
        /// </summary>
        /// <param name="sender">timer</param>
        /// <param name="e">event details</param>
        void TimerComplete(object sender, EventArgs e) {
            Update();
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
            Update();
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
            Update();
        }

        /// <summary>
        /// Updates the config output
        /// </summary>
        public void Update() {
            (mainGrid.Children[0] as IUpdateable).Update();
            if (mainGrid.Children[0] is SkillProperties) UpdateSkills();
            else UpdateClasses();
        }

        /// <summary>
        /// Updates skill data
        /// </summary>
        private void UpdateSkills() {
            fileLabel.Content = SKILL_FILE;

            string data = "";
            int si = skillList.SelectedIndex;
            skillList.Items.Clear();
            foreach (Skill skill in skills) {
                skillList.Items.Add(skill.name);

                // Basic
                data += skill.name + ":\n";
                data += "  type: " + skill.type + "\n";
                data += "  indicator: " + skill.indicator + "\n";
                data += "  max-level: " + skill.maxLevel + "\n";
                
                // Skill requirement
                if (skill.RequiresSkill) {
                    data += "  skill-req: " + skill.skillReq + "\n";
                    data += "  skill-req-level: " + skill.skillReqLevel + "\n";
                }

                // Item requirement
                if (skill.RequiresItem) {
                    data += "  item-req: " + skill.itemReq;
                }

                // Description
                if (skill.HasDescription) {
                    data += "  description:\n";
                    List<String> lines = TextUtil.SplitDescription(skill.description);
                    foreach (string line in lines) data += "  - '" + line + "'\n";
                }
                else data += "  description: []\n";

                // Actives
                if (skill.active.Count > 0) {
                    data += "  active:\n";
                    int index = 0;
                    List<String> used = new List<String>();
                    foreach (IEffect effect in skill.active) {
                        string key = effect.GetKey() + ";" + effect.GetTarget();
                        if (used.Contains(key)) continue;
                        else used.Add(key);
                        data += "    m" + index++ + ":\n";
                        data += "      effect: " + effect.GetKey() + "\n";
                        data += "      target: " + effect.GetTarget() + "\n";
                        data += "      group: " + effect.GetGroup() + "\n";
                    }
                }
                else data += "  active: {}\n";

                // Passives
                if (skill.passive.Count > 0) {
                    data += "  passive:\n";
                    int index = 0;
                    List<String> used = new List<String>();
                    foreach (IEffect effect in skill.passive) {
                        string key = effect.GetKey() + ";" + effect.GetTarget();
                        if (used.Contains(key)) continue;
                        else used.Add(key);
                        data += "    m" + index++ + ":\n";
                        data += "      effect: " + effect.GetKey() + "\n";
                        data += "      target: " + effect.GetTarget() + "\n";
                        data += "      group: " + effect.GetGroup() + "\n";
                    }
                }
                else data += "  passive: {}\n";

                // Embedded
                if (skill.RequiresEmbed && skill.embedded.Count > 0) {
                    data += "  embed:\n";
                    int index = 0;
                    List<String> used = new List<String>();
                    foreach (IEffect effect in skill.embedded) {
                        string key = effect.GetKey() + ";" + effect.GetTarget();
                        if (used.Contains(key)) continue;
                        else used.Add(key);
                        data += "    m" + index++ + ":\n";
                        data += "      effect: " + effect.GetKey() + "\n";
                        data += "      target: " + effect.GetTarget() + "\n";
                        data += "      group: " + effect.GetGroup() + "\n";
                    }
                }
                else data += "  embed: {}\n";

                // Basic attributes
                data += "  attributes:\n";
                foreach (Attribute attribute in skill.Attributes) {
                    if (attribute.Key.Equals("Range") && !skill.RequiresRange) continue;
                    if (attribute.Key.Equals("Radius") && !skill.RequiresRadius) continue;
                    data += "    " + attribute.Key + ":\n";
                    data += "      base: " + attribute.Initial + "\n";
                    data += "      scale: " + attribute.Scale + "\n";
                }

                // Active Attributes
                AttributeSet activeSet = new AttributeSet(AttributeSet.ACTIVE_PREFIX, skill.active);
                foreach (Attribute attribute in activeSet.attributes) {
                    data += "    " + attribute.Key + ":\n";
                    data += "      base: " + attribute.Initial + "\n";
                    data += "      scale: " + attribute.Scale + "\n";
                }

                // Passive Attributes
                AttributeSet passiveSet = new AttributeSet(AttributeSet.PASSIVE_PREFIX, skill.passive);
                foreach (Attribute attribute in passiveSet.attributes) {
                    data += "    " + attribute.Key + ":\n";
                    data += "      base: " + attribute.Initial + "\n";
                    data += "      scale: " + attribute.Scale + "\n";
                }

                // Embed Attributes
                AttributeSet embedSet = new AttributeSet(AttributeSet.EMBED_PREFIX, skill.embedded);
                if (skill.RequiresEmbed) {
                    foreach (Attribute attribute in embedSet.attributes) {
                        data += "    " + attribute.Key + ":\n";
                        data += "      base: " + attribute.Initial + "\n";
                        data += "      scale: " + attribute.Scale + "\n";
                    }
                }

                // Values
                List<String> keys = new List<String>();
                List<Value> values = new List<Value>();
                bool valueBase = false;
                foreach (IEffect effect in skill) {
                    effect.GetValues(values);

                    foreach (Value value in values) {
                        if (!valueBase) {
                            valueBase = true;
                            data += "  values:\n";
                        }
                        if (keys.Contains(value.Key)) continue;
                        else keys.Add(value.Key);

                        data += "    " + value.Key + ": " + value.Number + "\n";
                    }

                    values.Clear();
                }
            }
            skillList.SelectedIndex = si;

            configText.Text = data;
        }

        /// <summary>
        /// Updates class data
        /// </summary>
        private void UpdateClasses() {
            fileLabel.Content = CLASS_FILE;

            string data = "";
            int index = classList.SelectedIndex;
            classList.Items.Clear();
            foreach (Tree tree in trees) {
                classList.Items.Add(tree.name);

                // Basic values
                data += tree.name + ":\n";
                data += "  prefix: '" + tree.prefix + "'\n";
                data += "  max-level: " + tree.maxLevel + "\n";
                data += "  health-base: " + tree.healthBase + "\n";
                data += "  health-bonus: " + tree.healthScale + "\n";
                data += "  mana-base: " + tree.manaBase + "\n";
                data += "  mana-bonus: " + tree.manaBonus + "\n";

                // Inheritance
                if (tree.CanInherit) {
                    data += "  profess-level: " + Math.Max(tree.professLevel, 1) + "\n";
                    data += "  parent: " + tree.parent + "\n";
                    if (tree.inherit) {
                        data += "  inherit:\n";
                        data += "  - " + tree.parent + "\n";
                    }
                }
                else data += "  profess-level: 0\n";

                // Skills
                if (tree.skills.Count == 0) {
                    data += "  skills: []\n";
                }
                else {
                    data += "  skills:\n";
                    foreach (string skill in tree.skills) {
                        data += "  - " + skill + "\n";
                    }
                }
            }
            classList.SelectedIndex = index;
            configText.Text = data;
        }

        /// <summary>
        /// Loads a dropped file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DropFile(object sender, DragEventArgs e) {

            IDataObject dataObject = e.Data as IDataObject;
            FileInfo[] files = dataObject.GetData(DataFormats.FileDrop) as FileInfo[];

            foreach (FileInfo file in files) {
                
                // Skills file
                if (file.Exists && file.Name.ToLower().Equals("skills.yml")) {

                    using (StreamReader sreader = file.OpenText()) {
                        string data = "";

                        // Clear skill data
                        skills.Clear();
                        skillList.Items.Clear();
                        mainGrid.Children.Clear();

                        // Initialize data
                        Skill skill = null;
                        string effect, target, group;
                        effect = target = group = null;
                        bool a, p, c, m, v;
                        a = p = c = m = v = false;
                        List<Attribute> attributes = new List<Attribute>();
                        List<Value> values = new List<Value>();
                        Attribute currentAttribute = new Attribute("", 0, 0);

                        // Read through the file
                        while ((data = sreader.ReadLine()) != null) {

                            // Ignore comments
                            if (data.StartsWith("#") || data.Length == 0) {
                                continue;
                            }

                            // Sub-values
                            if (data.StartsWith("    ")) {
                                data = data.Substring(4);

                                // Attributes
                                if (a) {
                                    if (data.StartsWith("  base: ")) currentAttribute.Initial = int.Parse(data.Substring(8));
                                    else if (data.StartsWith("  scale: ")) currentAttribute.Scale = int.Parse(data.Substring(9));
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
                            a = m = c = p = v = false;

                            // New skill
                            if (!data.StartsWith(" ")) {
                                if (skill != null) {
                                    UpdateSkill(skill, attributes, values);
                                    attributes.Clear();
                                    values.Clear();
                                }
                                if (mainGrid.Children.Count > 0) (mainGrid.Children[0] as IUpdateable).Apply();
                                CreateNewSkill();
                                skill = skills[skills.Count - 1];
                                skill.name = data.Substring(0, data.IndexOf(':'));
                            }

                            // Description
                            else if (data.StartsWith("  - ")) {
                                string value = data.Substring(4);
                                if (value[0] == '\'') {
                                    value = value.Substring(1, value.LastIndexOf('\'') - 1);
                                }
                                if (skill.description.Length > 0) skill.description += ' ';
                                skill.description += value;
                            }

                            // Value groups
                            else if (data.StartsWith("  attributes:")) a = true;
                            else if (data.StartsWith("  values:")) v = true;
                            else if (data.StartsWith("  active:")) c = true;
                            else if (data.StartsWith("  passive:")) p = true;
                            else if (data.StartsWith("  embed:")) m = true;
                            else if (data.StartsWith("  type: ")) skill.type = data.Substring(8);
                            else if (data.StartsWith("  indicator: ")) skill.indicator = data.Substring(13);
                            else if (data.StartsWith("  item-req: ")) skill.itemReq = data.Substring(12);
                            else if (data.StartsWith("  skill-req:")) skill.skillReq = data.Substring(13);
                            else if (data.StartsWith("  skill-req-level: ")) skill.skillReqLevel = int.Parse(data.Substring(18));
                            else if (data.StartsWith("  max-level: ")) skill.maxLevel = int.Parse(data.Substring(13));
                        }

                        // Add the last attribute if there were any
                        if (!currentAttribute.Key.Equals("")) {
                            attributes.Add(new Attribute(currentAttribute.Key, currentAttribute.Initial, currentAttribute.Scale));
                        }

                        // Apply the loaded data
                        UpdateSkill(skill, attributes, values);
                        if (skills.Count == 0) CreateNewSkill();
                        else (mainGrid.Children[0] as IUpdateable).Apply();
                        Update();
                        explorerTab.SelectedItem = skillTab;
                    }
                }

                // Class file
                else if (file.Exists && file.Name.ToLower().Equals("classes.yml")) {

                    using (StreamReader sreader = file.OpenText()) {
                        string data = "";

                        // Clear class data
                        trees.Clear();
                        classList.Items.Clear();
                        mainGrid.Children.Clear();

                        // Initialize data
                        Tree tree = null;
                        bool skills = false;

                        // Read through the file
                        while ((data = sreader.ReadLine()) != null) {

                            // Ignore comments
                            if (data.StartsWith("#") || data.Length == 0) {
                                continue;
                            }

                            // New class
                            if (!data.StartsWith(" ")) {
                                if (mainGrid.Children.Count > 0) (mainGrid.Children[0] as IUpdateable).Apply();
                                CreateNewClass();
                                tree = trees[trees.Count - 1];
                                tree.name = data.Substring(0, data.IndexOf(':'));
                            }

                            // Prefix
                            else if (data.StartsWith("  prefix: '")) {
                                int index = data.IndexOf('\'') + 1;
                                tree.prefix = data.Substring(index, data.IndexOf('\'', index) - index);
                            }

                            // Class data
                            else if (data.StartsWith("  parent: ")) tree.parent = data.Substring(data.IndexOf(": ") + 2);
                            else if (data.StartsWith("  profess-level: ")) tree.professLevel = int.Parse(data.Substring(data.IndexOf(": ") + 2));
                            else if (data.StartsWith("  max-level: ")) tree.maxLevel = int.Parse(data.Substring(data.IndexOf(": ") + 2));
                            else if (data.StartsWith("  health-base: ")) tree.healthBase = int.Parse(data.Substring(data.IndexOf(": ") + 2));
                            else if (data.StartsWith("  health-scale: ")) tree.healthScale = int.Parse(data.Substring(data.IndexOf(": ") + 2));
                            else if (data.StartsWith("  mana-base: ")) tree.manaBase = int.Parse(data.Substring(data.IndexOf(": ") + 2));
                            else if (data.StartsWith("  mana-scale: ")) tree.manaBonus = int.Parse(data.Substring(data.IndexOf(": ") + 2));
                            else if (data.Equals("  inherit:")) tree.inherit = true;
                            else if (data.Equals("  inherit: []")) tree.inherit = false;
                            else if (data.Equals("  skills:")) skills = true;
                            else if (data.StartsWith("  - ") && skills) tree.skills.Add(data.Substring(4));
                        }

                        // Apply loaded data
                        if (trees.Count == 0) CreateNewClass();
                        else (mainGrid.Children[0] as IUpdateable).Apply();
                        Update();
                        explorerTab.SelectedItem = classTab;
                    }
                }
            }
        }

        /// <summary>
        /// Updates a skill with the list of attributes and values
        /// </summary>
        /// <param name="skill">skill to update</param>
        /// <param name="attributes">attributes to update with</param>
        /// <param name="values">values to update with</param>
        private void UpdateSkill(Skill skill, List<Attribute> attributes, List<Value> values) {
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
        }

        /// <summary>
        /// Updates the config file
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">event details</param>
        private void UpdateClicked(object sender, RoutedEventArgs e) {
            Update();
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
    }
}
