using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateJump : PlayerStateBase
{
    float jumpSpeed = 12.0f; //跳跃垂直速度

    public override void OnInit()
    {
        base.OnInit();
        playerState = PlayerState.Jump; //记录状态
        aniName = "Jump"; //记录动画名

        speed = 8.0f; //设定水平移动速度
    }

    public override void OnEnter()
    {
        //进入状态 播放对应状态动画
        animator.SetBool(aniName, true);

        //开始跳跃时 获取一次输入
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(h, 0, v);
        if (move != Vector3.zero) //有输入
        {
            //根据主摄像机 将direction转换为相对于相机的世界坐标
            move = Camera.main.transform.TransformDirection(move);
            move.y = 0; //y轴值清零
            move.Normalize();  //单位化向量大小 修正因摄像机Rotation导致的数值波动
            transform.rotation = Quaternion.LookRotation(move); //转向移动方向  
        }

        //设定动画混合树数值 播放跳跃动画
        animator.SetFloat("BlendNum", Random.Range(0, 3));

        //水平移动 赋值给 最终移动方向
        move *= speed;
        vertiMove.y = jumpSpeed; //跳跃向上速度
        HoriMove.Set(move.x, 0, move.z); //水平移动速度

        onGround = true; //开始时在地面上
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
    }

    public override void OnExcute()
    {
        Gravity(); //模拟重力

        //状态保护 进入动画后才执行方法
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(aniName))
        {
            cc.Move(HoriMove * Time.deltaTime); //无间隙 直接进行跳跃移动
            return;
        }

        if (onGround && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.3f)
        {
            //检查是否可进行状态行为
            if (player.StateActionCheck(PlayerState.Move))
            {
                //切换到移动状态
                manager.ChangeState<PlayerStateMove>();
                return;
            }
        }

        //保持移动
        cc.Move(HoriMove * Time.deltaTime);
    }

    public override void OnExit()
    {
        //离开状态 离开对应状态动画
        animator.SetBool(aniName, false);
    }
}
