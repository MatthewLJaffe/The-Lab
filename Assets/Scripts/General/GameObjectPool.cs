using System.Collections.Generic;
using UnityEngine;

namespace General
{
    public class GameObjectPool
    {
        private Queue<GameObject> _pool;
        private GameObject _prefab;
        private Transform _parent;
        public GameObjectPool(GameObject prefab, Transform parent = null)
        {
            _prefab = prefab;
            _parent = parent;
            _pool = new Queue<GameObject>();
        }

        public GameObject GetFromPool()
        {
            if (_pool.Count == 0)
                AddToPool();
            var goOutput =  _pool.Dequeue();
            goOutput.SetActive(true);
            return goOutput;
        }

        private void AddToPool()
        {
            GameObject newGameObject = GameObject.Instantiate(_prefab, _parent);
            newGameObject.GetComponent<IPooled>().MyPool = this;
            newGameObject.SetActive(false);
            _pool.Enqueue(newGameObject);
        }
        
        public void ReturnToPool(GameObject returning)
        {
            returning.SetActive(false);
            _pool.Enqueue(returning);
        }
    }
}