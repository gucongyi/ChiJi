using UnityEngine;

namespace CatsAndDogs {
    public class MapItem : MonoBehaviour {

        public enum Type {
            // 大类
            RandomCloseRangeWeapon,
            RandomMiddleRangeWeapon,
            RandomLongRangeWeapon,

            // 小类
            Shotgun1,
            Shotgun2,
            Shotgun3,
            Dart, // 飞镖
            Rifle,
            Bow,
            Machinegun,
            Bazooka, // 火箭筒
            BodyArmor,
            Gem,
        }
    }
}