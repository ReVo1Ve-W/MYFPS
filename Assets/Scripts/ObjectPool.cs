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

    void Awake()
    {
        poolDict = new Dictionary<string, Queue<GameObject>>();

        // 找一个 NavMesh 上的位置，避免 NavMeshAgent 初始化报错
        Vector3 spawnPos = transform.position;
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 100f, NavMesh.AllAreas))
            spawnPos = hit.position;

        foreach (var pool in pools)
        {
            var queue = new Queue<GameObject>();

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
            obj = Instantiate(pools[System.Array.FindIndex(pools, p => p.tag == tag)].prefab, p, Quaternion.identity, transform);
        }

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        return obj;
    }

    public void Despawn(string tag, GameObject obj)
    {
        if (!poolDict.TryGetValue(tag, out var queue))
        {
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        obj.transform.SetParent(transform);
        queue.Enqueue(obj);
    }
}
