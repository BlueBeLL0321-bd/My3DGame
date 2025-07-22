using UnityEngine;

namespace My3DGame
{
    /// <summary>
    /// 캐릭터의 대미지를 관리하는 partial 클래스
    /// partial 클래스 : 하나의 클래스를 두 개의 클래스로 분리해서 구현
    /// 대미지 데이터 구조체를 관리하는 partial 클래스
    /// </summary>
    public partial class Damageable : MonoBehaviour
    {
        // 대미지 데이터 구조체
        public struct DamageMessage
        {
            public MonoBehaviour damager;
            public float amount;
            public Vector3 direction;
            public Vector3 damageSource;
            public bool throwing;

            public bool stopCamera;
        }
    }
}

