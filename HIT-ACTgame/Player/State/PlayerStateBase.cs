using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    //状态机类型
    Idle,
    Move,
    Jump,
    Dodge,
    Guard,
    GuardDamage,
    GuardBreak,
    Combo1,
    Combo2,
    Combo3,
    Combo4,
    ThumpCharge,
    Thump,
    Thump1,
    Thump2,
    Skill1,
    Damage,
    Death,
    //非状态机类型 用于粒子效果播放传参
    EpPoint //有能量点时
}

public class PlayerStateBase : MonoBehaviour
{
    protected PlayerState playerState;
    public PlayerState PlayerState
    {
        get { return playerState; }
    }
    protected PlayerStateManager manager;
    protected PlayerParticle particle;
    protected Animator animator;
    protected Rigidbody rigid;
    protected CharacterController cc;
    protected PlayerCharacter player;
    protected SphereCamera camera;
    protected string aniName;

    protected float h; //键盘水平轴输入
    protected float v; //键盘垂直轴输入
    protected Vector3 vertiMove = new Vector3(); //垂直方向移动 重力
    protected Vector3 HoriMove = new Vector3(); //水平方向移动
    protected float speed; //移动速度
    protected float gravity = -35.0f; //重力

    //脚部球形检测 是否落地
    Vector3 checkPoint; //球形检测 圆心
    float radius = 0.25f; //球形检测 半径
    protected bool onGround; //是否在地面

    //初始化
    public virtual void OnInit()
    {
        manager = GetComponent<PlayerStateManager>();
        particle = GetComponent<PlayerParticle>();
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        cc = GetComponent<CharacterController>();
        player = GetComponent<PlayerCharacter>();
        camera = Camera.main.GetComponent<SphereCamera>();
    }

    //进入
    public virtual void OnEnter()
    {

    }

    //控制
    public virtual void OnControl()
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
        checkPoint = transform.position; //获取玩家位置
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
