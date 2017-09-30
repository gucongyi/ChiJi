using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;

namespace CatsAndDogs {
    [RequireComponent(typeof(Character))]
    public class CharacterBehaviour : MonoBehaviour {

        public float bulletSpeed = 200f;
        public float searchDis = 50f;
        public float searchAngle = 180f;
        public bool lockTargetEnemy = false;

        public Transform bulletPrefab;
        public UnityStandardAssets.Effects.ParticleSystemMultiplier particleSystemMultiplier;

        [SerializeField, NotEditableInInspector] private Transform targetEnemyTransform;
        [SerializeField, NotEditableInInspector] private Character character;
        [SerializeField, NotEditableInInspector] private Animator animator;
        [SerializeField, NotEditableInInspector] private PhotonView photonView;

        public void Reset() {
            bulletPrefab = Resources.Load<Transform>("Bullet");
            particleSystemMultiplier = Resources.Load<UnityStandardAssets.Effects.ParticleSystemMultiplier>("ExplosionMobile");

            character = GetComponent<Character>();
            animator = GetComponent<Animator>();
            photonView = GetComponent<PhotonView>();
        }

        private void OpenOtherHUDText()
        {
            if (targetEnemyTransform != null)
            {
                OtherHpController.mInstance.otherHpFollow.target = targetEnemyTransform;
                OtherHpController.mInstance.otherHpFollow.LockDistance = searchDis;
                OtherHpController.mInstance.otherHpFollow.mySelf = character.transform;
                OtherHpController.mInstance.hudTextController.gameObject.SetActive(true);
                OtherHpController.mInstance.hudTextController.ResetHp(targetEnemyTransform.GetComponent<Character>());
                targetEnemyTransform.GetComponent<Character>().isLocked = true;
            }
        }

        private void CloseOtherHUDText()
        {
            if (targetEnemyTransform != null)
            {
                targetEnemyTransform.GetComponent<Character>().isLocked = false;
                OtherHpController.mInstance.otherHpFollow.target = null;
                OtherHpController.mInstance.hudTextController.gameObject.SetActive(false);
            }
        }
        private void CloseOtherHUDTextIftargetEnemyTransformNull()
        {
            OtherHpController.mInstance.otherHpFollow.target = null;
            OtherHpController.mInstance.hudTextController.gameObject.SetActive(false);
        }

        private void FixedUpdate() {
            if (lockTargetEnemy) {
                RaycastHit hitInfo;
                if (targetEnemyTransform==null)//Destroy
                {
                    CloseOtherHUDTextIftargetEnemyTransformNull();
                    return;
                }
                if (Physics.Raycast(character.centerPosition, (targetEnemyTransform.position - character.centerPosition).normalized, out hitInfo, 50f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)) {
                    if (hitInfo.transform == targetEnemyTransform) {
                        transform.LookAt(targetEnemyTransform);
                        //OpenOtherHUDText();
                    } else {
                        lockTargetEnemy = false;
                        CloseOtherHUDText();
                        targetEnemyTransform = null;
                    }
                } else {
                    lockTargetEnemy = false;
                    CloseOtherHUDText();
                    targetEnemyTransform = null;
                }
            }
            else
            {
                CloseOtherHUDText();
            }
        }

        #region TakeDamage
        public void TakeDamage(float damage) {
            photonView.RPC("RPCTakeDamage", PhotonTargets.All, damage);
            if (character.hp <= 0f) {
                Die();
            }
        }
        [PunRPC]
        private void RPCTakeDamage(float damage) {
            if (character == null) return;
            if (character != null && character.hp == 0f) {
                return;
            }
            float bodyArmorDamage = character.backpack.BodyArmorTakeDamage(damage);
            float characterDamage = damage - bodyArmorDamage;
            character.hp -= characterDamage;
            UnityEngine.UI.Text damageText = Instantiate(character.damageTextPrefab, character.hudCanvas.transform);
            damageText.transform.localPosition = new Vector3(53.5f, 0f, 0f);
            damageText.transform.localRotation = Quaternion.identity;
            damageText.text = characterDamage.ToString();
        }
        #endregion

