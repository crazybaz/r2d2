using System;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static T GetParam<T>(Dictionary<string, object> vo, string param)
    {
        object option;

        if (!vo.TryGetValue(param, out option))
            throw new ArgumentException("Dictionary does not contain option: " + option);

        // EXCEPTIONS
        if (option.GetType() != typeof(T))
        {
            Debug.LogWarning("PARAM TYPE AND OPTION TYPE ARE DIFFERENT: " + typeof(T) + ", " + option.GetType());

            // CORRECTIONS
            if (typeof(T) == typeof(float))
                option = Convert.ToSingle(option);
        }

        return (T) option;
    }

    public static T FindInParent<T>(Transform transform) where T : class
    {
        if (transform.parent == null) return null;

        T component = transform.parent.GetComponent<T>();
        if (component == null)
            return FindInParent<T>(transform.parent);

        return component;
    }

    public static int TimeToHard(long time, GameConfig config)
    {
        time = time < 0 ? 0 : time;
        float hardBeforeDiscount = time / 60000.0f / config.TIME_HARD_RATE;
        long fullHours = time / 3600000;
        long discountHours = fullHours - config.DISCOUNT_HOUR > 0 ? fullHours - config.DISCOUNT_HOUR : 0;

        if (discountHours > 50)
            discountHours = 50;

        // как работает скидка: скажем, разница с ключевым часом равна 20 часам, тогда итоговое число будет:
        // кол-во валюты без скидки * 1 - 20 * 0.01 = кол-во валюты без скидки * 0.8
        return Mathf.CeilToInt(hardBeforeDiscount * (1 - discountHours * 0.01f));
    }

    public static long SoftToTime(Loot loot, GameConfig config)
    {
        return (long)Mathf.Ceil(loot[LootType.Soft] * config.SOFT_TIME_RATE * 1000.0f);
    }
}