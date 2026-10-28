using DeckBound.Combat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DeckBound.UI
{
    // Bara de HP + armura pentru un Combatant (erou sau inamic).
    public class HealthBarView : MonoBehaviour
    {
        [SerializeField] private Combatant target;

        [Header("HP")]
        [Tooltip("Image cu Image Type = Filled, Fill Method = Horizontal (bar_fill.png).")]
        [SerializeField] private Image fillImage;
        [SerializeField] private TMP_Text hpText;
        [SerializeField] private Color hpColor = new Color(0.75f, 0.12f, 0.12f);
        [Tooltip("Culoarea barei cand combatantul are armura.")]
        [SerializeField] private Color armoredColor = new Color(0.35f, 0.55f, 0.8f);
        [SerializeField, Min(0f)] private float fillSpeed = 3f; // 0 = instant

        [Header("Armura (optional)")]
        [Tooltip("Obiectul cu iconita de armura; se ascunde cand armura e 0.")]
        [SerializeField] private GameObject armorRoot;
        [SerializeField] private TMP_Text armorText;

        [Header("Poison (optional)")]
        [Tooltip("Obiectul cu iconita de poison; se ascunde cand poison e 0.")]
        [SerializeField] private GameObject poisonRoot;
        [SerializeField] private TMP_Text poisonText;

        [Header("Focus (optional, pentru erou)")]
        [SerializeField] private GameObject focusRoot;
        [SerializeField] private TMP_Text focusText;

        [Header("Vulnerable (optional)")]
        [SerializeField] private GameObject vulnerableRoot;
        [SerializeField] private TMP_Text vulnerableText;

        private float targetFill = 1f;

        public void SetTarget(Combatant combatant)
        {
            if (target != null) target.OnChanged -= Refresh;
            target = combatant;
            if (isActiveAndEnabled && target != null) target.OnChanged += Refresh;
            Refresh();
        }

        private void OnEnable()
        {
            if (target != null) target.OnChanged += Refresh;
            Refresh();
            if (fillImage != null) fillImage.fillAmount = targetFill;
        }

        private void OnDisable()
        {
            if (target != null) target.OnChanged -= Refresh;
        }

        private void Update()
        {
            if (fillImage == null) return;
            fillImage.fillAmount = fillSpeed > 0f
                ? Mathf.MoveTowards(fillImage.fillAmount, targetFill, fillSpeed * Time.deltaTime)
                : targetFill;
        }

        private void Refresh()
        {
            if (target == null) return;

            targetFill = target.MaxHp > 0 ? (float)target.CurrentHp / target.MaxHp : 0f;
            if (fillImage != null) fillImage.color = target.Armor > 0 ? armoredColor : hpColor;
            if (hpText != null) hpText.text = $"{target.CurrentHp}/{target.MaxHp}";

            if (armorRoot != null) armorRoot.SetActive(target.Armor > 0);
            if (armorText != null) armorText.text = target.Armor.ToString();

            if (poisonRoot != null) poisonRoot.SetActive(target.Poison > 0);
            if (poisonText != null) poisonText.text = target.Poison.ToString();

            if (focusRoot != null) focusRoot.SetActive(target.Focus > 0);
            if (focusText != null) focusText.text = target.Focus.ToString();

            if (vulnerableRoot != null) vulnerableRoot.SetActive(target.Vulnerable > 0);
            if (vulnerableText != null) vulnerableText.text = target.Vulnerable.ToString();
        }
    }
}
