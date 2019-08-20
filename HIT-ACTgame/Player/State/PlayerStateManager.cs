using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerStateManager : MonoBehaviour
{
    //当前状态
    protected PlayerStateBase currentState;
    public PlayerStateBase CurrentState
    {
        get { return currentState; }
    }
    
    //人物所有状态的集合
    private Dictionary<Type, PlayerStateBase> states = new Dictionary<Type, PlayerStateBase>();

    void Start()
    {
        //添加所以玩家状态到集合
        AddState<PlayerStateMove>();
        AddState<PlayerStateJump>();

        AddState<PlayerStateDodge>();
        AddState<PlayerStateGuard>();
        AddState<PlayerStateGuardDamage>();
        AddState<PlayerStateGuardBreak>();

        AddState<PlayerStateCombo1>();
        AddState<PlayerStateCombo2>();
        AddState<PlayerStateCombo3>();
        AddState<PlayerStateCombo4>();

        AddState<PlayerStateThumpCharge>();
        AddState<PlayerStateThump>();
        AddState<PlayerStateThump1>();
        AddState<PlayerStateThump2>();

        AddState<PlayerStateSkill1>();

        AddState<PlayerStateDamage>();
        AddState<PlayerStateDeath>();

        //初始状态为 移动
        ChangeState<PlayerStateMove>();
    }

    void OnEnable()
    {
        //切换场景后 初始状态为 移动
        if (states.Keys.Count > 0)
            ChangeState<PlayerStateMove>();
    }

    public void OnUpdate()
    {
        //游戏暂停时 不能操作
        if (Time.timeScale == 0)
            return;

        if (currentState)
        {
            currentState.OnControl();
            currentState.OnExcute();
            GetComponent<PlayerCharacter>().RefreshAttribute(); //刷新玩家属性
        }     
    }

    //添加新状态到集合
    void AddState<T>() where T : PlayerStateBase
    {
        //添加状态类脚本到物体
        PlayerStateBase state = gameObject.AddComponent<T>();
        state.OnInit(); //初始化状态
        states.Add(state.GetType(), state); //添加到状态集合
    }

    //切换状态
    public void ChangeState<T>() where T : PlayerStateBase
    {
        //旧状态离开回调
        if (currentState != null)
            currentState.OnExit();

        currentState = states[typeof(T)]; //切换状态

        currentState.OnEnter(); //新状态进入回调

        //Debug.Log("进入" + CurrentState.PlayerState);
    }
}
