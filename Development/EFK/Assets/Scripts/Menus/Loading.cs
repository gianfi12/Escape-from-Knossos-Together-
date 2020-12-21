using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{

    [SerializeField] private Slider loadingBar;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadLevelAsync());
    }

    IEnumerator LoadLevelAsync()
    {
        AsyncOperation gameLevel = SceneManager.LoadSceneAsync("Main");
        while (!gameLevel.isDone)
        {
            loadingBar.value = gameLevel.progress;
            yield return null;
        }
    }
}
