using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CivilDiscorse.TextAnalyzer
{
    public class Analyzer
    {
        protected List<string> _phraseList;
        protected string _text;
        
        public Dictionary<string, int> Hits { get; protected set; }
        public Analyzer(List<string> phraseList, string text)
        {
            _phraseList = phraseList;
            _text = text;
        }

        public Dictionary<string, int> FindPhraseCountUsingLinq()
        {
            var results = new Dictionary<string, int>();

            /*
             *var filewords = Directory.GetFiles(@"H:\\backstage\my work\categories file text\categories", "*.txt")    
        .Select(f => File.ReadAllText(f))                                                               // read all text in each file
        .SelectMany(s => words.Intersect(Regex.Split(s, @"\W|_")))                                      // intersect the set of words in each file with the master list of words, then flatten the list
        .GroupBy(s => s).ToDictionary(w => w.Key, w => w.Count());   
             *
             */

            //var r1 = _text.SelectMany(s => )

            return results;
        }

        public void FindPhrasesUsingRegex()
        {
            var results = new Dictionary<string, int>();

            string pattern = @"^.*\b(" + string.Join("|", _phraseList) + @")\b.*$";

            var matches = Regex.Matches(_text, pattern, RegexOptions.Multiline);
            foreach (Match match in matches)
            {
                string theBadWord = match.Groups[1].Value;
                if (results.ContainsKey(theBadWord))
                {
                    results[theBadWord]++;
                }
                else
                {
                    results.Add(theBadWord, 1);
                }
            }

            Hits = results;
        }
    }
}
