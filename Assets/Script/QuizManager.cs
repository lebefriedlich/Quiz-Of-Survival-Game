using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizManager : MonoBehaviour
{
    public GameObject PanelQuestion;
    public GameObject HealthBar;
    public GameObject enemy;
    public GameObject enemy1;
    public TMP_Text questionText;
    public Button[] optionButtons;
    public Animator[] heartAnimators;
    private int remainingHearts;
    private string correctAnswer;
    public UIGameplayLogic UIGameplayLogic;
    private int currentScrollID;
    private int totalScrolls = 5;
    private int answeredCorrectly = 0;
    public bool isGameDone = false;
    public GameObject[] allScrolls;
    public PlayerLogic playerLogic;

    [Header("SFX")]
    public AudioClip correctSFX;
    public AudioClip wrongSFX;
    public AudioSource audioSource;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        enemy.SetActive(false);
        enemy1.SetActive(false);
        remainingHearts = heartAnimators.Length;

        if (heartAnimators != null)
        {
            foreach (Animator heartAnimator in heartAnimators)
            {
                heartAnimator.enabled = false;
            }
        }
    }

    void Update()
    {
        if (!isGameDone)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                UIGameplayLogic.PanelMenu();
            }
        }
    }

    public void StartQuestion(int scrollID)
    {
        Debug.Log("Starting question for scroll ID: " + scrollID);
        currentScrollID = scrollID;

        HealthBar.SetActive(false);
        PanelQuestion.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (heartAnimators != null)
        {
            foreach (Animator heartAnimator in heartAnimators)
            {
                heartAnimator.enabled = false;
            }
        }

        string question = "";
        string[] options = new string[4];
        string answer = "";

        if (scrollID == 0)
        {
            question = "Siapa nama nabi yang menerima wahyu melalui kitab Injil?";
            options = new string[] { "Musa", "Isa", "Muhammad", "Ibrahim" };
            answer = "Isa";
        }
        else if (scrollID == 1)
        {
            question = "Apa nama kota tempat Nabi Muhammad SAW dilahirkan?";
            options = new string[] { "Madinah", "Makkah", "Ta'if", "Badr" };
            answer = "Makkah";
        }
        else if (scrollID == 2)
        {
            question = "Apa nama kitab yang diturunkan kepada Nabi Muhammad SAW?";
            options = new string[] { "Taurat", "Injil", "Al-Qur'an", "Zabur" };
            answer = "Al-Qur'an";
        }
        else if (scrollID == 3)
        {
            question = "Apa nama perintah yang diwajibkan umat Islam untuk dilakukan lima kali sehari?";
            options = new string[] { "Shalat", "Zakat", "Puasa", "Haji" };
            answer = "Shalat";
        }
        else if (scrollID == 4)
        {
            question = "Apa nama tempat yang digunakan umat Islam untuk beribadah?";
            options = new string[] { "Gereja", "Masjid", "Kuil", "Vihara" };
            answer = "Masjid";
        }
        else
        {
            question = "Pertanyaan tidak ditemukan.";
            options = new string[] { "N/A", "N/A", "N/A", "N/A" };
            answer = "N/A";
        }

        SetQuestion(question, options, answer);
    }

    public void SetQuestion(string question, string[] options, string answer)
    {
        questionText.text = question;
        correctAnswer = answer;

        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i < options.Length)
            {
                optionButtons[i].gameObject.SetActive(true);
                optionButtons[i].GetComponentInChildren<TMP_Text>().text = options[i];
                string selectedAnswer = options[i];

                optionButtons[i].onClick.AddListener(() => CheckAnswer(selectedAnswer));
            }
            else
            {
                optionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void CheckAnswer(string selectedAnswer)
    {
        Time.timeScale = 1f;
        if (selectedAnswer == correctAnswer)
        {
            audioSource.PlayOneShot(correctSFX);
            Debug.Log("Jawaban benar!");

            GameObject scrollToRemove = GameObject.Find("Scroll_" + currentScrollID);
            if (scrollToRemove != null)
            {
                Destroy(scrollToRemove);
                answeredCorrectly++;
                UIGameplayLogic.ScrollCollected(answeredCorrectly, totalScrolls);
                EndQuestion();
            }

            if (answeredCorrectly == totalScrolls)
            {
                Debug.Log("Semua scroll telah dijawab dengan benar!");
                UIGameplayLogic.GameResult(true);
            }

        }
        else
        {
            audioSource.PlayOneShot(wrongSFX);
            Debug.Log("Jawaban salah!");

            if (remainingHearts > 0)
            {
                remainingHearts--;
                if (heartAnimators[remainingHearts] != null)
                {
                    heartAnimators[remainingHearts].enabled = true;
                }
            }

            Invoke("EndQuestion", 2f);

            if (remainingHearts <= 0)
            {
                UIGameplayLogic.GameResult(false);
            }

            GameObject scrollToRemove = GameObject.Find("Scroll_" + currentScrollID);
            allScrolls = GameObject.FindGameObjectsWithTag("Scroll");

            HideAllScrolls();
            if (scrollToRemove != null)
            {
                if (currentScrollID == 0 || currentScrollID == 2 || currentScrollID == 4)
                {
                    enemy.transform.position = scrollToRemove.transform.position;
                    playerLogic.target = enemy.transform;

                    EnemyLogic enemyLogic = enemy.GetComponent<EnemyLogic>();
                    if (enemyLogic != null)
                    {
                        enemyLogic.resetAnimator();
                    }
                }
                else if (currentScrollID == 1 || currentScrollID == 3)
                {
                    enemy1.transform.position = scrollToRemove.transform.position;
                    playerLogic.target = enemy1.transform;

                    EnemyLogic enemyLogic = enemy1.GetComponent<EnemyLogic>();
                    if (enemyLogic != null)
                    {
                        enemyLogic.resetAnimator();
                    }
                }
            }
        }
    }

    private void HideAllScrolls()
    {
        foreach (GameObject scroll in allScrolls)
        {
            scroll.SetActive(false);
        }
    }

    public void ShowAllScrolls()
    {
        foreach (GameObject scroll in allScrolls)
        {
            scroll.SetActive(true);
        }
    }

    private void EndQuestion()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PanelQuestion.SetActive(false);
        HealthBar.SetActive(true);
        isGameDone = false;

        foreach (Button button in optionButtons)
        {
            button.onClick.RemoveAllListeners();
        }
    }
}
