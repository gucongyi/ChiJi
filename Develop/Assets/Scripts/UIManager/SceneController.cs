using UnityEngine;
using System.Collections;
public class SceneController : UnitySingleton<SceneController>
{
    public const float BattleScenePercent = 0.6f;
    public void LoadSceneAynsc(string sSceneName, System.Action<float> progress = null, System.Action completed = null)
    {
        StartCoroutine(LoadSceneCorotine(sSceneName, progress, completed));
    }

    IEnumerator LoadSceneCorotine(string sSceneName, System.Action<float> progress = null, System.Action completed = null)
    {
        
        float displayProgress = 0;
        if ("Battle" == sSceneName)
        {
            displayProgress = BattleScenePercent;
        }
        AsyncOperation ao = Application.LoadLevelAsync(sSceneName);
        while (!ao.isDone)
        {
            displayProgress += 0.01f;
            if ("Battle" == sSceneName)
            {
                float aoProgress = ao.progress * (1 - BattleScenePercent) + BattleScenePercent;
                displayProgress = displayProgress < aoProgress ? displayProgress : aoProgress;
            }
            else
            {
                displayProgress = displayProgress < ao.progress ? displayProgress : ao.progress;
            }
            if ("Battle" == sSceneName)
            {
                if (ao.progress>0.8f)
                {
                    displayProgress = 1f;
                }
                if (progress != null)
                {
                    progress(displayProgress);
                }
                yield return new WaitForEndOfFrame();
            }else if (progress != null)
            {
                progress(displayProgress);
            }
            
            yield return new WaitForEndOfFrame();
        }
        if ("Battle" == sSceneName)
        {
            if (completed != null)
            {
                completed();
            }
        }
        else
        {
            while (displayProgress < 1f)
            {
                displayProgress += 0.01f;

                if (progress != null)
                {
                    progress(displayProgress);
                }
                yield return new WaitForEndOfFrame();
            }
            if (completed != null)
            {
                completed();
            }
        }
        
    }
    

}
