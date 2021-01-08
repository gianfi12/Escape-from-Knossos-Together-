using UnityEngine;

public class ExitScript:MonoBehaviour
{
    public void  ReturnDesktop(){
        Application.Quit();
    }
    
    public void  ReturnGame(){
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Destroy(this.gameObject);
    }
    
}