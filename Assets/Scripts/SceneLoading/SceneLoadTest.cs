using MyGameDevTools.SceneLoading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneLoadTest : MonoBehaviour
{

    [SerializeField]
    private string toScene;



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MySceneManager.TransitionAsync(toScene, "LoadingScene");
        }
    }
}
