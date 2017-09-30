namespace CatsAndDogs {
    [System.Serializable]
    public class BodyArmor {

        public Type type;
        public float durability;

        public enum Type {
            LV1,
            LV2,
            LV3,
            LV4
        }

        private static float GetWeight(Type type) {
            return 8f;
        }

        public float GetWeight() {
            return GetWeight(type);
        }
    }
}

