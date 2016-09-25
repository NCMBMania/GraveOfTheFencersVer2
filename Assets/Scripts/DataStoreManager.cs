using NCMB;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStoreManager : SingletonMonoBehaviour<DataStoreManager>
{

    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    public void SaveGraveInfo(string userName, string deathMessage, GraveInfo.CurseType curseType, Vector3 position, Action callback)
    {
        //ユーザー名が空の場合"Unknown"に//
        userName = string.IsNullOrEmpty(userName) ? "Unknown" : userName;

        //プレイヤーが死んだ位置を加工//
        position = new Vector3(position.x, 0f, position.z);
        double[] positionDoubleArray = Utility.Vector3toDoubleArray(position);

        //データストアにGraveクラスを定義//
        NCMBObject ncmbObject = new NCMBObject("Grave");

        //Message・UserName・Position・CurseTypeをKeyに、それぞれValueを設定//
        ncmbObject.Add("Message", deathMessage);
        ncmbObject.Add("UserName", userName);
        ncmbObject.Add("Position", positionDoubleArray);
        ncmbObject.Add("CurseType", (int)curseType);
        ncmbObject.Add("CheckCounter", 0);

        //非同期でデータを保存する//
        ncmbObject.SaveAsync((NCMBException e) =>
        {
            if (e != null)
            {
                //エラー処理
                
            }
            else
            {
                if (callback != null) callback();
                UserAuth.Instance.SetLastGraveObject(ncmbObject);
           }
        });
    }

    public void CountUpGraveUsedNumger(string objectid)
    {
        //対応するオブジェクトidの発動回数フィールドをカウントアップする//
    }

    public void FetchGraveData(Action callback)
    {
        //PinPositionクラスを検索するクエリを作成
        NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("Grave");

        //日付順にソート//
        query.OrderByDescending("createDate");

        //最新10件までを取得する//
        query.Limit = 10;

        query.FindAsync((List<NCMBObject> graveList, NCMBException e) =>
        {
            if (e != null)
            {
                //データは見つからなかった//
                Debug.Log("c:"+e.ErrorCode +" m:" + e.ErrorMessage);
                //Status 0の場合、ErrorCodeはempty、 ErrorMessageは The request timed out//

                callback();
            }
            else
            {
                List<GraveInfo> graveInfoList = new List<GraveInfo>();

                foreach (NCMBObject graveObject in graveList)
                {
                    graveInfoList.Add(GenerateGraveInfoFromGraveObject(graveObject));
                }

                GraveObjectsManager.Instance.DisableAllGraves();

                //GraveInfoListをGraveObjectsManager.Instance.InstallationGraves()に引き渡し//
                GraveObjectsManager.Instance.InstallationGraves(graveInfoList);
                callback();
            }
        });
    }

    private static GraveInfo GenerateGraveInfoFromGraveObject(NCMBObject graveObject)
    {
        //取得結果をGraveInfo構造体に格納//
        GraveInfo graveInfo;
        graveInfo.userName = graveObject["UserName"] as string;
        graveInfo.deathMessage = graveObject["Message"] as string;
        graveInfo.objectId = graveObject.ObjectId;
        graveInfo.curseType = (GraveInfo.CurseType)Enum.ToObject(typeof(GraveInfo.CurseType), graveObject["CurseType"]);
        graveInfo.position = Utility.DoubleArrayListToVector3(graveObject["Position"] as ArrayList);
        graveInfo.isUsed = false;

        //バージョン違い対策//
        if (graveObject.ContainsKey("CheckCounter"))
        {
            graveInfo.checkCounter = Convert.ToInt32(graveObject["CheckCounter"]);
        }
        else
        {
            graveInfo.checkCounter = 0;
        }
      

        return graveInfo;
    }

    public void FetchLastGraveInfo(Action callback)
    {
        NCMBObject grave = UserAuth.Instance.GetLastGraveObject();
        grave.FetchAsync((NCMBException e) => {
            if (e != null)
            {
                //エラー処理
            }
            else
            {
                State_InGame.Instance.lastGraveInfo = GenerateGraveInfoFromGraveObject(grave);
                //成功時の処理
                if (callback != null) callback();
            }
        });
    }


    public void CountUpGraveCheckCounter(string graveId)
    {
        NCMBObject grave = new NCMBObject("Grave");
        grave.ObjectId = graveId;
        grave.FetchAsync((NCMBException e) => {
            if (e != null)
            {
                //エラー処理
            }
            else
            {
                //成功時の処理
                grave.Increment("CheckCounter");
                grave.SaveAsync();
            }
        });
    }
}