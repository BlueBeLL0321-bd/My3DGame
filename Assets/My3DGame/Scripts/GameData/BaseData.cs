using UnityEngine;
using System.Collections.Generic;

namespace My3DGame.GameData
{
    /// <summary>
    /// Data 부모 클래스
    /// 공통적인 데이터 : 데이터 이름 리스트
    /// 공통적인 기능 : 데이터 이름 리스트 얻어 오기, 데이터 개수 얻어 오기, 데이터 추가, 복사, 제거하기
    /// </summary>
    public class BaseData : ScriptableObject
    {
        #region Variables
        public List<string> names;                                                          // 데이터 이름 리스트

        public const string dataDirectory = "/My3DGame/ResourcesData/Resources/Data/";      // 데이터 파일 경로
        #endregion

        // 생성자
        public BaseData() { }

        #region Custom Method
        // 데이터 개수 얻어 오기
        public int GetDataCount()
        {
            // names 체크
            if (this.names == null)
                return 0;

            return names.Count;
        }

        // 데이터 이름 리스트 얻어 와서 툴 리스트에 이름 출력
        // showID : true면 이름 앞에 인덱스 붙이기, filterWord가 포함된 이름만 출력
        public string[] GetNameList(bool showID, string filterWord = "")
        {
            int length = GetDataCount();
            // 얻어 온 길이만큼 retList 배열 생성
            string[] retList = new string[length];

            // 반복문에서 retList 값을 생성한다
            for (int i = 0; i < length; i++)
            {
                // filterWord가 포함되어 있지 않으면 continue
                if(filterWord != "")
                {
                    if(names[i].ToLower().Contains(filterWord.ToLower()) == false)
                    {
                        continue;
                    }
                }

                if(showID)
                {
                    retList[i] = i.ToString() + " : " + names[i];
                }
                else
                {
                    retList[i] = names[i];
                }
            }

            return retList;
        }

        // 데이터 추가하기 : 매개 변수로 새로운 데이터 이름을 받아 추가한 후 데이터 리스트 최종 개수 리턴한다
        public virtual int AddData(string newName)
        {
            return GetDataCount();
        }

        // 데이터 추가하기 : 매개 변수로 받은 해당 인덱스의 데이터를 제거
        public virtual void RemoveData(int index)
        {
            
        }

        // 데이터 복사하기 : 매개 변수로 받은 해당 인덱스의 데이터를 복사해서 새로 데이터를 추가한다
        public virtual void CopyData(int index)
        {

        }
        #endregion
    }
}