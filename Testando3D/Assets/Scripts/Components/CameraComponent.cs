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
        public AnimationCurve Bobcurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f),
                                                            new Keyframe(1f, 0f), new Keyframe(1.5f, -1f),
                                                            new Keyframe(2f, 0f)); // sin curve for head bob
        public float m_CyclePositionX;
        public float m_CyclePositionY;
        public float m_BobBaseInterval = 2f;
        public float? m_Time; //{ get { return 1; } set; }
        public float _m_Time { get { m_Time = m_Time ?? Bobcurve[Bobcurve.length - 1].time; return m_Time.Value; } } //{ get { return 1; } set; }
        public Transform _CharacterTransform;
        public Quaternion? _m_CharacterTargetRot;
        public Quaternion m_CharacterTargetRot { get { _m_CharacterTargetRot = _m_CharacterTargetRot ?? _CharacterTransform.localRotation; return _m_CharacterTargetRot.Value; } set { _m_CharacterTargetRot = value; } } //{ get { return 1; } set; }
        //public Transform _CameraTransform;
        public Quaternion? _m_CameraTargetRot;
        public Quaternion m_CameraTargetRot { get { _m_CameraTargetRot = _m_CameraTargetRot ?? Camera.main.transform.localRotation; return _m_CameraTargetRot.Value; } set { _m_CameraTargetRot = value; } } //{ get { return 1; } set; }
        public bool lockCursor = true;
        public bool m_cursorIsLocked = true;

    }
}
