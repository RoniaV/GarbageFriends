using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeleteAllPlayerPrefMenu : MonoBehaviour
{
    private bool reloadingScene = false;

#if UNITY_EDITOR
    [MenuItem("MyUtils/Delete all PlayerPrefs")]
    static void DoSomething()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("PlayerPrefs deleted");
    }
#endif

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.F1) && !reloadingScene)
        {
            reloadingScene = true;
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
