using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeckBound.Combat
{
    public enum CombatState { NotStarted, PlayerTurn, EnemyTurn, Victory, Defeat }

    // Bucla de lupta: tura eroului -> tura inamicului -> repeta, pana moare cineva.
    public class CombatManager : MonoBehaviour
    {
        [Header("Participanti")]
        [SerializeField] private Combatant hero;
        [SerializeField] private Enemy enemy;

        [Header("Pachet")]
        [Tooltip("Daca lista e goala, se foloseste pachetul de start al Bruiser-ului.")]
        [SerializeField] private List<CardData> startingDeck = new List<CardData>();

        [Header("Reguli")]
        [SerializeField, Min(0)] private int energyPerTurn = 3;
        [SerializeField, Min(0)] private int cardsPerTurn = 3;
        [SerializeField, Min(0f)] private float enemyTurnDelay = 0.75f;
        [SerializeField] private bool startOnPlay = true;

        private const int MaxLogLines = 8;
        private readonly List<string> log = new List<string>();

        public Combatant Hero => hero;
        public Enemy Enemy => enemy;
        public DeckManager Deck { get; private set; }
        public int Energy { get; private set; }
        public int MaxEnergy => energyPerTurn;
        public int TurnNumber { get; private set; }
        public CombatState State { get; private set; } = CombatState.NotStarted;
        public IReadOnlyList<string> Log => log;

        // UI-ul asculta aici ca sa se redeseneze.
        public event Action OnCombatChanged;
        public event Action<CombatState> OnCombatEnded;

        private void Start()
        {
            if (startOnPlay) StartCombat();
        }

        public void StartCombat()
        {
            if (hero == null || enemy == null)
            {
                Debug.LogError("CombatManager: asigneaza Hero si Enemy in Inspector.", this);
                return;
            }

            StopAllCoroutines();
            log.Clear();
            TurnNumber = 0;

            hero.ResetForCombat();
            enemy.ResetForCombat();

            var deckData = startingDeck.Count > 0 ? startingDeck : StarterDecks.CreateBruiserDeck();
            Deck = new DeckManager(deckData);
            Deck.OnReshuffled += () => AddLog("Discard pile shuffled into the draw pile.");

            AddLog($"Combat starts: {hero.DisplayName} vs {enemy.DisplayName}.");
            StartPlayerTurn();
        }

        private void StartPlayerTurn()
        {
            TurnNumber++;
            State = CombatState.PlayerTurn;
            hero.ClearArmor();
            DecayPoison(enemy); // poison-ul aplicat de erou scade la inceputul turei lui
            Energy = energyPerTurn;
            Deck.Draw(cardsPerTurn);
            AddLog($"— Turn {TurnNumber} —");
            NotifyChanged();
        }

        public bool CanPlay(Card card)
        {
            return State == CombatState.PlayerTurn
                && card != null
                && Energy >= card.Cost
                && ContainsInHand(card);
        }

        public bool PlayCard(Card card)
        {
            if (!CanPlay(card)) return false;

            Energy -= card.Cost;

            if (card.Damage > 0)
            {
                int hpLoss = enemy.TakeDamage(card.Damage);
                AddLog($"{card.Name}: {enemy.DisplayName} loses {hpLoss} HP.");

                if (card.Lifesteal > 0)
                {
                    int healed = hero.Heal(card.Lifesteal);
                    AddLog($"{hero.DisplayName} heals {healed} HP.");
                }
            }

            if (card.Poison > 0)
            {
                enemy.AddPoison(card.Poison);
                AddLog($"{card.Name}: {enemy.DisplayName} gains {card.Poison} Poison ({enemy.Poison} total).");
            }

            if (card.Armor > 0)
            {
                hero.GainArmor(card.Armor);
                AddLog($"{card.Name}: {hero.DisplayName} gains {card.Armor} Armor.");
            }

            Deck.Discard(card);

            if (!CheckCombatEnd()) NotifyChanged();
            return true;
        }

        public void EndPlayerTurn()
        {
            if (State != CombatState.PlayerTurn) return;

            Deck.DiscardHand();
            StartCoroutine(EnemyTurnRoutine());
        }

        private IEnumerator EnemyTurnRoutine()
        {
            State = CombatState.EnemyTurn;

            // Final de tura erou: inamicul ia dmg din poison.
            TickPoison(enemy);
            NotifyChanged();
            if (CheckCombatEnd()) yield break;

            if (enemyTurnDelay > 0f) yield return new WaitForSeconds(enemyTurnDelay);

            DecayPoison(hero); // inceput de tura inamic

            if (!enemy.KeepArmorBetweenTurns) enemy.ClearArmor();
            var move = enemy.NextMove;
            int armorBefore = hero.Armor;
            int hpLoss = enemy.TakeTurn(hero);

            if (move != null && move.damage > 0)
            {
                int blocked = Mathf.Min(armorBefore, move.damage);
                AddLog($"{enemy.DisplayName} uses {move.moveName}: {blocked} blocked, {hpLoss} damage taken.");
            }
            if (move != null && move.armor > 0)
                AddLog($"{enemy.DisplayName} gains {move.armor} Armor.");
            if (move != null && move.poison > 0)
                AddLog($"{hero.DisplayName} gains {move.poison} Poison.");

            if (CheckCombatEnd()) yield break;

            // Final de tura inamic: eroul ia dmg din poison.
            TickPoison(hero);
            if (CheckCombatEnd()) yield break;

            StartPlayerTurn();
        }

        // Poison: la finalul turei, pierde HP cat are stackuri (ignora armura).
        private void TickPoison(Combatant target)
        {
            if (target.Poison <= 0 || target.IsDead) return;
            int hpLoss = target.LoseHp(target.Poison);
            AddLog($"{target.DisplayName} takes {hpLoss} Poison damage.");
        }

        // Poison: la inceputul turei pierde 1 stack.
        private void DecayPoison(Combatant target)
        {
            target.ReducePoison(1);
        }

        // Returneaza true daca lupta s-a terminat.
        private bool CheckCombatEnd()
        {
            if (enemy.IsDead) return EndCombat(CombatState.Victory, "Victory!");
            if (hero.IsDead) return EndCombat(CombatState.Defeat, "Defeat...");
            return false;
        }

        private bool EndCombat(CombatState result, string message)
        {
            State = result;
            AddLog(message);
            NotifyChanged();
            OnCombatEnded?.Invoke(result);
            return true;
        }

        private bool ContainsInHand(Card card)
        {
            foreach (var c in Deck.Hand)
            {
                if (c == card) return true;
            }
            return false;
        }

        private void AddLog(string line)
        {
            log.Add(line);
            if (log.Count > MaxLogLines) log.RemoveAt(0);
            Debug.Log($"[Combat] {line}");
        }

        private void NotifyChanged()
        {
            OnCombatChanged?.Invoke();
        }
    }
}
