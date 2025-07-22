using UnityEngine;

namespace My3DGame.AI
{
    /// <summary>
    /// 패트롤 하는 Enemy, Enemy를 상속받는다
    /// 기능 : Enemy 기능 + 패트롤
    /// </summary>
    public class EnemyPatrol : Enemy
    {
        #region Variables
        public Transform[] wayPoints;
        #endregion

        #region Unity Event Method
        protected override void Start()
        {
            base.Start();

            // 상속받은 후 추가로 새로운 상태 등록
            stateMachine.RegisterState(new PatrolState());
        }
        #endregion
    }
}

