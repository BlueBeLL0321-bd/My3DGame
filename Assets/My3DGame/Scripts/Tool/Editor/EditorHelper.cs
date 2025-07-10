using UnityEditor;
using UnityEngine;
using My3DGame.GameData;
using UnityObject = UnityEngine.Object;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;

namespace My3DGame.Tool
{
    /// <summary>
    /// 모든 툴에서 사용되는 공통 기능 구현
    /// </summary>
    public class EditorHelper
    {
        // 데이터 툴의 상단 레이어 UI (데이터 추가, 복사, 제거 버튼 구성)
        public static void EditToolTopLayer(BaseData data, ref int selection,
            ref UnityObject source, int uiWidth)
        {
            EditorGUILayout.BeginHorizontal();
            {
                // 추가 버튼
                if(GUILayout.Button("ADD", GUILayout.Width(uiWidth)))
                {
                    // 데이터 추가 처리
                    data.AddData("NewData");
                    selection = data.GetDataCount() - 1;
                    source = null;
                }
                // 복사 버튼
                if(GUILayout.Button("COPY", GUILayout.Width(uiWidth)))
                {
                    // 선택된 데이터를 복사하여 새로 추가한다
                    data.CopyData(selection);
                    selection = data.GetDataCount() - 1;
                    source = null;
                }
                // 제거 버튼 - 데이터가 두 개 이상일 때만 제거가 가능하다
                if(data.GetDataCount() > 1)
                {
                    if(GUILayout.Button("REMOVE", GUILayout.Width(uiWidth)))
                    {
                        source = null;
                        data.RemoveData(selection);
                    }
                }
                // 인덱스 예외 처리
                if(selection > (data.GetDataCount() - 1))
                {
                    selection = data.GetDataCount() - 1;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        // 데이터 이름 리스트 레이어
        public static void EditorToolListLayer(BaseData data, ref int selection,
            ref UnityObject source, int uiWidth, ref Vector2 scrollPosition)
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(uiWidth));
            {
                EditorGUILayout.Separator();        // 구분 공간 주기
                EditorGUILayout.BeginVertical("box");
                {
                    scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                    {
                        if(data.GetDataCount() > 0)
                        {
                            int lastSelection = selection;
                            selection = GUILayout.SelectionGrid(selection, data.GetNameList(true), 1);
                            if(lastSelection != selection)
                            {
                                source = null;
                            }
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
        }

        // 오브젝트 필드에서 입력 받은 에셋 오브젝트의 Resources 폴더 이하의 저장 경로 얻어 오기
        public static string GetPath(UnityObject pClip)
        {
            string pathString = string.Empty;
            pathString = AssetDatabase.GetAssetPath(pClip);
            Debug.Log($"Asset Full Path : {pathString}");

            string[] path_node = pathString.Split('/');
            bool findResources = false;
            for (int i = 0; i < path_node.Length - 1; i++)
            {
                if(findResources == false)
                {
                    if (path_node[i] == "Resources")
                    {
                        findResources = true;
                        pathString = string.Empty;
                    }
                }
                else
                {
                    pathString += path_node[i] + "/";
                }
            }

            Debug.Log($"Asset Find Path : {pathString}");
            return pathString;
        }

        // 데이터 이름 리스트로 enum 구조체 만들기 (enumName, builder 바꿔치기)
        public static void CreateEnumStructure(string enumName, StringBuilder data)
        {
            // enum 템플릿 가져오기
            string templateFilePath = "Assets/My3DGame/Editor/EnumTemplate.txt";
            string entityTemplate = File.ReadAllText(templateFilePath);

            entityTemplate = entityTemplate.Replace("$ENUM$", enumName);
            entityTemplate = entityTemplate.Replace("$DATA$", data.ToString());

            // 경로 체크
            string folderPath = "Assets/My3DGame/Scripts/GameData/";
            if(Directory.Exists(folderPath) == false)
            {
                Directory.CreateDirectory(folderPath);
            }
            // 파일 체크
            string filePath = folderPath + enumName + ".cs";
            if(File.Exists(filePath) == true)
            {
                // 기존 enum을 지운다
                File.Delete(filePath);
            }
            // enum을 새로 만든다
            File.WriteAllText(filePath, entityTemplate);
        }
    }
}

