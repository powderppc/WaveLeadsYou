using PowderPPC.System;
using UnityEngine;

namespace PowderPPC.Object.Stage
{
    /// <summary>
    /// シーン遷移時のオブジェクトのアニメーションを管理
    /// </summary>
    public class StageRedirectAnimator : MonoBehaviour
    {
        [SerializeField] SpriteRenderer sceneObj;
        [SerializeField] float sceneSpeed;

        bool isInitted = false;
        bool isCompleted;

        bool isEndCalled = false;
        float timer;

        private void Awake()
        {
            sceneObj.gameObject.SetActive(true);
        }

        private void Start()
        {
            var color = SceneRedirectTypeController.Color;
            sceneObj.color = color;
        }

        /// <summary>
        /// シーンロード時の初期化
        /// </summary>
        /// <param name="player"></param>
        public void Init(Vector2 pos)
        {
            isInitted = true;
            Vector3 pos3 = new Vector3(pos.x, pos.y, sceneObj.transform.position.z);
            sceneObj.transform.position = pos3;
        }

        /// <summary>
        /// 次のシーンに遷移する前の処理
        /// </summary>
        public void EndScene(Vector2 pos)
        {
            isEndCalled = true;
            timer = 0;
            sceneObj.color = SceneRedirectTypeController.Color;
            Vector3 pos3 = new Vector3(pos.x, pos.y, sceneObj.transform.position.z);
            sceneObj.transform.position = pos3;
        }

        private void Update()
        {
            if (isInitted && !isCompleted)
            {

                var scale = sceneObj.transform.localScale;
                var diff = Time.deltaTime * sceneSpeed;
                scale.x -= diff;
                scale.y -= diff;

                if (scale.x < 0)
                {
                    sceneObj.gameObject.SetActive(false);
                    isCompleted = true;
                }
                else
                {
                    sceneObj.transform.localScale = scale;
                }
            }
            else if (isEndCalled)
            {
                timer += Time.deltaTime;
                if (timer > 1f)
                {
                    sceneObj.gameObject.SetActive(true);
                    var scale = sceneObj.transform.localScale;
                    var diff = Time.deltaTime * sceneSpeed;
                    scale.x += diff;
                    scale.y += diff;

                    sceneObj.transform.localScale = scale;

                }

            }

        }
    }
}