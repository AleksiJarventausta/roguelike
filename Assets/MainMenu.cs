using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public UnityEngine.UI.InputField seedText;
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void Start()
    {
       int seed = Random.Range(0, 10000000);
       GlobalSettings.seed = seed;
       seedText.text = seed.ToString();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetSeed(string seed)
    {
        int result;
        if(int.TryParse(seedText.text, out result))
            GlobalSettings.seed = result;

    }
}
