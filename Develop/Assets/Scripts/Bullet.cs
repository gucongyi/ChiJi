using UnityEngine;

namespace CatsAndDogs {
    public class Bullet : MonoBehaviour {

        void OnCollisionEnter(Collision collision) {
            Destroy(gameObject);
        }
    }
}
