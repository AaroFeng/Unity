using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum NPCState
{
    //状态机类型
    Idle,
    Patrol
    //非状态机类型 用于粒子效果播放传参
}

public class NPCStateBase : MonoBehaviour
{
    protected NPCStateManagerBase manager;
    protected Animator animator;
    protected CharacterController cc;
    protected NPCState npcState;
    public NPCState NpcState
    {
        get { return npcState; }
    }
    protected string aniName;

    protected NPCCharacterBase npc;
    protected NavMeshAgent agent;
    protected float speed; //移动速度

    //初始化
    public virtual void OnInit()
    {
        manager = GetComponent<NPCStateManagerBase>();
        animator = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
        npc = GetComponent<NPCCharacterBase>();
    }

    //进入
    public virtual void OnEnter()
    {
        
    }

    //执行
    public virtual void OnExcute()
    {

    }

    //退出
    public virtual void OnExit()
    {

    }
}
