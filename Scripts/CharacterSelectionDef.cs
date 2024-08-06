using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionDef : MonoBehaviour
{
    [SerializeField] private List<GameObject> playerPrefabs = new List<GameObject>();
    public List<GameObject> playerAvatars = new List<GameObject>();
    private int selectedAvatarIndex = 0;
    private int avatarCount;

    void Start()
    {
        avatarCount = playerPrefabs.Count;
        SetAvatar(0);
        CreateAvatars();

        if (playerAvatars.Count > 0 && selectedAvatarIndex < playerAvatars.Count)
        {
            playerAvatars[selectedAvatarIndex].SetActive(true);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            AvatarSelectBack();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            AvatarSelectForward();
        }
    }

    private void CreateAvatars()
    {
        Quaternion rotation = Quaternion.Euler(0, 180, 0);
        foreach (GameObject playerPrefab in playerPrefabs)
        {
            GameObject avatar = Instantiate(playerPrefab, transform.position, rotation);
            avatar.transform.SetParent(transform);
            avatar.SetActive(false);
            playerAvatars.Add(avatar);
        }
    }

    public void AvatarSelectForward()
    {
        if (playerAvatars.Count == 0) return;
        playerAvatars[selectedAvatarIndex].SetActive(false);
        selectedAvatarIndex = (selectedAvatarIndex + 1) % avatarCount;
        SetAvatar(selectedAvatarIndex);
        playerAvatars[selectedAvatarIndex].SetActive(true);
    }

    public void AvatarSelectBack()
    {
        if (playerAvatars.Count == 0) return;
        playerAvatars[selectedAvatarIndex].SetActive(false);
        selectedAvatarIndex = (selectedAvatarIndex - 1 + avatarCount) % avatarCount;
        SetAvatar(selectedAvatarIndex);
        playerAvatars[selectedAvatarIndex].SetActive(true);
    }

    private void SetAvatar(int index)
    {
        selectedAvatarIndex = index;
        // Burada gerekli ayarlamaları yapabilirsiniz.
    }

    public void UpdateAvatarSelection(int avatarIndex)
    {
        for (int i = 0; i < playerAvatars.Count; i++)
        {
            playerAvatars[i].SetActive(i == avatarIndex);
        }
    }
}
