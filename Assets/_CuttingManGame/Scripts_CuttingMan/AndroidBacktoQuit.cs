using UnityEngine;

public class AndroidBacktoQuit : MonoBehaviour
{
    private void Start()
    {
        Input.backButtonLeavesApp = true;
    }
    //void Update()
    //{
    //    if (Application.platform == RuntimePlatform.Android)
    //    {

    //        if (Input.GetKeyDown(KeyCode.Escape))
    //        {

    //            Application.Quit();
    //        }
    //    }
    //}
}
