using UnityEngine;

namespace DeckBound.Combat
{
    // UI temporar (IMGUI), ca sa testezi bucla fara Canvas sau prefaburi.
    // Il inlocuim mai tarziu cu UI-ul real; logica ramane in CombatManager.
    [RequireComponent(typeof(CombatManager))]
    public class CombatDebugUI : MonoBehaviour
    {
        [SerializeField] private float scale = 1.5f;

        private CombatManager combat;
        // Actiunile din click se executa in Update, ca layout-ul IMGUI sa nu se schimbe in mijlocul unui eveniment.
        private System.Action pendingAction;

        private void Awake()
        {
            combat = GetComponent<CombatManager>();
        }

        private void Update()
        {
            var action = pendingAction;
            pendingAction = null;
            action?.Invoke();
        }

        private void OnGUI()
        {
            if (combat == null || combat.Deck == null) return;

            GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1f));
            float width = Screen.width / scale;

            GUILayout.BeginArea(new Rect(10, 10, width - 20, Screen.height / scale - 20));

            var hero = combat.Hero;
            var enemy = combat.Enemy;

            GUILayout.BeginHorizontal();
            GUILayout.Label($"<b>{hero.DisplayName}</b>  HP {hero.CurrentHp}/{hero.MaxHp}   Armor {hero.Armor}", RichLabel());
            GUILayout.FlexibleSpace();
            GUILayout.Label($"<b>{enemy.DisplayName}</b>  HP {enemy.CurrentHp}/{enemy.MaxHp}   Intent: {enemy.IntentText}", RichLabel());
            GUILayout.EndHorizontal();

            GUILayout.Space(8);
            GUILayout.Label($"Turn {combat.TurnNumber}   Energy {combat.Energy}/{combat.MaxEnergy}   " +
                            $"Draw {combat.Deck.DrawPile.Count}   Discard {combat.Deck.DiscardPile.Count}");

            GUILayout.Space(8);
            GUILayout.BeginHorizontal();
            foreach (var card in combat.Deck.Hand)
            {
                GUI.enabled = combat.CanPlay(card);
                string label = $"{card.Name}\n[{card.Cost}]\n{card.Description}";
                if (GUILayout.Button(label, GUILayout.Width(150), GUILayout.Height(90)))
                    pendingAction = () => combat.PlayCard(card);
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.Space(8);
            switch (combat.State)
            {
                case CombatState.PlayerTurn:
                    if (GUILayout.Button("End Turn", GUILayout.Width(150))) pendingAction = combat.EndPlayerTurn;
                    break;
                case CombatState.EnemyTurn:
                    GUILayout.Label("Enemy turn...");
                    break;
                case CombatState.Victory:
                case CombatState.Defeat:
                    GUILayout.Label(combat.State == CombatState.Victory ? "<b>VICTORY</b>" : "<b>DEFEAT</b>", RichLabel());
                    if (GUILayout.Button("Restart", GUILayout.Width(150))) pendingAction = combat.StartCombat;
                    break;
            }

            GUILayout.Space(8);
            foreach (var line in combat.Log) GUILayout.Label(line);

            GUILayout.EndArea();
        }

        private static GUIStyle RichLabel()
        {
            return new GUIStyle(GUI.skin.label) { richText = true };
        }
    }
}
