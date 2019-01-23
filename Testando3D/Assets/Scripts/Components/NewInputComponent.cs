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

        private void Awake()
        {
            controls.Player.Crouch.performed += x => Crouch();
            controls.Player.Fire.performed += x => Fire();
            controls.Player.Aim.performed += x => Aim();
            controls.Player.Reload.performed += x => Reload();
            controls.Player.PickItemsAndEquipments.performed += x => PickItemsAndEquipments();
            controls.Player.Jump.performed += x => Jump();
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
    }
}
