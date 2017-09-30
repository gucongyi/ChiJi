using UnityEngine;
using UnityEngine.UI;

namespace CatsAndDogs {
    public class LoginUIManager : MonoBehaviour {

        public Button loginButton;

        private void Awake() {
            loginButton.onClick.AddListener(delegate { SceneManager.LoadScene("Lobby"); });
        }
    }
}

