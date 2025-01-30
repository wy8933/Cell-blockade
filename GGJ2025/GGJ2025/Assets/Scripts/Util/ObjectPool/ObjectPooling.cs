using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ObjectPoolings
{
    public static class ObjectPooling
    {
        private static readonly Dictionary<GameObject, PrefabPool> _pools = new();
        private static GameObject _poolParent;

        public static PrefabPool GetOrCreate(GameObject prefab, string parent = "Object Pooling")
        {
            if (_poolParent == null)
            {
                _pools.Clear();
                _poolParent = new GameObject(parent);
                MonitorDestroy(_poolParent);
            }

            if (_pools.TryGetValue(prefab, out var pool))
                return pool;

            var newPool = new PrefabPool(prefab, _poolParent.transform);
            _pools.Add(prefab, newPool);
            return newPool;
        }

        public static (GameObject instance, PrefabPool pool) GetOrCreate(GameObject prefab, Vector3 position, Quaternion rotation, string parent = "Object Pooling")
        {
            var pool = GetOrCreate(prefab, parent);
            return pool.GetAt(position, rotation);
        }

        public static PrefabPool Get(GameObject prefab) => _pools.TryGetValue(prefab, out var pool) ? pool : null;

        public static async Task TimedRelease(this (GameObject instance, PrefabPool pool) pair, TimeSpan time)
        {
            await Task.Delay(time);
            pair.pool.Release(pair.instance);
        }

        private static void MonitorDestroy(GameObject obj)
        {
            Task.Run(async () =>
            {
                while (obj != null)
                {
                    await Task.Delay(100);
                }

                foreach (var (_, pool) in _pools)
                {
                    pool.Clear();
                    pool.Dispose();
                }
                _pools.Clear();
            });
        }
    }
}
