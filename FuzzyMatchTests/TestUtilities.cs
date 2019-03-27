﻿using System.Collections.Generic;
using System.IO;

namespace FuzzyMatch.Tests
{
    public static class TestUtilities
    {
        /// <summary>
        /// Stores a static reference to all the words in the words.txt file.
        /// </summary>
        public static List<string> DictionaryWords = DeserialiseDictionaryWords();

        /// <summary>
        /// Deserialises the words.txt file into a <see cref="List{T}"/> of <see cref="string"/>s.
        /// </summary>
        /// <returns></returns>
        public static List<string> DeserialiseDictionaryWords()
        {
            var words = new List<string>(466548);
            var wordFile = new StreamReader(File.OpenRead("words.txt"));
            using (wordFile)
            {
                string aWord = wordFile.ReadLine();
                while (!string.IsNullOrWhiteSpace(aWord))
                {
                    words.Add(aWord);
                    aWord = wordFile.ReadLine();
                }
            }

            return words;
        }
    }
}