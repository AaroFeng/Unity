using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcKiStateIdle : OrcKiStateBase
{
    float PatrolCD; //进入巡逻所需时间
    float PatrolTime; //当前累计时间

    public override void OnInit()
    {
        base.OnInit();
        orcKiState = OrcKiState.Idle;
        aniName = "Idle";
    }

    public override void OnEnter()
    {
        //播放对应动画
        animator.SetInteger("MoveNum", 0);

        //随机下次巡逻所需时间
        PatrolCD = Random.Range(orcKing.IdleCDMin, orcKing.IdleCDMax);
    }

    public override void OnExcute()
    {
        Gravity(); //模拟重力
        RefreshSkillCD(); //刷新技能CD

        //状态保护 进入动画之后才执行
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(aniName))
            return;

        if (orcKing.damage_)
        {
            //切换到 受伤状态
            if (manager.ChangeState<OrcKiStateDamage>())
                return;
        }

        //球形视野检测Player 巡逻半径
        viewPoint = transform.position;
        Collider[] players = Physics.OverlapSphere(viewPoint, orcKing.PatrolRadius, 1 << LayerMask.NameToLayer("Player"));
        
        foreach(var player in players)
        {
            Vector3 vec = player.transform.position - viewPoint;
            float angle = Vector3.Angle(transform.forward, vec);
            if (angle < orcKing.PatrolAngle / 2)
            {
                //切换到 追逐状态
                if (manager.ChangeState<OrcKiStateChase>())
                    return;
            }
            else
            {
                if (vec.magnitude < orcKing.PatrolBackRadius) //背面 有效半径
                {
                    //切换到 追逐状态
                    if (manager.ChangeState<OrcKiStateChase>())
                        return;
                }
            }
        }

        PatrolTime += Time.deltaTime; //累计时间

        if (PatrolTime > PatrolCD)
        {
            PatrolTime = 0; //归零累计时间
            //切换到 巡逻状态
            if (manager.ChangeState<OrcKiStatePatrol>())
                return;
        }
    }

    public override void OnExit()
    {
        
    }
}
