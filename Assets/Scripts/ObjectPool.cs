using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ObjectPool : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public Pool[] pools;
    private Dictionary<string, Queue<GameObject>> poolDict;
    private Dictionary<string, GameObject> prefabDict;
    private Dictionary<GameObject, string> spawnedTags;

    void Awake()
    {
        poolDict = new Dictionary<string, Queue<GameObject>>();
        prefabDict = new Dictionary<string, GameObject>();
        spawnedTags = new Dictionary<GameObject, string>();

        Vector3 spawnPos = transform.position;
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 100f, NavMesh.AllAreas))
            spawnPos = hit.position;

        foreach (var pool in pools)
        {
            var queue = new Queue<GameObject>();
            prefabDict[pool.tag] = pool.prefab;

            for (int i = 0; i < pool.size; i++)
            {
                var obj = Instantiate(pool.prefab, spawnPos, Quaternion.identity, transform);
                obj.SetActive(false);
                queue.Enqueue(obj);
            }

            poolDict[pool.tag] = queue;
        }
    }

    public GameObject Spawn(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDict.TryGetValue(tag, out var queue))
        {
            Debug.LogWarning($"ObjectPool: 没有找到 tag = {tag} 的池");
            return null;
        }

        GameObject obj;
        if (queue.Count > 0)
        {
            obj = queue.Dequeue();
        }
        else
        {
            Vector3 p = transform.position;
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 100f, NavMesh.AllAreas))
                p = hit.position;
            obj = Instantiate(prefabDict[tag], p, Quaternion.identity, transform);
        }

        spawnedTags[obj] = tag;

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        return obj;
    }

    public void Despawn(GameObject obj)
    {
        if (!spawnedTags.TryGetValue(obj, out var tag))
        {
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        obj.transform.SetParent(transform);
        poolDict[tag].Enqueue(obj);
    }
}
