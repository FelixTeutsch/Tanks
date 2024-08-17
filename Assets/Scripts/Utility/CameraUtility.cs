using UnityEngine;

namespace Utility
{
    public class CameraUtility : MonoBehaviour
    {
        public static float GetCameraWidth(Camera camera)
        {
            return 2f * camera.orthographicSize * camera.aspect;
        }

        public static float GetCameraHeight(Camera camera)
        {
            return 2f * camera.orthographicSize;
        }
    }
}