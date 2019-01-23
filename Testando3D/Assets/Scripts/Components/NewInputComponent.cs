using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class NewInputComponent: MonoBehaviour
    {
        public InputMaster controls;
        public static float teste1;
        public static float teste2;
        private void Awake()
        {
            controls.Player.Crouch.performed += x => Crouch();
            controls.Player.Fire.performed += x => Fire();
            controls.Player.Aim.performed += x => Aim();
            controls.Player.Reload.performed += x => Reload();
            controls.Player.PickItemsAndEquipments.performed += x => PickItemsAndEquipments();
            controls.Player.Jump.performed += x => Jump();

            controls.Player.Horizontal.performed += x => { teste1 = x.ReadValue<float>(); };
            controls.Player.Vertical.performed += x => { teste2 = x.ReadValue<float>(); };
        }

        private void OnEnable()
        {
            controls.Enable();
        }

        private void OnDisable()
        {
            controls.Disable();
        }

        void Crouch()
        {
            Debug.Log("abaixou");
        }
        void Fire()
        {
            Debug.Log("atirou");
        }
        void Aim()
        {
            Debug.Log("mirando");
        }
        void Reload()
        {
            Debug.Log("recarregando");
        }
        void PickItemsAndEquipments()
        {
            Debug.Log("pegou arma");
            if (GameManager.Instance.canEquip)
            {
                EquipmentManager.instance.Equip(GameManager.Instance.gunToEquip, gameObject);
                Destroy(GameManager.Instance.gunToDestroy);
                GameManager.Instance.pickUpText.text = "";
                GameManager.Instance.canEquip = false;
            }
        }
        void Jump()
        {
            Debug.Log("pulou");
        }
        void Horizontal(float a)
        {
            Debug.Log("moveu horizontal para: " + a);
        }
        void Vertical(float a)
        {
            Debug.Log("moveu vertical para: " + a);
        }
    }
}
