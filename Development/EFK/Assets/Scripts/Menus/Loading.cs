using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{

    [SerializeField] private Text loadingPoints;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadLevelAsync());
    }

    IEnumerator LoadLevelAsync()
    {
        while (true)
        {
                UpdateLoadingPoints();
                yield return new WaitForSeconds(1);
        }
    }

    private void UpdateLoadingPoints()
    {
        if (loadingPoints.text.Equals("...")) loadingPoints.text = ".";
        else loadingPoints.text = loadingPoints.text + ".";
    }
}