        #region GetTreatment
        public void GetTreatment(float hp) {
            photonView.RPC("RPCGetTreatment", PhotonTargets.All, hp);
        }
        [PunRPC]
        private void RPCGetTreatment(float hp) {
            character.hp += hp;
            if (character.hp > character.maxHP) {
                character.hp = character.maxHP;
            }
        }
        #endregion

        #region Die
        private bool dead = false;
        private void Die() {
            if (dead) {
                return;
            }
            dead = true;

            Hashtable hashTable = new Hashtable();
            hashTable["alive"] = false;
            PhotonView.Get(this).owner.SetCustomProperties(hashTable);
            PhotonView.Get(this).RPC("RPCDie", PhotonTargets.All);
        }
        [PunRPC]
        private void RPCDie() {
            animator.SetLayerWeight(1, 0f);
            animator.SetTrigger("Die");
            character.rigidbody.velocity = Vector3.zero;
            character.move.enabled = false;
            Destroy(gameObject, 2f);
        }
        #endregion

        #region ReNamePlayer
        public void ReNamePlayer(string name)
        {
            PhotonView.Get(this).RPC("RPCReNamePlayer", PhotonTargets.All, name);
        }
        [PunRPC]
        private void RPCReNamePlayer(string name)
        {
            gameObject.name = name;
        }
        #endregion

        #region Fire
        public void Fire() {
            int fireBulletNum = character.backpack.gun.type == Gun.Type.Shotgun ? 10 : 1;
            for (int i = 0; i < fireBulletNum; i++) {
                Vector3 targetVector = character.backpack.gun.CalcTargetVector();
                RaycastHit hitInfo;
                if (Physics.Raycast(transform.position + new Vector3(0f, 0.5f, 0f), transform.TransformVector(targetVector).normalized, out hitInfo, 50f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)) {
                    if (hitInfo.transform.tag == "Player") {
                        float damage = character.backpack.gun.CalcDamage(hitInfo.distance);
                        hitInfo.transform.GetComponent<CharacterBehaviour>().TakeDamage(damage);
                    }
                }
            }
            character.backpack.gun.bulletNumber -= 1;
            photonView.RPC("RPCFire", PhotonTargets.All);
            if (character.backpack.gun.bulletNumber <= 0) {
                Reload();
            }
        }
        [PunRPC]
        private void RPCFire() {
            Debug.Log("RPC: Fire");
            animator.SetBool("Fire", true);

            Vector3 fireTarget = transform.position + new Vector3(0f, 0.5f, 0f) + transform.forward * 50f;
            Vector3 bulletOrientation = transform.forward;
            Transform bullet = Instantiate(bulletPrefab, transform.position + new Vector3(0f, 0.5f, 0f) + 1f * transform.forward, Quaternion.identity);
            bullet.transform.LookAt(fireTarget);
            bullet.GetComponent<Rigidbody>().velocity = bulletSpeed * bulletOrientation;
            Destroy(bullet.gameObject, 3f);

            particleSystemMultiplier.Play();
        }
        #endregion

        #region EndFire
        public void EndFire() {
            photonView.RPC("RPCEndFire", PhotonTargets.All);
        }
        [PunRPC]
        private void RPCEndFire() {
            Debug.Log("RPC: EndFire");
            animator.SetBool("Fire", false);
        }
        #endregion

        #region Reload
        public bool reloading = false;
        public void Reload() {
            if (!enabled) {
                return;
            }
            if (reloading) {
                return;
            }
            if (character.backpack.gun.bulletNumber == character.backpack.gun.magazineSize) {
                return;
            }
            reloading = true;
            float reloadTime=character.GetComponent<CharacterAnimator>().reloadClipTime;
            character.bulletCanvas.GetComponent<BulletCanvasController>().Reload(reloadTime);
            photonView.RPC("RPCReload", PhotonTargets.All);
        }
        [PunRPC]
        private void RPCReload() {
            animator.SetTrigger("Reload");
            animator.ResetTrigger("Fire");
        }
        public void ReloadMagazine() {    // 动画事件触发
            Debug.Log("Reload finish");
            if (photonView.isMine) {
                character.backpack.gun.bulletNumber = character.backpack.gun.magazineSize;
                reloading = false;
            }
        }
        #endregion

