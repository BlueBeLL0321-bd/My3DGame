using UnityEditor;
using UnityEngine;
using My3DGame.GameData;
using UnityObject = UnityEngine.Object;
using My3DGame.Common;
using System.Text;

namespace My3DGame.Tool
{
    /// <summary>
    /// 이펙트 데이터를 관리하는 툴 제작
    /// </summary>
    public class EffectTool : EditorWindow
    {
        #region Variables
        // 이펙트 데이터
        private static EffectData effectData;

        // 선택된 이펙트 클립 이펙트 오브젝트
        private GameObject effectSource = null;

        // Editor UI
        public int uiWidthLarge = 300;      // 툴 창의 폭
        public int uiWidthMiddle = 200;
        private int selection = 0;          // 선택된 데이터의 현재 인덱스
        private Vector2 SP1 = Vector2.zero;
        private Vector2 SP2 = Vector2.zero;
        #endregion

        // 툴 Window 불러오기(show)
        [MenuItem("Tools/Effect Tool")]
        static void Init()
        {
            // EffectData 객체 생성하고 데이터 가져오기
            effectData = ScriptableObject.CreateInstance<EffectData>();
            effectData.LoadData();

            // Tool Window 열기
            EffectTool window = GetWindow<EffectTool>(false, "Effect Tool");
            window.Show();
        }

        #region Unity Event Method
        // 툴 Window UI 구성
        private void OnGUI()
        {
            // effectData 체크
            if (effectData == null)
                return;

            EditorGUILayout.BeginVertical();
            {
                UnityObject source = effectSource;

                // 데이터 툴의 상단 레이어 (데이터 추가, 복사, 제거 버튼 구성)
                EditorHelper.EditToolTopLayer(effectData, ref selection, ref source,
                    uiWidthMiddle);
                effectSource = (GameObject)source;

                // 데이터 부분
                EditorGUILayout.BeginHorizontal();
                {
                    // 데이터 이름 리스트 레이어
                    EditorHelper.EditorToolListLayer(effectData, ref selection, ref source,
                        uiWidthLarge, ref SP1);
                    effectSource = (GameObject)source;

                    // 선택된 데이터 설정 레이어
                    EditorGUILayout.BeginVertical();
                    {
                        SP2 = EditorGUILayout.BeginScrollView(SP2);
                        {
                            // 데이터 파일 체크
                            if(effectData.GetDataCount() > 0)
                            {
                                EditorGUILayout.BeginVertical();
                                {
                                    EditorGUILayout.Separator();    // 공간 띄우기
                                    // 선택 인덱스 값 쓰기
                                    EditorGUILayout.LabelField("ID", selection.ToString(),
                                        GUILayout.Width(uiWidthLarge));
                                    // 이름(string) 입력창 만들기
                                    effectData.names[selection] = EditorGUILayout.TextField("이펙트 이름",
                                        effectData.names[selection], GUILayout.Width(uiWidthLarge * 1.5f));
                                    // 이펙트 타입(enum) 입력창 만들기
                                    effectData.effectClips[selection].Type = (EffectType)EditorGUILayout.EnumPopup("이펙트 타입",
                                        effectData.effectClips[selection].Type,
                                        GUILayout.Width(uiWidthLarge));

                                    EditorGUILayout.Separator();    // 공간 띄우기
                                    // 이펙트 오브젝트 사전 로드
                                    if(effectSource == null
                                        && effectData.effectClips[selection].EffectName != string.Empty)
                                    {
                                        effectSource = (GameObject)Resources.Load(
                                            effectData.effectClips[selection].EffectPath
                                            + effectData.effectClips[selection].EffectName);
                                    }
                                    // 이펙트 오브젝트 입력창
                                    effectSource = (GameObject)EditorGUILayout.ObjectField("이펙트",
                                        effectSource, typeof(GameObject), false,
                                        GUILayout.Width(uiWidthLarge * 1.5f));
                                    // 이펙트 오브젝트에서 저장 경로와 이름 가져오기
                                    if(effectSource != null)
                                    {
                                        effectData.effectClips[selection].EffectName = effectSource.name;
                                        effectData.effectClips[selection].EffectPath = EditorHelper.GetPath(effectSource);
                                    }
                                    else
                                    {
                                        effectData.effectClips[selection].EffectName = string.Empty;
                                        effectData.effectClips[selection].EffectPath = string.Empty;
                                    }
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
                if(GUILayout.Button("Reload Settings"))
                {
                    // EffectData 객체 다시 생성하고 다시 파일에서 데이터 가져오기
                    effectData = ScriptableObject.CreateInstance<EffectData>();
                    effectData.LoadData();
                    // 초기화
                    selection = 0;
                    effectSource = null;
                }
                // 저장 버튼
                if(GUILayout.Button("Save"))
                {
                    // 툴에 저장된 값 파일로 저장하기
                    effectData.SaveData();
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
            string enumName = "EffectList";

            StringBuilder builder = new StringBuilder();
            builder.AppendLine();
            int length = effectData.GetDataCount();
            for (int i = 0; i < length; i++)
            {
                if(effectData.names[i] != string.Empty)
                {
                    builder.AppendLine("    " + effectData.names[i] + " = " + i + ",");
                }
            }
            // enumName, builder 바꿔치기
            EditorHelper.CreateEnumStructure(enumName, builder);
        }
        #endregion
    }
}

