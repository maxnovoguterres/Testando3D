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
        public float countDownRate;
        public CountDown countDown;
        public Transform bocal;
        public float bulletSpeed;
        public Animator animator;
        public float scopedFOV = 15f;
        public float normalFOV;
        public bool isScoped;
        public GameObject player;
        public float Damage;
        [Range(0,1)] public float Accuracy;
        public float CurrentAccuracy;
        public int MaxAmmo;
        public int Ammo;
        [Range(0, 1)] public float Recoil;
        public int qtdProjectile;
        public float distanceOfEachProjectile;
    }
}
