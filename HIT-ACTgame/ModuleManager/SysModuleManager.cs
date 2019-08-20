using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SysModuleManager
{
    //单例
    private static SysModuleManager instance = null;
    public static SysModuleManager Instance
    {
        get
        {
            if (instance == null)
                instance = new SysModuleManager();
            return instance;
        }
    }
    private SysModuleManager() { }

    private GameObject rootGameObj; //根物体
    public List<SysModule> modules = new List<SysModule>(); //模块集合
    private Dictionary<Type, SysModule> type_moduleMap = new Dictionary<Type, SysModule>(); //模块集合映射 用于通过类型获取模块

    //初始化根物体 引用类型赋值
    public void Initialize(GameObject rootGameObj)
    {
        this.rootGameObj = rootGameObj;
    }

    //更新所有模块OnUpdate方法
    public void OnUpdate()
    {
        for (int i = 0; i < modules.Count; i++)
        {
            var module = modules[i];
            if (module != null)
                module.OnUpdate();
        }
    }

    //添加新模块
    public T AddSysModule<T>() where T : SysModule
    {
        Type t = typeof(T);

        //模块集合映射中 是否存在该模块
        if (type_moduleMap.ContainsKey(t))
            return type_moduleMap[t] as T;

        //根物体添加 模块组件
        SysModule module = rootGameObj.AddComponent<T>();
        module.Initialize(); //初始化

        modules.Add(module); //添加进 模块集合
        type_moduleMap.Add(t, module); //添加进 模块集合映射

        return module as T;
    }

    //外部获取模块
    public T GetSysModule<T>() where T:SysModule
    {
        Type t = typeof(T);

        //模块集合映射中 是否存在该模块
        if (!type_moduleMap.ContainsKey(t))
        {
            //模块集合中 是否存在该模块
            for (int i = 0; i < modules.Count; i++)
            {
                var module = modules[i];
                //类型是否相同
                if (module.GetType().IsSubclassOf(t))
                    return module as T;
            }

            Debug.Log(t + "模块管理器中不存在该模块");
            return null;
        }

        return type_moduleMap[t] as T;
    }
}
