using System.Collections.Generic;
using UnityEngine;

namespace CatsAndDogs {
    public class Backpack : MonoBehaviour {

        // 有装备效果的物品
        public Gun gun;
        private BodyArmor bodyArmor;
        private Gem[] gems = new Gem[3] { null, null, null };

        [SerializeField, NotEditableInInspector] public List<BackpackItem> items = new List<BackpackItem>(); // 除了枪和防弹衣以外的物品
        [SerializeField, NotEditableInInspector] private float weight = 0f;
        [SerializeField, NotEditableInInspector] private Character character;
        [SerializeField, NotEditableInInspector] private CharacterBuff characterBuff;
        [SerializeField, NotEditableInInspector] private CharacterAnimator characterAnimator;

        private void Reset() {
            gems = new Gem[3] { null, null, null };
            character = GetComponent<Character>();
            characterBuff = GetComponent<CharacterBuff>();
            characterAnimator = GetComponent<CharacterAnimator>();
        }

        private void Start() {
            // 也就是 TakeGun 不带 SetAnimation
            Gun.Type type = Gun.Type.Pistol;
            Gun newGun = new Gun(type);
            AffectGun(newGun);
            newGun.bulletNumber = newGun.magazineSize; // AffectGun 影响弹夹容量后还需影响实时子弹数量
            weight += newGun.weight - gun.weight;
            gun = newGun;
            character.photonView.RPC("RPCSetGunModel", PhotonTargets.All, type);

            if (character.photonView.isMine) {
                BackPackManager.mIntance.OnUsePropEvent += UseItem;
                BackPackManager.mIntance.OnGiveUpPropEvent += RemoveItem;
            }
        }

        #region Item 相关方法
        public bool AddItem(BackpackItem.Type type) {
            BackpackItem item = new BackpackItem { type = type };
            items.Add(item);
            weight += item.GetWeight();
            return true;
        }
        
        private int FindItem(BackpackItem.Type type) {
            for (int i = items.Count - 1; i >= 0; i--) {
                if (items[i].type == type) {
                    return i;
                }
            }
            return -1;
        }

        public void UseItem(InfoHelper.BaseBkgInfo baseBkgInfo) {
            BackpackItem.Type type;
            switch(baseBkgInfo.id) {
                case "4001":
                    type = BackpackItem.Type.Restore;
                    break;
                case "4002":
                    type = BackpackItem.Type.Heal;
                    break;
                default:
                    return;
            }
            int itemIndex = FindItem(type);
            if (itemIndex == -1) {
                return;
            }
            UseItem(items[itemIndex]);
        }
        public bool UseItem(BackpackItem item) {
            // 先移除再使用
            if (!RemoveItem(item)) {
                return false;
            }
            switch (item.type) {
                case BackpackItem.Type.Restore:
                    float duration = 4f;
                    if (character.talent) {
                        for (int i = character.talent.propertyMultipliers.Length - 1; i >= 0; i--) {
                            CharacterTalent.PropertyMultiplier propertyMultiplier = character.talent.propertyMultipliers[i];
                            switch (propertyMultiplier.property) {
                                case CharacterTalent.Property.RestoreDuration:
                                    duration *= propertyMultiplier.multiplier;
                                    break;
                            }
                        }
                    }
                    characterBuff.AddBuff(CharacterBuff.Type.Restore, Time.time, duration);
                    break;
                case BackpackItem.Type.Heal:
                    character.GetComponent<CharacterBehaviour>().GetTreatment(200f);
                    break;
            }
            return true;
        }

