using UnityEngine;

public class ExitScript:MonoBehaviour
{
    public void  ReturnDesktop(){
        Application.Quit();
    }
    
    public void  ReturnGame(){
        Destroy(this.gameObject);
        Cursor.lockState = CursorLockMode.Locked;
    }
    
}