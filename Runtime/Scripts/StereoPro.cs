using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kmax
{
    internal enum FocusMode { Nerver, Once, Always }
    [RequireComponent(typeof(Camera))]
    public class StereoPro : MonoBehaviour
    {
        Camera _camera;
        [Header("相机")]
        [SerializeField] Transform leftEye;
        [SerializeField] Transform rightEye;
        // Start is called before the first frame update
        void Start()
        {
            _camera = GetComponent<Camera>();
            _camera.stereoTargetEye = StereoTargetEyeMask.Both;
            originalProjection = _camera.projectionMatrix;
            ResetProjection();
            if (convergenceMode == FocusMode.Once) Converge();
        }
        Matrix4x4 originalProjection;
        [SerializeField, Header("屏幕")]
        private VisualScreen targetScreen;
        [SerializeField] FocusMode convergenceMode;
        float left, right, bottom, top, nearClip, farClip;

        void OnValidate()
        {
            if (targetScreen.ScreenSize <= 0)
            {
                targetScreen.ScreenSize = 24;
            }
            if (targetScreen.ScaleFactor <= 0)
            {
                targetScreen.ScaleFactor = 1;
            }
            if (targetScreen.Ratio.x <= 0 || targetScreen.Ratio.y <= 0)
            {
                targetScreen.Ratio = new Vector2Int(16, 9);
            }
            targetScreen.CalculateRect();
        }

        void Update()
        {
            if (convergenceMode == FocusMode.Always) Converge();
        }

        void OnDrawGizmos()
        {
            targetScreen.DrawGizmos();
        }

        /// <summary>
        /// 双目聚焦
        /// </summary>
        public void Converge()
        {
            SetFrustum(transform, _camera.stereoTargetEye);
            SetFrustum(leftEye, StereoTargetEyeMask.Left);
            SetFrustum(rightEye, StereoTargetEyeMask.Right);
        }

        /// <summary>
        /// 重置投影矩阵
        /// </summary>
        public void ResetProjection()
        {
            _camera.projectionMatrix = originalProjection;
        }

        public void SetFrustum(Transform _cam, StereoTargetEyeMask eyeMask)
        {
            switch (eyeMask)
            {
                case StereoTargetEyeMask.Both:
                    _camera.projectionMatrix = TrackScreen(_cam);
                    break;
                case StereoTargetEyeMask.Left:
                    _camera.SetStereoViewMatrix(Camera.StereoscopicEye.Left, _cam.worldToLocalMatrix);
                    _camera.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left, TrackScreen(_cam));
                    break;
                case StereoTargetEyeMask.Right:
                    _camera.SetStereoViewMatrix(Camera.StereoscopicEye.Right, _cam.worldToLocalMatrix);
                    _camera.SetStereoProjectionMatrix(Camera.StereoscopicEye.Right, TrackScreen(_cam));
                    break;
                default: break;
            }
        }

        Matrix4x4 TrackScreen(Transform _cam)
        {
            if (targetScreen == null) return originalProjection;
            Vector3 ls = (targetScreen.LeftTop + targetScreen.LeftBottom) / 2f;
            Vector3 rs = (targetScreen.RightTop + targetScreen.RightBottom) / 2f;
            Vector3 bs = (targetScreen.LeftBottom + targetScreen.RightBottom) / 2f;
            Vector3 ts = (targetScreen.LeftTop + targetScreen.RightTop) / 2f;
            Debug.DrawLine(ls, rs, Color.green);
            Debug.DrawLine(bs, ts, Color.green);
            //Debug.DrawLine(ls, _cam.transform.position, Color.gray);
            //Debug.DrawLine(rs, _cam.transform.position, Color.gray);
            //Debug.DrawLine(bs, _cam.transform.position, Color.gray);
            //Debug.DrawLine(ts, _cam.transform.position, Color.gray);
            // Debug.Log($"world:{ls},{rs},{bs},{ts};");
            ls -= _cam.transform.position;
            rs -= _cam.transform.position;
            bs -= _cam.transform.position;
            ts -= _cam.transform.position;
            // Debug.Log($"local:{ls},{rs},{bs},{ts};");
            // _cam.transform.LookAt(targetScreen);
            nearClip = _camera.nearClipPlane;
            farClip = _camera.farClipPlane;
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
                _camera.projectionMatrix.ToString("F3"));
        }
    }

}