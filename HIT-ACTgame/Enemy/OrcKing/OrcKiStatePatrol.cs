using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcKiStatePatrol : OrcKiStateBase
{
    int point; //当前路径点
    int pointChange; //路径点改变值

    public override void OnInit()
    {
        base.OnInit();
        orcKiState = OrcKiState.Patrol;
        aniName = "Patrol";

        point = 0; //初始路径点
        pointChange = 1; //初始路径点改变值
    }

    public override void OnEnter()
    {
        speed = orcKing.PatrolSpeed; //设定水平移动速度

        //播放对应动画
        animator.SetInteger("MoveNum", 1);
        //设定巡逻路径点 与 速度
        agent.enabled = true; //打开自动寻路
        agent.SetDestination(orcKing.pathPoints[point].position);
        agent.speed = speed;

        //只有一个路径点
        if (orcKing.pathPoints.Length < 2)
        {
            point = 0;
            float distance = Vector3.Distance(transform.position, orcKing.pathPoints[point].position);
            if (distance < 0.3f) 
            {
                //进入空闲状态
                if (manager.ChangeState<OrcKiStateIdle>())
                    return;
            }
        }    
        else
        {
            //设定下个路径点
            if (point == orcKing.pathPoints.Length - 1) //到达终点
                pointChange = -1; //改变路径点修改方向
            else if (point == 0) //到达起点
                pointChange = 1; //改变路径点修改方向

            point += pointChange; //改变路径点                      
            agent.SetDestination(orcKing.pathPoints[point].position); //设定寻路点
        }  
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
        foreach (var player in players)
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

        //没有寻路路径时 使用CC的move移动 防止卡死
        if (!agent.hasPath && orcKing.pathPoints.Length > 1) 
        {
            base.OnExcute(); //模拟重力
            //计算路径点方向
            Vector3 direc = orcKing.pathPoints[point].position - transform.position;
            direc.y = 0; //忽略y值
            direc.Normalize(); //单位化
            direc *= speed; //乘速度
            //只改变水平向量值
            HoriMove.x = direc.x; 
            HoriMove.z = direc.z;
            //进行移动
            cc.Move(HoriMove * Time.deltaTime);
        }

        //计算敌人到路径点的距离 进行移动
        float distance = Vector3.Distance(transform.position, orcKing.pathPoints[point].position);
        if (distance < 0.3f)
        {
            //只有一个寻路点
            if (orcKing.pathPoints.Length < 2)
            {
                //进入空闲状态
                if (manager.ChangeState<OrcKiStateIdle>())
                    return;
            }

            //空闲状态巡逻点
            foreach (var pointIdle in orcKing.pathPointsIdle)
            {
                if (orcKing.pathPoints[point] == pointIdle)
                {
                    //设定转向 与 巡逻点一致
                    transform.rotation = orcKing.pathPoints[point].rotation;

                    //判断当前路径点是第几个空闲点 设定空闲状态动画
                    if (orcKing.pathPoints[point] == orcKing.pathPointsIdle[0])
                        animator.SetFloat("Blend", 0.0f);
                    else if (orcKing.pathPoints[point] == orcKing.pathPointsIdle[1])
                        animator.SetFloat("Blend", 1.0f);
                    else if (orcKing.pathPoints[point] == orcKing.pathPointsIdle[2])
                        animator.SetFloat("Blend", 2.0f);
                    else if (orcKing.pathPoints[point] == orcKing.pathPointsIdle[3])
                        animator.SetFloat("Blend", 3.0f);
                    else
                        animator.SetFloat("Blend", 0.0f);

                    //进入空闲状态
                    if (manager.ChangeState<OrcKiStateIdle>())
                        return;
                }
            }

            //不进入空闲状态 继续巡逻
            if (point == orcKing.pathPoints.Length - 1) //到达终点
                pointChange = -1; //改变路径点修改方向
            else if (point == 0) //到达起点
                pointChange = 1; //改变路径点修改方向

            point += pointChange; //改变路径点                      
            agent.SetDestination(orcKing.pathPoints[point].position); //设定寻路点
        }
    }

    public override void OnExit()
    {
        //关闭自动寻路
        agent.enabled = false;
    }
}
