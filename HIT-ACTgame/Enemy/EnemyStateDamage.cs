﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateDamage : EnemyStateBase
{
    public override void OnInit()
    {
        base.OnInit();
        enemyState = EnemyState.Damage;
        aniName = "Damage";
    }

    public override void OnEnter()
    {
        //播放对应动画
        animator.SetBool(aniName, true);
        //重置受伤触发
        enemy.damage_ = false;

        //冲击力 赋值
        HoriMove.Set(enemy.DamageImpact.x, 0, enemy.DamageImpact.z); //水平方向移动
        vertiMove.Set(0, enemy.DamageImpact.y, 0); //垂直方向移动 重力

        //播放粒子效果组
        particle.Play(enemyState);
    }

    public override void OnExcute()
    {
        Gravity(); //模拟重力

        //状态保护 进入动画之后才执行
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(aniName))
            return;

        //重置再次受伤
        if (animator.GetBool("DamageAgain") && !enemy.Death)
            animator.SetBool("DamageAgain", false);

        if (enemy.damage_ && !enemy.Death)
        {
            //连续受伤
            animator.SetBool("DamageAgain", true);
            //切换到 受伤状态
            if (manager.ChangeState<EnemyStateDamage>())
                return;
        }

        if(onGround) //判断是否落地
        {
            //受伤动画结束后
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.85f)
            {
                //是否死亡
                if (enemy.Death)
                {
                    //切换到 死亡状态
                    if (manager.ChangeState<EnemyStateDeath>())
                        return;
                }

                //切换到追逐状态
                if (manager.ChangeState<EnemyStateChase>())
                    return;
                //无追逐状态 切换空闲状态
                else if (manager.ChangeState<EnemyStateIdle>())
                    return;
            }
        }

        //----------

        //水平移动速度随时间衰减
        if (HoriMove.magnitude > 0.05f)
        {
            if(onGround)
                HoriMove += -HoriMove * Time.deltaTime * 10.0f;
            else
                HoriMove += -HoriMove * Time.deltaTime * 3.0f;
        }    
        else
            HoriMove = Vector3.zero;
        
        //动态设定水平速度
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.70f)
            cc.Move(HoriMove * Time.deltaTime);
    }

    public override void OnExit()
    {
        //退出对应动画
        animator.SetBool(aniName, false);

        //停止粒子效果组
        particle.Stop(enemyState);
    }
}
