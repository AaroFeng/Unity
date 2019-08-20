using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateGuardDamage : PlayerStateBase
{
    public override void OnInit()
    {
        base.OnInit();
        playerState = PlayerState.GuardDamage; //记录状态
        aniName = "GuardDamage"; //记录动画名
    }

    public override void OnEnter()
    {
        //进入状态 播放对应状态动画
        animator.SetInteger("Damage", 2);

        //播放粒子效果组
        particle.Play(playerState);
    }

    public override void OnControl()
    {
        //状态保护 进入动画后才执行方法
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(aniName))
            return;

        //重置再次受伤
        if (animator.GetBool("DamageAgain"))
            animator.SetBool("DamageAgain", false);

        //耐力值是否耗尽
        if (player.StateActionCheck(PlayerState.GuardBreak))
        {
            //切换到防卫破败状态
            manager.ChangeState<PlayerStateGuardBreak>();
            return;
        }

        //是否连续受伤
        if (player.StateActionCheck(PlayerState.GuardDamage))
        {
            //连续受伤
            animator.SetBool("DamageAgain", true);

            //切换到防卫受伤状态
            manager.ChangeState<PlayerStateGuardDamage>();
            return;
        }

        if (Input.GetKeyDown(KeyCode.R)) //键盘R键按下
        {
            if (player.StateActionCheck(PlayerState.Skill1))
            {
                //切换到技能1状态
                manager.ChangeState<PlayerStateSkill1>();
                return;
            }
        }

        //受伤动画结束后
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.85f)
        {
            if (player.StateActionCheck(PlayerState.Death))
            {
                //切换到 死亡状态
                manager.ChangeState<PlayerStateDeath>();
                return;
            }

            if (player.StateActionCheck(PlayerState.Guard))
            {
                //切换到防卫状态
                manager.ChangeState<PlayerStateGuard>();
                return;
            }
        }
    }

    public override void OnExcute()
    {
        Gravity(); //模拟重力

        //状态保护 进入动画后才执行方法
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(aniName))
            return;
    }

    public override void OnExit()
    {
        //离开状态 离开对应状态动画
        animator.SetInteger("Damage", 0);

        //停止粒子效果组
        particle.Stop(playerState);
    }
}
