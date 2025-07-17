using UnityEngine;

namespace My3DGame.Util
{
    /// <summary>
    /// 오브젝트를 지정한 위치와 회전값에 따라가도록 한다
    /// </summary>
    public class FixedUpdateFollow : MonoBehaviour
    {
        #region Variables
        public Transform toFollow;          // Attach Point 오브젝트
        #endregion

        #region Unity Event Method
        private void FixedUpdate()
        {
            transform.position = toFollow.position;
            transform.rotation = toFollow.rotation;
        }
        #endregion
    }
}

