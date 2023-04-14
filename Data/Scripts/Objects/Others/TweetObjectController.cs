using PowderPPC.Object.Player;
using PowderPPC.System.SaveData;
using PowderPPC.System.Sound;
using UnityEngine;

namespace PowderPPC.Object.Others
{
    /// <summary>
    /// プレイヤーが触れるとツイート機能が行われる
    /// </summary>
    public class TweetObjectController : MonoBehaviour
    {
        [SerializeField] ExplosionController explodeObj;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.gameObject.TryGetComponent<PlayerController>(out var _))
            {
                //爆発処理が無い時に発動
                if (!explodeObj.gameObject.activeSelf)
                {
                    explodeObj.gameObject.SetActive(true);
                    SoundManager.Instance.SoundGoal();
                    Tweet();
                }
            }
        }

        void Tweet()
        {
            var str = "";
            if (SaveManager.Instance.AchievementSaveData.ClearLevel > 7)
            {
                str = "EXを含むすべてのステージをクリアした！\n";
            }
            else
            {
                str = "すべてのステージをクリアした！\n";
            }
            naichilab.UnityRoomTweet.Tweet("wave-leads-you", str, "unity1week", "WaveLeadsYou");
        }
    }
}