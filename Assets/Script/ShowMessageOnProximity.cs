using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowMessageOnProximity : MonoBehaviour
{
    public GameObject PanelMessage;
    public GameObject PanelQuestion;
    public QuizManager quizManager;

    private bool isPlayerNearby = false;

    // Menambahkan referensi ke scroll yang berinteraksi
    public int scrollID;  // ID untuk scroll spesifik

    void Start()
    {
        // Dapatkan komponen ShowMessageOnProximity
        ShowMessageOnProximity proximityScript = GetComponent<ShowMessageOnProximity>();
        if (proximityScript == null)
        {
            Debug.LogError("ShowMessageOnProximity script not found on this GameObject!");
            return;
        }

        proximityScript.PanelMessage = GameObject.Find("Canvas").transform.Find("Panel Message")?.gameObject;
        proximityScript.PanelQuestion = GameObject.Find("Canvas").transform.Find("Panel Question")?.gameObject;
        proximityScript.quizManager = FindObjectOfType<QuizManager>();

        if (proximityScript.PanelMessage != null) proximityScript.PanelMessage.SetActive(false);
        else Debug.LogError("PanelMessage not found!");

        if (proximityScript.PanelQuestion != null) proximityScript.PanelQuestion.SetActive(false);
        else Debug.LogError("PanelQuestion not found!");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PanelMessage.SetActive(true);
            isPlayerNearby = true;

            string[] nameParts = gameObject.name.Split('_');
            if (nameParts.Length > 1 && int.TryParse(nameParts[nameParts.Length - 1], out int parsedID))
            {
                scrollID = parsedID;
                Debug.Log($"Scroll ID ditemukan: {scrollID}");
            }
            else
            {
                Debug.LogError($"Nama GameObject tidak valid untuk ID: {gameObject.name}");
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PanelMessage.SetActive(false);
            isPlayerNearby = false;
        }
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.P))
        {
            OpenObject();
        }
    }

    void OpenObject()
    {
        PanelMessage.SetActive(false);

        quizManager.StartQuestion(scrollID);
        quizManager.isGameDone = true;
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Object opened or interacted with!");
    }
}
