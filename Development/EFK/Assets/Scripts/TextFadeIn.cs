using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextFadeIn : MonoBehaviour
{
    [SerializeField] private GameObject returnButton;

    private void Start() {
        StartCoroutine(FadeTextToFullAlpha(2f, GetComponent<Text>()));
    }

    private void Update() {
        if (!returnButton.active && GetComponent<Text>().color.a >= 0.99) returnButton.SetActive(true);
    }

    public IEnumerator FadeTextToFullAlpha(float t, Text i) {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f) {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }

    public IEnumerator FadeTextToZeroAlpha(float t, Text i) {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f) {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }
}
