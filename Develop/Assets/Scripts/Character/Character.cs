using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;

namespace CatsAndDogs {
    /// <summary>
    /// 代表角色，包含角色的设计数据。
    /// 与角色无关的组件只能与 Character 交互。
    /// Character 可以与其它角色相关的组件进行交互。
    /// </summary>
    [RequireComponent(typeof(Rigidbody), typeof(PhotonView))]
    public class Character : MonoBehaviour {

        public enum Type { AssassinCat, NinjiaDog, AssaultCat, SoldierDog };

        // 设计数据
        public Type type;              // 角色类型
        public float maxHP = 5000f;    // 最大生命值
        public float originSpeed = 8f; // 原始移速
        public CharacterTalent talent; // 天赋
        public bool isLocked;
        public Vector3 centerPosition {
            get {
                return transform.position + new Vector3(0f, 0.5f, 0f);
            }
        } // 中心位置（目前用来作为锁定功能的射线起始位置）

        // 实时数据
        [NotEditableInInspector] public float hp;              // 生命
        [NotEditableInInspector] public float PreHp;
        [NotEditableInInspector] public float moveSpeed;       // 移动速度
        [NotEditableInInspector] public bool burrowed = false; // 是否在缩地状态下
        [NotEditableInInspector] public Vector2 velocityScale; // 速度缩放比例，由方向盘定义，取值范围为半径为1的圆形

        // 组件绑定（外部可能引用）
        [NotEditableInInspector] public new Rigidbody rigidbody;
        [NotEditableInInspector] public PhotonView photonView;
        [NotEditableInInspector] public LineRenderer gunLine;
        [NotEditableInInspector] public Canvas hudCanvas;
        [NotEditableInInspector] public Text damageTextPrefab;
        [NotEditableInInspector] public Canvas bulletCanvas;
        [NotEditableInInspector] public CharacterMove move;
        [NotEditableInInspector] public CharacterSkill skill;
        [NotEditableInInspector] public Backpack backpack;

        // 组件绑定（自用）
        [SerializeField, NotEditableInInspector] private PlayerHudTextController hudTextController;
        [SerializeField, NotEditableInInspector] private Text bulletText;

        public void Reset() {
            rigidbody = GetComponent<Rigidbody>();
            photonView = GetComponent<PhotonView>();
            gunLine = transform.Find("Line").GetComponent<LineRenderer>();
            bulletCanvas = transform.Find("bulletCanvas").GetComponent<Canvas>();
            damageTextPrefab = Resources.Load<Text>("DamagePop");
            move = GetComponent<CharacterMove>();
            skill = GetComponent<CharacterSkill>();
            backpack = GetComponent<Backpack>();
            hudCanvas = transform.Find("hudCanvas").GetComponent<Canvas>();
            hudTextController = hudCanvas.GetComponent<PlayerHudTextController>();
            bulletText = transform.Find("bulletCanvas/Text").GetComponent<Text>();
        }

        void Awake()
        {
            hudTextController.Init();
            if (photonView.isMine)
            {
                hudTextController.gameObject.SetActive(true);
            }
            else
            {
                hudTextController.gameObject.SetActive(false);
            }
            isLocked = false;
        }

        private void Start() {
            hp = maxHP;
            PreHp = maxHP;
            moveSpeed = originSpeed;

            SetSelfPlayerAlive(); // TODO: 考虑是否应该在这里执行
        }

        private void Update() {
            UpdateHUD();
            UpdateGunLine();
            if (photonView.isMine)
            {
                MinimapController.mInstance.UpdateMiniMapSelfPlayer(transform, "RoleMiniMap");
            }
        }

        private void SetSelfPlayerAlive() {
            Hashtable hashTable = new Hashtable();
            hashTable["alive"] = true;
            PhotonView.Get(this).owner.SetCustomProperties(hashTable);
        }

        private void UpdateHUD() {
            hudCanvas.transform.forward = hudCanvas.transform.position - Camera.main.transform.position;
            float hudValue = hp / maxHP;
            float delta = Mathf.Abs(PreHp - hp);
            if (delta > float.Epsilon)
            {
                if (PreHp > hp)//减少
                {
                    hudTextController.DecreaseToHp(hudValue);
                    if (!photonView.isMine)
                    {
                        if(isLocked)
                            OtherHpController.mInstance.hudTextController.DecreaseToHp(hudValue);
                    }
                }
                else if (PreHp < hp)//增加
                {
                    hudTextController.IncreaseToHp(hudValue);
                    if (!photonView.isMine)
                    {
                        if (isLocked)
                            OtherHpController.mInstance.hudTextController.IncreaseToHp(hudValue);
                    }
                }
            }
            
            //hpSlider.value = hp / maxHP;
            bulletText.text = backpack.gun.bulletNumber.ToString() + "/" + backpack.gun.magazineSize.ToString();
        }

        private void UpdateGunLine() {
            RaycastHit hitInfo;
            bool hit = Physics.Raycast(gunLine.transform.position, transform.forward, out hitInfo, 30f);
            Vector3 targetPos;
            if (hit) {
                targetPos = hitInfo.point;
            } else {
                targetPos = gunLine.transform.position + transform.forward * 30f;
            }
            gunLine.SetPosition(1, transform.InverseTransformPoint(targetPos) - gunLine.transform.localPosition);
        }
    }
}
