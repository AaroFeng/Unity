using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateAttack : EnemyStateBase
{
    public override void OnInit()
    {
        base.OnInit();
        enemyState = EnemyState.Attack;
        aniName = "Attack";
    }

    public override void OnEnter()
    {
        //播放对应动画
        animator.SetBool("Attack", true);

        //播放粒子效果组
        particle.Play(enemyState);
    }

    public override void OnExcute()
    {
        Gravity(); //模拟重力

        //状态保护 进入动画之后才执行
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(aniName))
            return;

        if (enemy.damage_)
        {
            //切换到 受伤状态
            if (manager.ChangeState<EnemyStateDamage>())
                return;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.8f)
        {
            //切换到攻击空闲状态
            if (manager.ChangeState<EnemyStateChase>())
                return;
        }

    }

    public override void OnExit()
    {
        //离开对应动画
        animator.SetBool("Attack", false);

        //停止粒子效果组
        particle.Stop(EnemyState);
    }
}
