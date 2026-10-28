using System;
using UnityEngine;

namespace DeckBound.Combat
{
    // Orice entitate cu HP si armura (erou sau inamic).
    public class Combatant : MonoBehaviour
    {
        [SerializeField] protected string displayName = "Bruiser";
        [SerializeField, Min(1)] protected int maxHp = 50;

        public string DisplayName => displayName;
        public int MaxHp => maxHp;
        public int CurrentHp { get; private set; }
        public int Armor { get; private set; }
        public int Poison { get; private set; }
        public int Focus { get; private set; }       // dmg bonus pentru urmatorul atac
        public int Vulnerable { get; private set; }  // ture ramase cu dmg primit marit

        public const float VulnerableMultiplier = 1.5f;
        public bool IsDead => CurrentHp <= 0;

        public event Action OnChanged;
        public event Action<Combatant> OnDied;

        protected virtual void Awake()
        {
            ResetForCombat();
        }

        public virtual void ResetForCombat()
        {
            CurrentHp = maxHp;
            Armor = 0;
            Poison = 0;
            Focus = 0;
            Vulnerable = 0;
            OnChanged?.Invoke();
        }

        // Armura absoarbe dmg-ul prima. Returneaza cat HP s-a pierdut efectiv.
        public int TakeDamage(int amount)
        {
            if (IsDead || amount <= 0) return 0;

            if (Vulnerable > 0) amount = Mathf.FloorToInt(amount * VulnerableMultiplier);

            int blocked = Mathf.Min(Armor, amount);
            Armor -= blocked;
            int hpLoss = amount - blocked;
            CurrentHp = Mathf.Max(0, CurrentHp - hpLoss);

            OnChanged?.Invoke();
            if (IsDead) OnDied?.Invoke(this);
            return hpLoss;
        }

        public void GainArmor(int amount)
        {
            if (IsDead || amount <= 0) return;
            Armor += amount;
            OnChanged?.Invoke();
        }

        // Returneaza cat HP s-a recuperat efectiv (nu trece de maxHp).
        public int Heal(int amount)
        {
            if (IsDead || amount <= 0) return 0;
            int before = CurrentHp;
            CurrentHp = Mathf.Min(maxHp, CurrentHp + amount);
            OnChanged?.Invoke();
            return CurrentHp - before;
        }

        // Pierdere de HP care ignora armura (ex. poison). Returneaza HP-ul pierdut efectiv.
        public int LoseHp(int amount)
        {
            if (IsDead || amount <= 0) return 0;

            int hpLoss = Mathf.Min(CurrentHp, amount);
            CurrentHp -= hpLoss;

            OnChanged?.Invoke();
            if (IsDead) OnDied?.Invoke(this);
            return hpLoss;
        }

        // Stackurile noi se aduna peste cele existente.
        public void AddPoison(int stacks)
        {
            if (IsDead || stacks <= 0) return;
            Poison += stacks;
            OnChanged?.Invoke();
        }

        public void ReducePoison(int stacks)
        {
            if (Poison == 0 || stacks <= 0) return;
            Poison = Mathf.Max(0, Poison - stacks);
            OnChanged?.Invoke();
        }

        public void AddFocus(int amount)
        {
            if (IsDead || amount <= 0) return;
            Focus += amount;
            OnChanged?.Invoke();
        }

        // Returneaza tot Focus-ul acumulat si il reseteaza (il foloseste un atac).
        public int ConsumeFocus()
        {
            int focus = Focus;
            if (focus == 0) return 0;
            Focus = 0;
            OnChanged?.Invoke();
            return focus;
        }

        // Duratele se aduna (2 ture + 2 ture = 4 ture).
        public void AddVulnerable(int turns)
        {
            if (IsDead || turns <= 0) return;
            Vulnerable += turns;
            OnChanged?.Invoke();
        }

        public void ReduceVulnerable(int turns)
        {
            if (Vulnerable == 0 || turns <= 0) return;
            Vulnerable = Mathf.Max(0, Vulnerable - turns);
            OnChanged?.Invoke();
        }

        public void ClearArmor()
        {
            if (Armor == 0) return;
            Armor = 0;
            OnChanged?.Invoke();
        }
    }
}
