using UnityEngine;
using ExitGames.Client.Photon;

namespace CatsAndDogs {
    public class PhotonManager : Photon.PunBehaviour {

        private static PhotonManager instance;
        public static PhotonManager Instance {
            get {
                return instance;
            }
        }

        private void Awake() {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                if (instance != this)
                    Destroy(gameObject);
            }
        }

        public void ConnectLobby() {
            PhotonNetwork.autoJoinLobby = true;
            // PhotonNetwork.automaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings("v4.2");
        }

        public void JoinOrCreateRoom(string roomName) {
            if (roomName == "") {
                roomName = "MyMatch";
            }
            RoomOptions roomOptions = new RoomOptions {
                IsVisible = true,
                IsOpen = true,
                MaxPlayers = 20
            };
            PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
        }

        public void JoinRoom(string name) {
            PhotonNetwork.JoinRoom(name);
        }

        public override void OnConnectedToMaster() {
            Debug.Log("OnConnectedToMaster");
        }

        public override void OnJoinedLobby() {
            Debug.Log("OnJoinedLobby");
        }

        public override void OnCreatedRoom() {
            Debug.Log("OnCreatedRoom");
        }

        public override void OnPhotonCreateRoomFailed(object[] codeAndMsg) {
            Debug.Log("OnPhotonCreateRoomFailed");
            Debug.Log(codeAndMsg[0].ToString());
            Debug.Log(codeAndMsg[1].ToString());
        }

        public override void OnJoinedRoom() {
            Debug.Log("OnJoinedRoom");
            //SceneManager.LoadScene("Battle");
            PhotonNetwork.LoadLevel("Battle");
        }

        public override void OnPhotonJoinRoomFailed(object[] codeAndMsg) {
            Debug.Log("OnPhotonJoinRoomFailed");
            Debug.Log(codeAndMsg[0].ToString());
            Debug.Log(codeAndMsg[1].ToString());
        }

        public override void OnLeftRoom() {
            // SceneManager.LoadScene("Lobby");
        }
    }
}
