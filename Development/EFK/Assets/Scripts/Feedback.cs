
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
        StartCoroutine(PostFeedback());
        InputField.text = "";
    } 
    
    IEnumerator PostFeedback() 
    {
        string URL =
            "https://docs.google.com/forms/d/e/1FAIpQLSdMqzcBly7D1ASCV4TellsgZFxfsuOvR08TONk9WG4xVQ6PzQ/formResponse";
        
        WWWForm form = new WWWForm();

        form.AddField("entry.548522869", "provaGame");
        form.AddField("entry.1313960569", "provaGame");
        form.AddField("entry.1656061589", "provaGame");

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