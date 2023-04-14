using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace PowderPPC.Object.Block
{
    public class UnTouchableBlockChainController : MonoBehaviour
    {

        [SerializeField] List<UnTouchableBlockController> blockObjList;


        private void Start()
        {
            //blockObjList(SerializeField)で値を持っている場合はそれを参照する
            if (!blockObjList.IsUnityNull() && blockObjList.Count != 0)
            {
                Combine();
            }

            //各ブロックの初期化
            MyInit();
        }


        /// <summary>
        /// ブロックの連結処理のみ行う
        /// </summary>
        void Combine()
        {
            var count = blockObjList.Count();
            //ブロックの連結
            if (count == 0)
            {
                return;
            }
            for (int i = 0; i < count; ++i)
            {
                if (i == 0)
                {
                    //後ろにのみ連結
                    blockObjList[i].Init(null, blockObjList[i + 1]);
                }
                else if (i == count - 1)
                {
                    //前のみ連結
                    blockObjList[i].Init(blockObjList[i - 1], null);
                }
                else
                {
                    blockObjList[i].Init(blockObjList[i - 1], blockObjList[i + 1]);
                }
            }
        }

        void MyInit()
        {
            foreach (var b in blockObjList)
            {
                b.MyInit();
            }
        }

        private void FixedUpdate()
        {
            var t = Time.deltaTime;
            //時間の更新
            foreach (var obj in blockObjList)
            {
                obj.MyTimerUpdate(t);
            }
            //伝導処理の更新
            foreach (var obj in blockObjList)
            {
                obj.MyTransmitUpdate();
            }
            //伝導結果を反映
            foreach (var obj in blockObjList)
            {
                obj.MyPosUpdate();
            }

        }



    }
}