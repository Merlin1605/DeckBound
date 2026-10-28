using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DeckBound.Combat
{
    // Cuvintele cheie din descrierile cartilor si explicatia lor (tooltip la hover).
    // Ca sa adaugi unul nou (ex. Poison), adauga-l in lista All.
    public static class Keywords
    {
        public class Keyword
        {
            public string Id;
            public string Title;
            public string Description;
        }

        public const string HighlightColor = "#E8B44A";

        public static readonly List<Keyword> All = new List<Keyword>
        {
            new Keyword
            {
                Id = "armor",
                Title = "Armor",
                Description = "Blocks incoming damage until it runs out. Armor is removed at the start of your turn.",
            },
            new Keyword
            {
                Id = "lifesteal",
                Title = "Lifesteal",
                Description = "When this card hits, heal HP equal to its Lifesteal value. Cannot heal above max HP.",
            },
            new Keyword
            {
                Id = "poison",
                Title = "Poison",
                Description = "At the end of your turn, a poisoned enemy loses HP equal to its Poison, ignoring Armor. At the start of your next turn, its Poison goes down by 1. New Poison stacks on top.",
            },
            new Keyword
            {
                Id = "focus",
                Title = "Focus",
                Description = "Your next attack deals bonus damage equal to your Focus, and that whole hit deals 50% more damage. Focus stacks up between turns and is used up by that attack.",
            },
            new Keyword
            {
                Id = "vulnerable",
                Title = "Vulnerable",
                Description = "Takes 50% more damage from attacks. Lasts for the given number of turns.",
            },
        };

        private static Regex pattern;

        public static Keyword Find(string id)
        {
            foreach (var keyword in All)
            {
                if (keyword.Id == id) return keyword;
            }
            return null;
        }

        // Transforma "Gain 3 Armor." in "Gain 3 <link="armor"><color=...>Armor</color></link>."
        // ca TextMeshPro sa stie unde e cuvantul cheie. Merge si pe descrierile scrise de mana.
        public static string Highlight(string text)
        {
            if (string.IsNullOrEmpty(text) || text.Contains("<link")) return text;

            if (pattern == null)
            {
                var titles = new List<string>();
                foreach (var keyword in All) titles.Add(Regex.Escape(keyword.Title));
                pattern = new Regex($@"\b({string.Join("|", titles)})\b", RegexOptions.IgnoreCase);
            }

            return pattern.Replace(text, match =>
            {
                var keyword = FindByTitle(match.Value);
                return $"<link=\"{keyword.Id}\"><color={HighlightColor}>{match.Value}</color></link>";
            });
        }

        private static Keyword FindByTitle(string title)
        {
            foreach (var keyword in All)
            {
                if (string.Equals(keyword.Title, title, System.StringComparison.OrdinalIgnoreCase)) return keyword;
            }
            return null;
        }
    }
}
