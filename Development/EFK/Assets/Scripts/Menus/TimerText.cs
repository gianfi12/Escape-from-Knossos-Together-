using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerText : MonoBehaviour
{
    private int time=0;
    private bool timeTrigger;
    private Text timerText;

    [SerializeField] private Color safeColor;
    [SerializeField] private Color triggerColor;
    [SerializeField] private PlayerControllerMap playerController;

    // Start is called before the first frame update
    void Start()
    {
        timerText = GetComponent<Text>();
        StartCoroutine("DecrementTimeEverySecond");
    }

    // Update is called once per frame
    void Update()
    {
        timerText.text = time.ToString();
        if (time <= 0 && timeTrigger)
        {
            timeTrigger = false;
            playerController.Explode();
        }   
            
    }

    public void SetTime(int time, bool trigger=true) {
        this.time = time;
        timeTrigger = trigger;

        if (timerText != null) {
            timerText.text = time.ToString();
            timerText.color = trigger ? triggerColor : safeColor;
        }
    }

    IEnumerator DecrementTimeEverySecond() {
        while (true) {
            yield return new WaitForSeconds(1);
            if (time > 0) time--;
            else time = 0;
        }
    }

    public void HalveTime() {
        time = time / 2;
    }

    // call increment time with negative value to decrement it
    public void IncrementTime(int value) {
        if (time <= 0) SetTime(value, true);
        else time += value;
    }
}
