using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        int index = Random.Range(1, list.Count() - 1);
        return list[index];
    }
}