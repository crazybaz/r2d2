using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class Dice
{
    private static int _Roll(int max)
    {
        return Random.Range(0, max);
    }

    public static bool Roll(int bid, int max = 100)
    {
        return _Roll(max) < bid;
    }

    public static int Roll(List<int> bids)
    {
        var sum = 0;
        var res = _Roll(bids.Sum());

        foreach (var bid in bids)
        {
            sum += bid;
            if (res < sum)
                return bids.IndexOf(bid);
        }

        throw new Exception("Something went wrong, review class Dice");
    }

    public static DropItem Roll(List<DropItem> dropItems, int? max = null)
    {
        var sum = 0;
        var res = _Roll(max ?? dropItems.Sum(dropItem => (int)dropItem.Probability));

        foreach (var dropItem in dropItems)
        {
            sum += (int)dropItem.Probability;
            if (res < sum)
                return dropItem;
        }

        return null;
    }
}