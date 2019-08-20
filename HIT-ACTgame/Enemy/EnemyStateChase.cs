using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateChase : EnemyStateBase
{
    public override void OnInit()
    {
        base.OnInit();
        enemyState = EnemyState.Chase;
        aniName = "Chase"; 
    }

    public override void OnEnter()
    {
        speed = enemy.ChaseSpeed; //设定水平移动速度

        //播放对应动画
        animator.SetInteger("MoveNum", 2);

        //设定速度
        agent.enabled = true; //打开自动寻路
        agent.speed = speed;
    }

    public override void OnExcute()
    {
        enemy.AtkTime += Time.deltaTime; //累计攻击CD时间

        //状态保护 进入动画之后才执行
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(aniName))
            return;

        if (GameObject.FindGameObjectWithTag("Player") == null) //找不到玩家
            return;

        if (enemy.damage_)
        {
            //切换到 受伤状态
            if (manager.ChangeState<EnemyStateDamage>())
                return;
        }

        //没有寻路路径时 使用CC的move移动 防止卡死
        if (agent.enabled && !agent.hasPath)
        {
            Gravity();
            //计算玩家方向
            Vector3 direc = GameObject.FindGameObjectWithTag("Player").transform.position - transform.position;
            direc.y = 0; //忽略y值
            direc.Normalize(); //单位化
            direc *= speed; //乘速度
            //只改变水平向量值
            HoriMove.x = direc.x;
            HoriMove.z = direc.z;
            //进行移动
            cc.Move(HoriMove * Time.deltaTime);
        }

        //球形视野检测Player 追逐半径
        viewPoint = transform.position;
        Collider[] players = Physics.OverlapSphere(viewPoint, enemy.ChaseRadius, 1 << LayerMask.NameToLayer("Player"));
        Vector3 vec = new Vector3();
        float angle = 0;

        if (players.Length < 1) //没有检测到玩家
        {
            //切换到 空闲状态
            if (manager.ChangeState<EnemyStateIdle>())
                return;
        }
        else //检测到玩家 计算角度
        {
            vec = players[0].transform.position - viewPoint;
            vec.y = 0;
            angle = Vector3.Angle(transform.forward, vec);

            if (angle > enemy.ChaseAngle / 2) //大于 正面有效角度
            {
                if (vec.magnitude > enemy.ChaseBackRadius) //背面有效半径
                {
                    //切换到 空闲状态
                    if (manager.ChangeState<EnemyStateIdle>())
                        return;
                } 
            }
        }

        //判断距离 追逐 或 空闲站立
        if (vec.magnitude > enemy.AtkRadius) //超出 普通攻击半径
        {
            //播放对应动画
            if (animator.GetFloat("ChaseBlend") > 0)
                animator.SetFloat("ChaseBlend", animator.GetFloat("ChaseBlend") - enemy.ChaseBlendSpeed);

            //打开自动寻路
            agent.enabled = true;
            agent.SetDestination(GameObject.FindGameObjectWithTag("Player").transform.position); //设定玩家位置为寻路点
        }
        else //小于 普通攻击半径
        {
            //播放对应动画
            if (animator.GetFloat("ChaseBlend") < 1)
                animator.SetFloat("ChaseBlend", animator.GetFloat("ChaseBlend") + enemy.ChaseBlendSpeed);

            //关闭自动寻路
            agent.enabled = false;
            //始终转向玩家
            transform.rotation = Quaternion.LookRotation(vec);

            //达到 普通攻击所需时间
            if (enemy.AtkTime > enemy.AtkCD)
            {
                enemy.AtkTime = 0; //累计时间清零
                //切换到 普通攻击状态
                if (manager.ChangeState<EnemyStateAttack>())
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
