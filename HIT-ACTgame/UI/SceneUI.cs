using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneUI : CanvasUIBase
{
    //UI粒子效果
    public ParticleSystem dialogParticle; //对话气泡
    public Text textName; //目标名
    public Image guideImage; //指示图
    public Text textDialog; //对话内容

    //球形范围检测
    Vector3 checkPoint; //球形检测范围 球心 
    float radius = 6.0f; //球形检测范围 半径
    float validAngle = 120.0f; //球形检测范围 有效角度

    public override void OnUpdate()
    {
        SphereForeach();
    }

    //球形检测 是否触发场景UI事件
    void SphereForeach()
    {
        if (Camera.main == null)
            return;

        //更新球形检测球心位置 玩家位置
        checkPoint = player.transform.position;
        //球形范围检测 获得collider数组
        Collider[] targets = Physics.OverlapSphere(checkPoint, radius, 1 << LayerMask.NameToLayer("SceneUI"));

        if (targets.Length < 1) //范围内无目标
        {
            //场景UI不激活
            if (dialogParticle.isPlaying)
                dialogParticle.Stop();
            textName.gameObject.SetActive(false);
            guideImage.gameObject.SetActive(false);
            textDialog.gameObject.SetActive(false);
            return;
        }

        //遍历 判断是否在 有效检测范围内
        Vector3 cameraforward = Camera.main.transform.forward; //摄像机前方向量
        cameraforward.y = 0; //忽略y值
        Collider faceTarget = targets[0]; //记录玩家面对 目标
        foreach (var target in targets)
        {
            //遍历寻找距离最近的目标
            //计算并比较夹角大小
            Vector3 oldVec = faceTarget.transform.position - checkPoint; //计算旧目标方向
            Vector3 newVec = target.transform.position - checkPoint; //计算新目标方向
            float oldAngle = Vector3.Angle(cameraforward, oldVec);
            float newAngle = Vector3.Angle(cameraforward, newVec);
            if (newAngle < oldAngle)
                faceTarget = target;
        }

        //判断 面对目标 与摄像机夹角
        Vector3 vec = faceTarget.transform.position - checkPoint; //计算player到target的向量
        vec.y = 0; //忽略高度差
        float angle = Vector3.Angle(cameraforward, vec); //计算 player前方向量 与 target方向向量 的 夹角
        //如果夹角 小于 有效角度
        if (angle < validAngle / 2)
        {
            //判断 面对目标Tag
            if (faceTarget.CompareTag("NPC")) //电脑角色
            {
                NPCCharacterBase npc = faceTarget.GetComponent<NPCCharacterBase>(); //获取目标NPC特性
                //目标名字位置
                textName.gameObject.SetActive(true); //激活物体
                Vector3 posTextName = npc.transform.position;
                posTextName.y += npc.Height + 0.15f; //高于NPC
                textName.transform.position = Camera.main.WorldToScreenPoint(posTextName); //世界坐标转屏幕坐标
                //目标名字大小缩放
                float newSize0 = 5.0f / Vector3.Distance(posTextName, Camera.main.transform.position);
                textName.transform.localScale = Vector3.one * newSize0;
                //目标名字内容
                textName.text = npc.GetComponent<NPCCharacterBase>().RoleName; //获取目标NPC名字

                //距离小于2.0 开启对话选项
                if (vec.magnitude < 2.0f)
                {
                    //对话气泡位置
                    Vector3 posDialogPar = npc.transform.position;
                    posDialogPar.y += npc.Height - 0.2f;
                    posDialogPar += Camera.main.transform.right * -0.5f;
                    dialogParticle.transform.position = posDialogPar;
                    //播放粒子效果
                    if (!dialogParticle.isPlaying)
                        dialogParticle.Play();

                    //指示图位置
                    guideImage.gameObject.SetActive(true);
                    Vector3 posGuideImage = npc.transform.position;
                    posGuideImage.y += npc.Height + 0.35f;
                    guideImage.transform.position = Camera.main.WorldToScreenPoint(posGuideImage);
                    //指示图大小缩放
                    float newSize1 = 5.0f / Vector3.Distance(posGuideImage, Camera.main.transform.position);
                    guideImage.transform.localScale = Vector3.one * newSize1;
                    //指示图sprite
                    guideImage.overrideSprite = Resources.Load("Prefab/UI/GuideImageE") as Sprite;
                }
                else
                {
                    //不激活 对话气泡 指示图
                    if (dialogParticle.isPlaying)
                        dialogParticle.Stop();
                    guideImage.gameObject.SetActive(false);
                }

                return;
            }
            else if (faceTarget.CompareTag("AccessPoint")) //出入口
            {
                //目标名字位置
                textName.gameObject.SetActive(true); //激活物体
                Vector3 posTextName = faceTarget.transform.position;
                posTextName.y += 1.5f;
                textName.transform.position = Camera.main.WorldToScreenPoint(posTextName); //世界坐标转屏幕坐标
                //目标名字大小缩放
                float newSize0 = 8.0f / Vector3.Distance(posTextName, Camera.main.transform.position);
                textName.transform.localScale = Vector3.one * newSize0;
                //目标名字内容
                switch (faceTarget.name)
                {
                    case "AccessPoint1-2":
                        if (SceneManager.GetActiveScene().name == "Village")
                            textName.text = "山林小路"; //设定文字内容
                        else
                            textName.text = "村庄";
                        break;
                    case "AccessPoint2-3":
                        if (SceneManager.GetActiveScene().name == "ForestRoad")
                            textName.text = "兽人山林";
                        else
                            textName.text = "山林小路";
                        break;
                    default:
                        break;
                }

                //距离小于2.0 开启操作选项
                if (vec.magnitude < 2.0f)
                {
                    //指示图位置
                    guideImage.gameObject.SetActive(true);
                    Vector3 posGuideImage = faceTarget.transform.position;
                    posGuideImage.y += 1.8f;
                    guideImage.transform.position = Camera.main.WorldToScreenPoint(posGuideImage);
                    //指示图大小缩放
                    float newSize1 = 8.0f / Vector3.Distance(posGuideImage, Camera.main.transform.position);
                    guideImage.transform.localScale = Vector3.one * newSize1;
                    //指示图sprite
                    guideImage.overrideSprite = Resources.Load("Prefab/UI/GuideImageE") as Sprite;
                    //键盘E键按下
                    if (Input.GetKeyDown(KeyCode.E))
                        EnterAccessPoint(faceTarget.name);
                }
                else //不激活 指示图
                    guideImage.gameObject.SetActive(false);

                return;
            }
        }

        //目标不在有效角度内 场景UI不激活
        if (dialogParticle.isPlaying)
            dialogParticle.Stop();
        guideImage.gameObject.SetActive(false);
        textName.gameObject.SetActive(false);
        textDialog.gameObject.SetActive(false);
    }

    //进入出入口
    void EnterAccessPoint(string accessPointName)
    {
        //判断出入口名字 与 当前场景
        switch(accessPointName)
        {
            case "AccessPoint1-2":
                if (SceneManager.GetActiveScene().name == "Village") 
                    sceneManager.LoadScene("ForestRoad",accessPointName);
                else
                    sceneManager.LoadScene("Village", accessPointName);
                break;
            case "AccessPoint2-3":
                if (SceneManager.GetActiveScene().name == "ForestRoad")
                    sceneManager.LoadScene("Forest", accessPointName);
                else
                    sceneManager.LoadScene("ForestRoad", accessPointName);
                break;
            default:

                break;
        }
    }
}
