using UnityEngine;
using UnityEngine.SceneManagement;
public class OntoGame : MonoBehaviour
{
    public void OnClick()
    {
        Debug.Log("weClick");
        SceneManager.LoadScene(1);
        
    }
    public void OnClick2()
    {
        Debug.Log("weClick");
        SceneManager.LoadScene(0);

    }
}