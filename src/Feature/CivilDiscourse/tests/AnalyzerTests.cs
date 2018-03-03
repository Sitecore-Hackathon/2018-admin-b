using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CivilDiscorse.TextAnalyzer.Tests
{
    [TestFixture]
    public class AnalyzerTests
    {
        [Test]
        public void Analyzer_OneLineOfTextWithOneOffendingWord_WordIsDetectedOnce()
        {
            var text = @"Communication is not possible. The shuttle has no power. We're fucked up the butt. Using the gravitational pull of a star to slingshot back in time? We are going to Starbase Montgomery for Engineering consultations prompted by minor read-out anomalies. Probes have recorded unusual levels of geological activity in all five planetary systems. Assemble a team. Look at records of the Drema quadrant. Would these scans detect artificial transmissions as well as natural signals?";
            var phraseList = new List<string>() {"fuck", "shit", "butt"};

            var theBadWord = "butt";

            var textAnalyzer = new Analyzer(phraseList, text);

            textAnalyzer.FindPhrasesUsingRegex();

            Assert.IsNotNull(textAnalyzer.Hits);
            Assert.IsNotNull(textAnalyzer.Hits[theBadWord]);
            Assert.AreEqual(1, textAnalyzer.Hits[theBadWord]);
        }

        [Test]
        public void Analyzer_ManyLineOfTextWithOneOffendingWord_WordIsDetectedOnce()
        {
            var text = 
@"Communication is not possible. 
The shuttle has no power. We're fucked up the butt. 
Using the gravitational pull of a star to 
slingshot back in time? We are going to 
Starbase Montgomery for Engineering 
consultations prompted by minor read-out 
anomalies. Probes have recorded unusual 
levels of geological activity in all five 
planetary systems. Assemble a team. Look at 
records of the Drema quadrant. Would these scans detect 
artificial transmissions as well as natural signals?";
            var phraseList = new List<string>() { "fuck", "shit", "butt" };

            var theBadWord = "butt";

            var textAnalyzer = new Analyzer(phraseList, text);

            textAnalyzer.FindPhrasesUsingRegex();

            Assert.IsNotNull(textAnalyzer.Hits);
            Assert.IsNotNull(textAnalyzer.Hits[theBadWord]);
            Assert.AreEqual(1, textAnalyzer.Hits[theBadWord]);
        }

        [Test]
        public void Analyzer_ManyLineOfTextWithManyOffendingWords_AllBadWordsDetectedTheRightNumberOfTimes()
        {
            var text =
@"Communication is not possible. 
The shuttle has no power. We're fucked up the butt. 
Using the gravitational pull of a star to 
slingshot back in time? We are going to 
Starbase Montgomery for shit Engineering 
consultations prompted by minor butt read-out 
anomalies. Probes have fuck recorded unusual 
levels of geological activity in all five 
planetary systems. Assemble a butt team. Look at 
records of the Drema quadrant. Would these shit scans detect 
artificial transmissions as well as fuck natural signals?";
            var phraseList = new List<string>() { "fuck", "shit", "butt" };

            var textAnalyzer = new Analyzer(phraseList, text);

            textAnalyzer.FindPhrasesUsingRegex();

            Assert.IsNotNull(textAnalyzer.Hits);
            Assert.IsNotNull(textAnalyzer.Hits["butt"]);
            Assert.AreEqual(3, textAnalyzer.Hits["butt"]);

            Assert.IsNotNull(textAnalyzer.Hits["fuck"]);
            Assert.AreEqual(2, textAnalyzer.Hits["fuck"]);

            Assert.IsNotNull(textAnalyzer.Hits["shit"]);
            Assert.AreEqual(2, textAnalyzer.Hits["shit"]);
        }
    }
}
