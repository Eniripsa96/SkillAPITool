using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace SkillAPITool {

    /// <summary>
    /// Set of attributes for a skill
    /// </summary>
    public class AttributeSet {

        public static readonly string
            ACTIVE_PREFIX = "",
            PASSIVE_PREFIX = "Passive ",
            EMBED_PREFIX = "Embed ";

        public List<Attribute> attributes = new List<Attribute>();
        private List<String> aliasedKeys = new List<String>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="active">whether or not the effects are active</param>
        /// <param name="effects">effect list</param>
        public AttributeSet(string prefix, List<IEffect> effects) {
            
            // Get which effects to use aliases for
            List<String> keys = new List<String>();
            List<String> ids = new List<String>();
            foreach (IEffect effect in effects) {
                string id = effect.GetKey() + ";" + effect.GetTarget();
                if (ids.Contains(id)) continue;
                else ids.Add(id);
                if (keys.Contains(effect.GetKey())) {
                    aliasedKeys.Add(effect.GetKey());
                }
                else keys.Add(effect.GetKey());
            }

            // Get the list of attributes
            List<Attribute> unaliased = new List<Attribute>();
            keys.Clear();
            foreach (IEffect effect in effects) {
                effect.GetAttributes(unaliased);

                // Apply prefixes and aliases
                foreach (Attribute attribute in unaliased) {
                    string key = attribute.Key;
                    if (aliasedKeys.Contains(effect.GetKey())) {
                        key = effect.GetTarget() + " " + key;
                    }
                    key = prefix + key;

                    // Prevent duplicates
                    if (keys.Contains(key)) continue;
                    else keys.Add(key);

                    attributes.Add(new Attribute(key, attribute.Initial, attribute.Scale));
                }

                unaliased.Clear();
            }
        }

        /// <summary>
        /// Checks if the effect is aliased
        /// </summary>
        /// <param name="effect">effect to check</param>
        /// <returns>true if aliased, false otherwise</returns>
        public bool IsAliased(IEffect effect) {
            return aliasedKeys.Contains(effect.GetKey());
        }
    }
}
