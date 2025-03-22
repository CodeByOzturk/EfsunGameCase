using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public string lastTime;
    
    public int collectedWheat;
    public int collectedFlourBag;
    public int collectedBread;

    public int productedWheat;
    public int productedFlourBag;

    public int productFlourBag;

    public Dictionary<int, int> productedBread = new Dictionary<int, int>();
    public Dictionary<int, int> productBread = new Dictionary<int, int>();
}
