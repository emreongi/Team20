using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Cinemachine;

public class GameSceneUI : MonoBehaviour
{
    // Panel
    public GameObject resumePanel;
    public bool resumePanelOpen = false;

    // Ses
    [SerializeField] private AudioListener audioListener;
    public bool Muted = false;

    // Leave
    public GameObject leavePanel;
    public bool wannaleave = false;

    //Kamera Kitleme
    public CinemachineFreeLook playerCamera;

    //Infi panel
    public GameObject infoPanel;
    bool infoopen;

    public GameObject EndPanel;
    public GameObject leavePanelEnd;
    private void Start()
    {
        GameObject camerafinder = GameObject.Find("FreeLook Camera");
        playerCamera = camerafinder.GetComponent<CinemachineFreeLook>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !resumePanelOpen && !(wannaleave | infoopen))
        {
            playerCamera.enabled = false;
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            resumePanel.SetActive(true);
            resumePanelOpen = true;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && resumePanelOpen)
        {
            Resume();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
        }
    }

    public void Resume()
    {
        playerCamera.enabled = true;
        UnityEngine.Cursor.visible = false;
        resumePanel.SetActive(false);
        resumePanelOpen = false;
    }

    public void Audio()
    {
        /*
        if (!Muted)
        {
            audioListener.enabled = false;
        }
        else
        {
            audioListener.enabled = true;
        }

        Muted = !Muted;
        */
        
    }

    public void Leave()
    {
        if (!wannaleave)
        {
            leavePanel.SetActive(true);
            resumePanel.SetActive(false);
            wannaleave = true;
        }
        else if (wannaleave)
        {
            Application.Quit();
        }
    }

    public void LeaveEnd()
    {

        if (!wannaleave)
        {
            EndPanel.SetActive(false);
            leavePanelEnd.SetActive(true);
            wannaleave = true;
        }
        else if (wannaleave)
        {
            Application.Quit();
        }
    }
    public void RejectLeaveOnEnd()
    {
        wannaleave = false;
        leavePanelEnd.SetActive(false);
        EndPanel.SetActive(true);

    }
    public void RejectLeave()
    {
        wannaleave = false;
        resumePanelOpen = true;
        leavePanel.SetActive(false);
        resumePanel.SetActive(true);
    }
    public void CloseInfo()
    {
        infoPanel.SetActive(false);
        resumePanel.SetActive(true);
        infoopen =false;
    }
    public void OpenInfo()
    {
        infoPanel.SetActive(true);
        resumePanel.SetActive(false );
        infoopen =true;
    }
    public void Homescreen()
    {
        SceneManager.LoadScene(1);
    }
}
