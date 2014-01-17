using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace SkillAPITool {

    /// <summary>
    /// A skill as defined by the editor
    /// </summary>
    public class Skill : IEnumerable<IEffect> {

        private static readonly List<String> RANGED = new List<String>(new string[] { "Target", "Target Area", "Linear" });
        private static readonly List<String> RADIUSED = new List<String>(new string[] { "Area", "Target Area" });
        private static readonly char[] SPLITTER = new char[] { ',' };

        public List<IEffect> active = new List<IEffect>();
        public List<IEffect> passive = new List<IEffect>();
        public List<IEffect> embedded = new List<IEffect>();

        public Attribute level = new Attribute("Level", 1, 1);
        public Attribute cost = new Attribute("Cost", 1, 0);
        public Attribute mana = new Attribute("Mana", 0, 0);
        public Attribute cooldown = new Attribute("Cooldown", 0, 0);
        public Attribute range = new Attribute("Range", 8, 0);
        public Attribute radius = new Attribute("Radius", 5, 0);
        public Attribute period = new Attribute("Period", 3, 0);
        public bool needsPermission = false;
        public string name;
        public string type = "Target";
        public string indicator = "Apple";
        public string itemReq = "";
        public string description = "";
        public string message = "";
        public string skillReq = "";
        public string permissions = "";
        public int maxLevel = 5;
        public int skillReqLevel = 1;

        private StringBuilder sb = new StringBuilder();
        public string data;

        /// <summary>
        /// Whether or not the skill requires an item
        /// </summary>
        public bool RequiresItem {
            get { return itemReq.Length > 0; }
        }

        /// <summary>
        /// Whether or not this skill requires another
        /// </summary>
        public bool RequiresSkill {
            get { return skillReq.Length > 0; }
        }

        /// <summary>
        /// Whether or not the skill requires embedded effects
        /// </summary>
        public bool RequiresEmbed {
            get {
                foreach (IEffect effect in active) {
                    if (effect is IEmbedable) return true;
                }
                foreach (IEffect effect in passive) {
                    if (effect is IEmbedable) return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Whether or not the skill requires a range value
        /// </summary>
        public bool RequiresRange {
            get {
                foreach (IEffect effect in this) {
                    if (RANGED.Contains(effect.GetTarget())) {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Whether or not the skill requires a radius value
        /// </summary>
        public bool RequiresRadius {
            get {
                foreach (IEffect effect in this) {
                    if (RADIUSED.Contains(effect.GetTarget())) {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Attribute array of the skill
        /// </summary>
        public Attribute[] Attributes {
            get { return new Attribute[] { level, cost, mana, cooldown,  range, radius, period }; }
        }

        /// <summary>
        /// Whether or not this skill has a description
        /// </summary>
        public bool HasDescription {
            get { return description.Replace(" ", "").Length > 0; }
        }

        /// <summary>
        /// Whether or not the skill has a message
        /// </summary>
        public bool hasMessage {
            get { return message.Replace(" ", "").Length > 0; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">skill name</param>
        public Skill(string name) {
            this.name = name;
        }

        /// <summary>
        /// Retrieves the enumerator
        /// </summary>
        /// <returns>enumerator to return</returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        /// <summary>
        /// Enumeration through both sets of effects
        /// </summary>
        /// <returns>enumeration through both sets of effects</returns>
        public IEnumerator<IEffect> GetEnumerator() {
            foreach (IEffect effect in active) {
                yield return effect;
            }
            foreach (IEffect effect in passive) {
                yield return effect;
            }
            if (RequiresEmbed) {
                foreach (IEffect effect in embedded) {
                    yield return effect;
                }
            }
        }

        public void Update() {

            sb.Clear();
                        
            // Basic
            sb.Append(name); 
            sb.Append(":\n");
            sb.Append("  type: "); 
            sb.Append(type); 
            sb.Append("\n");
            sb.Append("  indicator: "); 
            sb.Append(indicator); 
            sb.Append("\n");
            sb.Append("  max-level: "); 
            sb.Append(maxLevel); 
            sb.Append("\n");

            // Skill requirement
            if (RequiresSkill) {
                sb.Append("  skill-req: "); 
                sb.Append(skillReq); 
                sb.Append("\n");
                sb.Append("  skill-req-level: "); 
                sb.Append(skillReqLevel); 
                sb.Append("\n");
            }

            // Item requirement
            if (RequiresItem) {
                sb.Append("  item-req: "); 
                sb.Append(itemReq);
            }

            // Needs permission
            sb.Append("  needs-permission: ");
            sb.Append(needsPermission.ToString().ToLower());
            sb.Append("\n");

            // Description
            if (HasDescription) {
                sb.Append("  description:\n");
                List<String> lines = TextUtil.SplitDescription(description);
                foreach (string line in lines) {
                    sb.Append("  - '"); 
                    sb.Append(line); 
                    sb.Append("'\n");
                }
            }
            else sb.Append("  description: []\n");

            // Message
            if (hasMessage) {
                sb.Append("  message: '"); 
                sb.Append(message.Replace("'", "")); 
                sb.Append("'\n");
            }

            // Permissions
            string[] perms;
            if (permissions.Contains(",")) {
                perms = permissions.Split(SPLITTER);
            }
            else perms = new string[] { permissions };
            bool first = true;
            foreach (string perm in perms) {
                string p = perm.Replace(" ", "").Replace(",", "");
                if (p.Length > 0) {
                    if (first) {
                        first = false;
                        sb.Append("  permissions:\n");
                    }
                    sb.Append("  - ");
                    sb.Append(p);
                    sb.Append("\n");
                }
            }
            if (first) sb.Append("  permissions: []\n");

            // Actives
            if (active.Count > 0) {
                sb.Append("  active:\n");
                int index = 0;
                List<String> used = new List<String>();
                foreach (IEffect effect in active) {
                    string key = effect.GetKey() + ";" + effect.GetTarget();
                    if (used.Contains(key)) continue;
                    else if (effect is AttackModifierEffect) {
                        string key2 = "DefenseModifier;" + effect.GetTarget();
                        if (used.Contains(key2)) continue;
                        else used.Add(key);
                    }
                    else if (effect is DefenseModifierEffect) {
                        string key2 = "AttackModifier;" + effect.GetTarget();
                        if (used.Contains(key2)) continue;
                        else used.Add(key);
                    }
                    else used.Add(key);
                    sb.Append("    m"); sb.Append(index++); 
                    sb.Append(":\n");
                    sb.Append("      effect: "); 
                    sb.Append(effect.GetKey()); 
                    sb.Append("\n");
                    sb.Append("      target: "); 
                    sb.Append(effect.GetTarget()); 
                    sb.Append("\n");
                    sb.Append("      group: "); 
                    sb.Append(effect.GetGroup()); 
                    sb.Append("\n");
                }
            }
            else sb.Append("  active: {}\n");

            // Passives
            if (passive.Count > 0) {
                sb.Append("  passive:\n");
                int index = 0;
                List<String> used = new List<String>();
                foreach (IEffect effect in passive) {
                    string key = effect.GetKey() + ";" + effect.GetTarget();
                    if (used.Contains(key)) continue;
                    else if (effect is AttackModifierEffect) {
                        string key2 = "DefenseModifier;" + effect.GetTarget();
                        if (used.Contains(key2)) continue;
                        else used.Add(key);
                    }
                    else if (effect is DefenseModifierEffect) {
                        string key2 = "AttackModifier;" + effect.GetTarget();
                        if (used.Contains(key2)) continue;
                        else used.Add(key);
                    }
                    else used.Add(key);
                    sb.Append("    m"); 
                    sb.Append(index++); 
                    sb.Append(":\n");
                    sb.Append("      effect: "); 
                    sb.Append(effect.GetKey()); 
                    sb.Append("\n");
                    sb.Append("      target: "); 
                    sb.Append(effect.GetTarget()); 
                    sb.Append("\n");
                    sb.Append("      group: "); 
                    sb.Append(effect.GetGroup()); 
                    sb.Append("\n");
                }
            }
            else sb.Append("  passive: {}\n");

            // Embedded
            if (RequiresEmbed && embedded.Count > 0) {
                sb.Append("  embed:\n");
                int index = 0;
                List<String> used = new List<String>();
                foreach (IEffect effect in embedded) {
                    string key = effect.GetKey() + ";" + effect.GetTarget();
                    if (used.Contains(key)) continue;
                    else if (effect is AttackModifierEffect) {
                        string key2 = "DefenseModifier;" + effect.GetTarget();
                        if (used.Contains(key2)) continue;
                        else used.Add(key);
                    }
                    else if (effect is DefenseModifierEffect) {
                        string key2 = "AttackModifier;" + effect.GetTarget();
                        if (used.Contains(key2)) continue;
                        else used.Add(key);
                    }
                    else used.Add(key);
                    sb.Append("    m"); 
                    sb.Append(index++); 
                    sb.Append(":\n");
                    sb.Append("      effect: "); 
                    sb.Append(effect.GetKey()); 
                    sb.Append("\n");
                    sb.Append("      target: "); 
                    sb.Append(effect.GetTarget()); 
                    sb.Append("\n");
                    sb.Append("      group: "); 
                    sb.Append(effect.GetGroup()); 
                    sb.Append("\n");
                }
            }
            else sb.Append("  embed: {}\n");

            // Basic attributes
            sb.Append("  attributes:\n");
            foreach (Attribute attribute in Attributes) {
                if (attribute.Key.Equals("Range") && !RequiresRange) continue;
                if (attribute.Key.Equals("Radius") && !RequiresRadius) continue;
                if (attribute.Key.Equals("Period") && passive.Count == 0) continue;
                sb.Append("    "); 
                sb.Append(attribute.Key); 
                sb.Append(":\n");
                sb.Append("      base: "); 
                sb.Append(attribute.Initial); 
                sb.Append("\n");
                sb.Append("      scale: "); 
                sb.Append(attribute.Scale); 
                sb.Append("\n");
            }

            // Active Attributes
            AttributeSet activeSet = new AttributeSet(AttributeSet.ACTIVE_PREFIX, active);
            foreach (Attribute attribute in activeSet.attributes) {
                sb.Append("    "); 
                sb.Append(attribute.Key); 
                sb.Append(":\n");
                sb.Append("      base: "); 
                sb.Append(attribute.Initial); 
                sb.Append("\n");
                sb.Append("      scale: "); 
                sb.Append(attribute.Scale); 
                sb.Append("\n");
            }

            // Passive Attributes
            AttributeSet passiveSet = new AttributeSet(AttributeSet.PASSIVE_PREFIX, passive);
            foreach (Attribute attribute in passiveSet.attributes) {
                sb.Append("    "); 
                sb.Append(attribute.Key); 
                sb.Append(":\n");
                sb.Append("      base: "); 
                sb.Append(attribute.Initial); 
                sb.Append("\n");
                sb.Append("      scale: "); 
                sb.Append(attribute.Scale); 
                sb.Append("\n");
            }

            // Embed Attributes
            AttributeSet embedSet = new AttributeSet(AttributeSet.EMBED_PREFIX, embedded);
            if (RequiresEmbed) {
                foreach (Attribute attribute in embedSet.attributes) {
                    sb.Append("    "); 
                    sb.Append(attribute.Key); 
                    sb.Append(":\n");
                    sb.Append("      base: "); 
                    sb.Append(attribute.Initial); 
                    sb.Append("\n");
                    sb.Append("      scale: "); 
                    sb.Append(attribute.Scale); 
                    sb.Append("\n");
                }
            }

            // Values
            List<string> keys = new List<string>();
            List<Value> values = new List<Value>();
            bool valueBase = false;
            foreach (IEffect effect in this) {
                effect.GetValues(values);

                foreach (Value value in values) {
                    if (!valueBase) {
                        valueBase = true;
                        sb.Append("  values:\n");
                    }
                    if (keys.Contains(value.Key)) continue;
                    else keys.Add(value.Key);

                    sb.Append("    "); 
                    sb.Append(value.Key); 
                    sb.Append(": "); 
                    sb.Append(value.Number); 
                    sb.Append("\n");
                }

                values.Clear();
            }
            if (!valueBase) sb.Append("  values: {}\n");

            // Strings
            keys.Clear();
            List<StringValue> strings = new List<StringValue>();
            valueBase = false;
            foreach (IEffect effect in this) {
                effect.GetStrings(strings);

                foreach (StringValue value in strings) {
                    if (!valueBase) {
                        valueBase = true;
                        sb.Append("  strings:\n");
                    }
                    if (keys.Contains(value.Key)) continue;
                    else keys.Add(value.Key);

                    sb.Append("    ");
                    sb.Append(value.Key);
                    sb.Append(": ");
                    sb.Append(value.String);
                    sb.Append("\n");
                }

                strings.Clear();
            }
            if (!valueBase) sb.Append("  strings: {}\n");

            data = sb.ToString();
        }
    }
}
