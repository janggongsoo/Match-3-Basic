using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPool {
    public string poolName;
    public GameObject poolObj;
    public int poolCount = 0;

    public List<GameObject> poolList = new List<GameObject>();
	
    public void initialize(Transform _parent)
    {
        for(int i = 0; i < poolCount; i++)
        {
            poolList.Add(createItem(_parent));
        }
    }
    public void put_object(GameObject _item,Transform _parent)
    {
        _item.transform.SetParent(_parent);
        _item.SetActive(false);
        poolList.Add(_item);
    }

    public GameObject get_object(Transform _parent)
    {
        if (poolList.Count == 0)
        {
            poolList.Add(createItem(_parent));
        }
        GameObject item = poolList[0];
        poolList.RemoveAt(0);
        return item;
    }

    GameObject createItem(Transform _parent)
    {
        GameObject item = Object.Instantiate(poolObj) as GameObject;
        item.transform.SetParent(_parent);
        item.SetActive(false);

        return item;
    }
}
