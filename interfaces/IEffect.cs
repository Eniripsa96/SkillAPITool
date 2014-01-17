using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SkillAPITool {

    /// <summary>
    /// Effect methods
    /// </summary>
    public interface IEffect {

        /// <summary>
        /// Returns the key for the effect
        /// </summary>
        /// <returns>key</returns>
        string GetKey();

        /// <summary>
        /// Returns the target of the group
        /// </summary>
        /// <returns>target type key</returns>
        string GetTarget();

        /// <summary>
        /// Returns the group the skill is targeting
        /// </summary>
        /// <returns>group name</returns>
        string GetGroup();

        /// <summary>
        /// Gets the attributes for the skill
        /// </summary>
        /// <param name="list">list to add to</param>
        void GetAttributes(List<Attribute> list);

        /// <summary>
        /// Gets the values for the skill
        /// </summary>
        /// <param name="list">list to add to</param>
        void GetValues(List<Value> list);

        /// <summary>
        /// Gets the strings for the skill
        /// </summary>
        /// <param name="list">list to add to</param>
        void GetStrings(List<StringValue> list);

        /// <summary>
        /// Applies a loaded attribute
        /// </summary>
        /// <param name="attribute">attribute</param>
        void ApplyAttribute(Attribute attribute);

        /// <summary>
        /// Applies a loaded value
        /// </summary>
        /// <param name="value">value</param>
        void ApplyValue(Value value);

        /// <summary>
        /// Applies a loaded string
        /// </summary>
        /// <param name="value">value</param>
        void ApplyString(StringValue value);

        /// <summary>
        /// Removes the linear target option
        /// </summary>
        void RemoveLinear();
    }
}
