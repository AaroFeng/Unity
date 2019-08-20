using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OrcKiStateManager : MonoBehaviour
{
    protected OrcKiStateBase currentState; //当前状态
    public OrcKiStateBase CurrentState { get { return currentState; } }

    protected Dictionary<Type, OrcKiStateBase> states = new Dictionary<Type, OrcKiStateBase>(); //敌人所有状态的集合

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
        //添加状态
        AddState<OrcKiStateIdle>();
        AddState<OrcKiStatePatrol>();
        AddState<OrcKiStateChase>();
        AddState<OrcKiStateAttack>();
        AddState<OrcKiStateDamage>();
        AddState<OrcKiStateDeath>();
        AddState<OrcKiStateRepel>();
        AddState<OrcKiStateFlameJet>();
        AddState<OrcKiStateBulletShoot>();
        AddState<OrcKiStateTrackBulletShoot>();
        AddState<OrcKiStateRangeBulletShoot>();
        //初始状态
        ChangeState<OrcKiStateIdle>();
    }

    //添加新状态到字典
    protected void AddState<T>() where T : OrcKiStateBase
    {
        //添加状态类的脚本到物体
        OrcKiStateBase state = gameObject.AddComponent<T>();
        state.OnInit(); //初始化
        states.Add(state.GetType(), state); //添加到状态集合
    }

    //切换状态
    public bool ChangeState<T>() where T : OrcKiStateBase
    {
        if (!states.ContainsKey(typeof(T)))  //如果不存在该状态
            return false;

        if (currentState != null)
            currentState.OnExit(); //旧状态 离开回调

        currentState = states[typeof(T)]; //重新赋值当前状态

        currentState.OnEnter(); //新状态 进入回调

        return true;
    }
}
