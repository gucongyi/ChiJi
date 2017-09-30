using System.Collections;
using UnityEngine;

namespace CatsAndDogs {

    public sealed class SceneManager : MonoBehaviour {

        private static SceneManager instance;
        public static SceneManager Instance {
            get {
                return instance;
            }
        }

        public static void LoadScene(string name) {
            if (!instance) {
                Debug.Log("No SceneManager instance, LoadScene failed.");
                return;
            }
            instance.LoadSceneByName(name);
        }

        private void Awake() {
            instance = this;
        }

        private void LoadSceneByName(string name) {
            // TODO
            if (name == "Battle") {
                Debug.Log("Load Battle");
                PhotonNetwork.LoadLevel("Battle");
                return;
            }
            StartCoroutine(LoadSceneAsyncByName(name));
        }

        private IEnumerator LoadSceneAsyncByName(string name) {
            AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);
            while (!async.isDone) {
                yield return async;
            }
        }
    }
}

