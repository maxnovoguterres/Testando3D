using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public static class PlayerUtils
    {
        public static List<Rect> oneCameraRects = new List<Rect> { new Rect(0, 0, 1, 1) };
        public static List<Rect> twoCameraRects = new List<Rect> { new Rect(0, 0, 0.5f, 1), new Rect(0.5f, 0, 1, 1) };
        public static List<Rect> threeCameraRects = new List<Rect> { new Rect(0, 0, 0.5f, 1), new Rect(0.5f, 0, 0.5f, 0.5f), new Rect(0.5f, 0.5f, 0.5f, 0.5f) };
        public static List<Rect> fourCameraRects = new List<Rect> { new Rect(0, 0.5f, 0.5f, 0.5f), new Rect(0.5f, 0.5f, 0.5f, 0.5f), new Rect(0, 0, 0.5f, 0.5f), new Rect(0.5f, 0, 0.5f, 0.5f) };
    }
}
