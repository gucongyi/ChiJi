using UnityEngine;

namespace CatsAndDogs {
    public sealed class GameManager : MonoBehaviour {

        //private void Awake() {
        //    Application.targetFrameRate = 60;
        //    DontDestroyOnLoad(gameObject);
        //}

        //private void Start() {
        //    SceneManager.LoadScene("Login");
        //    style.normal.textColor = Color.black;
        //}

        private GUIStyle style = new GUIStyle();
        private float[] deltaTimes = new float[10];
        private int frameCounter = 0;
        private void OnGUI() {
            deltaTimes[frameCounter % 10] = Time.deltaTime;
            float tenFrameTime = 0f;
            for (int i = 0; i < 10; i++) {
                tenFrameTime += deltaTimes[i];
            }
            GUI.Label(new Rect(20, 20, 100, 30), (10f / tenFrameTime).ToString(), style);
            frameCounter++;
        }
    }
}

