using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleBase : MonoBehaviour
{
    //粒子效果
    public List<GameObject> particleList = new List<GameObject>();
    //材质效果
    public List<GameObject> materialList = new List<GameObject>();

    protected virtual void Start()
    {
        //初始化 停止全部粒子效果 与 材质效果
        for (int i = 0; i < particleList.Count; i++) 
        {
            ParticleStop(particleList[i]);
        }
        for (int i = 0; i < materialList.Count; i++)
        {
            MaterialStop(materialList[i]);
        }
    }

    public void Play() //播放 参数重载枚举类型
    {
        //示例内容
        //switch(State)
        //{
        //    case State.EpPoint:
        //        MaterialPlay(materialList[0]); //激活能量点武器材质                                               
        //        ParticlePlay(particleList[0]); //播放粒子效果组
        //        break;
        //    case State.Combo1:
        //        ParticlePlay(particleList[1]);
        //        break;
        //    default:
        //        Debug.Log(playerState.ToString() + ":无此类型粒子效果组");
        //        break;
        //}
    }

    public void Stop() //播放 参数重载枚举类型
    {
        //示例内容
        //switch (State)
        //{
        //    case State.EpPoint:
        //        MaterialStop(materialList[0]); //激活能量点武器材质                                               
        //        ParticleStop(particleList[0]); //播放粒子效果组
        //        break;
        //    case State.Combo1:
        //        ParticleStop(particleList[1]);
        //        break;
        //    default:
        //        Debug.Log(playerState.ToString() + ":无此类型粒子效果组");
        //        break;
        //}
    }

    protected void ParticlePlay(GameObject particles) //播放粒子效果组
    {
        if (!particles.GetComponent<ParticleSystem>())
            return;

        ParticleSystem par = particles.GetComponent<ParticleSystem>();

        par.Play();
    }

    protected void ParticleStop(GameObject particles) //停止粒子效果组
    {
        if (!particles.GetComponent<ParticleSystem>())
            return;

        ParticleSystem par = particles.GetComponent<ParticleSystem>();

        par.Stop();
    }

    protected void MaterialPlay(GameObject material) //播放材质效果
    {
        material.SetActive(true);
    }

    protected void MaterialStop(GameObject material) //停止材质效果
    {
        material.SetActive(false);
    }
}
