using System.Collections.Generic;
using UnityEngine;

namespace DeckBound.Combat
{
    // Inamic cu un pattern de mutari care se repeta in bucla (1, 2, 3, 1, 2, 3...).
    // Mutarile se editeaza din Inspector, in lista Moves.
    public class Enemy : Combatant
    {
        [Header("Pattern")]
        [Tooltip("Mutarile, in ordine. Dupa ultima se reia de la prima.")]
        [SerializeField] private List<EnemyMove> moves = new List<EnemyMove>
        {
            new EnemyMove("Slash", 4),
            new EnemyMove("Rend", 6),
            new EnemyMove("Reap", 8),
        };

        [Tooltip("Bifat: armura ramane de la o tura la alta. Debifat: dispare la inceputul turei inamicului.")]
        [SerializeField] private bool keepArmorBetweenTurns = true;

        private int moveIndex;

        public bool KeepArmorBetweenTurns => keepArmorBetweenTurns;

        // Mutarea pe care o va face in tura lui (ce arata intentia).
        public EnemyMove NextMove => moves.Count > 0 ? moves[moveIndex % moves.Count] : null;
        public string IntentText => NextMove != null ? NextMove.IntentText : "Waits";

        // Valori implicite cand adaugi componenta in Inspector.
        private void Reset()
        {
            displayName = "Cultist";
            maxHp = 20;
            moves = new List<EnemyMove>
            {
                new EnemyMove("Slash", 4),
                new EnemyMove("Rend", 6),
                new EnemyMove("Reap", 8),
            };
        }

        public override void ResetForCombat()
        {
            moveIndex = 0;
            base.ResetForCombat();
        }

        // Executa mutarea curenta si trece la urmatoarea. Returneaza dmg-ul care a trecut de armura.
        public int TakeTurn(Combatant target)
        {
            var move = NextMove;
            if (move == null) return 0;

            if (move.armor > 0) GainArmor(move.armor);
            int hpLoss = move.damage > 0 ? target.TakeDamage(move.damage) : 0;
            if (move.poison > 0) target.AddPoison(move.poison);

            moveIndex = (moveIndex + 1) % moves.Count;
            return hpLoss;
        }
    }
}
