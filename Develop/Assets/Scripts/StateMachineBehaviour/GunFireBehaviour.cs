using UnityEngine;

namespace CatsAndDogs {
    public class GunFireBehaviour : StateMachineBehaviour {

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (BattleSceneManager.Instance == null) {
                return;
            }
            if (BattleSceneManager.Instance.myCharacter == null) {
                return;
            }
            float fireSpeed = BattleSceneManager.Instance.myCharacter.backpack.gun.rateOfFire / stateInfo.length;
            if (stateInfo.IsName("Pistol-Dual-Fire")) {
                fireSpeed /= 2f;    // 这个动画有两次开火
            }
            animator.SetFloat("FireSpeed", fireSpeed);
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            animator.SetFloat("FireSpeed", 1f);
        }
    }
}
