using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NPCStateManagerBase : MonoBehaviour
{
    protected NPCStateBase currentState; //当前状态
    protected Dictionary<Type, NPCStateBase> states = new Dictionary<Type, NPCStateBase>(); //敌人所以状态的集合

    //状态属性值
    protected float idleCDMin; //最小空闲时间
    public float IdleCDMin { get { return idleCDMin; } }
    protected float idleCDMax; //最大空闲时间
    public float IdleCDMax { get { return idleCDMax; } }
    protected float patrolSpeed; //巡逻状态 移动速度
    public float PatrolSpeed { get { return patrolSpeed; } }

    void Start()
    {
        SetStartState();
    }

    void Update()
    {
        if (currentState)
            currentState.OnExcute();
    }

    //开始时添加状态
    protected virtual void SetStartState()
    {

    }

    //添加新状态到字典
    protected void AddState<T>() where T : NPCStateBase
    {
        //添加状态类的脚本到物体
        NPCStateBase state = gameObject.AddComponent<T>();
        state.OnInit(); //初始化
        states.Add(state.GetType(), state); //添加到状态集合
    }

    //切换状态
    public bool ChangeState<T>() where T : NPCStateBase
    {
        if (states[typeof(T)] == null) //如果不存在该状态
            return false;

        if (currentState != null)
            currentState.OnExit(); //旧状态 离开回调

        currentState = states[typeof(T)]; //重新赋值当前状态

        currentState.OnEnter(); //新状态 进入回调

        return true;
    }
}
