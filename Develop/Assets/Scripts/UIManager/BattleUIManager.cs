using UnityEngine;
using UnityEngine.UI;

namespace CatsAndDogs {
    public class BattleUIManager : MonoBehaviour {

        public float rotateAnglePerX = 0.252f; // 每像素旋转角。TODO: 改为与像素无关

        public Button shootButton;
        public Button skillButton;
        public Button reloadButton;
        public Button lockTargetEnemyButton;
        public Image skillButtonMask;
        public Text skillText;

        public Text matchInfoText;
        public Image matchEndPanel;
        public Text matchEndInfoText;
        public bool _rightPanelEnable = true;
        public bool rightPanelEnable {
            get {
                return _rightPanelEnable;
            }
            set {
                _rightPanelEnable = value;
                shootButton.gameObject.SetActive(_rightPanelEnable);
                if (character.skill.type != CharacterSkill.Type.None) {
                    skillButton.gameObject.SetActive(_rightPanelEnable);
                }
                reloadButton.gameObject.SetActive(_rightPanelEnable);
                lockTargetEnemyButton.gameObject.SetActive(_rightPanelEnable);
            }
        }

        public Sprite sprintSkillSprite;
        public Sprite tumbleSkillSprite;
        public Sprite burrowSkillSprite;
        public Sprite burrowOutSkillSprite;

        private Character character;
        private CharacterMove characterMove;
        private CharacterBehaviour characterBehaviour;
        private CharacterSkill characterSkill;

        public bool isInLockButton = false;

        public bool fire = false;
        private float fireCD = 0f;

        private static BattleUIManager instance;
        public static BattleUIManager Instance {
            get {
                return instance;
            }
        }

        private void Awake() {
            instance = this;
        }

        private void Start() {
            character = BattleSceneManager.Instance.myCharacter;
            characterMove = character.GetComponent<CharacterMove>();
            characterBehaviour = character.GetComponent<CharacterBehaviour>();
            characterSkill = character.GetComponent<CharacterSkill>();

            // TODO: 考虑GC
            skillButton.onClick.AddListener(delegate { characterSkill.Use(); });
            reloadButton.onClick.AddListener(delegate { characterBehaviour.Reload(); });
        }

        private void Update() {
            /// 被动更新部分
            // TODO: CD 判断不严谨
            if (fireCD > 0f) {
                fireCD = fireCD > Time.deltaTime ? fireCD - Time.deltaTime : 0f;
            }
            if (fire == true) {
                if (fireCD == 0f) {
                    if (character.backpack.gun.bulletNumber <= 0) {
                        // characterBehaviour.Reload();
                    } else {
                        if (characterBehaviour.enabled && !characterBehaviour.reloading) {
                            characterBehaviour.Fire();
                            fireCD = 1f / character.backpack.gun.rateOfFire;
                        }
                    }
                }
            } else {
                if (character != null) {
                    if (character.GetComponent<Animator>().GetBool("Fire")) {
                        characterBehaviour.EndFire();
                    }
                }
            }

            if (PhotonNetwork.room == null) {
                return;
            }
            string text = ((float)PhotonNetwork.room.CustomProperties["timeSinceBegin"]).ToString("F") + "\n";
            text += BattleSceneManager.Instance.alivePlayerNumber.ToString() + "/" + BattleSceneManager.Instance.totalPlayerNumber.ToString();
            matchInfoText.text = text;

            /// 主动更新部分
            character.velocityScale = UltimateJoystick.GetPosition("Move");

            // 调试代码
            if (character.velocityScale.sqrMagnitude < 0.01f) {
                character.velocityScale = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            }

            //if (_rightPanelEnable) {
            //    HandleTouch();
            //}

            // 技能 CD

            skillButtonMask.fillAmount = characterSkill.cooldown / characterSkill.skillCD;
        }

        public void FireBegin() {
            fire = true;
        }

        public void FireEnd() {
            fire = false;
        }

        public void OnClickLockTargetEnemyButton() {
            //if (characterBehaviour.lockTargetEnemy) {
            //    characterBehaviour.CancelLockTargetEnemy();
            //} else {
                characterBehaviour.LockTargetEnemy();
            //}
        }

        // TODO: 按钮直接绑定技能脚本的方法
        public void LockTargetEnemy() {
            characterBehaviour.LockTargetEnemy();
        }

        private Vector2 beginPosition = new Vector2(-1f, -1f);
        public bool rotateByTouchMove = false;
        private void HandleTouch() {
            for (int i = 0, length = Input.touchCount; i < length; i++) {
                Touch touch = Input.touches[i];
                // 记录开始位置
                if (touch.phase == TouchPhase.Began) {
                    beginPosition = touch.position;
                    rotateByTouchMove = false;
                }
                // 左半屏幕的滑动不考虑，TODO: 应该是左半屏幕开始的滑动不考虑？
                if (touch.position.x < 0.5f * Screen.width) {
                    continue;
                }
                // 正在射击时的滑动不考虑（实际上是指在射击按钮内滑动时）
                if (fire) {
                    continue;
                }
                // 滑动到锁定按钮内之后
                if (isInLockButton) {
                    characterBehaviour.lockTargetEnemy = false;
                    characterBehaviour.LockTargetEnemy();
                    continue;
                }
                // 横向滑动距离超过50之后取消锁定，TODO: 目前数值跟像素相关，应该改为与屏幕尺寸比例相关
                // Debug.Log(touch.position.x - beginPosition.x);
                if (Mathf.Abs(touch.position.x - beginPosition.x) > 50f) {
                    rotateByTouchMove = true;
                }
                // Debug.Log(rotateByTouchMove.ToString());
                // 取消锁定，滑动旋转方向
                if (rotateByTouchMove) {
                    character.transform.Rotate(Vector3.up, touch.deltaPosition.x * rotateAnglePerX);
                    characterBehaviour.lockTargetEnemy = false;
                }
            }
        }

        //// TODO: 这个接收动画事件的方法不应该在这里
        //public void Reload() {
        //    characterBehaviour.reloading = false;
        //    character.gun.bulletNumber = character.gun.magazineSize;
        //}
    }
}
