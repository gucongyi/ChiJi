using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

public class ResourceManager : UnitySingleton<ResourceManager>
{
    public static Object LoadAsset(string FilePath,string fileName,Type type=null)
    {
        string fullName=Path.Combine(FilePath, fileName);
        Object obj = null;
        if (type!=null)
        {
            obj=Resources.Load(fullName, type);
        }
        else
        {
            obj = Resources.Load(fullName);
        }
        return obj;
    }

    public static Object LoadAssetGo(string FilePath, string fileName,Type type=null)
    {
        Object initGo =null;
        Object obj=ResourceManager.LoadAsset(FilePath,fileName, type);
        if (obj!=null)
        {
            initGo = Object.Instantiate(obj);
        }
        return initGo;
    }

    public Dictionary<string, GameObject> dicPrefabGo = new Dictionary<string, GameObject>();

    public void AddDicPrefabGo(string prefabName, GameObject prefabGo)
    {
        if (!dicPrefabGo.ContainsKey(prefabName))
        {
            dicPrefabGo.Add(prefabName, prefabGo);
        }
    }
    public void RemoveDicPrefabGo(string prefabName)
    {
        if (dicPrefabGo.ContainsKey(prefabName))
        {
            GameObject go = null;
            if (dicPrefabGo.TryGetValue(prefabName, out go))
            {
                dicPrefabGo.Remove(prefabName);
                Object.Destroy(go);
            }
        }
    }

    public static void ResetDics()
    {
        loadList.Clear();
        ResourceCacheDic.Clear();
    }

    public static void LoadResourceAsync(string prefabName, System.Action<float> progress = null, System.Action<GameObject> completed = null,string path= "UI")
    {
        if (loadList.Contains(prefabName))
        {
//表示正在加载
            return;
        }

        loadList.Add(prefabName);

        if (ResourceCacheDic.ContainsKey(prefabName))
        {
            GameObject initGo = null;
            if (ResourceCacheDic.TryGetValue(prefabName, out initGo))
            {
                if (initGo!=null)
                {
                    Debug.Assert(initGo != null);
                    if (completed != null)
                    {
                        completed(initGo);
                        return;
                    }
                }
            }
        }

        string fullPath = Path.Combine(path, prefabName);
        AppController.mInstance.StartCoroutine(LoadResourceCorotine(prefabName,fullPath, progress, completed));

    }

    private static Dictionary<string,GameObject> ResourceCacheDic=new Dictionary<string, GameObject>();
    private static List<string> loadList = new List<string>();

    static IEnumerator LoadResourceCorotine(string prefabName,string fullPath, System.Action<float> progress = null, System.Action<GameObject> completed = null)
    {
        ResourceRequest request = Resources.LoadAsync(fullPath);
        float displayProgress = 0.05f;
        while (!request.isDone)
        {
            displayProgress = displayProgress < request.progress ? request.progress : displayProgress;
            if (progress != null)
            {
                progress(displayProgress);
            }
            yield return new WaitForEndOfFrame();
        }
        if (loadList.Contains(prefabName))
        {//加载完成出队
            loadList.Remove(prefabName);
        }
        if (completed != null)
        {
            Debug.Assert(request.asset != null);
            GameObject initGo = Object.Instantiate(request.asset) as GameObject;
            if (!ResourceCacheDic.ContainsKey(prefabName))
            {
                ResourceCacheDic.Add(prefabName, initGo);
                completed(initGo);
            }
            else
            {
                GameObject cacheGo = null;
                if (ResourceCacheDic.TryGetValue(prefabName, out cacheGo))
                {
                    if (cacheGo != null)
                    {
                        completed(cacheGo);
                    }
                    else
                    {
                        completed(initGo);
                        ResourceCacheDic.Remove(prefabName);
                        if (!ResourceCacheDic.ContainsKey(prefabName))
                            ResourceCacheDic.Add(prefabName, initGo);
                    }
                }
            }
            
        }
        
    }
}
