using UnityEditor;
using UnityEngine;
using AnimatorController = UnityEditor.Animations.AnimatorController;
using CatsAndDogs;

public class MenuExtension : MonoBehaviour {

    [MenuItem("GameObject/GenerateMeshCollider", false, 11)]
    private static void GenerateMeshCollider() {
        GenerateMeshColliderRecursive(Selection.activeTransform);
    }
    private static void GenerateMeshColliderRecursive(Transform transform) {
        MeshRenderer meshRenderer = transform.gameObject.GetComponent<MeshRenderer>();
        MeshCollider meshCollider = transform.gameObject.GetComponent<MeshCollider>();
        if (meshRenderer && !meshCollider) {
            transform.gameObject.AddComponent<MeshCollider>();
        }
        for (int i = 0, length = transform.childCount; i < length; i++) {
            GenerateMeshColliderRecursive(transform.GetChild(i));
        }
    }

    [MenuItem("GameObject/GenerateBoxCollider", false, 12)]
    private static void GenerateBoxCollider() {
        GenerateBoxColliderRecursive(Selection.activeTransform);
    }
    private static void GenerateBoxColliderRecursive(Transform transform) {
        MeshRenderer meshRenderer = transform.gameObject.GetComponent<MeshRenderer>();
        BoxCollider boxCollider = transform.gameObject.GetComponent<BoxCollider>();
        if (meshRenderer && !boxCollider) {
            transform.gameObject.AddComponent<BoxCollider>();
        }
        for (int i = 0, length = transform.childCount; i < length; i++) {
            GenerateBoxColliderRecursive(transform.GetChild(i));
        }
    }

