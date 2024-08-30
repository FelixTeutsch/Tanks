using UnityEngine;

namespace Utility
{
    public static class CameraUtility
    {
        public static float GetCameraWidth(Camera camera)
        {
            return 2f * camera.orthographicSize * camera.aspect;
        }

        public static float GetCameraHeight(Camera camera)
        {
            return 2f * camera.orthographicSize;
        }

        public static float GetCameraLeft(Camera camera)
        {
            return camera.transform.position.x - GetCameraWidth(camera) / 2;
        }

        public static float GetCameraRight(Camera camera)
        {
            return camera.transform.position.x + GetCameraWidth(camera) / 2;
        }
    }
}