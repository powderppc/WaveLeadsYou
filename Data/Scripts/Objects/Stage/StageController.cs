using PowderPPC.Object.Player;
using System.Collections.Generic;
using UnityEngine;

namespace PowderPPC.Object.Stage
{
    /// <summary>
    /// 1ステージを管理
    /// ゴールを検出して次のステージに移動する役目を果たす
    /// プレイヤーの死亡を検出した場合は同じステージを再読み込みする
    /// </summary>
    public class StageController : MonoBehaviour
    {
        [SerializeField] string stageName;
        public string StageName => stageName;

        [SerializeField] List<StageParameter> nextStageList;

        [SerializeField] PlayerController playerObj;

        /// <summary>
        /// ステージの再読み込みが必要かどうか
        /// </summary>
        public bool IsNeedLoadScene { get; private set; }
        /// <summary>
        /// ゴールされたかどうか
        /// 次のステージ読み込み時の画面遷移に使用
        /// </summary>
        public bool IsStageCleared { get; private set; }

        public PlayerController PlayerObj => playerObj;

        public GoalController GoaledObj { get; private set; }

        void Start()
        {
            IsNeedLoadScene = false;
            IsStageCleared = false;

        }


        // Update is called once per frame
        void Update()
        {
            //ゲームのサイクルを監視
            if (!IsNeedLoadScene)
            {
                foreach(var nextStage in nextStageList)
                {
                    if (nextStage.GoalObj.IsGoaled)
                    {
                        //次のステージが自身ではない場合のみ更新…対応はできてない（なぜかNullになるっぽいので）
                        StageLoader.NextStage = nextStage.NextStage;

                        EndCall(true);
                        //プレイヤーをゴール状態にする
                        playerObj.ForceGoal();
                        //ゴールしたオブジェクトを適用
                        GoaledObj = nextStage.GoalObj;
                        return;
                    }
                }
                if (playerObj.IsDied)
                {
                    //StageLoader.NextStage = myStage;
                    EndCall(false);
                    return;
                }

            }

        }

        void EndCall(bool isGoaled)
        {
            IsStageCleared = isGoaled;
            //ロードが必要であることを伝え、自身はこれ以上監視を行わない
            IsNeedLoadScene = true;
        }
    }
}