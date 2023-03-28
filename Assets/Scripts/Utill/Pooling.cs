using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pooling<T> where T : Component
{
    private T _BaseObj = null;

    private Transform _Root;

    private Queue<T> _PoolingObjs = new Queue<T>();

    private List<T> _UseObjs = new List<T>();

    public Pooling(int poolingCount, T baseObj, Transform root)
    {
        _BaseObj = baseObj;
        _Root = root;
       
        for (int i = 0; i < poolingCount; i++)
        {
            Create();
        }

        Delete(baseObj);
    }

    private void Create()
    {        
        var obj = Object.Instantiate(_BaseObj, _Root);

        obj.gameObject.SetActive(false);

        _PoolingObjs.Enqueue(obj);
    }

    public void Delete(T obj)
    {
        obj.transform.SetParent(_Root);
        obj.gameObject.SetActive(false);

        _UseObjs.Remove(obj);
        _PoolingObjs.Enqueue(obj);
    }

    public T Get()
    {
        if (_PoolingObjs.Count == 0)
            Create();

        var obj = _PoolingObjs.Dequeue();

        obj.gameObject.SetActive(true);

        return obj;
    }

    public T Get(float x, float y)
    {
        if (_PoolingObjs.Count == 0)
            Create();

        var obj = _PoolingObjs.Dequeue();
        var pos = obj.transform.position;

        pos.x = x;
        pos.y = y;

        obj.transform.position = pos;
        obj.gameObject.SetActive(true);

        return obj;
    }
}
