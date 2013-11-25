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
    /// Utilities for strings
    /// </summary>
    public class TextUtil {

        private static readonly int MAX_LENGTH = 25;
        private static readonly int MIN_LENGTH = 16;

        /// <summary>
        /// Splits a string into lines for a description
        /// </summary>
        /// <param name="s">string to split</param>
        /// <returns>split string</returns>
        public static List<String> SplitDescription(string s) {
            List<String> result = new List<String>();

            string part = "";
            string word = "";
            foreach (char c in s.ToCharArray()) {
                if (c == ' ') {
                    if (word.Length == 0) continue;
                    else AppendWord(result, ref part, ref word);
                }
                else word += c;
            }
            if (word.Length > 0) AppendWord(result, ref part, ref word);
            if (part.Length > 0) result.Add(part);

            return result;
        }

        /// <summary>
        /// Appends a word to the result of a description split
        /// </summary>
        /// <param name="result">current result</param>
        /// <param name="part">current part</param>
        /// <param name="word">current word</param>
        private static void AppendWord(List<String> result, ref string part, ref string word) {
            if (part.Length + word.Length + 1 > MAX_LENGTH) {
                if (part.Length < MIN_LENGTH) {
                    if (part.Length > 0) part += ' ';
                    part += word;
                    word = "";
                }
                else {
                    result.Add(part);
                    part = word;
                    word = "";
                }
            }
            else {
                if (part.Length > 0) part += ' ';
                part += word;
                word = "";
            }
        }
    }
}
