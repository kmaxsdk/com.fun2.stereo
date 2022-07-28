using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fun2
{
    internal enum ConvergeMode { Nerver, Once, Always }
    [RequireComponent(typeof(Camera))]
    public class StereoPro : MonoBehaviour
    {
        Camera _camera;
        public Camera CenterCamera { get => _camera != null ? _camera : GetComponent<Camera>(); }
        
        [Header("相机")]
        [SerializeField] Transform leftEye;
        [SerializeField] Transform rightEye;
        // Start is called before the first frame update
        void Start()
        {
            _camera = GetComponent<Camera>();
            CenterCamera.stereoTargetEye = StereoTargetEyeMask.Both;
            originalProjection = CenterCamera.projectionMatrix;
            ResetProjection();
            if (convergenceMode == ConvergeMode.Once) Converge();
        }
        Matrix4x4 originalProjection;
        [SerializeField, Header("屏幕")]
        private VisualScreen targetScreen;
        [SerializeField] ConvergeMode convergenceMode;
        float left, right, bottom, top, nearClip, farClip;


        void Update()
        {
            if (convergenceMode == ConvergeMode.Always) Converge();
        }

        void OnDrawGizmos()
        {
            targetScreen?.DrawGizmos();
        }

        /// <summary>
        /// 双目聚焦
        /// </summary>
        public void Converge()
        {
            SetFrustum(CenterCamera.stereoTargetEye);
            SetFrustum(StereoTargetEyeMask.Left);
            SetFrustum(StereoTargetEyeMask.Right);
        }

        /// <summary>
        /// 重置投影矩阵
        /// </summary>
        public void ResetProjection()
        {
            CenterCamera.projectionMatrix = originalProjection;
        }

        public void SetFrustum(StereoTargetEyeMask eyeMask)
        {
            Transform _cam = transform;
            switch (eyeMask)
            {
                case StereoTargetEyeMask.Both:
                    CenterCamera.projectionMatrix = TrackScreen(_cam);
                    break;
                case StereoTargetEyeMask.Left:
                    _cam = leftEye;
                    CenterCamera.SetStereoViewMatrix(Camera.StereoscopicEye.Left, _cam.worldToLocalMatrix);
                    CenterCamera.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left, TrackScreen(_cam));
                    break;
                case StereoTargetEyeMask.Right:
                    _cam = rightEye;
                    CenterCamera.SetStereoViewMatrix(Camera.StereoscopicEye.Right, _cam.worldToLocalMatrix);
                    CenterCamera.SetStereoProjectionMatrix(Camera.StereoscopicEye.Right, TrackScreen(_cam));
                    break;
                default: break;
            }
        }

        public Matrix4x4 GetFrustum(StereoTargetEyeMask eye)
        {
            switch (eye)
            {
                case StereoTargetEyeMask.Left: return TrackScreen(leftEye);
                case StereoTargetEyeMask.Right: return TrackScreen(rightEye);
                case StereoTargetEyeMask.Both:
                default: return TrackScreen(transform);
            }
        }

        internal Matrix4x4 TrackScreen(Transform _cam)
        {
            if (targetScreen == null) return originalProjection;
            Vector3 ls = (targetScreen.LeftTop + targetScreen.LeftBottom) / 2f;
            Vector3 rs = (targetScreen.RightTop + targetScreen.RightBottom) / 2f;
            Vector3 bs = (targetScreen.LeftBottom + targetScreen.RightBottom) / 2f;
            Vector3 ts = (targetScreen.LeftTop + targetScreen.RightTop) / 2f;
            Debug.DrawLine(ls, rs, Color.green);
            Debug.DrawLine(bs, ts, Color.green);
            ls -= _cam.transform.position;
            rs -= _cam.transform.position;
            bs -= _cam.transform.position;
            ts -= _cam.transform.position;
            // Debug.Log($"local:{ls},{rs},{bs},{ts};");
            // _cam.transform.LookAt(targetScreen);
            nearClip = CenterCamera.nearClipPlane;
            farClip = CenterCamera.farClipPlane;
            float near_m(Vector3 point2cam, Vector3 dir) // 近截面取模
            {
                float fm = Vector3.Dot(point2cam, dir);
                float df = Vector3.Dot(point2cam, _cam.transform.forward);
                return nearClip * fm / df;
            }
            left = near_m(ls, _cam.transform.right);
            right = near_m(rs, _cam.transform.right);
            bottom = near_m(bs, _cam.transform.up);
            top = near_m(ts, _cam.transform.up);
            return Matrix4x4.Frustum(left, right, bottom, top, nearClip, farClip);
        }
        public override string ToString()
        {
            return string.Format("original:\n{0}now:\n{1}",
                originalProjection.ToString("F3"),
                CenterCamera.projectionMatrix.ToString("F3"));
        }
    }

}