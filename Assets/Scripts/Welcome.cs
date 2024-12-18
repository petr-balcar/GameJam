using UnityEngine;
using UnityEngine.SceneManagement;

public class Welcome : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("Main");
    }
}
