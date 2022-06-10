using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kmax
{

    public class VisualScreen
    {
        private float screenSize = 24;
        private float scaleFactor = 1f;
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
                if (screens == null || screens.Count == 0)
                {
                    screens = new List<VisualScreen>();
                    screens.Add(new VisualScreen(24, 1));
                }
                return screens[0];
            }
        }

        public float ScreenSize { get => screenSize; set { screenSize = value; CalculateScreenRect();} }
        public float ScaleFactor { get => scaleFactor; set { scaleFactor = value; CalculateScreenRect(); } }
        private static List<VisualScreen> screens;

        int widthRatio, heightRatio;
        float width, height;
        VisualScreen(float size = 24, float factor = 1f, int wRatio = 16, int hRatio = 9)
        {
            screenSize = size;
            scaleFactor = factor;
            widthRatio = wRatio;
            heightRatio = hRatio;
            CalculateScreenRect();
        }

        private void CalculateScreenRect()
        {
            float size = screenSize * INCH2M;
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