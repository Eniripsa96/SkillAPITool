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

namespace SkillAPITool {

    /// <summary>
    /// Interface for updateable controls
    /// </summary>
    public interface IUpdateable {

        /// <summary>
        /// Updates the object to match control data
        /// </summary>
        void Update();

        /// <summary>
        /// Updates controls to match object data
        /// </summary>
        void Apply();

        /// <summary>
        /// Clears current data
        /// </summary>
        void Clear();
    }
}
