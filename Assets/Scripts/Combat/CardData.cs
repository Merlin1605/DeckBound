using System.Text;
using UnityEngine;

namespace DeckBound.Combat
{
    public enum CardType { Attack, Skill }

    // Definitia unei carti (asset). Creezi una din meniu: Create > DeckBound > Card.
    [CreateAssetMenu(fileName = "NewCard", menuName = "DeckBound/Card")]
    public class CardData : ScriptableObject
    {
        public string cardName = "Card";
        public CardType type = CardType.Attack;
        [Min(0)] public int energyCost = 1;
        public Sprite art;

        [Header("Efecte")]
        [Min(0)] public int damage;     // dmg aplicat inamicului
        [Min(0)] public int armor;      // armura primita de erou
        [Min(0)] public int lifesteal;  // HP recuperat cand cartea loveste
        [Min(0)] public int poison;     // stackuri de poison aplicate inamicului

        [Tooltip("Daca e gol, descrierea se genereaza din efecte.")]
        [TextArea] public string description;

        public string GetDescription()
        {
            if (!string.IsNullOrWhiteSpace(description))
                return description;

            var sb = new StringBuilder();
            if (damage > 0) sb.Append($"Deal {damage} damage. ");
            if (lifesteal > 0) sb.Append($"Lifesteal {lifesteal}. ");
            if (poison > 0) sb.Append($"Apply {poison} Poison. ");
            if (armor > 0) sb.Append($"Gain {armor} Armor. ");
            return sb.ToString().Trim();
        }

        // Creeaza o carte direct din cod, fara asset in proiect.
        public static CardData CreateRuntime(string name, CardType type, int cost,
            int damage = 0, int armor = 0, int lifesteal = 0, int poison = 0, Sprite art = null)
        {
            var card = CreateInstance<CardData>();
            card.art = art;
            card.name = name;
            card.cardName = name;
            card.type = type;
            card.energyCost = cost;
            card.damage = damage;
            card.armor = armor;
            card.lifesteal = lifesteal;
            card.poison = poison;
            return card;
        }
    }
}
