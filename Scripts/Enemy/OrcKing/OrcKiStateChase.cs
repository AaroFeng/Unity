using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcKiStateChase : OrcKiStateBase
{
    public override void OnInit()
    {
        base.OnInit();
        orcKiState = OrcKiState.Chase;
        aniName = "Chase"; 
    }

    public override void OnEnter()
    {
        speed = orcKing.ChaseSpeed; //设定水平移动速度

        //播放对应动画
        animator.SetInteger("MoveNum", 2);

        //设定速度
        agent.enabled = true; //打开自动寻路
        agent.speed = speed;
    }

    public override void OnExcute()
    {
        Gravity(); //模拟重力
        RefreshSkillCD(); //刷新技能CD

        //状态保护 进入动画之后才执行
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(aniName))
            return;

        if (GameObject.FindGameObjectWithTag("Player") == null) //找不到玩家
            return;

        if (orcKing.damage_)
        {
            //切换到 受伤状态
            if (manager.ChangeState<OrcKiStateDamage>())
                return;
        }

        //球形视野检测Player 追逐半径
        viewPoint = transform.position;
        Collider[] players = Physics.OverlapSphere(viewPoint, orcKing.ChaseRadius, 1 << LayerMask.NameToLayer("Player"));
        //计算与玩家方向与前方角度
        Vector3 vec = new Vector3();
        float angle = 0;

        if (players.Length < 1) //没有检测到玩家
        {
            //切换到 空闲状态
            if (manager.ChangeState<OrcKiStateIdle>())
                return;
        }
        else //检测到玩家
        {
            vec = players[0].transform.position - viewPoint;
            vec.y = 0;
            angle = Vector3.Angle(transform.forward, vec);

            if (angle > orcKing.ChaseAngle / 2) //大于 正面有效角度
            {
                if (vec.magnitude > orcKing.ChaseBackRadius) //背面有效半径
                {
                    //切换到 空闲状态
                    if (manager.ChangeState<OrcKiStateIdle>())
                        return;
                } 
            }
        }

        //击退技能
        if (vec.magnitude < orcKing.RepelRadius) //有效距离内
        {
            if (orcKing.RepelTime > orcKing.RepelCD) 
            {
                orcKing.RepelTime = 0; //重置CD累计时间
                //切换到 击退状态
                if (manager.ChangeState<OrcKiStateRepel>())
                    return;
            }
        }
        //在第一战斗阶段
        if (orcKing.Stage > 0)
        {
            //火焰喷射技能
            if (vec.magnitude < orcKing.FlameJetRadius)
            {
                if (orcKing.FlameJetTime > orcKing.FlameJetCD)
                {
                    orcKing.FlameJetTime = 0;
                    //切换到 火焰喷射状态
                    if (manager.ChangeState<OrcKiStateFlameJet>())
                        return;
                }
            }
            //玩家在正面直线方向上
            if (angle < 1.0f)
            {
                //发射火焰弹技能
                if (vec.magnitude < orcKing.BulletShootRadius && vec.magnitude > orcKing.BulletShootRadius * 0.2f)
                {
                    if (orcKing.BulletShootTime > orcKing.BulletShootCD)
                    {
                        orcKing.BulletShootTime = 0;
                        //切换到 发射火焰弹状态
                        if (manager.ChangeState<OrcKiStateBulletShoot>())
                            return;
                    }
                }
                //在第二战斗阶段
                if (orcKing.Stage > 1)
                {
                    //发射追踪弹技能
                    if (vec.magnitude < orcKing.TrackBulletShootRadius && vec.magnitude > orcKing.TrackBulletShootRadius * 0.2f)
                    {
                        if (orcKing.TrackBulletShootTime > orcKing.TrackBulletShootCD)
                        {
                            orcKing.TrackBulletShootTime = 0;
                            //切换到 发射追踪弹状态
                            if (manager.ChangeState<OrcKiStateTrackBulletShoot>())
                                return;
                        }
                    }
                    //发射范围弹技能
                    if (vec.magnitude < orcKing.RangeBulletShootRadius && vec.magnitude > orcKing.RangeBulletShootRadius * 0.2f)
                    {
                        if (orcKing.RangeBulletShootTime > orcKing.RangeBulletShootCD)
                        {
                            orcKing.RangeBulletShootTime = 0;
                            //切换到 发射范围弹状态
                            if (manager.ChangeState<OrcKiStateRangeBulletShoot>())
                                return;
                        }
                    }
                } 
            }
        }

        //判断距离 追逐 或 空闲站立
        if (vec.magnitude > orcKing.AtkRadius) //超出 普通攻击半径
        {
            //播放对应动画
            if (animator.GetFloat("ChaseBlend") > 0)
                animator.SetFloat("ChaseBlend", animator.GetFloat("ChaseBlend") - 0.03f);

            //打开自动寻路
            agent.enabled = true;
            agent.SetDestination(GameObject.FindGameObjectWithTag("Player").transform.position); //设定玩家位置为寻路点
        }
        else //小于 普通攻击半径
        {
            //播放对应动画
            if (animator.GetFloat("ChaseBlend") < 1)
                animator.SetFloat("ChaseBlend", animator.GetFloat("ChaseBlend") + 0.03f);

            //关闭自动寻路
            agent.enabled = false;
            //始终转向玩家
            transform.rotation = Quaternion.LookRotation(vec);

            //达到 普通攻击所需时间
            if (orcKing.AtkTime > orcKing.AtkCD)
            {
                orcKing.AtkTime = 0; //累计时间清零
                //切换到 普通攻击状态
                if (manager.ChangeState<OrcKiStateAttack>())
                    return;
            }
        }
    }

    public override void OnExit()
    {
        //关闭自动寻路
        agent.enabled = false;
    }
}
