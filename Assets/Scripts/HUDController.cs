using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class HUDController : MonoBehaviour
{
    public static HUDController instance { get; private set; }
    public Image healthMask;
    public TextMeshProUGUI points;
    public TextMeshProUGUI finalScore;
    public GameObject deathPanel;
    float healthOriginalSize;
    // Start is called before the first frame update
    void Awake ()
    {
        instance = this;
        healthOriginalSize = healthMask.rectTransform.rect.width;
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetHealth(float value)
    {
        healthMask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                                                     healthOriginalSize * value);
    }
    public void SetPoints(float value)
    {
        points.text = value.ToString();
        finalScore.text = value.ToString();
    }
    public void ShowDeathPanel(bool show)
    {
        deathPanel.SetActive(show);;
    }

    public void TryAgain()
    {
        GameController.instance.StartGame();
    }
    public void NewGame()
    {
        GameController.instance.StartRandomGame();
    }
    public void QuitGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);

    }
}
