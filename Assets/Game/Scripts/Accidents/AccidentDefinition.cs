using UnityEngine;

namespace Game.Accidents
{
    [CreateAssetMenu(fileName = "AccidentDefinition", menuName = "Game/Accidents/Accident Definition")]
    public class AccidentDefinition : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Accident type id, for example HullBreach. One definition can be used by multiple locations.")]
        public string Id;

        [Tooltip("Player-facing accident name.")]
        public string DisplayName;

        [TextArea]
        [Tooltip("Short player-facing accident description.")]
        public string Description;

        [Tooltip("UI icon for this accident.")]
        public Sprite Icon;

        [Header("Damage")]
        [Tooltip("Ship HP damage per second while this accident is active.")]
        public float DamagePerSecond = 1f;
    }
}
