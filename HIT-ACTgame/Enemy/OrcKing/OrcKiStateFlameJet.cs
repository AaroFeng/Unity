﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcKiStateFlameJet : OrcKiStateBase
{
    public override void OnInit()
    {
        base.OnInit();
        orcKiState = OrcKiState.FlameJet;
        aniName = "FlameJet";
    }

    public override void OnEnter()
    {
        //播放对应动画
        animator.SetInteger("Skill", 2);

        //播放粒子效果组
        particle.Play(orcKiState);
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
            //播放粒子效果组
            particle.Play(OrcKiState.Damage);
            //重置受伤触发
            orcKing.damage_ = false;
            //不切换受伤状态
        }

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.835f)
        {
            //停止粒子效果组
            particle.Stop(OrcKiState);
        }

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f)
        {
            //切换到追逐状态
            if (manager.ChangeState<OrcKiStateChase>())
                return;
        }
    }

    public override void OnExit()
    {
        //离开对应动画
        animator.SetInteger("Skill", 0);
    }
}
