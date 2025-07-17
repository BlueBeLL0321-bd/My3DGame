using UnityEngine;

namespace MySample
{
    public class TopDownCamera : MonoBehaviour
    {
        #region Variables
        public Transform target;                                                    // 카메라가 쫓아갈 플레이어

        [SerializeField] private float distance = 8f;                               // 플레이어로부터의 거리
        [SerializeField] private float height = 6f;                                 // 플레이어로부터의 높이
        [SerializeField] private float angle = 45f;                                 // 플레이어가 바라보는 방향 기준으로 45도

        [SerializeField] private float smoothSpeed = 0.5f;                          // 이동 속도

        [SerializeField] private float lookAtHeight = 1.8f;                         // 카메라가 바라보는 높이(캐릭터의 머리 위치)
        private Vector3 refVelocity;                                                // Smooth Damp 속도
        #endregion

        #region Unity Event Method
        private void Start()
        {
            // 카메라 초기 위치 설정
            HandleCamera();
        }

        private void LateUpdate()
        {
            // 카메라의 위치 조정
            HandleCamera();
        }

        private void OnDrawGizmos()
        {
            if (target == null)
                return;

            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);

            Vector3 lookAtPosition = target.position;
            lookAtPosition.y += lookAtHeight;
            Gizmos.DrawLine(this.transform.position, lookAtPosition);   // 타깃과 카메라 연결 선
            Gizmos.DrawSphere(lookAtPosition, 0.25f);                   // 타깃 위치에 구 그리기
            Gizmos.DrawSphere(this.transform.position, 0.25f);          // 카메라 위치에 구 그리기
        }
        #endregion

        #region Custom Method
        // 타깃(플레이어) 따라가기
        private void HandleCamera()
        {
            if (target == null)
                return;

            // 카메라 위치 설정(플레이어 기준)
            Vector3 worldPosition = (target.forward * -distance) + (Vector3.up * height);
            Vector3 rotateVector = Quaternion.AngleAxis(angle, Vector3.up) * worldPosition;

            // 카메라가 이동할 최종 위치
            Vector3 flatTargetPosition = target.position;
            flatTargetPosition.y += lookAtHeight;
            Vector3 finalPosition = flatTargetPosition + rotateVector;

            // 카메라 이동
            transform.position = Vector3.SmoothDamp(transform.position, finalPosition, 
                ref refVelocity, smoothSpeed);

            // 카메라의 타깃 바라보기
            transform.LookAt(flatTargetPosition);
        }
        #endregion
    }
}

