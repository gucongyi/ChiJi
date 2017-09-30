using System.Collections;
using UnityEngine;

namespace CatsAndDogs {
    public class BattleSceneManager : MonoBehaviour {

        [NotEditableInInspector] public int alivePlayerNumber = 0;
        [NotEditableInInspector] public int totalPlayerNumber = 0;

        public Transform TransDrawCircleRoot;
        [NotEditableInInspector] public Character myCharacter;

        [SerializeField, NotEditableInInspector] private CameraFollow cameraFollow;
        [SerializeField, NotEditableInInspector] private Transform[] spawnTransforms;

        private static BattleSceneManager instance;
        public static BattleSceneManager Instance {
            get {
                return instance;
            }
        }

        private void Reset() {
            cameraFollow = Camera.main.GetComponent<CameraFollow>();
            Transform spawnTransRoot = GameObject.Find("SpawnTransforms").transform;
            int spawnTransNum = spawnTransRoot.childCount;
            spawnTransforms = new Transform[spawnTransNum];
            for (int i = 1; i <= spawnTransNum; i++) {
                spawnTransforms[i - 1] = spawnTransRoot.GetChild(i - 1);
            }
        }

        private void Awake() {
            instance = this;
            if (AppController.mInstance != null)
            {
                AppController.mInstance.OpenMultiTouch();
            }
            if (!PhotonNetwork.connected) {
                SetOfflineAndCreateRoom();
            }

            SetMatchTime();

            // set spawn position and spawn rotation
            int positionIndex = PhotonNetwork.room.PlayerCount - 1;
            Vector3 spawnPosition = spawnTransforms[positionIndex].position;
            Quaternion spawnRotation = spawnTransforms[positionIndex].rotation;

            // init my character
            string name = AppController.mInstance.RoleNameScene();
            string modelName = "";
            switch(name) {
                case "AssassinCat":
                    modelName = "Characters/AssassinCat";
                    break;
                case "AssassinCat1":
                    modelName = "Characters/SoldierCat";
                    break;
                case "NinjiaDog":
                    modelName = "Characters/NinjiaDog01";
                    break;
                case "NinjiaDog1":
                    modelName = "Characters/SaintBernard";
                    break;
            }
            myCharacter = PhotonNetwork.Instantiate(modelName, spawnPosition, spawnRotation, 0).GetComponent<Character>();
            // PhotonNetwork.player.TagObject = myCharacter.gameObject;

            DrawCicle drawCicle = TransDrawCircleRoot.GetComponent<DrawCicle>();
            drawCicle.Player = myCharacter.transform.GetComponent<PlayerArrowController>();
            drawCicle.enabled = true;
            myCharacter.gameObject.name = "Player_" + PhotonNetwork.player.ID;
            myCharacter.GetComponent<CharacterBehaviour>().ReNamePlayer(myCharacter.gameObject.name);
 
            // set master component enabled
            myCharacter.GetComponent<CharacterMove>().enabled = true;
            myCharacter.GetComponent<CharacterAnimator>().enabled = true;
            myCharacter.gunLine.gameObject.SetActive(true);
            myCharacter.bulletCanvas.gameObject.SetActive(true);

            myCharacter.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            cameraFollow.target = myCharacter.transform;
        }

        private void Update() {
            if (PhotonNetwork.room == null) {
                return;
            }
            totalPlayerNumber = PhotonNetwork.room.PlayerCount;
            alivePlayerNumber = 0;
            for (int i = 0, length = totalPlayerNumber; i < length; i++) {
                if (PhotonNetwork.playerList[i] == null) {
                    continue;
                }
                if (PhotonNetwork.playerList[i].CustomProperties["alive"] == null) {
                    continue;
                }
                if ((bool)PhotonNetwork.playerList[i].CustomProperties["alive"]) {
                    alivePlayerNumber++;
                }
            }

            if (PhotonNetwork.isMasterClient) {
                ExitGames.Client.Photon.Hashtable hashTable = new ExitGames.Client.Photon.Hashtable();
                hashTable["timeSinceBegin"] = (float)PhotonNetwork.room.CustomProperties["timeSinceBegin"] + Time.deltaTime;
                PhotonNetwork.room.SetCustomProperties(hashTable);
            }

            if (!PhotonNetwork.offlineMode) {
                if (alivePlayerNumber <= 1 && (float)PhotonNetwork.room.CustomProperties["timeSinceBegin"] > 20f) {
                    MatchEnd();
                }
            }
        }

        private void OnGUI() {
                //Room room = PhotonNetwork.room;
                //if (room != null) {
                //    GUI.Label(new Rect(20, 20, 100, 30), room.Name);
                //    GUI.Label(new Rect(20, 50, 100, 30), room.PlayerCount.ToString() + "/" + room.MaxPlayers.ToString());
                //    GUI.Label(new Rect(20, 80, 100, 30), room.IsVisible.ToString());
                //    GUI.Label(new Rect(20, 110, 100, 30), room.IsOpen.ToString());
                //}
        }

        private void SetOfflineAndCreateRoom() {
            PhotonNetwork.offlineMode = true;
            RoomOptions roomOptions = new RoomOptions {
                IsVisible = true,
                IsOpen = true,
                MaxPlayers = 4
            };
            PhotonNetwork.CreateRoom("MyMatch", roomOptions, TypedLobby.Default);
        }

        private void SetMatchTime() {
            ExitGames.Client.Photon.Hashtable hashTable = new ExitGames.Client.Photon.Hashtable();
            hashTable["timeSinceBegin"] = 0f; // TODO: 采用正确的时间同步方式
            PhotonNetwork.room.SetCustomProperties(hashTable);
        }

        bool matchEnd = false;
        private void MatchEnd() {
            if (matchEnd == true) {
                return;
            }
            Debug.Log("Match end");
            BattleUIManager.Instance.matchEndPanel.gameObject.SetActive(true);
            string text = (bool)PhotonNetwork.player.CustomProperties["alive"] ? "大吉大利，今晚吃鸡" : "再接再厉";
            BattleUIManager.Instance.matchEndInfoText.text = text;
            StartCoroutine(LeaveMatch(5f));
            matchEnd = true;
        }
        private IEnumerator LeaveMatch(float waitTime) {
            PhotonNetwork.LeaveRoom();//清理当前场景
            yield return new WaitForSeconds(waitTime);
            AppController.mInstance.MySceneController.LoadSceneAynsc("Start", (float progress) =>
            {
                UIRoot.mInstance.LoadingBattleSlider.value = progress;
                if (progress>0.2f)
                {
                    UIRoot.mInstance.EnableLoading();
                }
            }, () =>
            {
                UIRoot.mInstance.DisableLoading();
                AppController.mInstance.ResetRoleAndLobbyUI();
            });
            
        }
    }
}

