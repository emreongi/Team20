using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkinTryoutCabinet : MonoBehaviour
{
    [SerializeField] private List<GameObject> playerAvatars = new List<GameObject>();
    [SerializeField] private List<GameObject> gunObjects = new List<GameObject>();
    [SerializeField] private GameObject forwardButton;
    [SerializeField] private GameObject backButton;
    private int playerRow = 0;
    private int playerAmount;
    private bool viewChangeActive = false;
    [SerializeField] private TMP_Text sellectButtonText;

    void Start()
    {
        playerAmount = playerAvatars.Count - 1;
        playerAvatars[playerRow].SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            BackPlayer();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ForwardPlayer();
        }
    }

    public void ViewChange()
    {
        viewChangeActive = !viewChangeActive;

        foreach (GameObject gunObject in gunObjects)
        {
            if (gunObject.name != "M416(Player)")
            {
                gunObject.SetActive(viewChangeActive);
            }
        }

        foreach (GameObject playerAvatar in playerAvatars)
        {
            playerAvatar.SetActive(false);
        }

        if (!viewChangeActive)
        {
            playerAvatars[playerRow].SetActive(true);
        }

        forwardButton.SetActive(!viewChangeActive);
        backButton.SetActive(!viewChangeActive);
    }

    public void SetMaterial(Material selectedMaterial)
    {
        foreach (GameObject gunObject in gunObjects)
        {
            Renderer renderer = gunObject.GetComponent<Renderer>();
            renderer.material = selectedMaterial;
        }
    }

    public void ForwardPlayer()
    {
        playerAvatars[playerRow].SetActive(false);

        if (playerRow < playerAmount)
        {
            playerRow++;
        }
        else
        {
            playerRow = 0;
        }

        playerAvatars[playerRow].SetActive(true);
    }

    public void BackPlayer()
    {
        playerAvatars[playerRow].SetActive(false);

        if (playerRow > 0)
        {
            playerRow--;
        }
        else
        {
            playerRow = playerAmount;
        }

        playerAvatars[playerRow].SetActive(true);
    }
}