using UnityEngine;
using UnityEngine.UI;

namespace CatsAndDogs {
    public class LobbyUIManager : MonoBehaviour {

        public Transform content;
        public Transform roomInfoPrefab;
        public InputField roomName;
        public Button createRoomButton;

        public float roomInfoRefreshCD = 0.1f;

        private float roomInfoRefreshSeconds = 0;

        private void Awake() {
            PhotonManager photonManager = PhotonManager.Instance;
            if (photonManager) {
                createRoomButton.onClick.AddListener(delegate { photonManager.JoinOrCreateRoom(roomName.text); }); // TODO: RemoveListener
            }

            if (PhotonManager.Instance) {
                PhotonManager.Instance.ConnectLobby();
            }
        }

        private void Update() {
            roomInfoRefreshSeconds += Time.deltaTime;
            if (roomInfoRefreshSeconds >= roomInfoRefreshCD) {
                RefreshRoomInfo();
                roomInfoRefreshSeconds -= roomInfoRefreshCD;
            }
        }

        // TODO: 优化
        private void RefreshRoomInfo() {
            PhotonManager photonManager = PhotonManager.Instance;
            RoomInfo[] roomInfoArray = PhotonNetwork.GetRoomList();
            int roomCount = roomInfoArray.Length;
            int contentCount = content.childCount;
            // 销毁列表中比房间数多的 UI
            if (contentCount > roomCount) {
                for (int i = contentCount - 1; i >= roomCount; i--) {
                    Destroy(content.GetChild(i).gameObject);
                }
                contentCount = roomCount;
            }
            // 修改已存在的 UI
            for (int i = 0; i < contentCount; i++) {
                RoomInfoManager roomInfoManager = content.GetChild(i).GetComponent<RoomInfoManager>();
                RoomInfo roomInfo = roomInfoArray[i];
                roomInfoManager.name.text = roomInfo.Name;
                roomInfoManager.isOpen.text = roomInfo.IsOpen.ToString();
                roomInfoManager.playerCount.text = roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers;
                if (photonManager) {
                    roomInfoManager.join.onClick.AddListener(delegate { photonManager.JoinRoom(roomInfo.Name); });
                }
            }
            // 添加房间数比列表多的 UI
            for (int i = contentCount; i < roomCount; i++) {
                Transform roomInfoTransform = Instantiate(roomInfoPrefab, content);
                RoomInfoManager roomInfoManager = roomInfoTransform.GetComponent<RoomInfoManager>();
                RoomInfo roomInfo = roomInfoArray[i];
                roomInfoManager.name.text = roomInfo.Name;
                roomInfoManager.isOpen.text = roomInfo.IsOpen.ToString();
                roomInfoManager.playerCount.text = roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers;
                if (photonManager) {
                    roomInfoManager.join.onClick.AddListener(delegate { photonManager.JoinRoom(roomInfo.Name); });
                }
            }
        }
    }
}

