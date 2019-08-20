using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatePatrol : EnemyStateBase
{
    int point; //当前路径点
    int pointChange; //路径点改变值

    public override void OnInit()
    {
        base.OnInit();
        enemyState = EnemyState.Patrol;
        aniName = "Patrol";

        point = 0; //初始路径点
        pointChange = 1; //初始路径点改变值
    }

    public override void OnEnter()
    {
        speed = enemy.PatrolSpeed; //设定水平移动速度

        //播放对应动画
        animator.SetInteger("MoveNum", 1);
        //设定巡逻路径点 与 速度
        agent.enabled = true; //打开自动寻路
        agent.SetDestination(enemy.pathPoints[point].position);
        agent.speed = speed;

        //只有一个路径点
        if (enemy.pathPoints.Length < 2)
        {
            point = 0;
            float distance = Vector3.Distance(transform.position, enemy.pathPoints[point].position);
            if (distance < 0.3f) 
            {
                //进入空闲状态
                if (manager.ChangeState<EnemyStateIdle>())
                    return;
            }
        }    
        else
        {
            //设定下个路径点
            if (point == enemy.pathPoints.Length - 1) //到达终点
                pointChange = -1; //改变路径点修改方向
            else if (point == 0) //到达起点
                pointChange = 1; //改变路径点修改方向

            point += pointChange; //改变路径点                      
            agent.SetDestination(enemy.pathPoints[point].position); //设定寻路点
        }  
    }

    public override void OnExcute()
    {
        //状态保护 进入动画之后才执行
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(aniName))
            return;

        if (enemy.damage_)
        {
            //切换到 受伤状态
            if (manager.ChangeState<EnemyStateDamage>())
                return;
        }

        //球形视野检测Player 巡逻半径
        viewPoint = transform.position;
        Collider[] players = Physics.OverlapSphere(viewPoint, enemy.PatrolRadius, 1 << LayerMask.NameToLayer("Player"));
        foreach (var player in players)
        {
            Vector3 vec = player.transform.position - viewPoint;
            float angle = Vector3.Angle(transform.forward, vec);
            if (angle < enemy.PatrolAngle / 2)
            {
                //切换到 追逐状态
                if (manager.ChangeState<EnemyStateChase>())
                    return;
            }
            else
            {
                if (vec.magnitude < enemy.PatrolBackRadius) //背面 有效半径
                {
                    //切换到 追逐状态
                    if (manager.ChangeState<EnemyStateChase>())
                        return;
                }
            }
        }

        //没有寻路路径时 使用CC的move移动 防止卡死
        if (!agent.hasPath && enemy.pathPoints.Length > 1) 
        {
            Gravity();
            //计算路径点方向
            Vector3 direc = enemy.pathPoints[point].position - transform.position;
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
        float distance = Vector3.Distance(transform.position, enemy.pathPoints[point].position);
        if (distance < 0.3f)
        {
            //只有一个寻路点
            if (enemy.pathPoints.Length < 2)
            {
                //进入空闲状态
                if (manager.ChangeState<EnemyStateIdle>())
                    return;
            }

            //空闲状态巡逻点
            foreach (var pointIdle in enemy.pathPointsIdle)
            {
                if (enemy.pathPoints[point] == pointIdle)
                {
                    //设定转向 与 巡逻点一致
                    transform.rotation = enemy.pathPoints[point].rotation;

                    //判断当前路径点是第几个空闲点 设定空闲状态动画
                    if (enemy.pathPoints[point] == enemy.pathPointsIdle[0])
                        animator.SetFloat("Blend", 0.0f);
                    else if (enemy.pathPoints[point] == enemy.pathPointsIdle[1])
                        animator.SetFloat("Blend", 1.0f);
                    else if (enemy.pathPoints[point] == enemy.pathPointsIdle[2])
                        animator.SetFloat("Blend", 2.0f);
                    else if (enemy.pathPoints[point] == enemy.pathPointsIdle[3])
                        animator.SetFloat("Blend", 3.0f);
                    else
                        animator.SetFloat("Blend", 0.0f);

                    //进入空闲状态
                    if (manager.ChangeState<EnemyStateIdle>())
                        return;
                }
            }

            //不进入空闲状态 继续巡逻
            if (point == enemy.pathPoints.Length - 1) //到达终点
                pointChange = -1; //改变路径点修改方向
            else if (point == 0) //到达起点
                pointChange = 1; //改变路径点修改方向

            point += pointChange; //改变路径点                      
            agent.SetDestination(enemy.pathPoints[point].position); //设定寻路点
        }
    }

    public override void OnExit()
    {
        //关闭自动寻路
        agent.enabled = false;
    }
}
