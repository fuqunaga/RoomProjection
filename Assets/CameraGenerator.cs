using System.Collections.Generic;
using UnityEngine;

namespace RoomProjection
{
    public class CameraGenerator : MonoBehaviour
    {
        public Transform _eyePoint;
        public int _cameraDepth = -10;

        Room _room;
        Dictionary<Room.Face, Camera> _faceToCamera = new Dictionary<Room.Face, Camera>();

        #region Unity
        void Start()
        {
            _room = FindObjectOfType<Room>();
            _room.GetFaceDatas().ForEach(data =>
            {
                var go = new GameObject(data.face.ToString(), typeof(Camera));
                var trans = go.transform;
                trans.SetParent(transform);
                trans.rotation = Room.FaceToRot(data.face);

                var camera = go.GetComponent<Camera>();
                _faceToCamera[data.face] = camera;
            });
        }

        private void Update()
        {
            var eyePointRoomLocal = _room.transform.InverseTransformPoint(_eyePoint.position);

            _room.GetFaceDatas().ForEach(data =>
            {
                UpdateCamera(_faceToCamera[data.face], data, eyePointRoomLocal);
            });
        }
        #endregion

        void UpdateCamera(Camera camera, Room.FaceData faceData, Vector3 eyePointRoomLocal)
        {
            camera.depth = _cameraDepth;
            camera.ResetProjectionMatrix();

            // アスペクト比を求める
            var faceSize = faceData.size;
            camera.aspect = faceSize.x / faceSize.y;

            // 画角を求める
            var positionOffset = Quaternion.Inverse(Room.FaceToRot(faceData.face)) * eyePointRoomLocal;
            var distance = faceData.distance + positionOffset.z;
            camera.fieldOfView = 2f * Mathf.Atan2(faceSize.y * 0.5f, distance) * Mathf.Rad2Deg;

            // farClipPlaneをセット
            camera.farClipPlane = distance * 1000f;

            // レンズシフト
            var shift = new Vector2(positionOffset.x / faceSize.x, positionOffset.y / faceSize.y) * 2f;
            var projectionMatrix = camera.projectionMatrix;
            projectionMatrix[0, 2] = shift.x;
            projectionMatrix[1, 2] = shift.y;

            camera.projectionMatrix = projectionMatrix;
        }
    }
}