
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStoryScript:MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject textSkipToMenu;
    [SerializeField] private GameObject inputMenuManager;
    [SerializeField] private List<GameObject> texts;
    private bool _destroyStarted = false;
    private int _lastText = 0;
    private void Awake()
    {
        menu.SetActive(false);
        inputMenuManager.SetActive(false);
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
        textSkipToMenu.GetComponent<Animator>().Play("ToFadeInText");
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if(!_destroyStarted)
            {
                _destroyStarted = true;
                textSkipToMenu.GetComponent<Animator>().Play("ToFadeOutText");
                foreach (GameObject gameObject in texts)
                {
                    gameObject.GetComponent<Animator>().Play("ToFadeOutText");
                }

                StartCoroutine("destroyOnFinishedAnimation");

            }
        }
    }

    IEnumerator destroyOnFinishedAnimation()
    {
        yield return null;
        while (checkIfAnimationFinished()) yield return new WaitForSeconds(1f);
        Destroy(gameObject);
        menu.SetActive(true);
        inputMenuManager.SetActive(true);
    }

    bool checkIfAnimationFinished()
    {
        foreach (GameObject gameObject in texts)
        {
            if (gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
                return true;
        }

        return false;
    }
}
