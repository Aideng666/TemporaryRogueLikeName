using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] GameObject content;
    [SerializeField] GameObject emptyHeartPrefab;
    [SerializeField] GameObject halfHeartPrefab;
    [SerializeField] GameObject fullHeartPrefab;

    PlayerController player;

    int previousHealth = 0;

    public static PlayerHealthUI Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.Instance;

        SetHearts();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetCurrentHealth() != previousHealth)
        {
            SetHearts();
        }

        previousHealth = player.GetCurrentHealth();
    }

    public void SetHearts()
    {
        foreach (Image image in content.GetComponentsInChildren<Image>())
        {
            Destroy(image.gameObject);
        }

        int currentHeartCheck = player.GetCurrentHealth();

        for (int i = 0; i < player.GetMaxHealth() / 2; i++)
        {
            if (currentHeartCheck >= 2)
            {
                Instantiate(fullHeartPrefab, content.transform);
                currentHeartCheck -= 2;
            }
            else if (currentHeartCheck == 1)
            {
                Instantiate(halfHeartPrefab, content.transform);
                currentHeartCheck--;
            }
            else if (currentHeartCheck == 0)
            {
                Instantiate(emptyHeartPrefab, content.transform);
            }
        }
    }
}
