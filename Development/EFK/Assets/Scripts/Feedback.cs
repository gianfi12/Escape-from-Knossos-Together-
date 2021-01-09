
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Feedback : MonoBehaviour
{
    [SerializeField] private InputField InputField;
    [SerializeField] private GameObject confirmation;

    public void SendFeedback()
    {
        string feedback = InputField.text;
        StartCoroutine(PostFeedback(feedback));
        InputField.text = "";
    } 
    
    IEnumerator PostFeedback(String feedback) 
    {
        string URL =
            "https://docs.google.com/forms/d/e/1FAIpQLSdMqzcBly7D1ASCV4TellsgZFxfsuOvR08TONk9WG4xVQ6PzQ/formResponse";
        
        WWWForm form = new WWWForm();

        form.AddField("entry.548522869", feedback);

        UnityWebRequest www = UnityWebRequest.Post(URL, form);

        yield return www.SendWebRequest();
        
        print(www.error);
        
        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
        }
        
        SpawnConfirmation();
    }

    public void SpawnConfirmation()
    {
        GameObject gameObject = Instantiate(confirmation);
        Destroy (gameObject,5);
    }

    public void Quit()
    {
        Application.Quit();
    }
}