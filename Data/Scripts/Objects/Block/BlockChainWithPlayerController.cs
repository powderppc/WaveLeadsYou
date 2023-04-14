using PowderPPC.Object.Player;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PowderPPC.Object.Block
{
    /// <summary>
    /// ※未使用
    /// ブロックの初期生成及び連結処理を行うクラス
    /// プレイヤーが乗っていた場合はブロックに追従させる
    /// </summary>
    public class BlockChainWithPlayerController : MonoBehaviour
    {
        [SerializeField] GameObject blockObj;
        [SerializeField] int blockNum;

        [SerializeField] PlayerController playerObj;

        public List<BlockWithPlayerController> BlockList { get; private set; }
        /// <summary>
        /// ブロックのサイズ（中心からのスプライトの大きさ）
        /// </summary>
        Vector2 blockSize;

        /// <summary>
        /// プレイヤーと接触判定を持っているかどうか
        /// 接触している場合Trueで、FixedUpdateのタイミングでFalseになる
        /// IsPlayerInChildのとき、一定距離以上離れてなければ解除されない
        /// </summary>
        public bool IsStayPlayer { get; private set; }

        BlockWithPlayerController BlockWithPlayer;

        /// <summary>
        /// 切断済みかどうか
        /// </summary>
        bool isAlreadyDissConnected;

        void Awake()
        {
            BlockList = new List<BlockWithPlayerController>();

            //実体化
            for (int i = 0; i < blockNum; ++i)
            {
                var obj = Instantiate(blockObj);
                BlockList.Add(obj.GetComponent<BlockWithPlayerController>());
                obj.transform.SetParent(transform);

                if (i == 0)
                {
                    blockSize = blockObj.GetComponent<SpriteRenderer>().bounds.extents;
                }
            }

            //ブロックの連結、座標の調整
            for (int i = 0; i < blockNum; ++i)
            {
                var pos = BlockList[i].transform.position;

                //min:-x-1 max:x+1
                pos.x = blockSize.x * (2 * (i + 1) - blockNum - 1);
                BlockList[i].transform.position = pos;

                if (i == 0)
                {
                    //後ろにのみ連結
                    BlockList[i].Init(null, BlockList[i + 1]);
                }
                else if (i == blockNum - 1)
                {
                    //前のみ連結
                    BlockList[i].Init(BlockList[i - 1], null);
                }
                else
                {
                    BlockList[i].Init(BlockList[i - 1], BlockList[i + 1]);
                }
            }
        }

        private void FixedUpdate()
        {
            //プレイヤーを子として所持しているブロックが存在する場合、
            //それが一定以上の距離が離れていないかを計算（Collisionでの判定ができないため）
            if (BlockWithPlayer)
            {
                //プレイヤーがジャンプ等で解放されていないかを確認
                if (playerObj.IsPlayerOnSky)
                {
                    //一定以上離れているので切り離す
                    Debug.Log("Dissconnect Player Own Jump");
                    DisConnectPlayerToBlock();

                }

                else if (Mathf.Abs(BlockWithPlayer.transform.position.x
                    - playerObj.transform.position.x) > 0.45f)
                {
                    //一定以上離れているので切り離す
                    Debug.Log("Dissconnect Lenght over");
                    Debug.Log($"Block {BlockWithPlayer.transform.position.x}");
                    Debug.Log($"Player {playerObj.transform.position.x}");
                    DisConnectPlayerToBlock();

                }
            }

            //追従する対象のブロックを選択
            //触れているブロックのうち座標の高いやつを抽出
            var block = BlockList.Where(b => b.IsStayPlayer)
                .OrderByDescending(b => b.transform.position.y)
                //.OrderBy(b => b.transform.position.x * playerObj.PlayerAngleToSign)
                .FirstOrDefault();

            IsStayPlayer = block;


            if (IsStayPlayer)
            {
                var result = playerObj.TryChangeStateOnGroundByBlock();
                //地上判定にすることに成功した場合は、座標の追従をする
                if (result)
                {
                    //対象のブロックの切り替え
                    if (BlockWithPlayer != block)
                    {
                        BlockWithPlayer?.RemovePlayer();
                        playerObj.transform.parent = block.transform;
                        block.SetPlayer();
                        BlockWithPlayer = block;
                        playerObj.MyRigidbody.bodyType = RigidbodyType2D.Kinematic;

                        isAlreadyDissConnected = false;

                        Debug.Log("Connect");
                    }

                }
            }
            else
            {
                //接触しているブロックが存在しない場合は、プレイヤーをブロックから切り離す
                //子が存在する場合は当たり判定が消えてるので除外
                if (!BlockWithPlayer)
                {
                    if (!isAlreadyDissConnected)
                    {
                        Debug.Log("Dissconnect Not Stay Player");
                    }
                    DisConnectPlayerToBlock();
                    ReleasePlayerState();
                }

            }


        }

        /// <summary>
        /// ブロックとプレイヤーを切り離す
        /// </summary>
        private void DisConnectPlayerToBlock()
        {
            //if (isAlreadyDissConnected)
            //{
            //    return;
            //}
            isAlreadyDissConnected = true;

            BlockWithPlayer?.RemovePlayer();
            BlockWithPlayer = null;
            playerObj.transform.parent = null;
            playerObj.MyRigidbody.bodyType = RigidbodyType2D.Dynamic;
        }

        /// <summary>
        /// プレイヤーを開放する（空中に放り出す）
        /// </summary>
        private void ReleasePlayerState()
        {
            playerObj.TryChangeStateOnSkyByBlock();
        }

    }
}