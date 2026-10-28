using System.Collections.Generic;

namespace DeckBound.Combat
{
    // Pachete de start generate din cod, folosite cand CombatManager nu are carti asignate.
    public static class StarterDecks
    {
        public static List<CardData> CreateBruiserDeck()
        {
            var strike = CardData.CreateRuntime("Strike", CardType.Attack, 1, damage: 5);
            var defend = CardData.CreateRuntime("Defend", CardType.Skill, 1, armor: 3);
            var bloodStrike = CardData.CreateRuntime("Blood Strike", CardType.Attack, 1, damage: 5, lifesteal: 3);

            var deck = new List<CardData>();
            for (int i = 0; i < 3; i++) deck.Add(strike);
            for (int i = 0; i < 3; i++) deck.Add(defend);
            deck.Add(bloodStrike);
            return deck;
        }
    }
}
