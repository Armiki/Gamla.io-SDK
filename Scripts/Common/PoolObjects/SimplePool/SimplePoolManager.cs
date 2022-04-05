using System;
using System.Collections.Generic;
using System.Linq;
using Game.PoolObjects;
using UnityEngine;
using Gamla.Scripts.Logic;

public class SimplePoolManager : MonoBehaviour
{
    [Serializable]
    class PreWarmBindings
    {
        public GameObject prefab;
        public int count;
    }
    
	[SerializeField] private bool _logStatus;
	[SerializeField] private Transform _root;
	[SerializeField] private List<PreWarmBindings> _preWarmPrefabs;
	
	private Dictionary<string, ObjectSimplePool<PoolObject>> _prefabLookup;
	private Dictionary<PoolObject, ObjectSimplePool<PoolObject>> _instanceLookup;

	private bool _dirty = false;
	private bool _asyncwarminprogress = false;

	public static SimplePoolManager instance => _instance;
	private static SimplePoolManager _instance;

	void Awake () 
	{
		_prefabLookup = new Dictionary<string, ObjectSimplePool<PoolObject>>();
		_instanceLookup = new Dictionary<PoolObject, ObjectSimplePool<PoolObject>>();

		_instance = this;

		foreach (var preWarm in _preWarmPrefabs)
		{
			PreWarmPool(preWarm.prefab, preWarm.count);
		}
	}

	void Update()
	{
		if(_logStatus && _dirty)
		{
			PrintStatus();
			_dirty = false;
		}
	}
	
	void PreWarmPool(GameObject go, int size)
	{
		if (_prefabLookup.ContainsKey(go.name)) {
			Debug.LogWarning("Pool for prefab " + go.name + " has already been created");
		}
		var pool = new ObjectSimplePool<PoolObject>(() => InstantiatePrefab(go), size);
		_prefabLookup[go.name] = pool;
		_dirty = true;
	}

	public void WarmPool(string path, int size)
	{
		if (_prefabLookup.ContainsKey(path)) {
			Debug.LogWarning("Pool for prefab " + path + " has already been created");
		}
		var pool = new ObjectSimplePool<PoolObject>(() => InstantiatePrefab(path), size);
		_prefabLookup[path] = pool;
		_dirty = true;
	}
	
	public static PoolObject Spawn(string path, bool isAddressable, Transform parent = null)
	{
		return instance.SpawnObject(path, Vector3.zero, Quaternion.identity, isAddressable, parent);
	}
	
	public static PoolObject Spawn(string path, Vector3 position, Quaternion rotation, bool isAddressable, Transform parent)
	{
		return instance.SpawnObject(path, position, rotation, isAddressable, parent);
	}

	public PoolObject SpawnObject(string path, Vector3 position, Quaternion rotation, bool isAddressable, Transform parent)
	{
		if (!_prefabLookup.ContainsKey(path) && !isAddressable) {
			WarmPool(path, 1);
		}
		

		PoolObject clone = null;
		clone = _prefabLookup[path].GetItem();

		if (!(clone is null)) {
			clone.Spawn(parent != null ? parent : _root, position, rotation);
			_instanceLookup.Add(clone, _prefabLookup[path]);
			_dirty = true;
		}
		return clone;
	}
	
	public static void Release(PoolObject clone)
	{
		instance.ReleaseObject(clone);
	}

	public void ReleaseObject(PoolObject clone)
	{
		if(_instanceLookup.ContainsKey(clone)) {
			_instanceLookup[clone].ReleaseItem(clone);
			_instanceLookup.Remove(clone);
			_dirty = true;
		}else {
			Debug.LogWarning("No pool contains the object: " + clone);
		}
	}

	private PoolObject InstantiatePrefab(GameObject prefab)
	{
		var go = Instantiate(prefab);
		if (_root != null) go.transform.SetParent(_root);
		go.transform.localPosition = Vector3.zero;
		go.transform.localScale = Vector3.one;
		var poolObject = go.GetComponent<PoolObject>();
		if (poolObject == null) go.AddComponent<PoolObject>();
		go.name = prefab.name;
		go.SetActive(false);
		return poolObject;
	}

	private PoolObject InstantiatePrefab(string path)
	{
		PoolObject go = null;
		var prefab = GamlaResourceManager.GamlaResources.GetResource(path);
		if (prefab != null) {
			go = InstantiatePrefab(prefab);
		}
		return go;
	}

	public void PrintStatus()
	{
		foreach (KeyValuePair<string, ObjectSimplePool<PoolObject>> keyVal in _prefabLookup)
		{
			Debug.Log(string.Format("Object Pool for Prefab: {0} In Use: {1} Total {2}", keyVal.Key, keyVal.Value.CountUsedItems, keyVal.Value.Count));
		}
	}
	
	[ContextMenu("Warm Pool")]
	public void WarmPoolContext()
	{
		WarmPool("GUI/UIMarker/TabLockedMarker", 3);
	}
	
	[ContextMenu("Release Object")]
	public void ReleaseObjectContext()
	{
		//var parent = GameObject.Find("WinBag");
		_instanceLookup.First().Key.Release();
		//Destroy(parent);
	}
	
	[ContextMenu("Spawn addressable")]
	public void TestAddressableSpawn()
	{
		Spawn("Content/GUIEffects/AddCoinEffect", true);
	}
}


