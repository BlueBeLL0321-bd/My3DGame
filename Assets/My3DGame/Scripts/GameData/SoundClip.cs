using My3DGame.Common;
using My3DGame.Manager;
using UnityEngine;

namespace My3DGame.GameData
{
    /// <summary>
    /// 사운드 클립 속성 관리하는 클래스
    /// 속성 : 사운드 아이디, 이름, 타입, 오디오 클립 에셋 이름, 오디오 클립 에셋 경로
    /// 속성 : 볼륨, 피치, 루프, 3D 사운드, 최소 거리, 최대 거리
    /// 기능 : 오디오 클립 에셋 로딩, 에셋 해제, 오디오 클립 가져오기
    /// </summary>

    public class SoundClip
    {
        #region Variables
        private AudioClip audioClip = null;     // 오디오 클립 에셋 경로 있는 오디오 클립
        #endregion

        #region Property
        public int Id { get; set; }
        public string Name { get; set; }
        public SoundType Type { get; set; }
        public string ClipPath { get; set; }      // 오디오 클립 저장 경로
        public string ClipName { get; set; }      // 오디오 클립 이름

        public float IsLoop { get; set; }
        public float Volume { get; set; }
        public float Pitch { get; set; }
        public float SpatialBlend { get; set; }
        public float MinDistance { get; set; }
        public float MaxDistance { get; set; }
        #endregion

        #region Constructor
        public SoundClip() { }
        public SoundClip(string clipPath, string clipName)
        {
            this.ClipPath = clipPath;
            this.ClipName = clipName;
        }
        #endregion

        #region Custom Method
        // 오디오 클립 경로에 있는 오디오 클립 가져오기
        public void PreLoad()
        {
            // 에셋 경로
            var clipFullPath = ClipPath + ClipName;
            // 경로가 있고 아직 effectPrefab을 로딩하지 않았으면
            if (clipFullPath != string.Empty && audioClip == null)
            {
                audioClip = Resources.Load<AudioClip>(clipFullPath);
            }
        }

        // 오디오 클립 해제
        public void ReleaseClip()
        {
            if (audioClip != null)
            {
                audioClip = null;
            }
        }

        // 오디오 클립 가져오기
        public AudioClip GetAudioClip()
        {
            // audioClip 체크해서 null이면 리소스 로드
            if (this.audioClip == null)
            {
                PreLoad();
            }

            return audioClip;
        }
        #endregion
    }
}

