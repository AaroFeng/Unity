using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WormShCharacter : EnemyCharacterBase
{
    GameObject venom;

    void Awake()
    {
        //设定属性值
        enemyName = "蠕虫射手"; //名字
        height = 2.0f; //身高
        hpMax = 80; //最大体力值
        atk = 40; //攻击力

        //设定状态属性值
        idleCDMin = 2.0f;
        idleCDMax = 2.0f;
        patrolSpeed = 1.2f;
        chaseSpeed = 3.5f;
        atkCD = 3.0f; //普通攻击CD
        //设定状态检测范围
        patrolRadius = 5.0f; //巡逻视野
        patrolBackRadius = 5.0f; //巡逻 背面视野半径
        patrolAngle = 180; //巡逻 正面视野角度

        chaseRadius = 15.0f; //追逐视野
        chaseBackRadius = 15.0f; //追逐 背面视野半径
        chaseAngle = 180; //追逐 正面视野角度

        attackRadius = 15.0f; //攻击范围

        chaseBlendSpeed = 0.05f; //追逐状态Blend变化速度
    }

    //动画帧事件 近战毒液喷洒
    public override void Attack1Check()
    {
        //设定攻击范围
        checkPoint = transform.position; //范围球心
        atkRadius = 4.0f; //范围半径
        atkAngle = 120; //范围内 有效角度
        realAtk = atk * 1.0f; //真实伤害
        atkDirection = transform.forward; //攻击范围 方向

        impactHori = 6.0f; //水平冲击力
        impactVeti = 0.0f; //垂直冲击力

        SphereForeach();

        //实例化毒液弹
        venom = SysModuleManager.Instance.GetSysModule<SysPool>().CreateObj("Venom");
        venom.transform.position = GetComponent<EnemyParticle>().particleList[0].transform.position; //设置位置为 发射口
        venom.GetComponent<Rigidbody>().velocity = transform.forward * 10.0f; //设置初始速度
    }

    //毒液弹 范围检测
    void VenomCheck()
    {
        //设定攻击范围
        checkPoint = venom.transform.position; //范围球心
        atkRadius = 0.3f; //范围半径
        atkAngle = 360; //范围内 有效角度
        realAtk = atk * 1.5f; //真实伤害
        atkDirection = transform.forward; //攻击范围 方向

        impactHori = 5.0f; //水平冲击力
        impactVeti = 0.0f; //垂直冲击力

        if (SphereForeach()) //碰撞就消失
        {
            //播放碰撞爆炸特效
            ParticleSystem particle = GetComponent<EnemyParticle>().particleList[2].GetComponent<ParticleSystem>();
            particle.transform.position = venom.transform.position;
            particle.Play();

            SysModuleManager.Instance.GetSysModule<SysPool>().RemoveObj(venom);
            venom = null; //取消引用
        }
            
    }

    public override void Update()
    {
        base.Update();

        //缩地隐藏状态时 隐藏体力条和名字
        if(GetComponent<EnemyStateManagerBase>().CurrentState.EnemyState.Equals(EnemyState.Idle) ||
            GetComponent<EnemyStateManagerBase>().CurrentState.EnemyState.Equals(EnemyState.Patrol))
        {
            if (hpSlider != null) hpSlider.SetActive(false);
            if (nameText != null) nameText.SetActive(false);
        }
        else
        {
            if (hpSlider != null) hpSlider.SetActive(true);
            if (nameText != null) nameText.SetActive(true);
        }

        //受伤无击退效果
        if (damage)
            damageImpact = Vector3.zero;

        //毒液弹碰撞检测
        if (venom != null)
            VenomCheck();
    }
}
