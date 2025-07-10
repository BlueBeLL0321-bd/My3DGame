using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace My3DGame.Manager
{
    /// <summary>
    /// 리소스를 관리하는 클래스 : 리소스 로드, 리소스 로드한 오브젝트로 게임 오브젝트 Instantiate
    /// </summary>
    public class ResourcesManager : MonoBehaviour
    {
        #region Custom Method
        // 매개 변수로 받은 경로에 있는 에셋을 UnityObject로 가져오기
        public static UnityObject Load(string path)
        {
            return Resources.Load(path);
        }

        // 매개 변수로 받은 경로에 있는 에셋을 UnityObject로 가져와서 Instantiate 하기
        public static GameObject LoadAndInstantiate(string path)
        {
            UnityObject source = Load(path);
            // source 체크
            if (source != null)
                return null;

            return GameObject.Instantiate(source) as GameObject;
        }
        #endregion
    }
}

