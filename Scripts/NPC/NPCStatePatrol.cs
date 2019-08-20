using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCStatePatrol : NPCStateBase
{
    int point; //当前路径点
    int pointChange; //路径点改变值

    public override void OnInit()
    {
        base.OnInit();
        npcState = NPCState.Patrol;
        aniName = "Patrol";

        point = 0; //初始路径点
        pointChange = 1; //初始路径点改变值
    }

    public override void OnEnter()
    {
        speed = manager.PatrolSpeed; //设定水平移动速度

        //播放对应动画
        animator.SetInteger("Move", 1);
        //设定巡逻路径点 与 速度
        agent.enabled = true; //打开自动寻路
        agent.SetDestination(npc.pathPoints[point].position);
        agent.speed = speed;

        //只有一个路径点
        if (npc.pathPoints.Length < 2)
        {
            point = 0;
            float distance = Vector3.Distance(transform.position, npc.pathPoints[point].position);
            if (distance < 0.3f)
            {
                //进入空闲状态
                if (manager.ChangeState<NPCStateIdle>()) 
                    return;
            }
        }
        else
        {
            //设定下个路径点
            if (point == npc.pathPoints.Length - 1) //到达终点
                pointChange = -1; //改变路径点修改方向
            else if (point == 0) //到达起点
                pointChange = 1; //改变路径点修改方向

            point += pointChange; //改变路径点                      
            agent.SetDestination(npc.pathPoints[point].position); //设定寻路点
        }
    }

    public override void OnExcute()
    {
        //状态保护 进入动画之后才执行
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(aniName))
            return;

        //计算到路径点的距离 进行移动
        float distance = Vector3.Distance(transform.position, npc.pathPoints[point].position);
        if (distance < 0.3f)
        {
            //只有一个寻路点
            if (npc.pathPoints.Length < 2)
            {
                //进入空闲状态
                if (manager.ChangeState<NPCStateIdle>())
                    return;
            }

            //空闲状态巡逻点
            foreach (var pointIdle in npc.pathPointsIdle)
            {
                if (npc.pathPoints[point] == pointIdle) 
                {
                    //设定转向 与 巡逻点一致
                    transform.rotation = npc.pathPoints[point].rotation;

                    //判断当前路径点是第几个空闲点 设定空闲状态动画
                    if (npc.pathPoints[point] == npc.pathPointsIdle[0])
                        animator.SetFloat("Blend", 0.0f);
                    else if (npc.pathPoints[point] == npc.pathPointsIdle[1])
                        animator.SetFloat("Blend", 1.0f);
                    else if (npc.pathPoints[point] == npc.pathPointsIdle[2])
                        animator.SetFloat("Blend", 2.0f);
                    else if (npc.pathPoints[point] == npc.pathPointsIdle[3])
                        animator.SetFloat("Blend", 3.0f);
                    else if (npc.pathPoints[point] == npc.pathPointsIdle[4])
                        animator.SetFloat("Blend", 4.0f);
                    else if (npc.pathPoints[point] == npc.pathPointsIdle[5])
                        animator.SetFloat("Blend", 5.0f);
                    else if (npc.pathPoints[point] == npc.pathPointsIdle[6])
                        animator.SetFloat("Blend", 6.0f);
                    else
                        animator.SetFloat("Blend", 0.0f);

                    //进入空闲状态
                    if (manager.ChangeState<NPCStateIdle>())
                        return;
                }
            }

            //不进入空闲状态 继续巡逻
            if (point == npc.pathPoints.Length - 1) //到达终点
                pointChange = -1; //改变路径点修改方向
            else if (point == 0) //到达起点
                pointChange = 1; //改变路径点修改方向

            point += pointChange; //改变路径点                      
            agent.SetDestination(npc.pathPoints[point].position); //设定寻路点
        }
    }

    public override void OnExit()
    {
        //关闭自动寻路
        agent.enabled = false;
    }
}
