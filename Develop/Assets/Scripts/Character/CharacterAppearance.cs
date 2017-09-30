using UnityEngine;

namespace CatsAndDogs {
    public class CharacterAppearance : MonoBehaviour {

        public Transform leftHandWeaponPosition;
        public Transform rightHandWeaponPosition;

        [SerializeField, NotEditableInInspector] private SkinnedMeshRenderer bodySkinnedMeshRenderer;
        [SerializeField, NotEditableInInspector] private Material originMaterial;
        [SerializeField, NotEditableInInspector] private Material invisibleMaterial;

        private void Reset() {
            bodySkinnedMeshRenderer = transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();
            originMaterial = bodySkinnedMeshRenderer.sharedMaterial;
            #if UNITY_EDITOR
                invisibleMaterial = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Material>("Default-Particle.mat");
            #endif
        }

        public void SetVisible(bool visible) {
            if (visible) {
                bodySkinnedMeshRenderer.material = originMaterial;
            } else {
                bodySkinnedMeshRenderer.material = invisibleMaterial;
            }
        }
    }
}
