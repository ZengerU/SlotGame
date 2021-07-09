using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public static class ExtensionMethods
{
    public static IEnumerable<T> GetComponentsAtDepth<T>(this Transform tra, int depth) where T : Component
    {
        List<Transform> list = new List<Transform> {tra};
        return GetComponentsRecursively<T>(0, depth, list);
    }

    private static IEnumerable<T> GetComponentsRecursively<T>(int currentDepth, int targetDepth,
        IEnumerable<Transform> transforms)
    {
        if (currentDepth == targetDepth)
        {
            List<T> returnList = new List<T>();
            foreach (Transform transform in transforms)
            {
                T comp = transform.GetComponent<T>();
                if (comp != null)
                    returnList.Add(comp);
            }

            return returnList;
        }

        List<Transform> nextDepth = new List<Transform>();
        foreach (Transform transform in transforms)
        {
            nextDepth.AddRange(transform.Cast<Transform>().ToList());
        }

        return GetComponentsRecursively<T>(++currentDepth, targetDepth, nextDepth);
    }

    public static T GetRandomElementFromList<T>(this IEnumerable<T> arr)
    {
        List<T> list = arr.ToList();
        if (list.Count == 1)
            return list[0];
        int index = Random.Range(1, list.Count() - 1);
        return list[index];
    }
    public static void SetTransformation(this Transform trans, float? x = null, float? y = null, float? z = null)
    {
        trans.position = new Vector3(x ?? trans.position.x,y ?? trans.position.y,z ?? trans.position.z);
    }
    public static void MoveTransformation(this Transform trans, float? x = null, float? y = null, float? z = null)
    {
        float tmpX = trans.position.x + (x ?? 0);
        float tmpY = trans.position.y + (y ?? 0);
        float tmpZ = trans.position.z + (z ?? 0);
        trans.position = new Vector3(tmpX,tmpY,tmpZ);
    }
    public static void SetRotationAxis(this Transform trans, float? x = null, float? y = null, float? z = null)
    {
        trans.eulerAngles = new Vector3(x ?? trans.eulerAngles.x, y ?? trans.eulerAngles.y, z ?? trans.eulerAngles.z);
    }
    
    public static void SetText(this TextMeshProUGUI txt, float value)
    {
        txt.text = Math.Floor(value) == value ? $"{value}" : $"{value:n1}";
    }

    public static Color Invisible => new Color(1, 1, 1, 0);
}