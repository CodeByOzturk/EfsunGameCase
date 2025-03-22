using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BreadFactory : MonoBehaviour
{
    private SaveLoadManager saveLoadManager;
    private GameData gameData;
    private IFactoryModel _model;
    private GameManager _gameManager;

    [SerializeField] private TextMeshProUGUI breadText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI product_capacityText;
    [SerializeField] private Slider progressBar;

    private bool _isProducing = true;

    [SerializeField] private int capacity;
    [SerializeField] private int product;
    [SerializeField] private int producted;
    [SerializeField] private float productTime;
    [SerializeField] private int requiredFlourPerBread;
    [SerializeField] private int ID;

    [SerializeField] private Button button_1;
    [SerializeField] private Button button_2;

    private float _lastClickTime = 0f;
    private const float DoubleClickThreshold = 0.3f;

    private void Awake()
    {
        _model = new IFactoryModel(capacity, productTime);
        _gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        saveLoadManager = FindObjectOfType<SaveLoadManager>();
        gameData = saveLoadManager.LoadGame();

        if (_model == null) return;

        producted = PlayerPrefs.GetInt("productedBread" + ID);
        product = PlayerPrefs.GetInt("productBread" + ID);

        TimeController();
        UpdateStoredProduct();

        _model.storedProduct.Subscribe(UpdateBreadUI).AddTo(this);
        _model.TimeLeft.Subscribe(UpdateTimeUI).AddTo(this);


        product_capacityText.text = $"{product + producted}/{capacity}";
        StartProductionLoop().Forget();

        button_1.onClick.AddListener(AddProductionOrder);
        button_2.onClick.AddListener(CancelProductionOrder);
    }

    private async UniTaskVoid StartProductionLoop()
    {
        while (_isProducing)
        {
            if (_model.IsFull.Value)
            {
                await UniTask.WaitUntil(() => !_model.IsFull.Value);
            }

            if (product > 0)
            {
                await _model.ProduceProduct();
                product--;
                PlayerPrefs.SetInt("productBread" + ID, product);
            }
            else
            {
                _isProducing = false;
                UpdateButtonState();
            }

            await UniTask.Delay(100);
        }
    }

    private void UpdateBreadUI(int breadAmount)
    {
        PlayerPrefs.SetInt("productedBread" + ID, breadAmount);
        breadText.text = $"{breadAmount}";
        UpdateButtonState();
    }

    private void UpdateTimeUI(float timeLeft)
    {
        if (product > 0)
        {
            timeText.text = $"{timeLeft} sn";
            progressBar.value = 1 - (timeLeft / productTime);

            if (timeLeft == 0)
            {
                timeText.text = "Idle";
                progressBar.value = 0;
            }
        }
        else
        {
            timeText.text = "Idle";
            progressBar.value = 0;
        }
        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        bool canProduce = _model.storedProduct.Value < capacity;
        button_1.interactable = _gameManager?.flourBagCount >= requiredFlourPerBread && canProduce && (product + producted) < capacity;
        button_2.interactable = product > 0;
    }

    private void OnMouseDown()
    {
        if (gameObject.CompareTag("BreadFactory"))
        {
            if (Input.GetMouseButtonDown(0))
            {
                _gameManager?.CloseAllButton();
                ShowProductionButtons();

                if (Time.time - _lastClickTime < DoubleClickThreshold)
                {
                    CollectBreads();
                }
                _lastClickTime = Time.time;
            }
        }
    }

    private void ShowProductionButtons()
    {
        button_1.gameObject.SetActive(true);
        button_2.gameObject.SetActive(true);
        UpdateButtonState();
    }

    private void CollectBreads()
    {
        int collectedBread = _model.storedProduct.Value;
        if (collectedBread > 0)
        {
            _model.storedProduct.Value = 0;
            _gameManager?.UpdateBreadDisplay(collectedBread);
            producted = 0;
            product_capacityText.text = $"{product + producted}/{capacity}";
            PlayerPrefs.SetInt("productedBread" + ID, producted);
        }
    }

    public void AddProductionOrder()
    {
        if (_model.storedProduct.Value < capacity)
        {
            product++;
            PlayerPrefs.SetInt("productBread" + ID, product);
            _gameManager?.UpdateFlourBagDisplay(-requiredFlourPerBread);
            product_capacityText.text = $"{product + producted}/{capacity}";

            if (!_isProducing)
            {
                _isProducing = true;
                StartProductionLoop().Forget();
            }

            UpdateButtonState();
        }
    }

    public void CancelProductionOrder()
    {
        if (product > 0)
        {
            product--;
            PlayerPrefs.SetInt("productBread" + ID, product);
            product_capacityText.text = $"{product + producted}/{capacity}";
            _gameManager?.UpdateFlourBagDisplay(requiredFlourPerBread);
            UpdateButtonState();

            if (product == 0)
            {
                timeText.text = "Idle";
                progressBar.value = 0;
                _isProducing = false;
            }
        }
    }

    public void UpdateStoredProduct()
    {
        producted = PlayerPrefs.GetInt("productedBread" + ID);
        _model.storedProduct.Value = producted;
        product_capacityText.text = $"{product + producted}/{capacity}";
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
        double totalSeconds = timeSinceLastProduction.TotalSeconds;

        int producedAmount = Mathf.Min((int)(totalSeconds / productTime), product);

        if (producedAmount > 0)
        {
            producted += producedAmount;
            product -= producedAmount;

            PlayerPrefs.SetInt("productBread" + ID, product);
            PlayerPrefs.SetInt("productedBread" + ID, producted);

            product_capacityText.text = $"{product + producted}/{capacity}";
        }
    }
}
