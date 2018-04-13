using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager> {
    public List<ObjectPool> poolList = new List<ObjectPool>();
    Hashtable table;
	// Use this for initialization
	void Start () {
        table = new Hashtable();
		for(int i = 0; i < poolList.Count; i++)
        {
            poolList[i].initialize(transform);
            table.Add(poolList[i].poolName, poolList[i]);
        }
	}
	
	public void pushObject(string _itemName, GameObject _item, Transform _parent)
    {

        ObjectPool pool = getPoolItem(_itemName);
        if (pool != null)
        {
            pool.put_object(_item, _parent == null ? transform : _parent);

        }
        else
        {
            Debug.Log(_itemName + " pool not found");
        }
    }
    public GameObject getObject(string _itemName,Transform _parent)
    {

        ObjectPool pool = getPoolItem(_itemName);

        if (pool == null)
        {
            return null;

        }
        return pool.get_object(_parent == null ? transform : _parent);
    }

    ObjectPool getPoolItem(string _itemName)
    {
        if (table.ContainsKey(_itemName))
        {
            return table[_itemName] as ObjectPool;
        }
        
        return null;
    }
}
