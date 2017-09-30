using UnityEngine;
using System.Collections;
public abstract class Singleton<T> where T : new()
{
    private static T _instance;
    static object _lock = new object();
    public static T Ins
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new T();
                }
            }
            return _instance;
        }
    }
    public static T PeekIns
    {
        get
        {
            return _instance;
        }
    }
}

public class UnitySingleton<T> : MonoBehaviour
    where T : Component
{
    protected static T mInstance;
    public static T Ins
    {
        get
        {
            return mInstance;
        }
    }
    public static GameObject GenerateObject(string sName = "")
    {
        if (mInstance == null)
        {
            GameObject obj = new GameObject(sName);
            mInstance = (T)obj.AddComponent(typeof(T));
            DontDestroyOnLoad(obj);
            return obj;
        }

        return mInstance.gameObject;
    }
}
