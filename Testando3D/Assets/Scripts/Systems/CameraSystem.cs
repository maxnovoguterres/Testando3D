using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Assets.Scripts.Components;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    public class CameraSystem : ComponentSystem
    {
        public struct Camera
        {
            public CameraComponent cameraComponent;
            public Transform transform;
        }

        public struct Player
        {
            public InputComponent inputComponent;
            public PlayerMovementComponent movementComponent;
            public Transform transform;
            public Animator animator;
            public Rigidbody rb;
            public CharacterController characterController;
        }

        Camera? camera;
        Player? player;
        protected override void OnUpdate()
        {
            if (camera == null) camera = GetEntities<Camera>()[0];
            if (player == null) player = GetEntities<Player>()[0];

            UpdateCameraPosition(player.Value.characterController, player.Value.movementComponent, camera.Value.transform, camera.Value.cameraComponent);
            LookRotation(player.Value.transform, camera.Value.transform, camera.Value.cameraComponent);
        }

        private void UpdateCameraPosition(CharacterController characterController, PlayerMovementComponent movementComponent, Transform transform, CameraComponent cameraComponent)
        {
            Vector3 newCameraPosition;

            if (characterController.velocity.magnitude > 0 && characterController.isGrounded)
            {
                transform.localPosition =
                    DoHeadBob(characterController.velocity.magnitude +
                                      (movementComponent.walkSpeed * (Input.GetAxis("Sprint") != 0 ? 1f : 0.5f)), transform, cameraComponent);
                newCameraPosition = transform.localPosition;
                newCameraPosition.y = transform.localPosition.y;
            }
            else
            {
                newCameraPosition = transform.localPosition;
                newCameraPosition.y = transform.localPosition.y;
            }
            transform.localPosition = newCameraPosition;
        }

        public Vector3 DoHeadBob(float speed, Transform transform, CameraComponent cameraComponent)
        {
            var originalCameraPosition = transform.localPosition;
            float xPos = originalCameraPosition.x + (cameraComponent.Bobcurve.Evaluate(cameraComponent.m_CyclePositionX) * 0.01f);
            float yPos = originalCameraPosition.y + (cameraComponent.Bobcurve.Evaluate(cameraComponent.m_CyclePositionY) * 0.01f);

            cameraComponent.m_CyclePositionX += (speed * Time.deltaTime) / cameraComponent.m_BobBaseInterval;
            cameraComponent.m_CyclePositionY += ((speed * Time.deltaTime) / cameraComponent.m_BobBaseInterval) * 1f;

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

        public void LookRotation(Transform character, Transform camera, CameraComponent cameraComponent)
        {
            float yRot = Input.GetAxis("Mouse X") * cameraComponent.XSensibility;
            float xRot = Input.GetAxis("Mouse Y") * cameraComponent.YSensibility;

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

            UpdateCursorLock(cameraComponent);
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

        public void UpdateCursorLock(CameraComponent cameraComponent)
        {
            //if the user set "lockCursor" we check & properly lock the cursos
            if (cameraComponent.lockCursor)
                InternalLockUpdate(cameraComponent);
        }

        private void InternalLockUpdate(CameraComponent cameraComponent)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                cameraComponent.m_cursorIsLocked = false;
            }
            else if (Input.GetMouseButtonUp(0))
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
    }
}
