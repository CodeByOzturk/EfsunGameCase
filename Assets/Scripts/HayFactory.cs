using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HayFactory : MonoBehaviour
{
    private SaveLoadManager saveLoadManager;
    private GameData gameData;
    private IFactoryModel _model;
    private GameManager _gameManager;

    [SerializeField] private TextMeshProUGUI wheatText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Slider progressBar;

    private float _lastClickTime = 0f;
    private const float DoubleClickThreshold = 0.3f;
    [SerializeField] private float productTime;
    [SerializeField] private int capacity;

    private void Awake()
    {
        _model = new IFactoryModel(capacity, productTime);
        _gameManager = FindObjectOfType<GameManager>();
        saveLoadManager = FindObjectOfType<SaveLoadManager>();
    }

    private void Start()
    {
        gameData = saveLoadManager.LoadGame();
        UpdateStoredProduct();

        _model.storedProduct.Subscribe(UpdateWheatUI).AddTo(this);
        _model.TimeLeft.Subscribe(UpdateTimeUI).AddTo(this);

        StartProductionLoop().Forget();
    }

    private async UniTaskVoid StartProductionLoop()
    {
        TimeController();

        while (true)
        {
            if (_model.IsFull.Value)
            {
                await UniTask.WaitUntil(() => !_model.IsFull.Value);
            }
            await _model.ProduceProduct();
        }
    }


    private void UpdateWheatUI(int wheatAmount)
    {
        PlayerPrefs.SetInt("productedWheat", wheatAmount);
        wheatText.text = $"{wheatAmount}";
    }

    private void UpdateTimeUI(float timeLeft)
    {
        timeText.text = timeLeft > 0 ? $"{timeLeft} sn" : "Full";
        progressBar.value = 1 - (timeLeft / 20f);
    }

    private void OnMouseDown()
    {
        if (gameObject.CompareTag("HayFactory"))
        {
            _gameManager?.CloseAllButton();
            if (Input.GetMouseButtonDown(0) && Time.time - _lastClickTime < DoubleClickThreshold)
            {
                int collectedWheat = _model.storedProduct.Value;

                if (collectedWheat > 0)
                {
                    _model.storedProduct.Value = 0;
                    _gameManager?.UpdateWheatDisplay(collectedWheat);
                }

            }
            _lastClickTime = Time.time;
        }


    }

    public void UpdateStoredProduct()
    {
        _model.storedProduct.Value = gameData.productedWheat;
    }
    private void TimeController()
    {
        string lastProductionTimeString = gameData.lastTime;

        if (string.IsNullOrEmpty(lastProductionTimeString) || !DateTime.TryParse(lastProductionTimeString, out DateTime lastProductionTime))
        {
            lastProductionTime = DateTime.Now;
        }

        DateTime currentProductionTime = DateTime.Now;
        TimeSpan timeSinceLastProduction = currentProductionTime - lastProductionTime;

        int producedAmount = (int)(timeSinceLastProduction.TotalSeconds / productTime);

        _model.storedProduct.Value = Mathf.Min(_model.storedProduct.Value + producedAmount, capacity);
    }
}
