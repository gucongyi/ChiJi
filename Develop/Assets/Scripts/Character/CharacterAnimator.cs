using UnityEngine;

namespace CatsAndDogs {
    [RequireComponent(typeof(Animator), typeof(Rigidbody), typeof(Character))]
    public class CharacterAnimator : MonoBehaviour {

        [HideInInspector] public float reloadClipTime = 0f;
        [SerializeField] private Animator animator;
        [SerializeField] private new Rigidbody rigidbody;                             // 依赖 rigidbody.velocity 计算 animator.speed .
        [SerializeField, NotEditableInInspector] private Character character;         // 依赖 character.velocityScale .
        [SerializeField, NotEditableInInspector] private CharacterMove characterMove;
        private AnimatorOverrideController animatorOverrideController;

        public void Reset() {
            animator = GetComponent<Animator>();
            rigidbody = GetComponent<Rigidbody>();
            character = GetComponent<Character>();
            characterMove = GetComponent<CharacterMove>();

            enabled = false;
        }

        private void Start() {
            animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            animator.runtimeAnimatorController = animatorOverrideController;
        }

        private void Update() {
            animator.SetFloat("Velx", character.velocityScale.x);
            animator.SetFloat("Vely", character.velocityScale.y);

            SetAnimatorSpeed(rigidbody.velocity);
        }

        private void SetAnimatorSpeed(Vector3 velocity) {
            bool isInMovementAndUpperIdle = animator.GetCurrentAnimatorStateInfo(0).IsName("Movement") && animator.GetCurrentAnimatorStateInfo(1).IsName("Armed-Idle");
            if (!isInMovementAndUpperIdle) {
                animator.speed = 1f;
                return;
            }
            Vector3 animatorVelocity = Vector3.zero;
            AnimatorClipInfo[] animatorClipInfoArray = animator.GetCurrentAnimatorClipInfo(0);
            for (int i = 0, length = animatorClipInfoArray.Length; i < length; i++) {
                AnimatorClipInfo animationClipInfo = animatorClipInfoArray[i];
                animatorVelocity += animationClipInfo.weight * animationClipInfo.clip.averageSpeed;
            }
            float moveSpeed = velocity.magnitude;
            float animatorMoveSpeed = animatorVelocity.magnitude;
            if (animatorMoveSpeed < 0.01f) {
                animator.speed = 1f;
            } else {
                animator.speed = moveSpeed / animatorMoveSpeed;
            }
        }

        public void SetAnimationClips(Gun.Type type) {
            character.photonView.RPC("RPCSetAnimationClips", PhotonTargets.All, type);
        }

        [PunRPC]
        private void RPCSetAnimationClips(Gun.Type type) {
            // TODO: 一是移除这些 ResetTrigger，二是移除 reloadClipTime.
            animator.ResetTrigger("Pistol");
            animator.ResetTrigger("Shotgun");
            animator.ResetTrigger("Rifle");
            animator.ResetTrigger("Machinegun");
            switch (type) {
                case Gun.Type.Pistol:
                    animator.SetTrigger("Pistol");
                    reloadClipTime = 2f;
                    break;
                case Gun.Type.Shotgun:
                    animator.SetTrigger("Shotgun");
                    reloadClipTime = 7f / 3f;
                    break;
                case Gun.Type.Rifle:
                    animator.SetTrigger("Rifle");
                    reloadClipTime = 2f;
                    break;
                case Gun.Type.Machinegun:
                    animator.SetTrigger("Machinegun");
                    reloadClipTime = 2f;
                    break;
            }
        }
    }
}
