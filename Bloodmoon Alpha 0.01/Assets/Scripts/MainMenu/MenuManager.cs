using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    private string scenesellected;
    private TMP_InputField input;

    public void StartGame()
    {
        if (scenesellected != null)
        {
            for (int i = 0; true; i++)
            {
                string scene = SceneUtility.GetScenePathByBuildIndex(i);
                Debug.Log(scene);
                Debug.Log(scenesellected);
                if (scene == "")
                {
                    break;
                }
                if (scene == "Assets/Scenes/"+scenesellected+".unity") 
                { 
                    SceneManager.LoadScene(i);
                    break;
                }
            }
        }
    }

    public void changeScene()
    {
        if (input == null)
        {
            input = GetComponentInChildren<TMP_InputField>();
        }
        scenesellected = input.text;
    }

    public void Exit()
    {
        Application.Quit();
    }
}
