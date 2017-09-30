using UnityEngine;

namespace CatsAndDogs {
    [RequireComponent(typeof(Character))]
    public class CharacterMove : MonoBehaviour {

        [SerializeField, NotEditableInInspector] private Character character;

        private void Reset() {
            character = GetComponent<Character>();
            enabled = false;
        }

        private void FixedUpdate() {
            Vector2 velocityScale = character.velocityScale;
            float moveSpeed = character.moveSpeed;
            character.rigidbody.velocity = transform.TransformVector(new Vector3(velocityScale.x * moveSpeed, character.rigidbody.velocity.y, velocityScale.y * moveSpeed));
        }

        public void EnableMove() {
            if (character.photonView.isMine) {
                enabled = true;
            }
        }

        public void StopMove() {
            if (character.photonView.isMine) {
                character.rigidbody.velocity = Vector3.zero;
            }
        }
    }
}
