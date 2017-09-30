using UnityEngine;

namespace CatsAndDogs {
    public class CameraFollow : MonoBehaviour {

        public Transform target;
        public Vector3 positionOffset = new Vector3(0f, 2f, 0f);
        public float rotationOffset = 30f;
        public float distance = 6f;

        private void Update() {
            if (target) {
                transform.position = target.position + positionOffset + (Quaternion.AngleAxis(rotationOffset, target.right) * (-target.forward)) * distance;
                transform.LookAt(target.position + positionOffset);
            }
        }
    }
}
