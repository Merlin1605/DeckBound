using System;
using UnityEngine;

namespace DeckBound.Combat
{
    // O mutare din pattern-ul unui inamic. Damage si Armor pot fi folosite impreuna.
    [Serializable]
    public class EnemyMove
    {
        public string moveName = "Attack";
        [Min(0)] public int damage;
        [Min(0)] public int armor;
        [Min(0)] public int poison; // stackuri aplicate eroului

        public EnemyMove() { }

        public EnemyMove(string moveName, int damage, int armor = 0, int poison = 0)
        {
            this.moveName = moveName;
            this.damage = damage;
            this.armor = armor;
            this.poison = poison;
        }

        public bool IsAttack => damage > 0;

        public string IntentText
        {
            get
            {
                var parts = new System.Collections.Generic.List<string>();
                if (damage > 0) parts.Add($"Attacks for {damage}");
                if (armor > 0) parts.Add($"gains {armor} Armor");
                if (poison > 0) parts.Add($"applies {poison} Poison");
                if (parts.Count == 0) return "Waits";
                string text = string.Join(", ", parts);
                return char.ToUpper(text[0]) + text.Substring(1);
            }
        }
    }
}
