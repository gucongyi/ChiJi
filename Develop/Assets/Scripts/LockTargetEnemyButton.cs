using UnityEngine;
using UnityEngine.EventSystems;

namespace CatsAndDogs {
    public class LockTargetEnemyButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler {

        public BattleUIManager battleUIManager;

        public void OnPointerDown(PointerEventData eventData) {
        }

        public void OnPointerUp(PointerEventData eventData) {
        }

        public void OnPointerEnter(PointerEventData eventData) {
            battleUIManager.LockTargetEnemy();
            battleUIManager.isInLockButton = true;
        }

        public void OnPointerExit(PointerEventData eventData) {
            battleUIManager.isInLockButton = false;
        }
    }
}