        #region LockTargetEnemy
        public void LockTargetEnemy() {
            targetEnemyTransform = SearchTargetEnemyV2();
            if (targetEnemyTransform == null) {
                lockTargetEnemy = false;
                Debug.Log("Lock target enemy failed.");
            } else {
                OpenOtherHUDText();
                lockTargetEnemy = true;
                Debug.Log("Lock target enemy success.");
            }
        }
        public void CancelLockTargetEnemy() {
            lockTargetEnemy = false;
            CloseOtherHUDText();
            targetEnemyTransform = null;
            Debug.Log("Cancel target enemy lock.");
        }
        private Transform SearchTargetEnemyV2() {
            List<Transform> enemyTransforms = new List<Transform>();
            for (int i = PhotonNetwork.otherPlayers.Length - 1; i >= 0; i--) {
                PhotonPlayer player = PhotonNetwork.otherPlayers[i];
                if (player == null) {
                    continue;
                }
                GameObject enemyGameObject = (GameObject)player.TagObject;
                if (enemyGameObject == null) {
                    continue;
                }
                enemyTransforms.Add(enemyGameObject.transform);
            }
            if (enemyTransforms.Count == 0) {
                return null;
            }
            float minDis = searchDis;
            int minIndex = -1;
            for (int i = enemyTransforms.Count - 1; i >= 0; i--) {
                Transform enemyTransform = enemyTransforms[i];
                Vector3 vector = enemyTransform.position - transform.position;
                float distance = vector.magnitude;
                if (distance >= minDis) {
                    continue;
                }
                if (Vector2.Angle(new Vector2(transform.forward.x, transform.forward.z), new Vector2(vector.x, vector.z)) > 0.5f * searchAngle) {
                    continue;
                }
                minDis = distance;
                minIndex = i;
            }
            if (minIndex == -1) {
                return null;
            };
            return enemyTransforms[minIndex];
        }
        #endregion

        #region TumbleForward
        public void TumbleForward() {
            photonView.RPC("RPCTumbleForward", PhotonTargets.All);
        }
        [PunRPC]
        private void RPCTumbleForward() {
            animator.SetTrigger("RollForward");
        }
        #endregion
        #region TumbleBack
        public void TumbleBack() {
            photonView.RPC("RPCTumbleBack", PhotonTargets.All);
        }
        [PunRPC]
        private void RPCTumbleBack() {
            animator.SetTrigger("RollBack");
        }
        #endregion
        #region TumbleRight
        public void TumbleRight() {
            photonView.RPC("RPCTumbleRight", PhotonTargets.All);
        }
        [PunRPC]
        private void RPCTumbleRight() {
            animator.SetTrigger("RollRight");
        }
        #endregion
        #region TumbleLeft
        public void TumbleLeft() {
            photonView.RPC("RPCTumbleLeft", PhotonTargets.All);
        }
        [PunRPC]
        private void RPCTumbleLeft() {
            animator.SetTrigger("RollLeft");
        }
        #endregion

        #region Burrow
        public void Burrow() {
            character.burrowed = true;
            character.rigidbody.velocity = Vector3.zero;
            character.move.enabled = false;
            enabled = false;
            photonView.RPC("RPCBurrow", PhotonTargets.All);
        }
        [PunRPC]
        private void RPCBurrow() {
            animator.SetTrigger("Burrow");
        }
        #endregion

        #region BurrowOut
        public void BurrowOut() {
            photonView.RPC("RPCBurrowOut", PhotonTargets.All);
        }
        [PunRPC]
        private void RPCBurrowOut() {
            animator.SetTrigger("BurrowOut");
        }
        public void OnBurrowedOut() {    // 动画事件触发
            if (photonView.isMine) {
                character.burrowed = false;
                character.move.enabled = true;
                enabled = true;
            }
        }
        #endregion
    }
}

