using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace ObjectPoolings
{
    public class PrefabPool : ObjectPool<GameObject>
    {
        public readonly GameObject Prefab;

        public PrefabPool(GameObject prefab, Transform parent = null)
            : base(() => Create(prefab, parent), Get, ReleaseInstance, Destroy)
        {
            Prefab = prefab;
        }

        private static GameObject Create(GameObject prefab, Transform parent)
        {
            var go = Object.Instantiate(prefab, parent);
            MonitorDestroy(go, prefab);
            return go;
        }

        private static void Get(GameObject obj) => obj.SetActive(true);

        private static void ReleaseInstance(GameObject obj) => obj.SetActive(false);

        private static void Destroy(GameObject obj) => Object.Destroy(obj);

        public (GameObject go, PrefabPool pool) GetAt(Vector3 position, Quaternion rotation = default)
        {
            var go = Get()!;
            go.transform.position = position;
            go.transform.rotation = rotation;
            return (go, this);
        }

        private static void MonitorDestroy(GameObject obj, GameObject prefab)
        {
            Task.Run(async () =>
            {
                while (obj != null)
                {
                    await Task.Delay(100);
                }

                ObjectPooling.Get(prefab)?.Release(obj);
            });
        }
    }
}
