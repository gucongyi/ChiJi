using UnityEngine;

public static class EngineExtension {

    #region GetOrAddComponent
    public static Component GetOrAddComponent(this Component self, System.Type type) {
        Component comp = self.GetComponent(type);
        if (comp == null) {
            comp = self.gameObject.AddComponent(type);
        }
        return comp;
    }
    public static Component GetOrAddComponent(this GameObject self, System.Type type) {
        Component comp = self.GetComponent(type);
        if (comp == null) {
            comp = self.gameObject.AddComponent(type);
        }
        return comp;
    }
    public static T GetOrAddComponent<T>(this Component self) where T : Component {
        T comp = self.GetComponent<T>();
        if (comp == null) {
            comp = self.gameObject.AddComponent<T>();
        }
        return comp;
    }
    public static T GetOrAddComponent<T>(this GameObject self) where T : Component {
        T comp = self.GetComponent<T>();
        if (comp == null) {
            comp = self.AddComponent<T>();
        }
        return comp;
    }
    #endregion
}
