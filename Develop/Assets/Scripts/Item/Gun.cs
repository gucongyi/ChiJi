using UnityEngine;

namespace CatsAndDogs {
    [System.Serializable]
    public class Gun {

        // 设计数据
        public Type type = Type.Rifle;    // 类型
        public float[,] damageByDistance; // 距离对应的伤害
        public float rateOfFire;          // 射速，每秒射出子弹数
        public int magazineSize;          // 弹夹容量
        public float reloadTime;          // 装填秒数
        public float weight;              // 重量
        public float accuracyRadiusCoef;  // “散布圈半径”
        public float accuracyPowCoef;     // “散布（幂）系数”

        // 实时数据
        public int bulletNumber = 30;

        // 枪械类型
        public enum Type {
            Pistol,     // 手枪
            Rifle,      // 步枪
            Machinegun, // 机枪
            Shotgun     // 霰弹枪
        }

        private static float accuracyDistanceCoef = 100f; // “画圈距离”

        public Gun(Type gunType) {
            type = gunType;
            Init();
        }

        public void Init() {
            switch (type) {
                case Type.Shotgun:
                    damageByDistance = new float[,] { { 10f, 30f }, { 15f, 20f }, { 20f, 10f }, { 50f, 5f } };
                    rateOfFire = 1f / 0.3f;
                    magazineSize = 10;
                    reloadTime = 1.2f;
                    weight = 25f;
                    accuracyRadiusCoef = 7.5f;
                    accuracyPowCoef = 1f;
                    break;
                case Type.Rifle:
                    damageByDistance = new float[,] { { 10f, 200f }, { 20f, 150f }, { 30f, 120f }, { 50f, 100f } };
                    rateOfFire = 1f / 0.15f;
                    magazineSize = 30;
                    reloadTime = 1.5f;
                    weight = 20f;
                    accuracyRadiusCoef = 4f;
                    accuracyPowCoef = 2f;
                    break;
                case Type.Pistol:
                    damageByDistance = new float[,] { { 10f, 100f }, { 20f, 80f }, { 30f, 60f }, { 50f, 50f } };
                    rateOfFire = 1f / 0.2f;
                    magazineSize = 15;
                    reloadTime = 1f;
                    weight = 10f;
                    accuracyRadiusCoef = 2.5f;
                    accuracyPowCoef = 3f;
                    break;
                case Type.Machinegun:
                    damageByDistance = new float[,] { { 10f, 100f }, { 20f, 80f }, { 30f, 60f }, { 50f, 51f } };
                    rateOfFire = 1f / 0.1f;
                    magazineSize = 100;
                    reloadTime = 1f;
                    weight = 30f;
                    accuracyRadiusCoef = 10f;
                    accuracyPowCoef = 1f;
                    break;
            }
            bulletNumber = magazineSize;
        }

        // 计算随机弹道方向（local 坐标）
        public Vector3 CalcTargetVector() {
            float angle = Random.Range(0f, 2 * Mathf.PI);
            float baseNum = Random.Range(0f, 1f);
            float dis = Mathf.Pow(baseNum, accuracyPowCoef);
            float x = dis * Mathf.Cos(angle);
            float y = dis * Mathf.Sign(angle);
            return new Vector3(x, y, accuracyDistanceCoef);
        }

        // 计算伤害
        public float CalcDamage(float distance) {
            for (int i = 0, length = damageByDistance.GetLength(0); i < length; i++) {
                if (distance < damageByDistance[i, 0]) {
                    return damageByDistance[i, 1];
                }
            }
            return 0f;
        }
    }
}


