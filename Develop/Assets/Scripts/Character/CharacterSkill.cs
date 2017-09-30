using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace CatsAndDogs {
    [RequireComponent(typeof(Character), typeof(CharacterBuff))]
    public class CharacterSkill : MonoBehaviour {

        public static float sprintCD = 10f;
        public static float sprintDuration = 5f;
        public static float tumbleCD = 5f;
        public static float burrowCD = 3f;

        public float sprintSpeedRate = 1.5f;
        public Type type = Type.None;
        public float skillCD = 1f;
        public float cooldown = 0f;
        public float duration = 0f;

        [SerializeField, NotEditableInInspector] private Character character;
        [SerializeField, NotEditableInInspector] private CharacterBuff characterBuff;
        [SerializeField, NotEditableInInspector] private CharacterBehaviour characterBehaviour;

        public enum Type {
            Sprint, // 冲刺
            Tumble, // 翻滚
            Burrow, // 钻地
            None
        }

        private void Reset() {
            character = GetComponent<Character>();
            characterBuff = GetComponent<CharacterBuff>();
            characterBehaviour = GetComponent<CharacterBehaviour>();
        }

        private void Start() {
            // SetSkill(Type.Burrow);
        }

        private void Update() {
            if (cooldown > 0f) {
                cooldown -= Time.deltaTime;
            }
            if (cooldown < 0f) {
                cooldown = 0f;
            }

            if (Input.GetKeyDown(KeyCode.K)) {
                GetComponent<Animator>().speed = 1f;
                Use();
            }
        }

        public void SetSkill(Type type) {
            BattleUIManager.Instance.skillButton.gameObject.SetActive(true);
            this.type = type;
            switch(type) {
                case Type.Sprint:
                    skillCD = sprintCD;
                    duration = sprintDuration;
                    if (character.talent) {
                        for (int i = character.talent.propertyMultipliers.Length - 1; i >= 0; i--) {
                            CharacterTalent.PropertyMultiplier propertyMultiplier = character.talent.propertyMultipliers[i];
                            switch (propertyMultiplier.property) {
                                case CharacterTalent.Property.SprintDuration:
                                    duration *= propertyMultiplier.multiplier;
                                    break;
                            }
                        }
                    }
                    BattleUIManager.Instance.skillText.text = "疾跑";
                    BattleUIManager.Instance.skillButton.GetComponent<UnityEngine.UI.Image>().sprite = BattleUIManager.Instance.sprintSkillSprite;
                    BattleUIManager.Instance.skillButtonMask.sprite = BattleUIManager.Instance.sprintSkillSprite;
                    break;
                case Type.Tumble:
                    skillCD = tumbleCD;
                    if (character.talent) {
                        for (int i = character.talent.propertyMultipliers.Length - 1; i >= 0; i--) {
                            CharacterTalent.PropertyMultiplier propertyMultiplier = character.talent.propertyMultipliers[i];
                            switch (propertyMultiplier.property) {
                                case CharacterTalent.Property.TumbleCooldown:
                                    skillCD *= propertyMultiplier.multiplier;
                                    break;
                            }
                        }
                    }
                    BattleUIManager.Instance.skillText.text = "翻滚";
                    BattleUIManager.Instance.skillButton.GetComponent<UnityEngine.UI.Image>().sprite = BattleUIManager.Instance.tumbleSkillSprite;
                    BattleUIManager.Instance.skillButtonMask.sprite = BattleUIManager.Instance.tumbleSkillSprite;
                    break;
                case Type.Burrow:
                    skillCD = burrowCD;
                    BattleUIManager.Instance.skillText.text = "钻地";
                    BattleUIManager.Instance.skillButton.GetComponent<UnityEngine.UI.Image>().sprite = BattleUIManager.Instance.burrowSkillSprite;
                    BattleUIManager.Instance.skillButtonMask.sprite = BattleUIManager.Instance.burrowSkillSprite;
                    break;
            }
            cooldown = skillCD;
        }
        // 同步背包接口
        public bool SetSkill(string id) {
            Type type;
            switch (id) {
                case "5001":
                    type = Type.Sprint;
                    break;
                case "5002":
                    type = Type.Tumble;
                    break;
                case "5003":
                    type = Type.Burrow;
                    break;
                default:
                    return false;
            }
            SetSkill(type);
            return true;
        }

        public bool Use() {
            if (cooldown > 0f) {
                return false;
            }
            switch(type) {
                case Type.Sprint:
                    characterBuff.AddBuff(CharacterBuff.Type.Sprint, Time.time, duration);
                    Camera.main.DOFieldOfView(80f, 1f);
                    StartCoroutine(DOFieldOfViewBack(4f));
                    break;
                case Type.Tumble:
                    character.move.enabled = false;
                    float x = character.velocityScale.x;
                    float y = character.velocityScale.y;
                    if (Mathf.Abs(y) >= Mathf.Abs(x)) {
                        if (y >= 0f) {
                            characterBehaviour.TumbleForward();
                            character.rigidbody.velocity = transform.forward * character.originSpeed * 1.5f;
                        } else {
                            characterBehaviour.TumbleBack();
                            character.rigidbody.velocity = -transform.forward * character.originSpeed * 1.5f;
                        }
                    } else {
                        if (x >= 0f) {
                            characterBehaviour.TumbleRight();
                            character.rigidbody.velocity = transform.right * character.originSpeed * 1.5f;
                        } else {
                            characterBehaviour.TumbleLeft();
                            character.rigidbody.velocity = -transform.right * character.originSpeed * 1.5f;
                        }
                    }
                    break;
                case Type.Burrow:
                    if (character.burrowed) {
                        characterBehaviour.BurrowOut();
                        transform.Find("ColliderSector").GetComponent<SectorCheck>().OpenCheckWhenBurrowOut();
                        BattleUIManager.Instance.skillButton.GetComponent<UnityEngine.UI.Image>().sprite = BattleUIManager.Instance.burrowSkillSprite;
                        BattleUIManager.Instance.skillButtonMask.sprite = BattleUIManager.Instance.burrowSkillSprite;
                    } else {
                        characterBehaviour.Burrow();
                        transform.Find("ColliderSector").GetComponent<SectorCheck>().CloseCheckWhenBurrowIn();
                        BattleUIManager.Instance.skillButton.GetComponent<UnityEngine.UI.Image>().sprite = BattleUIManager.Instance.burrowOutSkillSprite;
                        BattleUIManager.Instance.skillButtonMask.sprite = BattleUIManager.Instance.burrowOutSkillSprite;
                    }
                    break;
                default:
                    return false;
            }
            cooldown = skillCD;
            return true;
        }

        private IEnumerator DOFieldOfViewBack(float waitTime) {
            yield return new WaitForSeconds(waitTime);
            Camera.main.DOFieldOfView(60f, 1f);
        }
    }
}
