using PowderPPC.Object.Player;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace PowderPPC.Object.Block
{
    /// <summary>
    /// ブロックの初期生成及び連結処理を行うクラス
    /// </summary>
    public class BlockChainController : MonoBehaviour
    {
        [SerializeField] BlockController blockObj;
        [SerializeField] int blockNum;

        PlayerController playerObj;

        [SerializeField] List<BlockController> blockObjList;

        /// <summary>
        /// ブロックのサイズ（中心からのスプライトの大きさ）
        /// </summary>
        Vector2 blockSize;

        /// <summary>
        /// プレイヤーへの干渉を行える範囲
        /// </summary>
        BoxCollider2D myTrigger;

        /// <summary>
        /// 波の最大の高さ
        /// </summary>
        float maxAmplitude;

        /// <summary>
        /// 波の最大高さを元のサイズに戻すスピード
        /// </summary>
        [SerializeField] float amplitudeRevertSpeed;
        /// <summary>
        /// プレイヤーへの干渉領域のデフォルトの高さ
        /// </summary>
        float initAmplitude;
        /// <summary>
        /// 元の判定からどれだけ大きくするか
        /// </summary>
        [SerializeField] Vector2 expandSize;

        void Awake()
        {
            myTrigger = GetComponent<BoxCollider2D>();

        }



        private void Start()
        {
            //blockObjList(SerializeField)で値を持っている場合はそれを参照する
            if (!blockObjList.IsUnityNull() && blockObjList.Count != 0)
            {
                blockNum = blockObjList.Count;
                blockSize = blockObj.gameObject.GetComponent<SpriteRenderer>().bounds.extents;

                Combine();
            }
            else
            {
                AwakeWithInstantiate();
            }

            //プレイヤー干渉領域の設定
            {
                var size = new Vector2();
                size.x = blockObjList.Count() * (blockSize.x * 2);
                size.y = blockSize.y * 2;
                myTrigger.size = size + expandSize;
                initAmplitude = size.y;
            }

            //各ブロックの初期化
            MyInit();
        }

        /// <summary>
        /// blockObjList(SerializeField)が値を持っていない場合の実体化、座標更新、連結処理
        /// </summary>
        void AwakeWithInstantiate()
        {
            //実体化
            if (blockNum == 0)
            {
                return;
            }
            blockObjList = new List<BlockController>();
            for (int i = 0; i < blockNum; ++i)
            {
                var obj = Instantiate(blockObj);
                blockObjList.Add(obj);
                obj.transform.SetParent(transform);

                if (i == 0)
                {
                    blockSize = blockObj.gameObject.GetComponent<SpriteRenderer>().bounds.extents;
                }
            }

            //ブロックの連結、座標の調整
            for (int i = 0; i < blockNum; ++i)
            {
                var pos = blockObjList[i].transform.position;

                //min:-x-1 max:x+1
                pos.x = blockSize.x * (2 * (i + 1) - blockNum - 1) + transform.position.x;
                pos.y = transform.position.y;
                blockObjList[i].transform.position = pos;

                //ブロック数が1の時は連結処理しない
                if (blockNum == 1)
                    break;

                if (i == 0)
                {
                    //後ろにのみ連結
                    blockObjList[i].Init(null, blockObjList[i + 1]);
                }
                else if (i == blockNum - 1)
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

        /// <summary>
        /// ブロックの連結処理のみ行う
        /// </summary>
        void Combine()
        {
            //ブロックの連結
            if (blockObjList.Count() <= 1)
            {
                return;
            }
            for (int i = 0; i < blockObjList.Count(); ++i)
            {
                if (i == 0)
                {
                    //後ろにのみ連結
                    blockObjList[i].Init(null, blockObjList[i + 1]);
                }
                else if (i == blockNum - 1)
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
            foreach(var b in blockObjList)
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
            var amplitude = 0f;
            foreach (var obj in blockObjList)
            {
                amplitude = Mathf.Max(amplitude, obj.MyPosUpdate());
            }
            UpdateAmplitude(amplitude);

            //プレイヤーが領域内に存在するなら諸々更新する
            if (!playerObj) return;

            var isStay = blockObjList.Any(b => b.IsStayPlayer);
            var isStayWithHighpos = blockObjList.Any(b => b.IsStayPlayerWithHighPos);

            if (isStay)
            {
                //プレイヤーとブロックが触れている状態なので、地上判定を試みる
                var result = playerObj.TryChangeStateOnGroundByBlock();
            }
            else
            {
                //プレイヤーとは触れていないものの、上昇中のオブジェクトがある場合は、空中判定にするのは控える
                if (!isStayWithHighpos)
                {
                    //無ければ空中判定を試みる
                    playerObj.TryChangeStateOnSkyByBlock();
                }
            }
        }

        /// <summary>
        /// BlockListが持ってる波の高さに応じて自身のプレイヤーに対する干渉領域を大きくする
        /// </summary>
        /// <param name="a"></param>
        private void UpdateAmplitude(float a)
        {
            if (maxAmplitude < a + expandSize.y)
            {
                maxAmplitude = a + expandSize.y;
            }
            else
            {
                if (maxAmplitude > initAmplitude)
                {
                    maxAmplitude -= amplitudeRevertSpeed * Time.deltaTime;
                }
            }

            //プレイヤー干渉領域（Y座標のみ）を更新
            var size = myTrigger.size;
            size.y = maxAmplitude + expandSize.y;
            myTrigger.size = size;
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.TryGetComponent<PlayerController>(out var p))
            {
                playerObj = p;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.TryGetComponent<PlayerController>(out var p))
            {
                playerObj?.TryChangeStateOnSkyByBlock();
                playerObj = null;
            }
        }

    }
}