        public void RemoveItem(InfoHelper.BaseBkgInfo baseBkgInfo) {
            BackpackItem.Type type;
            switch (baseBkgInfo.id) {
                case "4001":
                    type = BackpackItem.Type.Restore;
                    break;
                case "4002":
                    type = BackpackItem.Type.Heal;
                    break;
                case "2001":
                    type = BackpackItem.Type.Gem1;
                    break;
                case "2002":
                    type = BackpackItem.Type.Gem2;
                    break;
                case "2003":
                    type = BackpackItem.Type.Gem3;
                    break;
                case "2004":
                    type = BackpackItem.Type.Gem4;
                    break;
                case "2005":
                    type = BackpackItem.Type.Gem5;
                    break;
                default:
                    return;
            }
            int itemIndex = FindItem(type);
            if (itemIndex == -1) {
                return;
            }
            RemoveItem(items[itemIndex]);
        }
        public bool RemoveItem(BackpackItem item) {
            if (items.Remove(item)) {
                weight -= item.GetWeight();
                return true;
            }
            return false;
        }
        #endregion

        #region Gun 相关方法
        public bool TakeGun(Gun.Type type) {
            Gun newGun = new Gun(type);
            AffectGun(newGun);
            newGun.bulletNumber = newGun.magazineSize; // AffectGun 影响弹夹容量后还需影响实时子弹数量
            weight += newGun.weight - gun.weight;
            gun = newGun;
            character.photonView.RPC("RPCSetGunModel", PhotonTargets.All, type);
            characterAnimator.SetAnimationClips(type);
            return true;
        }
        [PunRPC]
        private void RPCSetGunModel(Gun.Type type) {
            RPCRemoveGunModel();
            switch (type) {
                case Gun.Type.Pistol:
                    Transform pistolPrefab = Resources.Load<Transform>("AssassinCatWeapon");
                    if (pistolPrefab == null) {
                        Debug.LogError("Error: No Assets/Resources/AssassinCatWeapon.prefab");
                        return;
                    }
                    Transform pistol = Instantiate(pistolPrefab);
                    if (pistol.GetComponent<PropController>())
                    {
                        pistol.GetComponent<PropController>().enabled = false;
                    }
                    pistol.parent = character.GetComponent<CharacterAppearance>().rightHandWeaponPosition;
                    pistol.localPosition = Vector3.zero;
                    pistol.localRotation = Quaternion.Euler(0f, 90f, 90f);
                    if (character.type == Character.Type.AssassinCat) {
                        pistol = Instantiate(pistolPrefab);
                        pistol.GetComponent<PropController>().enabled = false;
                        pistol.parent = character.GetComponent<CharacterAppearance>().leftHandWeaponPosition;
                        pistol.localPosition = Vector3.zero;
                        pistol.localRotation = Quaternion.Euler(0f, -90f, -90f);
                    }
                    pistol.GetComponentInChildren<BoxCollider>().enabled = false;
                    break;
                case Gun.Type.Shotgun:
                    Transform shotgunPrefab = Resources.Load<Transform>("ScatterGun");
                    if (shotgunPrefab == null) {
                        Debug.LogError("Error: No Assets/Resources/ScatterGun.prefab");
                        return;
                    }
                    Transform shotgun = Instantiate(shotgunPrefab);
                    shotgun.GetComponent<PropController>().enabled = false;
                    shotgun.parent = character.GetComponent<CharacterAppearance>().rightHandWeaponPosition;
                    shotgun.localPosition = Vector3.zero;
                    shotgun.localRotation = Quaternion.Euler(0f, 90f, 90f);
                    shotgun.GetComponentInChildren<BoxCollider>().enabled = false;
                    break;
                case Gun.Type.Rifle:
                    Transform riflePrefab = Resources.Load<Transform>("Rifle");
                    if (riflePrefab == null) {
                        Debug.LogError("Error: No Assets/Resources/Rifle.prefab");
                        return;
                    }
                    Transform rifle = Instantiate(riflePrefab);
                    rifle.GetComponent<PropController>().enabled = false;
                    rifle.parent = character.GetComponent<CharacterAppearance>().rightHandWeaponPosition;
                    rifle.localPosition = Vector3.zero;
                    rifle.localRotation = Quaternion.Euler(0f, 90f, 90f);
                    rifle.GetComponentInChildren<BoxCollider>().enabled = false;
                    break;
                case Gun.Type.Machinegun:
                    Transform machinegunPrefab = Resources.Load<Transform>("Gatlin");
                    if (machinegunPrefab == null) {
                        Debug.LogError("Error: No Assets/Resources/Gatlin.prefab");
                        return;
                    }
                    Transform machinegun = Instantiate(machinegunPrefab);
                    machinegun.GetComponent<PropController>().enabled = false;
                    machinegun.parent = character.GetComponent<CharacterAppearance>().rightHandWeaponPosition;
                    machinegun.localPosition = Vector3.zero;
                    machinegun.localRotation = Quaternion.Euler(0f, 90f, 90f);
                    machinegun.GetComponentInChildren<BoxCollider>().enabled = false;
                    break;
            }
        }
        [PunRPC]
        private void RPCRemoveGunModel() {
            int leftChildCount = character.GetComponent<CharacterAppearance>().leftHandWeaponPosition.childCount;
            int rightChildCount = character.GetComponent<CharacterAppearance>().rightHandWeaponPosition.childCount;
            for (int i = leftChildCount - 1; i >= 0; i--) {
                Destroy(character.GetComponent<CharacterAppearance>().leftHandWeaponPosition.GetChild(i).gameObject);
            }
            for (int i = rightChildCount - 1; i >= 0; i--) {
                Destroy(character.GetComponent<CharacterAppearance>().rightHandWeaponPosition.GetChild(i).gameObject);
            }
        }

