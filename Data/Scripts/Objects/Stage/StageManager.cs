using PowderPPC.System;
using PowderPPC.System.SaveData;
using PowderPPC.System.Sound;
using UnityEngine;

namespace PowderPPC.Object.Stage
{
    /// <summary>
    /// ステージの生成を管理
    /// StageLoaderからステージを読み込み、そのPrefabをInstantiateする
    /// </summary>
    public class StageManager : MonoBehaviour
    {
        /// <summary>
        /// ゲーム起動時に読み込むステージ
        /// StageLoaderからのステージ読み込みに失敗した場合もこれが生成される(W1未クリア時)
        /// </summary>
        [SerializeField] StageController defaultStage;
        /// <summary>
        /// ゲーム起動時に読み込むステージ
        /// W1クリア状態のとき、これが生成される
        /// StageLoaderからのステージ読み込みに失敗した場合もこれが生成される
        /// </summary>
        [SerializeField] StageController titleStage;

        [SerializeField] LoadSceneController loadSceneController;

        StageController madeStage;

        bool isCalledNext;

        bool isLoadCompleted;
        [SerializeField] StageRedirectAnimator redirectAnimator;

        // Use this for initialization
        void Start()
        {
            isCalledNext = false;
            isLoadCompleted = false;

            bool isClearW1 = SaveManager.Instance.AchievementSaveData.ClearLevel > 0;
            var stage = StageLoader.NextStage;
            if (stage == null)
            {
                stage = isClearW1 ? titleStage : defaultStage;
                //Debug.Log("Load Default Stage");
            }
            madeStage = Instantiate(stage);
            //Debug.Log($"StageName : {madeStage.StageName}");
        }

        // Update is called once per frame
        void Update()
        {
            //ステージ開始時のシーン遷移処理
            if (!isLoadCompleted)
            {
                if(madeStage.PlayerObj!= null)
                {
                    redirectAnimator.Init(madeStage.PlayerObj.transform.position);
                    isLoadCompleted = true;
                }
            }


            //ステージのサイクル終了を監視
            if (!isCalledNext && madeStage.IsNeedLoadScene)
            {
                isCalledNext = true;
                //ステージクリアかどうかの判定をして、Staticクラスに保存
                if(madeStage.IsStageCleared)
                {
                    //エフェクトに用いる色を決定
                    SceneRedirectTypeController.Color = madeStage.GoaledObj.Mysprite.color;
                    //エフェクトの起動
                    redirectAnimator.EndScene(madeStage.GoaledObj.transform.position);
                    //ゴール用効果音再生
                    SoundManager.Instance.SoundFadeout();

                }
                else
                {
                    //エフェクトに用いる色を決定
                    SceneRedirectTypeController.Color = new Color(0, 0, 0);
                    //エフェクトの起動
                    redirectAnimator.EndScene(new Vector2());
                }

                loadSceneController.LoadScene(LoadSceneController.SceneName.Main);


            }
        }
    }
}