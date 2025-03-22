using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class FlourFactory : MonoBehaviour
{
    private SaveLoadManager saveLoadManager;
    private GameData gameData;
    private IFactoryModel _model;
    private GameManager _gameManager;

    [SerializeField] private TextMeshProUGUI flourBagText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI product_capacityText;
    [SerializeField] private Slider progressBar;

    private bool _isProducing = true;

    [SerializeField] private int capacity;
    [SerializeField] private int product;
    [SerializeField] private int producted;
    [SerializeField] private float productTime;

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

        producted = gameData.productedFlourBag;
        product = gameData.productFlourBag;

        TimeController();
        UpdateStoredProduct();

        _model.storedProduct.Subscribe(UpdateFlourBagUI).AddTo(this);
        _model.TimeLeft.Subscribe(UpdateTimeUI).AddTo(this);

        PlayerPrefs.SetInt("productFlourBag", product);

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
                _model.TimeLeft.Value = productTime;
                await _model.ProduceProduct();
                product--;
                PlayerPrefs.SetInt("productFlourBag", product);
            }
            else
            {
                _isProducing = false;
                UpdateButtonState();
            }

            await UniTask.Delay(100);
        }
    }


    private void UpdateFlourBagUI(int flourBagAmount)
    {
        PlayerPrefs.SetInt("productedFlourBag", flourBagAmount);
        flourBagText.text = $"{flourBagAmount}";
        UpdateButtonState();
    }

    private void UpdateTimeUI(float timeLeft)
    {
        if (product > 0)
        {
            timeText.text = $"{timeLeft} sn";
            progressBar.value = 1 - (timeLeft / 40f);

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
        button_1.interactable = _gameManager?.wheatCount > 0 && canProduce && (product + producted) < capacity;
        button_2.interactable = product > 0;
    }

    private void OnMouseDown()
    {
        if (gameObject.CompareTag("FlourFactory"))
        {
            if (Input.GetMouseButtonDown(0))
            {
                _gameManager?.CloseAllButton();
                ShowProductionButtons();

                if (Time.time - _lastClickTime < DoubleClickThreshold)
                {
                    CollectFlourBags();
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

    private void CollectFlourBags()
    {
        int collectedFlourBag = _model.storedProduct.Value;
        if (collectedFlourBag > 0)
        {
            _model.storedProduct.Value = 0;
            _gameManager?.UpdateFlourBagDisplay(collectedFlourBag);
            producted = 0;
            product_capacityText.text = $"{product + producted}/{capacity}";
            PlayerPrefs.SetInt("productedFlourBag", producted);
        }
    }

    public void AddProductionOrder()
    {
        if (_model.storedProduct.Value < capacity)
        {
            product++;
            PlayerPrefs.SetInt("productFlourBag", product);
            _gameManager?.UpdateWheatDisplay(-1);
            product_capacityText.text = $"{product + producted}/{capacity}";

            if (!_isProducing && product > 0)
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
            PlayerPrefs.SetInt("productFlourBag", product);
            product_capacityText.text = $"{product + producted}/{capacity}";
            _gameManager?.UpdateWheatDisplay(1);
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
        _model.storedProduct.Value = gameData.productedFlourBag;
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
            // Üretilen ürünleri "producted" kısmına ekle
            producted += producedAmount;
            product -= producedAmount;

            PlayerPrefs.SetInt("productFlourBag", product);
            PlayerPrefs.SetInt("productedFlourBag", producted);
            gameData.productedFlourBag = PlayerPrefs.GetInt("productedFlourBag");
            product_capacityText.text = $"{product + producted}/{capacity}";
        }
    }
}
