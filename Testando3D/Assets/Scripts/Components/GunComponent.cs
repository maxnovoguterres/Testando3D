using Assets.Scripts.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Entities;
using UnityEngine.UI;

namespace Assets.Scripts.Components
{
    public class GunComponent : MonoBehaviour
    {
        public float FireRate;
        public float fireCoolDown;
        public Transform bocal;
        public float bulletSpeed;
        public Animator animator;
        public float scopedFOV = 15f;
        public float normalFOV;
        public bool isScoped;
        public GameObject player;
        public Entity playerEntity;
        public float Damage;
        [Range(0,1)] public float Accuracy;
        [Range(0,1)] public float IncreaseAccuracy;
        public float CurrentAccuracy;
        public int MaxAmmo;
        public int CurrentAmmo;
        public int ExtraAmmo;
        public Sprite AmmoImage;
        [Range(0, 1)] public float Recoil;
        [Range(0, 1)] public float IncreaseRecoil;
        public int qtdProjectile;
        public float distanceOfEachProjectile;
        public float reloadTimer = 0;
        public CountDown timer;
    }
}
