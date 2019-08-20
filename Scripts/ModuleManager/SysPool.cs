using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SysPool : SysModule
{
    //设置缓存预制物列表
    List<GameObject> prefabs = new List<GameObject>();
    //设置对应缓存预制物的最大缓存数
    List<int> setBufferCount = new List<int>();
    //缓存池
    Dictionary<string, List<GameObject>> pools = new Dictionary<string, List<GameObject>>();
    //使用池 取出使用的缓存池物体
    Dictionary<string, List<GameObject>> usePools = new Dictionary<string, List<GameObject>>();
    //默认缓存数
    private int defaultCount = 10;

    GameObject poolPrefabs; //所有预制物父物体 集合管理

    //初始化
    public override bool Initialize()
    {
        //实例化 缓存池预制物的父物体
        poolPrefabs = new GameObject();
        poolPrefabs.name = "PoolPrefabs";
        DontDestroyOnLoad(poolPrefabs);

        return true;
    }

    //开始执行
    public override void Run(object userData)
    {

    }

    //更新
    public override void OnUpdate()
    {

    }

    //释放
    public override void Dispose()
    {

    }

    //添加缓存预制物
    public void AddPrefab(GameObject prefab)
    {
        prefabs.Add(prefab);
    }
    //添加缓存预制物 设定缓存数
    public void AddPrefab(GameObject prefab,int setCount)
    {
        prefabs.Add(prefab);
        setBufferCount.Add(setCount); //设置预制物的缓存数
    }

    public void BufferPrefabs() //实例化 缓存预制物列表
    {
        //初始化时实例化 缓存预制物
        for (int i = 0; i < prefabs.Count; i++)
        {
            //预制物体的 生成缓存表
            List<GameObject> objs = new List<GameObject>();

            //预制物体的 缓存数
            int bufferCount = defaultCount;
            if (i < setBufferCount.Count) //如果用户设置了最大缓存数
                bufferCount = setBufferCount[i];

            //生成当前预制物父物体
            GameObject parent = new GameObject();
            parent.name = "Prefabs_" + prefabs[i].name;
            DontDestroyOnLoad(parent);
            parent.transform.SetParent(poolPrefabs.transform);

            for (int j = 0; j < bufferCount; j++) //生成缓存预制物体
            {
                GameObject _obj = Instantiate(prefabs[i]); //实例化
                DontDestroyOnLoad(_obj); //不销毁
                _obj.name = prefabs[i].name; //设定名字
                _obj.SetActive(false); //设定非激活
                objs.Add(_obj); //加入 生成缓存表
                _obj.transform.SetParent(parent.transform); //设置父物体
            }
            //将生成缓存表 添加进缓存池字典
            pools.Add(prefabs[i].name, objs);
            //使用池 生成对应空池
            usePools.Add(prefabs[i].name, new List<GameObject>());
        }
    }

    public GameObject CreateObj(string name) //获取预制物体
    {
        if (pools[name].Count > 0)
        {
            pools[name][0].SetActive(true); //缓存池内 该预制物体缓存表第一个 设置激活
            GameObject obj = pools[name][0]; //记录引用
            pools[name].Remove(obj); //取出缓存池
            usePools[name].Add(obj); //加入使用池
            return obj; //返回取出的缓存物体
        }

        //如果 缓存池内 该物体缓存表 数量为0
        //遍历检查 需缓存物目录
        for (int i = 0; i < prefabs.Count; i++)
        {
            //需缓存物目录 有该物体
            if (prefabs[i].name.Equals(name))
            {
                GameObject _obj = Instantiate(prefabs[i]); //实例化
                _obj.name = name; //设定名字
                usePools[name].Add(_obj); //加入使用池
                return _obj; //返回 新实例化的物体
            }
        }

        Debug.Log(name + "：试图取出的该物体不在需缓存物目录中");
        return null;
    }

    public void RemoveObj(GameObject _obj) //移除预制物体
    {
        //该预制物体的 缓存数
        int bufferCount = defaultCount;
        //如果用户设置了缓存数
        if (prefabs.FindIndex(item => item.name.Equals(_obj.name)) <= setBufferCount.Count - 1) 
            bufferCount = setBufferCount[prefabs.FindIndex(item => item.name.Equals(_obj.name))];
        
        //缓存池中 该物体数量 大于 设定缓存数
        if (pools[_obj.name].Count >= bufferCount)
        {
            usePools[_obj.name].Remove(_obj); //取出使用池
            Destroy(_obj); //销毁该物体
            
        }
        else
        {
            //缓存池 回收物体
            _obj.SetActive(false); //设定非激活
            usePools[_obj.name].Remove(_obj); //取出使用池
            pools[_obj.name].Add(_obj); //加入缓存池
        }
    }

    public void RemoveAllObj() //移除所有使用中预制物体
    {
        //遍历 缓存预制物列表
        for (int i = 0; i < prefabs.Count; i++) 
        {   
            //遍历移除所有 缓存池 缓存物体 
            for (int j = 0; j < usePools[prefabs[i].name].Count; j++) 
            {
                //不激活
                RemoveObj(usePools[prefabs[i].name][j]);
            }
        }
    }

    public void RemoveAllObj(string name) //移除所有使用中预制物体
    {
        //遍历移除所有 缓存池 缓存物体 
        for (int i = 0; i < usePools[name].Count; i++)
        {
            //不激活
            RemoveObj(usePools[name][i]);
        }
    }
}
