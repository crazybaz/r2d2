using System.Collections.Generic;
using UnityEngine;

public class ComponentExtention
{
    public static T CopyComponent<T>(Component sourceComponent, GameObject target) where T : Component
    {
        var type = sourceComponent.GetType();
        var targetComponent = target.AddComponent(type);

        foreach (var field in type.GetFields())
        {
            // Debug.Log("FIELD " + field.Name + " VALUE " + field.GetValue(sourceComponent));
            field.SetValue(targetComponent, field.GetValue(sourceComponent));
        }

        return targetComponent as T;
    }

    public static List<T> CopyComponents<T>(GameObject source, GameObject target) where T : Component
    {
        var res = new List<T>();

        foreach (var component in source.GetComponents<T>())
            res.Add(CopyComponent<T>(component, target));

        return res;
    }
}