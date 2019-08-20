using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SysModule : MonoBehaviour
{
    //初始化
    public virtual bool Initialize()
    {
        return true;
    }
    
    //开始执行
    public virtual void Run(object userData)
    {

    }

    //更新
    public virtual void OnUpdate()
    {

    }

    //释放
    public virtual void Dispose()
    {

    }
}
