using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI wheatCollectedText;
    [SerializeField] private TextMeshProUGUI flourBagCollectedText;
    [SerializeField] private TextMeshProUGUI breadCollectedText;
    public int wheatCount;
    public int flourBagCount;
    private int breadCount;

    [SerializeField] private Button[] buttons;

    private SaveLoadManager saveLoadManager;
    private GameData gameData;

    private void Start()
    {
        saveLoadManager = FindObjectOfType<SaveLoadManager>();
        gameData = saveLoadManager.LoadGame();

        var wheat = gameData.collectedWheat;
        var flourBag = gameData.collectedFlourBag;
        var bread = gameData.collectedBread;

        wheatCollectedText.text = wheat.ToString();
        flourBagCollectedText.text = flourBag.ToString();
        breadCollectedText.text = bread.ToString();

        wheatCount = wheat;
        flourBagCount = flourBag;
        breadCount = bread;

        PlayerPrefs.SetInt("collectedWheat", wheatCount);
        PlayerPrefs.SetInt("collectedFlourBag", flourBagCount);
        PlayerPrefs.SetInt("collectedBread", breadCount);
        PlayerPrefs.SetString("lastTime", gameData.lastTime);
    }

    public void UpdateWheatDisplay(int wheatAmount)
    {
        wheatCount += wheatAmount;
        wheatCollectedText.text = $"{wheatCount}";

        PlayerPrefs.SetInt("collectedWheat", wheatCount);
    }

    public void UpdateFlourBagDisplay(int flourBagAmount)
    {
        flourBagCount += flourBagAmount;
        flourBagCollectedText.text = $"{flourBagCount}";

        PlayerPrefs.SetInt("collectedFlourBag", flourBagCount);
    }

    public void UpdateBreadDisplay(int breadAmount)
    {
        breadCount += breadAmount;
        breadCollectedText.text = $"{breadCount}";

        PlayerPrefs.SetInt("collectedBread", PlayerPrefs.GetInt("collectedBread") + breadCount);
    }

    public int GetFlourBagCount()
    {
        return flourBagCount;
    }

    public void ConsumeFlourBags(int amount)
    {
        flourBagCount = Mathf.Max(0, flourBagCount - amount);
        flourBagCollectedText.text = $"{flourBagCount}";
    }

    public int GetBreadCount()
    {
        return breadCount;
    }

    public void ConsumeBreads(int amount)
    {
        breadCount = Mathf.Max(0, breadCount - amount);
        breadCollectedText.text = $"{breadCount}";
    }

    public void CloseAllButton()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].gameObject.SetActive(false);
        }
    }

    private void OnApplicationQuit()
    {
        gameData.productedWheat = PlayerPrefs.GetInt("productedWheat");
        gameData.collectedWheat = PlayerPrefs.GetInt("collectedWheat");

        gameData.productFlourBag = PlayerPrefs.GetInt("productFlourBag");
        gameData.productedFlourBag = PlayerPrefs.GetInt("productedFlourBag");
        gameData.collectedFlourBag = PlayerPrefs.GetInt("collectedFlourBag");

        gameData.collectedBread = PlayerPrefs.GetInt("collectedBread");

        string currentTime = DateTime.Now.ToString("o");
        PlayerPrefs.SetString("lastTime", currentTime);
        gameData.lastTime = PlayerPrefs.GetString("lastTime");

        PlayerPrefs.Save();
        saveLoadManager.SaveGame(gameData);

    }

}
