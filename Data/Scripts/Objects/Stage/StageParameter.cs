using System;
using UnityEngine;

namespace PowderPPC.Object.Stage
{
    /// <summary>
    /// 次のステージに遷移するためのステージPrefabとゴールを管理
    /// </summary>
    [Serializable]
    public class StageParameter
    {
        /// <summary>
        /// 次のステージ
        /// </summary>
        [SerializeField] StageController nextStage;
        public StageController NextStage => nextStage;

        /// <summary>
        /// 次のステージに行くためのゴール
        /// </summary>
        [SerializeField] GoalController goalObj;
        public GoalController GoalObj => goalObj;
    }
}