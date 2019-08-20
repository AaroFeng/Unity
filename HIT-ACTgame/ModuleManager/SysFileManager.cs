using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class SysFileManager : SysModule
{
    //初始化
    public override bool Initialize()
    {
        return true;
    }

    //开始执行
    public override void Run(object userData)
    {

    }

    //更新
    public override void OnUpdate()
    {

    }

    //释放
    public override void Dispose()
    {

    }

    //保存游戏档案
    public void SaveGameFile()
    {
        FileGameSave save = CreateFileGameSave(); //保存当前场景信息与玩家位置状态

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = new FileStream(Application.persistentDataPath + "/gamefile.dat", FileMode.OpenOrCreate); //文件流 保存路径
        bf.Serialize(file, save); //序列化保存

        file.Close();

        SysModuleManager.Instance.GetSysModule<SysUIEnv>().DialogCanvas.Tips("保存进度成功"); //UI提示
    }

    private FileGameSave CreateFileGameSave()
    {
        FileGameSave save = new FileGameSave();

        save.sceneName = SysModuleManager.Instance.GetSysModule<SysSceneManager>().NowScene; //记录当前场景

        PlayerCharacter player = SysModuleManager.Instance.GetSysModule<SysPlayerManager>().PlayerRole.GetComponent<PlayerCharacter>(); //获取当前玩家角色属性脚本
        //记录玩家当前位置与属性状态
        save.player.PosX = player.transform.position.x; //位置
        save.player.PosY = player.transform.position.y;
        save.player.PosZ = player.transform.position.z;
        save.player.RotX = player.transform.rotation.x; //旋转
        save.player.RotY = player.transform.rotation.y;
        save.player.RotZ = player.transform.rotation.z;
        save.player.RotW = player.transform.rotation.w;
        save.player.atk = player.Atk; //攻击力
        save.player.hp = player.Hp; //体力值
        save.player.hpMax = player.HpMax; //最大体力值
        save.player.sp = player.Sp; //耐力值
        save.player.spMax = player.SpMax; //最大耐力值
        save.player.ep = player.Ep; //能量值
        save.player.epMax = player.EpMax; //最大能量值
        save.player.epPoint = player.EpPoint; //能量点
        save.player.epPointMax = player.EpPointMax; //最大能量点
        save.player.spUseup = player.SpUseup; //耐力耗尽
        //记录玩家摄像机位置与水平旋转
        save.player.cameraRotX = Camera.main.GetComponent<SphereCamera>().watchPoint.rotation.x;
        save.player.cameraRotY = Camera.main.GetComponent<SphereCamera>().watchPoint.rotation.y;
        save.player.cameraRotZ = Camera.main.GetComponent<SphereCamera>().watchPoint.rotation.z;
        save.player.cameraRotW = Camera.main.GetComponent<SphereCamera>().watchPoint.rotation.w;
        save.player.cameraDistance = Camera.main.GetComponent<SphereCamera>().Distance;
        save.player.cameraAngleLerp = Camera.main.GetComponent<SphereCamera>().AngleLerp;
        
        //记录场景内敌人集合位置与属性状态
        GameObject[] sceneEnemys = GameObject.FindGameObjectsWithTag("Enemy"); //查找全场景敌人
        for (int i = 0; i < sceneEnemys.Length; i++)
        {
            FileEnemy enemy = new FileEnemy();
            enemy.PosX = sceneEnemys[i].transform.position.x; //记录位置
            enemy.PosY = sceneEnemys[i].transform.position.y;
            enemy.PosZ = sceneEnemys[i].transform.position.z;
            enemy.RotX = sceneEnemys[i].transform.rotation.x; //旋转
            enemy.RotY = sceneEnemys[i].transform.rotation.y;
            enemy.RotZ = sceneEnemys[i].transform.rotation.z;
            enemy.RotW = sceneEnemys[i].transform.rotation.w;
            enemy.hp = sceneEnemys[i].GetComponent<EnemyCharacterBase>().Hp; //记录体力值
            enemy.death = sceneEnemys[i].GetComponent<EnemyCharacterBase>().Death; //记录死亡状态

            save.enemys.Add(enemy); //加入存档的敌人集合
        }

        //记录场景内兽人首领位置
        GameObject sceneOrcKing = GameObject.FindGameObjectWithTag("OrcKing"); //查找全场景敌人
        if (sceneOrcKing != null)
        {
            FileOrcKing orcking = new FileOrcKing();
            orcking.PosX = sceneOrcKing.transform.position.x; //记录位置
            orcking.PosY = sceneOrcKing.transform.position.y;
            orcking.PosZ = sceneOrcKing.transform.position.z;
            orcking.RotX = sceneOrcKing.transform.rotation.x; //旋转
            orcking.RotY = sceneOrcKing.transform.rotation.y;
            orcking.RotZ = sceneOrcKing.transform.rotation.z;
            orcking.RotW = sceneOrcKing.transform.rotation.w;
            orcking.hp = sceneOrcKing.GetComponent<OrcKiCharacter>().Hp; //记录体力值
            orcking.death = sceneOrcKing.GetComponent<OrcKiCharacter>().Death; //记录死亡状态

            save.orcKing = orcking; //加入存档的敌人集合
        }

        return save;
    }

    //载入游戏档案
    public void LoadGameFile()
    {
        //检查是否有存档文件
        if (File.Exists(Application.persistentDataPath + "/gamefile.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = new FileStream(Application.persistentDataPath + "/gamefile.dat", FileMode.Open); //打开指定路径文件
            FileGameSave save = bf.Deserialize(file) as FileGameSave; //反序列化游戏存档数据
            file.Close();

            //重新载入场景并设置玩家位置与属性状态
            SysModuleManager.Instance.GetSysModule<SysSceneManager>().LoadScene(save.sceneName, "Load");
            StartCoroutine("GameFileSet", save);
        }
        else
            SysModuleManager.Instance.GetSysModule<SysUIEnv>().DialogCanvas.Tips("无进度存档"); //UI提示;
    }

    IEnumerator GameFileSet(FileGameSave save)
    {
        yield return null;

        while (true)
        {
            //场景是否加载完成
            if (SysModuleManager.Instance.GetSysModule<SysSceneManager>().NowScene.Equals(save.sceneName))
                break;
            else
                yield return null;
        }

        yield return null;

        //载入玩家当前位置与属性状态
        PlayerCharacter player = SysModuleManager.Instance.GetSysModule<SysPlayerManager>().PlayerRole.GetComponent<PlayerCharacter>();
        player.transform.position = new Vector3(save.player.PosX, save.player.PosY, save.player.PosZ); //位置
        player.transform.rotation = new Quaternion(save.player.RotX, save.player.RotY, save.player.RotZ, save.player.RotW); //旋转
        player.Atk = save.player.atk; //攻击力
        player.Hp = save.player.hp; //体力值
        player.HpMax = save.player.hpMax; //最大体力值
        player.Sp = save.player.sp; //耐力值
        player.SpMax = save.player.spMax; //最大耐力值
        player.Ep = save.player.ep; //能量值
        player.EpMax = save.player.epMax; //最大能量值
        player.EpPoint = save.player.epPoint; //能量点
        player.EpPointMax = save.player.epPointMax; //最大能量点
        player.SpUseup = save.player.spUseup; //耐力耗尽
        //载入玩家摄像机位置与水平旋转
        Quaternion horiRot = new Quaternion(save.player.cameraRotX, save.player.cameraRotY, save.player.cameraRotZ, save.player.cameraRotW);
        Camera.main.GetComponent<SphereCamera>().SetCameraPos(save.player.cameraDistance, save.player.cameraAngleLerp, horiRot);

        //载入敌人集合当前位置与属性状态
        GameObject[] sceneEnemys = GameObject.FindGameObjectsWithTag("Enemy"); //查找全场景敌人
        for (int i = 0; i < sceneEnemys.Length; i++)
        {
            FileEnemy enemy = save.enemys[i];

            sceneEnemys[i].transform.position = new Vector3(enemy.PosX, enemy.PosY, enemy.PosZ); //位置
            sceneEnemys[i].transform.rotation = new Quaternion(enemy.RotX, enemy.RotY, enemy.RotZ, enemy.RotW); //旋转
            sceneEnemys[i].GetComponent<EnemyCharacterBase>().Hp = enemy.hp; //载入体力值
            sceneEnemys[i].GetComponent<EnemyCharacterBase>().Death = enemy.death; //载入死亡状态
        }

        //载入场景内兽人首领位置
        GameObject sceneOrcKing = GameObject.FindGameObjectWithTag("OrcKing"); //查找全场景敌人
        if (sceneOrcKing != null)
        {
            FileOrcKing orcking = save.orcKing;

            sceneOrcKing.transform.position = new Vector3(orcking.PosX, orcking.PosY, orcking.PosZ); //位置
            sceneOrcKing.transform.rotation = new Quaternion(orcking.RotX, orcking.RotY, orcking.RotZ, orcking.RotW); //旋转
            sceneOrcKing.GetComponent<OrcKiCharacter>().Hp = orcking.hp; //载入体力值
            sceneOrcKing.GetComponent<OrcKiCharacter>().Death = orcking.death; //载入死亡状态
        }

        Debug.Log("载入存档成功");
    }
}

