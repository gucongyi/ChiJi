using UnityEngine;

namespace CatsAndDogs {
    [System.Serializable]
    public class BackpackItem {

        public Type type;

        public enum Type {
            Restore,
            Heal,
            Gem1,
            Gem2,
            Gem3,
            Gem4,
            Gem5,
        }

        private static float GetWeight(Type type) {
            float weight = 0f;
            switch (type) {
                case Type.Restore:
                    weight = 4f;
                    break;
                case Type.Heal:
                    weight = 4f;
                    break;
                case Type.Gem1:
                    weight = 2f;
                    break;
                case Type.Gem2:
                    weight = 3f;
                    break;
                case Type.Gem3:
                    weight = 4f;
                    break;
                case Type.Gem4:
                    weight = 1f;
                    break;
                case Type.Gem5:
                    weight = 2f;
                    break;
            }
            return weight;
        }

        public float GetWeight() {
            return GetWeight(type);
        }
    }
}

