using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SkillAPITool {

    /// <summary>
    /// Filters for text boxes
    /// </summary>
    class Filter {

        private static readonly SolidColorBrush VALID_BRUSH = new SolidColorBrush(Color.FromArgb((byte)255, (byte)255, (byte)255, (byte)255));
        private static readonly SolidColorBrush INVALID_BRUSH = new SolidColorBrush(Color.FromArgb((byte)255, (byte)255, (byte)85, (byte)85));

        private static readonly int[] INT_RANGE = new int[] { 48, 57 };
        private static readonly int[] DOUBLE_RANGE = new int[] { 46, 46, 48, 57 };
        private static readonly int[] WORD_RANGE = new int[] { 32, 32, 48, 57, 65, 90, 97, 122 };

        private static readonly char[] LIST_SPLIT = new char[] { ',' };

        /// <summary>
        /// Filters a text box to only include integers
        /// </summary>
        /// <param name="sender">text box</param>
        /// <param name="e">event details</param>
        public static void FilterInt(object sender, RoutedEventArgs e) {
            TextBox box = (TextBox)sender;

            // Filter the text
            string content = FilterRange(box, INT_RANGE);
            while (content.Length > 0 && content[0] == '0') content = content.Substring(1);

            // If no text was valid, make it 0
            if (content.Length == 0) content = "0";

            // Update box
            if (box.Text != content) {
                box.Text = content;
                box.SelectionStart = box.Text.Length;
            }
        }

        /// <summary>
        /// Filters a negative integer value
        /// </summary>
        /// <param name="sender">text box</param>
        /// <param name="e">event details</param>
        public static void FilterNInt(object sender, RoutedEventArgs e) {
            TextBox box = (TextBox)sender;

            // Filter the text
            bool negative = false;
            string nCheck = box.Text;
            while (nCheck.Contains('-')) {
                nCheck = nCheck.Remove(nCheck.IndexOf('-'), 1);
                negative = !negative;
            }
            string content = FilterRange(box, INT_RANGE);
            while (content.Length > 0 && content[0] == '0') content = content.Substring(1);

            // If no text was valid, make it 0
            if (content.Length == 0) content = "0";
            
            // Negative numbers
            if (negative) content = "-" + content;

            // Update box
            if (box.Text != content) {
                box.Text = content;
                box.SelectionStart = box.Text.Length;
            }
        }

        /// <summary>
        /// Filters a text box to only include double values
        /// </summary>
        /// <param name="sender">text box</param>
        /// <param name="e">event details</param>
        public static void FilterDouble(object sender, RoutedEventArgs e) {
            TextBox box = (TextBox)sender;

            // Filter the text
            string content = FilterRange(box, DOUBLE_RANGE);
            while (content.Length > 1 && content[0] == '0' && content[1] != '.') content = content.Substring(1);

            // If no text was valid, make it 0
            if (content.Length == 0 || content[0] == '.') content = "0" + content;

            // Prevent multiple decimal points
            int first = content.IndexOf('.');
            int last = content.LastIndexOf('.');
            if (first != last) content = content.Remove(last, 1);

            // Update box
            if (box.Text != content) {
                box.Text = content;
                box.SelectionStart = box.Text.Length;
            }
        }

        /// <summary>
        /// Filters a negative integer value
        /// </summary>
        /// <param name="sender">text box</param>
        /// <param name="e">event details</param>
        public static void FilterNDouble(object sender, RoutedEventArgs e) {
            TextBox box = (TextBox)sender;

            // Filter the text
            bool negative = false;
            string nCheck = box.Text;
            while (nCheck.Contains('-')) {
                nCheck = nCheck.Remove(nCheck.IndexOf('-'), 1);
                negative = !negative;
            }

            // Filter the text
            string content = FilterRange(box, DOUBLE_RANGE);
            while (content.Length > 1 && content[0] == '0' && content[1] != '.') content = content.Substring(1);

            // If no text was valid, make it 0
            if (content.Length == 0 || content[0] == '.') content = "0" + content;

            // Prevent multiple decimal points
            int first = content.IndexOf('.');
            int last = content.LastIndexOf('.');
            if (first != last) content = content.Remove(last, 1);

            // Negative numbers
            if (negative) content = "-" + content;

            // Update box
            if (box.Text != content) {
                box.Text = content;
                box.SelectionStart = box.Text.Length;
            }
        }

        /// <summary>
        /// Filters a text box to only include letters and spaces
        /// </summary>
        /// <param name="sender">text box</param>
        /// <param name="e">event details</param>
        public static void FilterWords(object sender, RoutedEventArgs e) {
            TextBox box = (TextBox)sender;

            // Filter the text
            string content = FilterRange(box, WORD_RANGE);
            while (content.Contains("  ")) content = content.Replace("  ", " ");
            while (content.Length > 0 && content[0] == ' ') content = content.Substring(1);

            // Move cursor to end
            if (!content.Equals(box.Text)) {
                box.Text = content;
                box.SelectionStart = box.Text.Length;
            }
        }

        /// <summary>
        /// Filters a text box to only include characteres in the provided ranges
        /// </summary>
        /// <param name="box">text box to filter</param>
        /// <param name="ranges">ranges in the format { min, max ... }</param>
        private static string FilterRange(TextBox box, params int[] ranges) {
            if (box.Text.Length == 0) return "";
            string content = "";
            foreach (char c in box.Text.ToCharArray()) {

                // Make sure it falls within the ranges
                int value = (int)c;
                for (int i = 0; i < ranges.Length; i += 2) {
                    if (value >= ranges[i] && value <= ranges[i + 1]) {
                        content += c;
                    }
                }
            }

            return content;
        }

        /// <summary>
        /// Filters a material box to indicate whether or not it is valid
        /// </summary>
        /// <param name="sender">text box</param>
        /// <param name="e">event details</param>
        public static void FilterMaterial(object sender, RoutedEventArgs e) {
            TextBox box = (TextBox)sender;
            if (MaterialDictionary.IsValid(box.Text)) box.Background = VALID_BRUSH;
            else box.Background = INVALID_BRUSH;
        }

        /// <summary>
        /// Filters a material list box to indicate whether or not it is valid list
        /// </summary>
        /// <param name="sender">text box</param>
        /// <param name="e">event details</param>
        public static void FilterMaterials(object sender, RoutedEventArgs e) {
            TextBox box = (TextBox)sender;
            string[] pieces;
            if (box.Text.Contains(",")) pieces = box.Text.Split(LIST_SPLIT);
            else pieces = new string[] { box.Text };

            bool valid = true;
            foreach (string mat in pieces) {
                if (mat.Length > 0 && !MaterialDictionary.IsValid(mat)) valid = false;
            }

            if (valid) box.Background = VALID_BRUSH;
            else box.Background = INVALID_BRUSH;
        }
    }
}
