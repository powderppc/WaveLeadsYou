using PowderPPC.System.SaveData;
using UnityEngine;

namespace PowderPPC.Object.Stage.World
{
    /// <summary>
    /// ステージクリア時にクリアレベルを更新するためのクラス
    /// n-4クリア後のステージに配置する
    /// </summary>
    public class WorldUnLocker : MonoBehaviour
    {
        /// <summary>
        /// 自身が持つアンロックレベル
        /// </summary>
        [SerializeField] int unLockLevel;

        // Use this for initialization
        void Start()
        {
            SaveManager.Instance.AchievementSaveData.SaveClearLevel(unLockLevel);
        }


    }
}