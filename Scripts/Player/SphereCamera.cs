using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereCamera : MonoBehaviour
{
    Transform player; //玩家
    public Transform skyBox; //天空盒

    public Transform watchPoint; //注视目标点
    float watchPointHeight = 1.7f; //注视目标点高度

    float distance = 2.8f; //当前 摄像机到目标点距离
    public float Distance { get { return distance; } }
    float distanceMax = 5.0f; //到目标点最大距离
    float distanceMin = 1.0f; //到目标点最小距离
    float distanceSpeed = 0.3f; //距离增减速度

    float rotationY; //水平旋转
    float rotationYSpeed = 1.5f; //水平旋转速度

    float angleLerp; //当前垂直角度 插值系数
    public float AngleLerp { get { return angleLerp; } }
    float angleMax = 80.0f; //最大垂直角度
    float angleMin = -60.0f; //最小垂直角度
    float angleSpeed = 0.02f; //垂直旋转速度

    float moveSpeed = 0.3f; //相机移动速度 差值实现
    Vector3 finalVec = new Vector3(); //最终偏移向量

    //相机震动参数
    bool shake = false; //是否震动
    float scale; //震动幅度
    float shakeSpeed = 1.0f; //震动速度 相机移动速度的倍数
    float shakeHz; //震动频率
    float shakeHzTime; //震动频率累计时间
    float shakeTime; //震动时长
    float deltaTime; //震动累计时间
    Vector3 shakeVec = new Vector3(); //震动偏移向量

    void OnEnable()
    {
        //寻找标签 获得玩家Transform
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            //初始化摄像机位置
            SetCameraPos(2.8f, 0f, player.rotation);
        }
    }

    void Update()
    {
        if (!gameObject.activeSelf)
            return;

        ChangeDistance(); //滚轮增减 摄像机到目标点距离
        ChangeRotationY(); //水平旋转
        ChangeAngle(); //垂直旋转
        FinalCameraPos(); //摄像机最终位置

        skyBox.position = player.position; //天空盒跟随玩家
    }

    void ChangeDistance() //滚轮增减 摄像机到目标点距离
    {
        //接收鼠标滚轮输入
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            //改变 当前到目标点距离
            distance += distanceSpeed;
            //限制 到目标点距离最大值
            if (distance > distanceMax)
            {
                distance = distanceMax;
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            //改变 当前到目标点距离
            distance -= distanceSpeed;
            //限制 到目标点距离最小值
            if (distance < distanceMin)
            {
                distance = distanceMin;
            }
        }
    }

    void ChangeRotationY() //水平旋转
    {
        //获得鼠标X轴移动值 修改水平旋转值
        rotationY = Input.GetAxis("Mouse X") * rotationYSpeed;
        //注视目标点 水平旋转
        watchPoint.Rotate(0, rotationY, 0);
    }

    void ChangeAngle() //垂直旋转
    {
        //鼠标Y轴移动 修改 垂直角度插值系数
        angleLerp -= Input.GetAxis("Mouse Y") * angleSpeed;

        //限制 垂直角度插值系数 最大最小值
        if (angleLerp > angleMax / 90.0f) //除90度 获得 最大 垂直角度插值系数
        {
            angleLerp = angleMax / 90.0f;
        }
        else if (angleLerp < angleMin / 90.0f) 
        {
            angleLerp = angleMin / 90.0f;
        }

        //判断 当前 垂直角度插值系数 正负
        //根据 注视目标点的 后方向量 上方或下方向量 与 当前垂直角度插值系数
        //获得 偏移向量的方向
        if (angleLerp > 0)
            finalVec = Vector3.Lerp(-watchPoint.forward, watchPoint.up, angleLerp);
        else
            finalVec = Vector3.Lerp(-watchPoint.forward, -watchPoint.up, -angleLerp); //垂直角度插值系数 取正

        finalVec.Normalize(); //单位化偏移向量长度为1
        finalVec *= distance; //设定 偏移向量的长度
    }

    void FinalCameraPos() //摄像机最终位置
    {
        if (shake) Shakeing(); //震动偏移

        //注视目标点位置 跟随 玩家位置
        Vector3 PointPos = player.position;
        PointPos.y += watchPointHeight; //修改 注视目标点 高度
        PointPos += shakeVec; //增加震动偏移
        watchPoint.position = PointPos;

        //摄像机位置 根据注视目标点位置 进行偏移 增加震动偏移
        Vector3 cameraPos = watchPoint.position + finalVec + shakeVec;
        CoverCheck(ref cameraPos); //检查是否被遮挡
        //弹簧移动效果 插值实现
        transform.position = Vector3.Lerp(transform.position, cameraPos, moveSpeed * shakeSpeed);
        //摄像机看向 注视目标点
        transform.LookAt(watchPoint.position); 
    }

    void CoverCheck(ref Vector3 cameraPos) //检查是否被遮挡
    {
        RaycastHit hit; //碰撞信息
        //注视目标点 向 摄像机方向 发射射线 忽略玩家层
        Physics.Raycast(watchPoint.position, finalVec, out hit, distance, ~(1 << LayerMask.NameToLayer("Player")));

        //非玩家碰撞器
        if (hit.collider != null)
            cameraPos = hit.point; //修改摄像机新位置为碰撞点
    }

    public void Shake(float scale, float shakeSpeed, float shakeHz, float shakeTime)
    {
        deltaTime = 0f; //归零震动累计时间 连续震动

        this.scale = scale; //设定震动幅度
        this.shakeSpeed = shakeSpeed; //设定震动速度
        this.shakeHz = shakeHz; //设定震动频率
        this.shakeTime = shakeTime; //设定震动幅度
        shakeHzTime = shakeHz; //直接开始第一次震动
        shake = true; //开始震动
    }

    void Shakeing()
    {
        shakeHzTime += Time.deltaTime; //累计震动频率时间
        deltaTime += Time.deltaTime; //累计震动时间

        shakeSpeed *= 0.95f; //震动速度衰减

        //是否到达震动时长
        if (deltaTime > shakeTime)
        {
            deltaTime = 0f; //归零震动累计时间
            shakeSpeed = 1.0f; //重置震动速度
            shakeVec = Vector3.zero; //归零震动偏移
            shake = false; //结束震动
        }

        //是否到达震动频率
        if (shakeHzTime > shakeHz)
        {
            shakeHzTime = 0f; //归零
            shakeHz *= 1.1f; //震动频率衰减
            //相机震动偏移向量
            shakeVec = Random.onUnitSphere * scale;
        }
    }

    public void SetCameraPos(float distance,float angleLerp,Quaternion horiRot)
    {
        //初始化注视目标点位置
        Vector3 PointPos = player.position;
        PointPos.y += watchPointHeight;
        watchPoint.transform.position = PointPos;
        watchPoint.rotation = horiRot;

        //设置摄像机位置
        this.distance = distance;
        this.angleLerp = angleLerp;
        ChangeAngle();
        //摄像机位置 根据注视目标点位置 进行偏移
        Vector3 cameraPos = watchPoint.position + finalVec;
        //直接设置位置
        transform.position = cameraPos;
        //摄像机看向 注视目标点
        transform.LookAt(watchPoint.position);
    }
}
