using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace WordPuzzleHelperBot.Classes
{
    public class WordsBuilder
    {
        public readonly DictionaryBuilder dictionaryBuilder;
        public HashSet<string> FoundWordsAll { get; private set; }
        public HashSet<string> FoundWordsLessCommon { get; private set; }

        public WordsBuilder()
        {
            dictionaryBuilder = new DictionaryBuilder();
            FoundWordsAll = new HashSet<string>();
            FoundWordsLessCommon = new HashSet<string>();
        }

        /// <summary>
        /// Causes validation of usersInput and calls a method to do permutations.
        /// </summary>
        /// <param name="usersInput"></param>
        /// <returns>true if validation successful and permutations are done; otherwise, false.</returns>
        public bool FindWords(string usersInput)
        {
            if (!ValidateUserInput(usersInput))
                return false;

            ClearHashSetsWithFoundWords();
            DoPermutations(usersInput.ToCharArray(), "");

            Console.WriteLine($"Result sent.\n");

            return true;
        }

        /// <summary>
        /// Makes all possible permutations of letters and finds real words.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="letters"></param>
        /// <param name="soFar"></param>
        private void DoPermutations(char[] letters, string soFar)
        {
            if (!string.IsNullOrEmpty(soFar) && dictionaryBuilder.MainDictionary.ContainsKey(soFar) && soFar.Length >= 3)
            {
                FoundWordsAll.Add(soFar);
                if (dictionaryBuilder.MainDictionary[soFar] == 1)
                    FoundWordsLessCommon.Add(soFar);
            }

            if (letters.Length == 0)
                return;

            for (int i = 0; i < letters.Length; i++)
            {
                DoPermutations(letters.Where((x, y) => y != i).ToArray(), soFar + letters[i]);
            }
        }

        /// <summary>
        /// Validates user's input.
        /// </summary>
        /// <param name="usersInput"></param>
        /// <returns>true if usersInput passes all the conditions; otherwise, false</returns>
        public bool ValidateUserInput(string usersInput)
        {
            // If string is null or empty return false.
            if (String.IsNullOrEmpty(usersInput) || String.IsNullOrWhiteSpace(usersInput))
                return false;

            // If the number of characters (length of inputed string) is less then 3 or more then 7 return false.
            int length = usersInput.Length;

            if (length < 3 || length > 7)
                return false;

            // If at least one character is not a letter return false
            if (usersInput.Any(ch => !Char.IsLetter(ch)))
                return false;

            // If at least one letter is not cyrillic return false.
            foreach (char ch in usersInput)
            {
                if (!Regex.IsMatch(ch.ToString(), @"[а-яё]", RegexOptions.IgnoreCase))
                    return false;
            }

            return true;
        }

        private void ClearHashSetsWithFoundWords()
        {
            FoundWordsAll.Clear();
            FoundWordsLessCommon.Clear();
        }

        /*
        public async Task AddToMainDictionary(string messageText)
        {
            await dictionaryBuilder.AddToMainDictionary(messageText);
        }

        public async Task AddToLessCommonWordsDictionary(string messageText)
        {
            await dictionaryBuilder.AddToLessCommonWordsDictionary(messageText);
        }
        */
    }
}