        // 类似换宝石之类的操作后，重新计算枪属性
        public bool ResetGun() {
            int bulletNum = gun.bulletNumber;
            gun.Init();
            AffectGun(gun);
            gun.bulletNumber = bulletNum;
            return true;
        }

        public bool DropGun() {
            if (gun.type == Gun.Type.Pistol) {
                return false;
            }
            Gun newGun = new Gun(Gun.Type.Pistol);
            AffectGun(newGun);
            weight = weight - gun.weight + newGun.weight;
            gun = newGun;
            RPCRemoveGunModel();
            return true;
        }

        private void AffectGun(Gun gun) {
            if (gun == null) {
                return;
            }

            // 根据天赋修改枪的属性
            if (character.talent) {
                switch (gun.type) {
                    case Gun.Type.Pistol:
                        for (int i = character.talent.propertyMultipliers.Length - 1; i >= 0; i--) {
                            CharacterTalent.PropertyMultiplier propertyMultiplier = character.talent.propertyMultipliers[i];
                            switch (propertyMultiplier.property) {
                                case CharacterTalent.Property.PistolMagazineSize:
                                    gun.magazineSize = (int)(gun.magazineSize * propertyMultiplier.multiplier);
                                    gun.bulletNumber = gun.magazineSize;
                                    break;
                                case CharacterTalent.Property.PistolRateOfFire:
                                    gun.rateOfFire *= propertyMultiplier.multiplier;
                                    break;
                            }
                        }
                        break;
                    case Gun.Type.Shotgun:
                        for (int i = character.talent.propertyMultipliers.Length - 1; i >= 0; i--) {
                            CharacterTalent.PropertyMultiplier propertyMultiplier = character.talent.propertyMultipliers[i];
                            switch (propertyMultiplier.property) {
                                case CharacterTalent.Property.ShotgunAccuracyRadiusCoef:
                                    gun.accuracyRadiusCoef *= propertyMultiplier.multiplier;
                                    break;
                            }
                        }
                        break;
                    case Gun.Type.Rifle:
                        for (int i = character.talent.propertyMultipliers.Length - 1; i >= 0; i--) {
                            CharacterTalent.PropertyMultiplier propertyMultiplier = character.talent.propertyMultipliers[i];
                            switch (propertyMultiplier.property) {
                                case CharacterTalent.Property.RifleAccuracyRadiusCoef:
                                    gun.accuracyRadiusCoef *= propertyMultiplier.multiplier;
                                    break;
                            }
                        }
                        break;
                    case Gun.Type.Machinegun:
                        for (int i = character.talent.propertyMultipliers.Length - 1; i >= 0; i--) {
                            CharacterTalent.PropertyMultiplier propertyMultiplier = character.talent.propertyMultipliers[i];
                            switch (propertyMultiplier.property) {
                                case CharacterTalent.Property.MachinegunAccuracyRadiusCoef:
                                    gun.accuracyRadiusCoef *= propertyMultiplier.multiplier;
                                    break;
                            }
                        }
                        break;
                }
            }

            // 根据宝石修改枪的属性
            for (int i = gems.Length - 1; i >= 0; i--) {
                Gem gem = gems[i];
                if (gem == null) {
                    continue;
                }
                switch(gem.type) {
                    case Gem.Type.DamageByDistance:
                        for (int j = 0; j < 4; j++) {
                            gun.damageByDistance[j, 1] += 10f;
                        }
                        break;
                    case Gem.Type.Distance:
                        for (int j = 0; j < 4; j++) {
                            gun.damageByDistance[j, 0] += 5f;
                        }
                        break;
                    case Gem.Type.MagazineSize:
                        gun.magazineSize += 15;
                        break;
                    case Gem.Type.RateOfFire:
                        gun.rateOfFire += 0.5f;
                        break;
                    case Gem.Type.ReloadTime:
                        gun.reloadTime -= 0.3f;
                        if (gun.reloadTime < 0f) {
                            gun.reloadTime = 0f;
                        }
                        break;
                }
            }
        }
        #endregion

