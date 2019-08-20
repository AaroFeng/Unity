using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrcLaCharacter : EnemyCharacterBase
{
    void Awake()
    {
        //设定属性值
        enemyName = "兽人枪兵"; //名字
        height = 2.6f; //身高
        hpMax = 200; //最大体力值
        atk = 15; //攻击力

        //设定状态属性值
        idleCDMin = 15.0f;
        idleCDMax = 20.0f;
        patrolSpeed = 1.2f;
        chaseSpeed = 3.5f;
        atkCD = 2.5f; //普通攻击CD
        //设定状态检测范围
        patrolRadius = 10.0f; //巡逻视野
        patrolBackRadius = 1.5f; //巡逻 背面视野半径
        patrolAngle = 180; //巡逻 正面视野角度

        chaseRadius = 18.0f; //追逐视野
        chaseBackRadius = 7.0f; //追逐 背面视野半径
        chaseAngle = 220; //追逐 正面视野角度

        attackRadius = 3.0f; //攻击范围
    }

    //动画帧事件
    public override void Attack1Check()
    {
        //设定攻击范围
        checkPoint = transform.position; //范围球心
        atkRadius = 3.5f; //范围半径
        atkAngle = 40; //范围内 有效角度
        realAtk = atk * 1.0f; //真实伤害
        atkDirection = transform.forward; //攻击范围 方向

        impactHori = 18.0f; //水平冲击力
        impactVeti = 2.0f; //垂直冲击力

        SphereForeach();
    }
}
