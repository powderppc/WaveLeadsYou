using PowderPPC.System.SaveData;
using UnityEngine;

namespace PowderPPC.Object.Stage.World
{
    /// <summary>
    /// ステージ選択画面のゴールにアタッチし、アンロックレベルに応じて自身を制御する
    /// </summary>
    public class WorldUnLockLevelController : MonoBehaviour
    {
        GoalController goalObj;
        /// <summary>
        /// 自身をアンロックするために必要なレベル
        /// Worldいくつを攻略したかの数値をそのまま使う(W1クリアで解放なら 1 )
        /// </summary>
        [SerializeField] int unlockLevel;

        /// <summary>
        /// アンロック状態の時に自身を見せるかどうか
        /// </summary>
        [SerializeField] bool isInvisible;
        // Use this for initialization
        void Start()
        {
            goalObj = GetComponent<GoalController>();

            //セーブデータからクリア状態を取得
            var level = SaveManager.Instance.AchievementSaveData.ClearLevel;

            if (level >= unlockLevel)
            {
                //エフェクトを出さずにゴール扱いする
                goalObj.UnLock(false);
            }
            else
            {
                goalObj.Lock();
                if (isInvisible)
                {
                    goalObj.gameObject.SetActive(false);
                }
            }
        }

    }
}