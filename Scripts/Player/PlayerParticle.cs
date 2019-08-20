using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParticle : ParticleBase
{
    public PlayerWeapon playerWeapon;

    protected override void Start()
    {
        base.Start();
        playerWeapon.GetComponent<Collider>().enabled = false; //武器碰撞火花粒子效果
    }

    public void Play(PlayerState playerState)
    {
        switch (playerState)
        {
            case PlayerState.EpPoint:                                               
                ParticlePlay(particleList[0]); //播放粒子效果组
                MaterialPlay(materialList[0]); //激活能量点武器材质
                break;
            case PlayerState.Combo1:
                ParticlePlay(particleList[1]);
                playerWeapon.GetComponent<Collider>().enabled = true;
                break;
            case PlayerState.Combo2:
                ParticlePlay(particleList[1]);
                playerWeapon.GetComponent<Collider>().enabled = true;
                break;
            case PlayerState.Combo3:
                ParticlePlay(particleList[1]);
                playerWeapon.GetComponent<Collider>().enabled = true;
                break;
            case PlayerState.Combo4:
                ParticlePlay(particleList[1]);
                playerWeapon.GetComponent<Collider>().enabled = true;
                break;
            case PlayerState.ThumpCharge:
                ParticlePlay(particleList[2]);
                break;
            case PlayerState.Thump:
                ParticlePlay(particleList[3]);
                ParticlePlay(particleList[4]);
                playerWeapon.GetComponent<Collider>().enabled = true;
                break;
            case PlayerState.Thump1:
                ParticlePlay(particleList[3]);
                ParticlePlay(particleList[5]);
                playerWeapon.GetComponent<Collider>().enabled = true;
                break;
            case PlayerState.Thump2:
                ParticlePlay(particleList[3]);
                ParticlePlay(particleList[6]);
                playerWeapon.GetComponent<Collider>().enabled = true;
                break;
            case PlayerState.Skill1:
                ParticlePlay(particleList[7]);
                MaterialPlay(materialList[1]);
                playerWeapon.GetComponent<Collider>().enabled = true;
                break;
            case PlayerState.GuardDamage:
                ParticleRandomPlay(particleList[8]);
                break;
            case PlayerState.Damage:
                RandomPositionDirection(particleList[9]);
                ParticlePlay(particleList[9]);
                break;
            case PlayerState.Dodge:
                ParticlePlay(particleList[10]);
                break;
            default:
                //Debug.Log(playerState.ToString() + ":无此类型粒子效果组");
                break;
        }
    }

    public void Stop(PlayerState playerState)
    {
        switch (playerState)
        {
            case PlayerState.EpPoint:
                ParticleStop(particleList[0]); //停止粒子效果组
                MaterialStop(materialList[0]); //停止能量点武器材质
                break;
            case PlayerState.Combo1:
                ParticleStop(particleList[1]);
                playerWeapon.GetComponent<Collider>().enabled = false;
                break;
            case PlayerState.Combo2:
                ParticleStop(particleList[1]);
                playerWeapon.GetComponent<Collider>().enabled = false;
                break;
            case PlayerState.Combo3:
                ParticleStop(particleList[1]);
                playerWeapon.GetComponent<Collider>().enabled = false;
                break;
            case PlayerState.Combo4:
                ParticleStop(particleList[1]);
                playerWeapon.GetComponent<Collider>().enabled = false;
                break;
            case PlayerState.ThumpCharge:
                ParticleStop(particleList[2]);
                break;
            case PlayerState.Thump:
                ParticleStop(particleList[3]);
                ParticleStop(particleList[4]);
                playerWeapon.GetComponent<Collider>().enabled = false;
                break;
            case PlayerState.Thump1:
                ParticleStop(particleList[3]);
                ParticleStop(particleList[5]);
                playerWeapon.GetComponent<Collider>().enabled = false;
                break;
            case PlayerState.Thump2:
                ParticleStop(particleList[3]);
                ParticleStop(particleList[6]);
                playerWeapon.GetComponent<Collider>().enabled = false;
                break;
            case PlayerState.Skill1:
                ParticleStop(particleList[7]);
                MaterialStop(materialList[1]);
                playerWeapon.GetComponent<Collider>().enabled = false;
                break;
            case PlayerState.GuardDamage:
                ParticleAllChildStop(particleList[8]);
                break;
            case PlayerState.Damage:
                ParticleStop(particleList[9]);
                break;
            case PlayerState.Dodge:
                ParticleStop(particleList[10]);
                break;
            default:
                //Debug.Log(playerState.ToString() + ":无此类型粒子效果组");
                break;
        }
    }

    void ParticleRandomPlay(GameObject particles) //防御受伤 随机粒子效果与位置
    {
        //随机获取碰撞粒子效果
        ParticleSystem particle = particles.transform.GetChild(Random.Range(0, particles.transform.childCount - 1)).GetComponent<ParticleSystem>();
        //获取碰撞点 并赋值粒子效果position
        particle.transform.localPosition = new Vector3(0, 0, Random.Range(0f, 1.5f));
        //播放粒子效果
        particle.Play();
    }

    void ParticleAllChildStop(GameObject particles)
    {
        for(int i=0;i<particles.transform.childCount;i++)
        {
            ParticleSystem par = particles.transform.GetChild(i).GetComponent<ParticleSystem>();
            if (par != null)
                par.Stop();
        }
    }

    void RandomPositionDirection(GameObject particles) //受伤溅血 随机 位置与方向
    {
        Transform par = particles.transform;
        par.localPosition = new Vector3(0, Random.Range(0.7f, 1.7f), 0); //随机高度
        par.localRotation = Quaternion.Euler(Random.Range(-60, 60), Random.Range(0, 360), 0); //随机方向
    }
}
