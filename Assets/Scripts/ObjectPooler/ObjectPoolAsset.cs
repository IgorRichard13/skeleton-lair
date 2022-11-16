using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace R2.Utilities.ObjectPool {
    [CreateAssetMenu(fileName = "ObjectPoolAsset", menuName = "Utilities/ObjectPooler")]
    public class ObjectPoolAsset : ScriptableObject
    {
        public Pool[] pools;
        [System.NonSerialized]
        Dictionary<string, Pool> poolDict = new Dictionary<string, Pool>();
        [System.NonSerialized]
        Transform parentTransform;
        public bool preWarm;

        public void Init()
        {
            parentTransform = new GameObject("Object Pool").transform;

            for (int i = 0; i < pools.Length; i++)
            {
                poolDict.Add(pools[i].poolName, pools[i]);

                if(preWarm)
                {
                    pools[i].PreWarmObject(parentTransform);
                }
            }
        }

        public GameObject GetObject(string id)
        {
            poolDict.TryGetValue(id, out Pool value);
            return value.GetObject(parentTransform);
        }
    }

    [System.Serializable]
    public class Pool
    {
        public string poolName;
        public GameObject prefab;
        public int budget;

        [System.NonSerialized]
        List<GameObject> createdObjects = new List<GameObject>();
        [System.NonSerialized]
        int index;

        public GameObject GetObject(Transform parent)
        {
            if(prefab == null)
            {
                Debug.LogError("Prefab is null");
                return null;
            }

            GameObject retVal = new();
            retVal = null;
            
            if (createdObjects.Count < budget)
            {
                GameObject go = GameObject.Instantiate(prefab) as GameObject;
                go.SetActive(false);
                go.transform.parent = parent;
                createdObjects.Add(go);
                retVal = go;
            }
            else
            {
                retVal = createdObjects[index];
                index++;
                if (index > createdObjects.Count - 1)
                {
                    index = 0;
                }
            }

            retVal.SetActive(false);
            
            return retVal;
        }

        public void PreWarmObject(Transform parent)
        {
            for (int i = 0; i < budget; i++)
            {
                GameObject go = GameObject.Instantiate(prefab) as GameObject;
                go.SetActive(false);
                go.transform.parent = parent;
                createdObjects.Add(go);
            }
        }
    }
}
