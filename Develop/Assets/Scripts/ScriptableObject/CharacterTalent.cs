using UnityEngine;

namespace CatsAndDogs {
    [CreateAssetMenu(fileName = "CharacterTalent", menuName = "CatsAndDogs/CharacterTalent", order = 1)]
    public class CharacterTalent : ScriptableObject {

        public enum Property {
            // Gun
            PistolMagazineSize,
            PistolRateOfFire,
            ShotgunAccuracyRadiusCoef,
            RifleAccuracyRadiusCoef,
            MachinegunAccuracyRadiusCoef,

            // Skill
            SprintDuration,
            TumbleCooldown,
            BurrowSpeed,

            // Item
            RestoreDuration,

        }

        [System.Serializable]
        public struct PropertyMultiplier {
            public Property property;
            public float multiplier;
        }

        public string talentName;
        public int icon;
        public string description;
        public PropertyMultiplier[] propertyMultipliers;
    }
}
