using UnityEngine;
using System.Collections.Generic;

namespace CatsAndDogs {
    [RequireComponent(typeof(Character))]
    public class CharacterBuff : MonoBehaviour {

        public List<Buff> buffs = new List<Buff>();

        [SerializeField, NotEditableInInspector] Character character;

        public enum Type {
            Sprint,  // 加速
            Restore, // 恢复血量
        }

        public class Buff {
            public Type type;
            public float beginTime;
            public float duration;
            public int internalTimes;
        }

        private void Reset() {
            character = GetComponent<Character>();
        }

        private void Update() {
            for (int i = buffs.Count - 1; i >= 0; i--) {
                Buff buff = buffs[i];
                TakeEffect(buff);
            }

                // 持续时间过期就移除 buff
            for (int i = buffs.Count - 1; i >= 0; i--) {
                Buff buff = buffs[i];
                if (Time.time > buff.beginTime + buff.duration) {
                    RemoveBuffEffect(buff);
                    buffs.RemoveAt(i);
                }
            }
        }

        public void AddBuff(Type type, float beginTime, float duration) {
            Buff buff = new Buff {
                type = type,
                beginTime = beginTime,
                duration = duration,
                internalTimes = 0
            };
            buffs.Add(buff);

            // 刚添加 buff 时进行的操作
            switch(type) {
                case Type.Sprint:
                    character.moveSpeed = character.originSpeed * 1.5f;
                    break;
            }
        }

        private void TakeEffect(Buff buff) {
            switch(buff.type) {
                case Type.Sprint:
                    break;
                case Type.Restore:
                    float buffTime = Time.time - buff.beginTime;
                    if (buffTime / 1f > buff.internalTimes) {
                        character.GetComponent<CharacterBehaviour>().GetTreatment(50f);
                        buff.internalTimes += 1;
                    }
                    break;
            }
        }

        private void RemoveBuffEffect(Buff buff) {
            switch(buff.type) {
                case Type.Sprint:
                    character.moveSpeed = character.originSpeed * 1f;
                    break;
            }
        }
    }
}
