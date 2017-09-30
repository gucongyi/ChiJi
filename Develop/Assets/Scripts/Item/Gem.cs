namespace CatsAndDogs {
    [System.Serializable]
    public class Gem {

        public Type type;

        public enum Type {
            DamageByDistance,
            Distance,
            RateOfFire,
            MagazineSize,
            ReloadTime,
            Weight,
            AccuracyRadiusCoef,
            AccuracyPowCoef
        }
    }
}
