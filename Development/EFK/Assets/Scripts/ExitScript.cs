using UnityEngine;

public class ExitScript:MonoBehaviour
{
    public void  ClickYes(){
        Application.Quit();
    }
    
    public void  ClickNo(){
        Destroy(this.gameObject);
    }
    
}