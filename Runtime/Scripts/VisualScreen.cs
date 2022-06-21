using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fun2
{
    [CreateAssetMenu(menuName = "Fun2/VisualScreen")]
    public class VisualScreen : ScriptableObject
    {
        [SerializeField] private float screenSize = 24;
        [SerializeField] private float scaleFactor = 1f;
        [SerializeField] private Vector2Int ratio = new Vector2Int(16, 9);
        const float INCH2M = 0.0254f;//英寸转米
        float left, right, bottom, top;
        public Vector3 Position;
        public float Width => width;
        public float Height => height;

        public Vector3 LeftTop => new Vector3(left, top, 0) + Position;
        public Vector3 RightTop => new Vector3(right, top, 0) + Position;
        public Vector3 LeftBottom => new Vector3(left, bottom, 0) + Position;
        public Vector3 RightBottom => new Vector3(right, bottom, 0) + Position;
        public static VisualScreen Main
        {
            get
            {
                if (current == null)
                {
                    current = new VisualScreen(24, 1);
                }
                return current;
            }
        }

        public float ScreenSize { get => screenSize; set { screenSize = value; CalculateRect(); } }
        public float ScaleFactor { get => scaleFactor; set { scaleFactor = value; CalculateRect(); } }

        public Vector2Int Ratio { get => ratio; set { ratio = value; CalculateRect(); } }

        private static VisualScreen current;

        float width, height;
        public VisualScreen()
        {
            CalculateRect();
            current = this;
        }

        VisualScreen(float size = 24, float factor = 1f, int wRatio = 16, int hRatio = 9)
        {
            screenSize = size;
            scaleFactor = factor;
            ratio.x = wRatio;
            ratio.y = hRatio;
            CalculateRect();
            current = this;
        }

        void OnEnable()
        {
            CalculateRect();
        }

        void OnValidate()
        {
            if (ScreenSize <= 0)
            {
                ScreenSize = 24;
            }
            if (ScaleFactor <= 0)
            {
                ScaleFactor = 1;
            }
            if (Ratio.x <= 0 || Ratio.y <= 0)
            {
                Ratio = new Vector2Int(16, 9);
            }
            CalculateRect();
        }

        internal void CalculateRect()
        {
            float size = screenSize * INCH2M;
            var widthRatio = ratio.x;
            var heightRatio = ratio.y;
            float sizeRatio = Mathf.Sqrt(widthRatio * widthRatio + heightRatio * heightRatio);
            width = size * widthRatio / sizeRatio * scaleFactor;
            height = size * heightRatio / sizeRatio * scaleFactor;
            right = width / 2f;
            left = -right;
            top = height / 2f;
            bottom = -top;
        }

        public void DrawGizmos()
        {
            DrawGizmos(Color.green);
        }
        public void DrawGizmos(Color c)
        {
            Color oldC = Gizmos.color;
            Gizmos.color = c;
            Gizmos.DrawLine(LeftTop, RightTop);
            Gizmos.DrawLine(LeftBottom, RightBottom);
            Gizmos.DrawLine(LeftTop, LeftBottom);
            Gizmos.DrawLine(RightTop, RightBottom);
            Gizmos.color = oldC;
        }
    }

}