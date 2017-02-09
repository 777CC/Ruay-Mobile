using UnityEngine;
using System;

public class JsonHelper
{
    public static T[] getJsonArray<T>(string json)
    {
        if (!string.IsNullOrEmpty(json))
        {
            string newJson = "{ \"array\": " + json + "}";
            Debug.Log(newJson);
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.array;
        }
        else
        {
            return null;
        }
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}