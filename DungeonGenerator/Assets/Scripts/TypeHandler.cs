using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class TypeHandler
{
    public static System.Type[] GetAllDerivedTypes(this System.AppDomain aAppDomain, System.Type aType)
    {
        var result = new List<System.Type>();
        var assemblies = aAppDomain.GetAssemblies();

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (type.IsSubclassOf(aType))
                    result.Add(type);
            }
        }

        return result.ToArray();
    }
    public static System.Type[] GetAllDerivedTypes<T>(this System.AppDomain aAppDomain)
    {
        return GetAllDerivedTypes(aAppDomain, typeof(T));
    }

    public static T[] GetAllInstances<T>() where T : ScriptableObject
    {
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);

        T[] instanceList = new T[guids.Length];

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);

            instanceList[i] = AssetDatabase.LoadAssetAtPath<T>(path);
        }

        return instanceList;
    }
}
