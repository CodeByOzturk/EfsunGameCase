using System;
using Cysharp.Threading.Tasks;
using UniRx;

public class IFactoryModel
{
    public readonly IntReactiveProperty storedProduct = new IntReactiveProperty(0);
    public readonly ReactiveProperty<float> TimeLeft = new ReactiveProperty<float>(0);
    public IReadOnlyReactiveProperty<bool> IsFull;
    private readonly int _capacity;
    private readonly float _productionTime;

    public IFactoryModel(int capacity, float productionTime)
    {
        _capacity = capacity;
        _productionTime = productionTime;
        IsFull = storedProduct.Select(product => product >= _capacity).ToReactiveProperty();
    }

    public async UniTask ProduceProduct()
    {
        TimeLeft.Value = _productionTime;
        while (TimeLeft.Value > 0)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            TimeLeft.Value--;
        }

        if (storedProduct.Value < _capacity)
        {
            storedProduct.Value++;
        }
    }
}
