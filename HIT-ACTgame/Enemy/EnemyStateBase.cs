using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    //状态机类型
    Idle,
    Patrol,
    Chase,
    AttackIdle,
    Attack,
    Damage,
    Death
    //非状态机类型 用于粒子效果播放传参

}

public class EnemyStateBase : MonoBehaviour
{
    protected EnemyStateManagerBase manager;
    protected Animator animator;
    protected CharacterController cc;
    protected EnemyState enemyState;
    public EnemyState EnemyState
    {
        get { return enemyState; }
    }
    protected string aniName;

    protected EnemyCharacterBase enemy;
    protected NavMeshAgent agent;
    protected float speed; //移动速度
    //不同状态下的 动态移动
    protected Vector3 vertiMove = new Vector3(); //垂直方向移动 重力
    protected Vector3 HoriMove = new Vector3(); //水平方向移动
    float gravity = -35.0f; //重力

    protected EnemyParticle particle;

    //球形视野
    protected Vector3 viewPoint; //视野中心点

    //脚部球形检测 是否落地
    Vector3 checkPoint; //球形检测 圆心
    float radius = 0.25f; //球形检测 半径
    protected bool onGround; //是否在地面

    //初始化
    public virtual void OnInit()
    {
        manager = GetComponent<EnemyStateManagerBase>();
        animator = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
        enemy = GetComponent<EnemyCharacterBase>();
        particle = GetComponent<EnemyParticle>();
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

    protected void Gravity()//模拟重力
    {
        //球形检测范围 是否落地
        checkPoint = transform.position; //获取自身位置
        checkPoint.y += 0.1f;
        Collider[] standards = Physics.OverlapSphere(checkPoint, radius, 1 << LayerMask.NameToLayer("Standard"));
        //检测是否离地
        if (onGround && standards.Length < 1)
            onGround = false;
        //检测是否落地
        else if (!onGround && standards.Length > 0)
            onGround = true;

        //不在地面时 重力速度加大
        if (!onGround && vertiMove.y > gravity)
            vertiMove.y += gravity * Time.deltaTime;
        //在地面时 重力速度减小至0
        else if (onGround && vertiMove.y < -5)
        {
            vertiMove.y += -gravity * Time.deltaTime;
            if (vertiMove.y > -5)
            {
                vertiMove.y = -5;
            }
        }

        //重力速度移动
        cc.Move(vertiMove * Time.deltaTime);
    }
}