        #region BodyArmor 相关方法
        public bool SetBodyArmor(BodyArmor.Type type, float durability) {
            BodyArmor newBodyArmor = new BodyArmor { type = type, durability = durability };
            if (bodyArmor == null) {
                weight += newBodyArmor.GetWeight();
            } else {
                weight += newBodyArmor.GetWeight() - bodyArmor.GetWeight();
            }
            bodyArmor = newBodyArmor;
            return true;
        }

        public bool SetBodyArmorDurability(float durability) {
            if (bodyArmor == null) {
                return false;
            }
            if (BackPackManager.mIntance!=null)
            {
                BackPackManager.mIntance.SetArmorDuarable(durability);
            }
            bodyArmor.durability = durability;
            if (durability <= 0f) {
                bodyArmor = null;
            }
            return true;
        }
        public float BodyArmorTakeDamage(float fullDamage) {
            if (bodyArmor == null) {
                return 0f;
            }
            float takeDamageRate = 0f;
            switch (bodyArmor.type) {
                case BodyArmor.Type.LV1:
                    takeDamageRate = 0.2f;
                    break;
                case BodyArmor.Type.LV2:
                    takeDamageRate = 0.3f;
                    break;
                case BodyArmor.Type.LV3:
                    takeDamageRate = 0.4f;
                    break;
            }
            float bodyArmorDamage = fullDamage * takeDamageRate;
            SetBodyArmorDurability(bodyArmor.durability - bodyArmorDamage);
            return bodyArmorDamage;
        }
        #endregion

