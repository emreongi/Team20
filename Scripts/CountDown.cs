using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class CountDown : MonoBehaviour
{
    private float timeRemaining = 0;
    public bool timerIsRunning = true;
    public TMP_Text timeText;
    private SceneTransitionManager sceneTransitionManager;
    [SerializeField] private string sceneName;
    [SerializeField] private PlayableDirector playableDirector;

    private bool hasTriggeredTransition = false;

    void Start()
    {
        DisplayTime(timeRemaining);
        sceneTransitionManager = GameObject.Find("GameManager").GetComponent<SceneTransitionManager>();
    }

    void Update()
    {
        if (timerIsRunning)
        {
            timeRemaining += Time.deltaTime;
            DisplayTime(timeRemaining);

            if (timeRemaining >= 300 && !hasTriggeredTransition)
            {
                playableDirector.Play();
                Invoke("SceneTransition", 10);
            }
        }
    }

    void SceneTransition()
    {
        hasTriggeredTransition = true;
        sceneTransitionManager.MakeSceneTransition(sceneName);
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}