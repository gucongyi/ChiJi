using UnityEngine;
using UnityEngine.EventSystems;

namespace CatsAndDogs {
    public class DragRotate : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerExitHandler {

        private Character character;
        private CharacterBehaviour characterBehaviour;

        private bool slide = false;

        private void Start() {
            character = BattleSceneManager.Instance.myCharacter;
            characterBehaviour = character.GetComponent<CharacterBehaviour>();
        }

        public void OnBeginDrag(PointerEventData eventData) {
            slide = true;
        }

        public void OnDrag(PointerEventData eventData) {
            if (slide) {
                SlideBehaviour(eventData);
            }
        }

        public void OnEndDrag(PointerEventData eventData) {
            slide = false;
        }

        public void OnPointerExit(PointerEventData eventData) {
            slide = false;
        }

        private void SlideBehaviour(PointerEventData pointerEventData) {
            if (!BattleUIManager.Instance.rightPanelEnable) {
                return;
            }

            if (Mathf.Abs(pointerEventData.position.x - pointerEventData.pressPosition.x) / (float)Screen.width > 50f / 1334f) {
                BattleUIManager.Instance.rotateByTouchMove = true;
            }

            // 取消锁定，滑动旋转方向
            if (BattleUIManager.Instance.rotateByTouchMove) {
                character.transform.Rotate(Vector3.up, pointerEventData.delta.x * BattleUIManager.Instance.rotateAnglePerX * 1334f / (float)Screen.width);
                if (characterBehaviour.lockTargetEnemy) {
                    Debug.Log("滑动取消锁定");
                    characterBehaviour.lockTargetEnemy = false;
                }
            }
        }
    }
}
