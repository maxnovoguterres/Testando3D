using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Assets.Scripts.Components;
using UnityEngine;
using Assets.Scripts.Helpers;
using Assets.Scripts.Input.Shared;

namespace Assets.Scripts.Systems
{
    public class CameraSystem : ComponentSystem
    {
        public struct Camera
        {
            public ComponentArray<CameraComponent> cameraComponent;
            public ComponentArray<Transform> transform;
            public readonly int Length;
        }

        public struct Player
        {
            public ComponentArray<PlayerComponent> playerComponent;
            public ComponentDataArray<_Input> inputComponent;
            public ComponentDataArray<PlayerMovement> movementComponent;
            public ComponentArray<Transform> transform;
            public ComponentArray<Animator> animator;
            public ComponentArray<Rigidbody> rb;
            public ComponentArray<CharacterController> characterController;
        }

        [Inject] Camera camera;
        [Inject] Player player;

        protected override void OnUpdate()
        {
            for (int i = 0; i < camera.Length; i++)
            {
                //if (player == null) player = GetEntities<Player>()[0];

                var _player = player;
                //var playerInput = _player.inputComponent[0];
                var playerMovement = _player.movementComponent[i];


                if (playerMovement.previouslyGrounded == 0 && player.characterController[i].isGrounded)
                    //{
                    //bobCycleCD.Rate = .2f;
                    camera.cameraComponent[i].bobCycleCD.StartToCount();
                //}
                if (!camera.cameraComponent[i].bobCycleCD.ReturnedToZero)
                    DoBobCycleStep1(camera.cameraComponent[i]);

                UpdateCameraPosition(player.characterController[i], playerMovement, camera.transform[i], camera.cameraComponent[i], player.playerComponent[i]);
                LookRotation(player.transform[i], camera.transform[i], camera.cameraComponent[i], player.playerComponent[i]);
            }
        }

        private void UpdateCameraPosition(CharacterController characterController, PlayerMovement movementComponent, Transform transform, CameraComponent cameraComponent, PlayerComponent playerComponent)
        {
            Vector3 newCameraPosition;

            if (characterController.velocity.magnitude > 0 && characterController.isGrounded)
            {
                transform.localPosition =
                    DoHeadBob(characterController.velocity.magnitude +
                                      (movementComponent.walkSpeed * (playerComponent.GetButton("Run") ? 1f : 0.5f)), transform, cameraComponent);
                newCameraPosition = transform.localPosition;
                newCameraPosition.y = transform.localPosition.y - cameraComponent.jumpOffSet;
            }
            else
            {
                newCameraPosition = transform.localPosition;
                newCameraPosition.y = transform.localPosition.y - cameraComponent.jumpOffSet;
            }
            transform.localPosition = newCameraPosition;
        }

        public Vector3 DoHeadBob(float speed, Transform transform, CameraComponent cameraComponent)
        {
            var originalCameraPosition = transform.localPosition;
            float xPos = originalCameraPosition.x + (cameraComponent.Bobcurve.Evaluate(cameraComponent.m_CyclePositionX) * 0.01f);
            float yPos = originalCameraPosition.y + (cameraComponent.Bobcurve.Evaluate(cameraComponent.m_CyclePositionY) * 0.01f);

            cameraComponent.m_CyclePositionX += (speed * Time.deltaTime) / cameraComponent.m_BobBaseInterval;
            cameraComponent.m_CyclePositionY += ((speed * Time.deltaTime) / cameraComponent.m_BobBaseInterval) * 2f;

            if (cameraComponent.m_CyclePositionX > cameraComponent._m_Time)
            {
                cameraComponent.m_CyclePositionX = cameraComponent.m_CyclePositionX - cameraComponent._m_Time;
            }
            if (cameraComponent.m_CyclePositionY > cameraComponent._m_Time)
            {
                cameraComponent.m_CyclePositionY = cameraComponent.m_CyclePositionY - cameraComponent._m_Time;
            }

            return new Vector3(xPos, yPos, 0.3f);
        }

        public void LookRotation(Transform character, Transform camera, CameraComponent cameraComponent, PlayerComponent playerComponent)
        {
            //float yRot = Input.GetAxis("Mouse X") * cameraComponent.XSensibility;
            //float xRot = Input.GetAxis("Mouse Y") * cameraComponent.YSensibility;
            float yRot = playerComponent.GetAxis("MouseX") * (playerComponent.gamepad != null ? 40 : 1) * cameraComponent.XSensibility; //XSensibility = 1
            float xRot = playerComponent.GetAxis("MouseY") * (playerComponent.gamepad != null ? 40 : 1) * cameraComponent.YSensibility; //YSensibility = 1

            cameraComponent.m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
            cameraComponent.m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

            cameraComponent.m_CameraTargetRot = ClampRotationAroundXAxis(cameraComponent.m_CameraTargetRot);

            if (cameraComponent.Smooth)
            {
                character.localRotation = Quaternion.Slerp(character.localRotation, cameraComponent.m_CharacterTargetRot,
                    cameraComponent.SmoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp(camera.localRotation, cameraComponent.m_CameraTargetRot,
                    cameraComponent.SmoothTime * Time.deltaTime);
            }
            else
            {
                character.localRotation = cameraComponent.m_CharacterTargetRot;
                camera.localRotation = cameraComponent.m_CameraTargetRot;
            }

            UpdateCursorLock(cameraComponent, playerComponent);
        }

        public void SetCursorLock(bool value, CameraComponent cameraComponent)
        {
            cameraComponent.lockCursor = value;
            if (!cameraComponent.lockCursor)
            {//we force unlock the cursor if the user disable the cursor locking helper
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public void UpdateCursorLock(CameraComponent cameraComponent, PlayerComponent playerComponent)
        {
            //if the user set "lockCursor" we check & properly lock the cursos
            if (cameraComponent.lockCursor)
                InternalLockUpdate(cameraComponent, playerComponent);
        }

        private void InternalLockUpdate(CameraComponent cameraComponent, PlayerComponent playerComponent)
        {
            //if (Input.GetKeyUp(KeyCode.Escape))
            if (playerComponent.GetButtonUp("CursorOn"))
            {
                cameraComponent.m_cursorIsLocked = false;
            }
            //else if (Input.GetMouseButtonUp(0))
            else if (playerComponent.GetButtonUp("CursorOff"))
            {
                cameraComponent.m_cursorIsLocked = true;
            }

            if (cameraComponent.m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (!cameraComponent.m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            angleX = Mathf.Clamp(angleX, -90f, 90f);

            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }

        public void DoBobCycleStep1(CameraComponent cameraComponent)
        {
            //if(bobCycleCD.Rate == .1f)
            //{
            //    DoBobCycleStep2();
            //    return;
            //}
            cameraComponent.jumpOffSet = Mathf.Lerp(0f, 0.1f, cameraComponent.bobCycleCD.CoolDown / 0.2f);
            cameraComponent.bobCycleCD.DecreaseTime();

            //if (bobCycleCD.ReturnedToZero)
            //{
            //    bobCycleCD.Rate = .1f;
            //    bobCycleCD.StartToCount();
            //}
        }
        //public void DoBobCycleStep2()
        //{
        //    jumpOffSet = Mathf.Lerp(0.1f, 0f, bobCycleCD.CoolDown / 0.2f);
        //    bobCycleCD.DecreaseTime();

        //    if (bobCycleCD.ReturnedToZero)
        //        jumpOffSet = 0f;
        //}
    }
}
