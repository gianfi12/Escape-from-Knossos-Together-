
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStoryScript:MonoBehaviour
{
    [SerializeField] private GameObject textSkipToMenu;
    [SerializeField] private List<GameObject> texts;
    private bool _destroyStarted = false;
    private int _lastText = 0;
    private void Awake()
    {
        StartCoroutine("pressGoToMainMenu");
        StartCoroutine("displayTextSequence");
    }

    IEnumerator displayTextSequence()
    {
        yield return null;
        while (!_destroyStarted && _lastText<texts.Count)
        {
            yield return new WaitForSeconds(2f);
            texts[_lastText].GetComponent<Animator>().Play("ToFadeInText");
            _lastText++;
        }
    }

    IEnumerator pressGoToMainMenu()
    {
        yield return new WaitForSeconds(5f);
        if(!_destroyStarted) textSkipToMenu.GetComponent<Animator>().Play("ToFadeInText");
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if(!_destroyStarted)
            {
                _destroyStarted = true;
                if(textSkipToMenu.GetComponent<Text>().color.a!=0) textSkipToMenu.GetComponent<Animator>().Play("ToFadeOutText");
                foreach (GameObject gameObject in texts)
                {
                    if(gameObject.GetComponent<Text>().color.a!=0) 
                        gameObject.GetComponent<Animator>().Play("ToFadeOutText");
                }

                StartCoroutine("destroyOnFinishedAnimation");

            }
        }
    }

    IEnumerator destroyOnFinishedAnimation()
    {
        yield return null;
        while (checkIfAnimationNotFinished()) yield return new WaitForSeconds(1f);
        Destroy(gameObject);
        SceneManager.LoadScene("MainMenu");
    }

    bool checkIfAnimationNotFinished()
    {
        if (textSkipToMenu.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) return true;
        foreach (GameObject gameObject in texts)
        {
            if (gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
                return true;
        }

        return false;
    }
}