        public bool MountGem(BackpackItem item, int index) {
            Gem.Type gemType;
            switch (item.type) {
                case BackpackItem.Type.Gem1:
                    gemType = Gem.Type.DamageByDistance;
                    break;
                case BackpackItem.Type.Gem2:
                    gemType = Gem.Type.Distance;
                    break;
                case BackpackItem.Type.Gem3:
                    gemType = Gem.Type.MagazineSize;
                    break;
                case BackpackItem.Type.Gem4:
                    gemType = Gem.Type.RateOfFire;
                    break;
                case BackpackItem.Type.Gem5:
                    gemType = Gem.Type.ReloadTime;
                    break;
                default:
                    return false;
            }
            if (gems[index] != null) {
                BackpackItem.Type newItemType = BackpackItem.Type.Gem1;
                switch (gems[index].type) {
                    case Gem.Type.DamageByDistance:
                        newItemType = BackpackItem.Type.Gem1;
                        break;
                    case Gem.Type.Distance:
                        newItemType = BackpackItem.Type.Gem2;
                        break;
                    case Gem.Type.MagazineSize:
                        newItemType = BackpackItem.Type.Gem3;
                        break;
                    case Gem.Type.RateOfFire:
                        newItemType = BackpackItem.Type.Gem4;
                        break;
                    case Gem.Type.ReloadTime:
                        newItemType = BackpackItem.Type.Gem5;
                        break;
                }
                items.Add(new BackpackItem { type = newItemType });
            }
            Gem gem = new Gem { type = gemType };
            gems[index] = gem;
            ResetGun();
            items.Remove(item);
            return true;
        }
        public void UnmountGem(int index) {
            if (gems[index] != null) {
                BackpackItem.Type newItemType = BackpackItem.Type.Gem1;
                switch (gems[index].type) {
                    case Gem.Type.DamageByDistance:
                        newItemType = BackpackItem.Type.Gem1;
                        break;
                    case Gem.Type.Distance:
                        newItemType = BackpackItem.Type.Gem2;
                        break;
                    case Gem.Type.MagazineSize:
                        newItemType = BackpackItem.Type.Gem3;
                        break;
                    case Gem.Type.RateOfFire:
                        newItemType = BackpackItem.Type.Gem4;
                        break;
                    case Gem.Type.ReloadTime:
                        newItemType = BackpackItem.Type.Gem5;
                        break;
                }
                items.Add(new BackpackItem { type = newItemType });
            }
            gems[index] = null;
            ResetGun();
        }

        /// 同步背包接口
        public bool ReplaceGun(string id) {
            Gun.Type type;
            switch (id) {
                case "1001":
                    type = Gun.Type.Shotgun;
                    break;
                case "1002":
                    type = Gun.Type.Rifle;
                    break;
                case "1003":
                    type = Gun.Type.Pistol;
                    break; 
                case "1004":
                    type = Gun.Type.Machinegun;
                    break;
                default:
                    return false;
            }
            return TakeGun(type);
        }
        public bool ReplaceWear(string id, float durability) {
            BodyArmor.Type type;
            switch (id) {
                case "6001":
                    type = BodyArmor.Type.LV1;
                    break;
                case "6002":
                    type = BodyArmor.Type.LV2;
                    break;
                case "6003":
                    type = BodyArmor.Type.LV3;
                    break;
                case "6004":
                    type = BodyArmor.Type.LV4;
                    break;
                default:
                    return false;
            }
            return SetBodyArmor(type, durability);
        }
        public bool AddGem(string id) {
            BackpackItem.Type type;
            switch (id) {
                case "2001":
                    type = BackpackItem.Type.Gem1;
                    break;
                case "2002":
                    type = BackpackItem.Type.Gem2;
                    break;
                case "2003":
                    type = BackpackItem.Type.Gem3;
                    break;
                case "2004":
                    type = BackpackItem.Type.Gem4;
                    break;
                case "2005":
                    type = BackpackItem.Type.Gem5;
                    break;
                default:
                    return false;
            }
            AddItem(type);
            return true;
        }
        public bool MountGem(string id, int index) {
            BackpackItem.Type type;
            switch (id) {
                case "2001":
                    type = BackpackItem.Type.Gem1;
                    break;
                case "2002":
                    type = BackpackItem.Type.Gem2;
                    break;
                case "2003":
                    type = BackpackItem.Type.Gem3;
                    break;
                case "2004":
                    type = BackpackItem.Type.Gem4;
                    break;
                case "2005":
                    type = BackpackItem.Type.Gem5;
                    break;
                default:
                    return false;
            }
            int itemIndex = FindItem(type);
            if (itemIndex == -1) {
                return false;
            }
            return MountGem(items[itemIndex], index);
        }
        public bool AddMedicine(string id) {
            BackpackItem.Type type; 
            switch (id) {
                case "4001":
                    type = BackpackItem.Type.Restore;
                    break;
                case "4002":
                    type = BackpackItem.Type.Heal;
                    break;
                default:
                    return false;
            }
            AddItem(type);
            return true;
        }
    }
}
