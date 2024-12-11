using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class UIGameplayLogic : MonoBehaviour
{
    public Image HealthBar;
    public TMP_Text HealthText;
    public TMP_Text ScrollText;
    public GameObject PanelGameResult;
    public Text GameResultText;
    public static bool GameIsPaused = false;
    public GameObject PanelPause;
    public QuizManager QuizManager;
    public int currentScroll = 0;

    [Header("SFX")]
    public AudioMixer audioMixer;
    public AudioClip winSFX;
    public AudioClip loseSFX;
    public AudioClip PlayerDeathSFX;
    public AudioSource audioSource;
    public Slider musicSlider;

    void Start()
    {
        if (PlayerPrefs.HasKey("music"))
        {
            float music = PlayerPrefs.GetFloat("music");
            musicSlider.value = music;
            setMusic(music);
        }
    }

    public void updateHealthBar(float currentHealth, float maxHealth)
    {
        HealthBar.fillAmount = currentHealth / maxHealth;
        HealthText.text = currentHealth.ToString();

        if (currentHealth <= 0f)
        {
            audioSource.clip = PlayerDeathSFX;
            audioSource.Play();
            StartCoroutine(WaitPlayerDeath());
        }
    }

    private IEnumerator WaitPlayerDeath()
    {
        yield return new WaitForSeconds(1f);
        GameResult(false);
    }

    public void ScrollCollected(int currentScroll, int totalScroll)
    {
        this.currentScroll = currentScroll;
        ScrollText.text = currentScroll + " / " + totalScroll;
    }

    public void GameResult(bool isWin)
    {
        Time.timeScale = 0f;
        QuizManager.isGameDone = true;
        PanelGameResult.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Color brown = new Color(0.65f, 0.16f, 0.16f);

        if (isWin)
        {
            audioSource.clip = winSFX;
            audioSource.Play();
            GameResultText.color = brown;
            GameResultText.text = "You Win!";
        }
        else
        {
            audioSource.loop = false;
            audioSource.PlayOneShot(loseSFX);
            GameResultText.color = brown;
            GameResultText.text = "You Lose!";
        }
    }

    public void GameResultDecision(bool tryAgain)
    {
        if (tryAgain)
        {
            SceneManager.LoadScene("Gameplay");
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void PanelMenu()
    {
        if (GameIsPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Resume()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PanelPause.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PanelPause.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void TryAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Gameplay");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void setMusic(float music)
    {
        audioMixer.SetFloat("music", music);

        PlayerPrefs.SetFloat("music", music);
    }
}
