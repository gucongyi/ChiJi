using UnityEngine;
using DG.Tweening;

public class DamagePop : MonoBehaviour {

    [SerializeField, NotEditableInInspector] private Vector3[] path;
    [SerializeField, NotEditableInInspector] private UnityEngine.UI.Text text;

    private void Reset() {
        text = GetComponent<UnityEngine.UI.Text>();
        path = new Vector3[2];
        path[0] = new Vector3(130f, -30f, 0f);
        path[1] = new Vector3(90f, 30f, 0f);
    }

    private void Start() {
        Destroy(gameObject, 1f);
    }

    void Update () {
        text.DOFade(0f ,1f);
        transform.DOLocalPath(path, 2f, PathType.CatmullRom);
	}
}
