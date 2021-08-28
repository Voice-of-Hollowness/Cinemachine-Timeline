using UnityEngine;
using UnityEngine.SceneManagement;
public class OntoADeath : MonoBehaviour
{
   public void OntoADeathScreen()
   {
       Cursor.visible = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);

    }
}
