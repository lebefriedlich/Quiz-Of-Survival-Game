using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class mainscript : MonoBehaviour
{
    public GameObject menupanel;
    public GameObject infopanel;
    public GameObject optionpanel;
    public GameObject creditPanel;


    // Start is called before the first frame update
    void Start()
    {
        menupanel.SetActive(true);
        infopanel.SetActive(false);
        optionpanel.SetActive(false);
        creditPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartButton(string scenename)
    {
        SceneManager.LoadScene(scenename);
    }

    public void InfoButton()
    {
        menupanel.SetActive(false);
        infopanel.SetActive(true);
        optionpanel.SetActive(false);
        creditPanel.SetActive(false);
    }

    public void BackButton()
    {
        menupanel.SetActive(true);
        infopanel.SetActive(false);
        optionpanel.SetActive(false);
        creditPanel.SetActive(false);
    }

    public void OptionButton()
    {
        menupanel.SetActive(false);
        infopanel.SetActive(false);
        optionpanel.SetActive(true);
        creditPanel.SetActive(false);
    }

    public void CreditButton()
    {
        menupanel.SetActive(false);
        infopanel.SetActive(false);
        optionpanel.SetActive(false);
        creditPanel.SetActive(true);
    }

    public void QuitButton()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
