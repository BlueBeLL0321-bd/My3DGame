using UnityEditor;
using UnityEngine;
using My3DGame.GameData;
using UnityObject = UnityEngine.Object;
using My3DGame.Common;
using System.Text;

namespace My3DGame.Tool
{
    public class SoundTool : EditorWindow
    {
        #region Variables
        // 사운드 데이터
        private static SoundData soundData;

        // 선택된 오디오 클립
        private AudioClip audioSource = null;

        // Editor UI
        public int uiWidthLarge = 300;      // 툴 창의 폭
        public int uiWidthMiddle = 200;
        private int selection = 0;          // 선택된 데이터의 현재 인덱스
        private Vector2 SP1 = Vector2.zero;
        private Vector2 SP2 = Vector2.zero;
        #endregion

        // 툴 Window 불러오기(show)
        [MenuItem("Tools/Sound Tool")]
        static void Init()
        {
            // SoundData 객체 생성하고 데이터 가져오기
            soundData = ScriptableObject.CreateInstance<SoundData>();
            soundData.LoadData();

            // Tool Window 열기
            SoundTool window = GetWindow<SoundTool>(false, "Sound Tool");
            window.Show();
        }

        #region Unity Event Method
        // 툴 Window UI 구성
        private void OnGUI()
        {
            // soundData 체크
            if (soundData == null)
                return;

            EditorGUILayout.BeginVertical();
            {
                UnityObject source = audioSource;

                // 데이터 툴의 상단 레이어 (데이터 추가, 복사, 제거 버튼 구성)
                EditorHelper.EditToolTopLayer(soundData, ref selection, ref source,
                    uiWidthMiddle);
                audioSource = (AudioClip)source;

                // 데이터 부분
                EditorGUILayout.BeginHorizontal();
                {
                    // 데이터 이름 리스트 레이어
                    EditorHelper.EditorToolListLayer(soundData, ref selection, ref source,
                        uiWidthLarge, ref SP1);
                    audioSource = (AudioClip)source;

                    // 선택된 데이터 설정 레이어
                    EditorGUILayout.BeginVertical();
                    {
                        SP2 = EditorGUILayout.BeginScrollView(SP2);
                        {
                            // 데이터 파일 체크
                            if (soundData.GetDataCount() > 0)
                            {
                                EditorGUILayout.BeginVertical();
                                {
                                    EditorGUILayout.Separator();    // 공간 띄우기
                                    // 선택 인덱스 값 쓰기
                                    EditorGUILayout.LabelField("ID", selection.ToString(),
                                        GUILayout.Width(uiWidthLarge));
                                    // 이름(string) 입력창 만들기
                                    soundData.names[selection] = EditorGUILayout.TextField("사운드 이름",
                                        soundData.names[selection], GUILayout.Width(uiWidthLarge * 1.5f));
                                    // 사운드 타입(enum) 입력창 만들기
                                    soundData.soundClips[selection].Type = (SoundType)EditorGUILayout.EnumPopup("사운드 타입",
                                        soundData.soundClips[selection].Type,
                                        GUILayout.Width(uiWidthLarge));

                                    EditorGUILayout.Separator();    // 공간 띄우기
                                    // 오디오 클립 사전 로드
                                    if (audioSource == null
                                        && soundData.soundClips[selection].ClipName != string.Empty)
                                    {
                                        audioSource = Resources.Load<AudioClip>(
                                            soundData.soundClips[selection].ClipPath
                                            + soundData.soundClips[selection].ClipName);
                                    }
                                    // 오디오 클립 입력창
                                    audioSource = (AudioClip)EditorGUILayout.ObjectField("사운드 클립",
                                        audioSource, typeof(AudioClip), false,
                                        GUILayout.Width(uiWidthLarge * 1.5f));
                                    // 오디오 클립에서 저장 경로와 이름 가져오기
                                    if (audioSource != null)
                                    {
                                        soundData.soundClips[selection].ClipName = audioSource.name;
                                        soundData.soundClips[selection].ClipPath = EditorHelper.GetPath(audioSource);
                                    }
                                    else
                                    {
                                        soundData.soundClips[selection].ClipName = string.Empty;
                                        soundData.soundClips[selection].ClipPath = string.Empty;
                                    }

                                    // 추가된 속성 입력창 구현
                                }
                                EditorGUILayout.EndVertical();
                            }
                        }
                        EditorGUILayout.EndScrollView();
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();    // 공간 띄우기
            // 하단 - 로드, 저장 버튼
            EditorGUILayout.BeginHorizontal();
            {
                // 로드 버튼
                if (GUILayout.Button("Reload Settings"))
                {
                    // SoundData 객체 다시 생성하고 다시 파일에서 데이터 가져오기
                    soundData = ScriptableObject.CreateInstance<SoundData>();
                    soundData.LoadData();
                    // 초기화
                    selection = 0;
                    audioSource = null;
                }
                // 저장 버튼
                if (GUILayout.Button("Save"))
                {
                    // 툴에 저장된 값 파일로 저장하기
                    soundData.SaveData();
                    // 새로 설정된 값으로 enum을 새로 만든다
                    CreateEnumStructure();
                    // 에셋 폴더 내용을 새로 강제로 갱신한다
                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        public void CreateEnumStructure()
        {
            string enumName = "SoundList";

            StringBuilder builder = new StringBuilder();
            builder.AppendLine();
            int length = soundData.GetDataCount();
            for (int i = 0; i < length; i++)
            {
                if (soundData.names[i] != string.Empty)
                {
                    builder.AppendLine("    " + soundData.names[i] + " = " + i + ",");
                }
            }
            // enumName, builder 바꿔치기
            EditorHelper.CreateEnumStructure(enumName, builder);
        }
        #endregion
    }
}

