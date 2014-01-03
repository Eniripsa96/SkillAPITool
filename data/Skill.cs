using System;
using System.Collections.Generic;
using System.Collections;

namespace SkillAPITool {

    /// <summary>
    /// A skill as defined by the editor
    /// </summary>
    public class Skill : IEnumerable<IEffect> {

        private static readonly List<String> RANGED = new List<String>(new string[] { "Target", "Target Area", "Linear" });
        private static readonly List<String> RADIUSED = new List<String>(new string[] { "Area", "Target Area" });

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
        public string name;
        public string type = "Target";
        public string indicator = "Apple";
        public string itemReq = "";
        public string description = "";
        public string message = "";
        public string skillReq = "";
        public int maxLevel = 5;
        public int skillReqLevel = 1;
        

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
    }
}
