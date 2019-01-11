using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class CameraComponent : MonoBehaviour
    {
        public float XSensibility = 1f;
        public float YSensibility = 1f;
        public bool Smooth = true;
        public float SmoothTime = 15f;


        [HideInInspector] public AnimationCurve Bobcurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f),
                                                            new Keyframe(1f, 0f), new Keyframe(1.5f, -1f),
                                                            new Keyframe(2f, 0f)); // sin curve for head bob
        [HideInInspector] public float m_CyclePositionX;
        [HideInInspector] public float m_CyclePositionY;
        [HideInInspector] public float m_BobBaseInterval = 2f;
        [HideInInspector] public float? m_Time;
        [HideInInspector] public float _m_Time { get { m_Time = m_Time ?? Bobcurve[Bobcurve.length - 1].time; return m_Time.Value; } }
        [HideInInspector] public Transform _CharacterTransform;
        [HideInInspector] public Quaternion? _m_CharacterTargetRot;
        [HideInInspector] public Quaternion m_CharacterTargetRot { get { _m_CharacterTargetRot = _m_CharacterTargetRot ?? _CharacterTransform.localRotation; return _m_CharacterTargetRot.Value; } set { _m_CharacterTargetRot = value; } }
        [HideInInspector] public Quaternion? _m_CameraTargetRot;
        [HideInInspector] public Quaternion m_CameraTargetRot { get { _m_CameraTargetRot = _m_CameraTargetRot ?? Camera.main.transform.localRotation; return _m_CameraTargetRot.Value; } set { _m_CameraTargetRot = value; } }
        [HideInInspector] public bool lockCursor = true;
        [HideInInspector] public bool m_cursorIsLocked = true;
    }
}
