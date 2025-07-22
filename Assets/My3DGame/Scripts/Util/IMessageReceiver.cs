using UnityEngine;

namespace My3DGame.Util
{
    /// <summary>
    /// 메시지 타입
    /// </summary>
    public enum MessageType
    {
        DAMAGED,
        DEAD,
        RESPAWN,
        // 추가되는 메시지
    }

    /// <summary>
    /// 메시지 전달 메서드 정의
    /// </summary>
    public interface IMessageReceiver
    {
        public void OnReceiveMessage(MessageType type, object sender, object msg);
    }
}

