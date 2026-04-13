using Microsoft.VisualStudio.TestTools.UnitTesting;
using ollama_langflow;
using OllamaSharp;

namespace ollama_langflow.test
{
    [TestClass]
    public class OllamaTests
    {
        private readonly List<string> questions = new()
    {
        "What unusual feature defines the geography of Velbrek?",
        "What item did Sphodebarb carry instead of a wizard's staff, and what was its unique trait?",
        "Who or what are the members of the \"Order of the Thirteen-Shelled Hypothesis\"?",
        "How did The Meow gain endless life?",
        "What substance represented a version of death without finality?",
        "How was the conflict between Sphodebarb and The Meow resolved?",
        "Who was Yalp, and why was he important?",
        "What did Sphodebarb and Nink (the cat) choose after the loop was broken?",
        "Welche ungewöhnliche Eigenschaft definiert die Geographie von Velbrek?",
        "Welchen Gegenstand trug Sphodebarb anstelle eines Zauberstabs, und was war seine besondere Eigenschaft?",
        "Wer oder was sind die Mitglieder des \"Ordens der Dreizehn-Schaligen Hypothese\"?",
        "Wie erlangte The Meow endloses Leben?",
        "Welche Substanz stellte eine Form des Todes ohne Endgültigkeit dar?",
        "Wie wurde der Konflikt zwischen Sphodebarb und The Meow gelöst?",
        "Wer war Yalp, und warum war er wichtig?",
        "Wofür entschieden sich Sphodebarb und Nink (die Katze), nachdem die Schleife durchbrochen war?",
        };

        [TestMethod]
        public async Task Run_All_NoHistory()
        {
            Consts.HISTORY = false;

            var runner = new Conversation();

            for (var i = 0; i < 3; i++)
            {
                foreach (var q in questions)
                {
                    await runner.RunQuestion(q);
                    Console.WriteLine($"Question: {q} run {i + 1} completed");
                    await Task.Delay(300);
                }
            }
        }

        [TestMethod]
        public async Task Run_All_WithHistory()
        {
            Consts.HISTORY = true;

            var runner = new Conversation();
            for (var i = 0; i < 3; i++)
            {
                foreach (var q in questions)
                {
                    await runner.RunQuestion(q);
                    Console.WriteLine($"Question: {q} run {i + 1} completed");
                    await Task.Delay(300);
                }
            }
        }
    }
}
