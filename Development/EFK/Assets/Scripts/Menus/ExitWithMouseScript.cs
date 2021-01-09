using UnityEngine;

public class ExitWithMouseScript:MonoBehaviour
{
    public void  ReturnDesktop(){
        Application.Quit();
    }
    
    public void  ReturnGame(){
        Destroy(this.gameObject);
    }
    
}