using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPool : MonoBehaviour
{
    List<GameObject> availableItems = new List<GameObject>();
    List<GameObject> availableSkills = new List<GameObject>();

    public static ItemPool Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        CreatePool();
    }

    public GameObject GetItemFromPool(int itemIndex)
    {
        var instance = availableItems[itemIndex];

        instance.SetActive(true);
        return instance;
    }

    public GameObject GetSkillFromPool(int itemIndex)
    {
        var instance = availableSkills[itemIndex];

        instance.SetActive(true);
        return instance;
    }

    private void CreatePool()
    {
        var statItemTypes = System.AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(StatItem));
        var orbitalItemTypes = System.AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(OrbitalItem));
        //var familiarItemTypes = System.AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(FamiliarItem));
        //var effectItemTypes = System.AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(EffectItem));

        for (int i = 0; i < statItemTypes.Length; i++)
        {
            GameObject itemToAdd = new GameObject("Stat Item", new System.Type[] { statItemTypes[i] });
            itemToAdd.transform.SetParent(transform);

            AddItemToPool(itemToAdd);
        }

        for (int i = 0; i < orbitalItemTypes.Length; i++)
        {
            GameObject itemToAdd = new GameObject("Orbital Item", new System.Type[] { statItemTypes[i] });
            itemToAdd.transform.SetParent(transform);

            AddItemToPool(itemToAdd);
        }

        //for (int i = 0; i < familiarItemTypes.Length; i++)
        //{
        //    GameObject itemToAdd = new GameObject("Familiar Item", new System.Type[] { statItemTypes[i] });
        //    itemToAdd.transform.SetParent(transform);

        //    AddItemToPool(itemToAdd);
        //}

        //for (int i = 0; i < effectItemTypes.Length; i++)
        //{
        //    GameObject itemToAdd = new GameObject("Effect Item", new System.Type[] { statItemTypes[i] });
        //    itemToAdd.transform.SetParent(transform);

        //    AddItemToPool(itemToAdd);
        //}

        var skillTypes = System.AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(Skill));

        for (int i = 0; i < skillTypes.Length; i++)
        {
            GameObject skillToAdd = new GameObject("Skill", new System.Type[] { skillTypes[i] });
            skillToAdd.transform.SetParent(transform);

            AddSkillToPool(skillToAdd);
        }
    }

    public void AddItemToPool(GameObject instance)
    {
        instance.SetActive(false);
        availableItems.Add(instance);
    }

    public void AddSkillToPool(GameObject instance)
    {
        instance.SetActive(false);
        availableSkills.Add(instance);
    }
}
