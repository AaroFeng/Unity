using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateThump1 : PlayerStateBase
{
    public override void OnInit()
    {
        base.OnInit();
        playerState = PlayerState.Thump1; //记录状态
        aniName = "Thump1"; //记录动画名
    }

    public override void OnEnter()
    {
        //进入状态 播放对应状态动画
        animator.SetInteger("Thump", 1);

        //开始时 获取一次输入 进行转向
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        Vector3 turn = new Vector3(h, 0, v);
        //判断动画 根据摄像机方向转换direction 玩家转向
        if (turn != Vector3.zero) //有输入
        {
            //根据主摄像机 将direction转换为相对于相机的世界坐标
            turn = Camera.main.transform.TransformDirection(turn);
            turn.y = 0; //y轴值清零
            turn.Normalize();  //单位化向量大小 修正因摄像机Rotation导致的数值波动

            transform.rotation = Quaternion.LookRotation(turn); //转向输入方向
        }

        //播放粒子效果组
        particle.Play(playerState);
    }

    public override void OnControl()
    {
        //状态保护 进入动画后才执行方法
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(aniName))
            return;

        //检查是否可进行状态行为
        if (player.StateActionCheck(PlayerState.Damage))
        {
            //不切换到受伤状态
        }

        //重击1动画结束后
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.85f)
        {
            //检查是否可进行状态行为
            if (player.StateActionCheck(PlayerState.Move))
            {
                //切换到移动状态
                manager.ChangeState<PlayerStateMove>();
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

        //应用动画位移

        //相机震动
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.15f &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.18f)
            camera.Shake(0.02f, 6.0f, 0.002f, 0.2f);
        else if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.36f &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.80f)
            camera.Shake(0.03f, 5.0f, 0.001f, 0.3f);
    }

    public override void OnExit()
    {
        //离开状态 离开对应状态动画
        animator.SetInteger("Thump", 0);

        //停止粒子效果组
        particle.Stop(playerState);
    }
}
