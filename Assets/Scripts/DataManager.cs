using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private SaveLoadManager saveLoadManager;
    private GameData gameData;
    private void Start()
    {
        if (PlayerPrefs.HasKey("lastTime"))
        {
            PlayerPrefs.SetFloat("lastTime", 0);
        }


        if (PlayerPrefs.HasKey("collectedWheat"))
        {
            PlayerPrefs.SetInt("collectedWheat", 0);
        }
        if (PlayerPrefs.HasKey("collectedFlourBag"))
        {
            PlayerPrefs.SetInt("collectedFlourBag", 0);
        }
        if (PlayerPrefs.HasKey("collectedBread"))
        {
            PlayerPrefs.SetInt("collectedBread", 0);
        }



        if (PlayerPrefs.HasKey("productedWheat"))
        {
            PlayerPrefs.SetInt("productedWheat", 0);
        }
        if (PlayerPrefs.HasKey("productedFlourBag"))
        {
            PlayerPrefs.SetInt("productedFlourBag", 0);
        }


        if (PlayerPrefs.HasKey("productFlourBag"))
        {
            PlayerPrefs.SetInt("productFlourBag", 0);
        }

        saveLoadManager = FindObjectOfType<SaveLoadManager>();
        gameData = saveLoadManager.LoadGame();

        /*
            gameData.lastTime = "0";

            gameData.collectedWheat = 0;
            gameData.collectedFlourBag = 0;
            gameData.collectedBread = 0;

            gameData.productedWheat = 0;
            gameData.productedFlourBag = 0;

            gameData.productFlourBag = 0;

            saveLoadManager.SaveGame(gameData);
        */
    }
}
