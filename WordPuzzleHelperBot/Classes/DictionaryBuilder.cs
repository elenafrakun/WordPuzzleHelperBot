using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WordPuzzleHelperBot.Classes
{
    public class DictionaryBuilder
    {
        public static string MainDictionaryPath { get; private set; }
        public static string LessCommonWordsDictionaryPath { get; private set; }
        public Dictionary<string, int> MainDictionary { get; private set; }

        public DictionaryBuilder()
        {
            MainDictionaryPath = "Dictionaries\\MainDictionary.txt";
            LessCommonWordsDictionaryPath = "Dictionaries\\LessCommonWordsDictionary.txt";
            MainDictionary = new Dictionary<string, int>();
            OpenDictionaryFile();
        }

        /// <summary>
        /// Fills the MainDictionary.
        /// </summary>
        /// <returns></returns>
        public bool OpenDictionaryFile()
        {
            if (MainDictionary.Any())
                MainDictionary.Clear();

            try
            {
                using (StreamReader reader = new StreamReader(MainDictionaryPath))
                {
                    string[] lessCommonWordsDictionary = File.ReadAllLines(LessCommonWordsDictionaryPath);
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (lessCommonWordsDictionary.Contains(line))
                            MainDictionary.Add(line, 1);
                        else
                            MainDictionary.Add(line, 0);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception caught: {e}");
                return false;
            }

            return true;
        }


        //to do
        /*
        public async Task<bool> AddToMainDictionary(string messageText)
        {

        }

        public async Task AddToLessCommonWordsDictionary(string messageText)
        {

        }
        */
    }
}