    private static string[] characterComponentNames = {
        "UnityEngine.Animator",
        "UnityEngine.Rigidbody",
        "UnityEngine.CapsuleCollider",
        "PhotonView",
        "PhotonTransformView",
        "PhotonAnimatorView",
        "CatsAndDogs.Character",
        "CatsAndDogs.CharacterBuff",
        "CatsAndDogs.CharacterAppearance",
        "CatsAndDogs.CharacterMove",
        "CatsAndDogs.CharacterAnimator",
        "CatsAndDogs.CharacterBehaviour",
        "CatsAndDogs.CharacterSkill",
        "CatsAndDogs.Backpack",
        "CatsAndDogs.CharacterPUN",
        "PlayerArrowController",
    };
    private static string[] childPrefabNames = {
        "ColliderSector",
        "Line",
        "UI/hudCanvas",
        "bulletCanvas",
        "Arrow",
        "ExplosionMobile"
    };
    [MenuItem("GameObject/MakeCharacter", false, 13)]
    private static void MakeCharacter() {
        GameObject gameObject = Selection.activeTransform.gameObject;
        gameObject.tag = "Player";

        #region AddComponents
        System.Type[] componentTypes = new System.Type[characterComponentNames.Length];
        for (int i = 0, length = componentTypes.Length; i < length; i++) {
            string componentName = characterComponentNames[i];
            System.Type type = System.Reflection.Assembly.Load("Assembly-CSharp").GetType(componentName);
            if (type == null) {
                type = System.Reflection.Assembly.Load("UnityEngine").GetType(componentName);
            }
            if (type == null) {
                Debug.LogError("Making character failed, no such component: " + componentName);
                return;    // 在这里就返回了，不对原来的对象做任何操作。
            }
            componentTypes[i] = type;
        }
        for (int i = 0, length = componentTypes.Length; i < length; i++) {
            gameObject.GetOrAddComponent(componentTypes[i]);
        }
        #endregion

        #region InstantiateChildren
        Transform[] prefabs = new Transform[childPrefabNames.Length];
        for (int i = 0, length = prefabs.Length; i < length; i++) {
            string prefabName = childPrefabNames[i];
            Transform prefab = Resources.Load<Transform>(prefabName);
            if (prefab == null) {
                Debug.LogError("Making character failed, no such prefab resource: " + prefabName);
                return;    // 在这里就返回了，不对原来的对象做任何操作。
            }
            prefabs[i] = prefab;
        }
        for (int i = 0, length = prefabs.Length; i < length; i++) {
            Transform instance = Instantiate(prefabs[i]);
            instance.name = instance.name.Substring(0, instance.name.Length - 7);    // Remove "(Clone)"
            instance.parent = gameObject.transform;

            // Do things by names here
            Vector3 localPosition = Vector3.zero;
            Quaternion localRotation = Quaternion.identity;
            switch (instance.name) {
                case "ColliderSector":
                    localPosition = new Vector3(0f, 1f, 0f);
                    instance.GetComponent<SectorCheck>().SelfTrans = gameObject.transform;
                    break;
                case "Line":
                    localPosition = new Vector3(0f, 0.924f, 0.1f);
                    break;
                case "hudCanvas":
                    localPosition = new Vector3(-0.873f, 0.671f, -0.076f);
                    Rect hudCanvasRect = instance.GetComponent<RectTransform>().rect;
                    hudCanvasRect.width = 213.925f;
                    hudCanvasRect.height = 306.27f;
                    localRotation = Quaternion.Euler(21.468f, -23.481f, 0f);
                    instance.localScale = new Vector3(0.003469856f, 0.003469856f, 0.6939712f);
                    break;
                case "bulletCanvas":
                    localPosition = new Vector3(0.454f, 1.254f, -0.935f);
                    localRotation = Quaternion.Euler(15.498f, 43.947f, 0f);
                    instance.localScale = new Vector3(0.007147854f, 0.007147854f, 0.7147855f);
                    instance.gameObject.SetActive(false);
                    break;
                case "Arrow":
                    localPosition = new Vector3(0f, 0.1f, 0f);
                    break;
                case "ExplosionMobile":
                    localPosition = new Vector3(0f, 0.5f, 0.864f);
                    break;
            }
            instance.localPosition = localPosition;
            instance.localRotation = localRotation;
        }
        #endregion

        #region SetAnimator
        Animator animator = gameObject.GetComponent<Animator>();
        AnimatorController animatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/Models/AssassinCat/AssassinCatAnimatorController.controller");
        if (animatorController == null) {
            Debug.LogError("Error: No Assets/Models/AssassinCat/AssassinCatAnimatorController.controller");
            return;
        }
        animator.runtimeAnimatorController = animatorController;
        Avatar avatar = AssetDatabase.LoadAssetAtPath<Avatar>("Assets/Models/AssassinCat/AssassinCat.fbx");
        if (avatar == null) {
            Debug.LogError("Error: No Assets/Models/AssassinCat/AssassinCat.fbx");
            return;
        }
        animator.avatar = avatar;
        animator.applyRootMotion = false;
        animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
        #endregion

        #region SetRigidbody
        Rigidbody rigidbody = gameObject.GetOrAddComponent<Rigidbody>();
        rigidbody.mass = 100f;
        rigidbody.drag = 0f;
        rigidbody.angularDrag = 0f;
        rigidbody.useGravity = true;
        rigidbody.isKinematic = false;
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        #endregion

        #region SetCollider
        CapsuleCollider collider = gameObject.GetOrAddComponent<CapsuleCollider>();
        collider.isTrigger = false;
        PhysicMaterial physicMaterial = AssetDatabase.LoadAssetAtPath<PhysicMaterial>("Assets/PhysicsMaterials/NoFriction.physicMaterial");
        if (physicMaterial == null) {
            Debug.LogError("Error: No Assets/PhysicsMaterials/NoFriction.physicMaterial");
            return;
        }
        collider.material = physicMaterial;
        collider.center = new Vector3(0f, 0.75f, 0f);
        collider.radius = 0.25f;
        collider.height = 1.5f;
        collider.direction = 1;
        #endregion

        #region SetPhotonView
        PhotonView photonView = gameObject.GetOrAddComponent<PhotonView>();
        photonView.synchronization = ViewSynchronization.UnreliableOnChange;
        PhotonTransformView photonTransformView = gameObject.GetOrAddComponent<PhotonTransformView>();
        PhotonAnimatorView photonAnimatorView = gameObject.GetOrAddComponent<PhotonAnimatorView>();
        photonView.ObservedComponents = new System.Collections.Generic.List<Component> {
            photonTransformView,
            photonAnimatorView
        };
        photonTransformView.m_PositionModel.SynchronizeEnabled = true;
        photonTransformView.m_PositionModel.TeleportEnabled = true;
        photonTransformView.m_PositionModel.TeleportIfDistanceGreaterThan = 3f;
        photonTransformView.m_PositionModel.InterpolateOption = PhotonTransformViewPositionModel.InterpolateOptions.EstimatedSpeed;
        photonTransformView.m_PositionModel.ExtrapolateOption = PhotonTransformViewPositionModel.ExtrapolateOptions.Disabled;
        photonTransformView.m_PositionModel.DrawErrorGizmo = true;
        photonTransformView.m_RotationModel.SynchronizeEnabled = true;
        photonTransformView.m_RotationModel.InterpolateOption = PhotonTransformViewRotationModel.InterpolateOptions.RotateTowards;
        photonTransformView.m_RotationModel.InterpolateRotateTowardsSpeed = 180f;
        photonTransformView.m_ScaleModel.SynchronizeEnabled = false;
        photonAnimatorView.SetParameterSynchronized("Velx", PhotonAnimatorView.ParameterType.Float, PhotonAnimatorView.SynchronizeType.Continuous);
        photonAnimatorView.SetParameterSynchronized("Vely", PhotonAnimatorView.ParameterType.Float, PhotonAnimatorView.SynchronizeType.Continuous);
        #endregion

        #region SetCharacter
        Character character = gameObject.GetOrAddComponent<Character>();
        character.Reset();
        #endregion

        #region SetChildren
        PlayerArrowController playerArrowController = gameObject.GetOrAddComponent<PlayerArrowController>();
        playerArrowController.ArrowGo = gameObject.transform.Find("Arrow").gameObject;
        #endregion
    }
}
