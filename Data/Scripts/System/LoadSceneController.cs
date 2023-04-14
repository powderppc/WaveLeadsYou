using UnityEngine;
using UnityEngine.SceneManagement;

namespace PowderPPC.System
{
    public class LoadSceneController : MonoBehaviour
    {
        [SerializeField] float SceneTimeMax;
        float sceneTimeNow;

        public enum SceneName
        {
            None,
            Main,
        }
        SceneName nextScene;


        // Use this for initialization
        void Start()
        {
            sceneTimeNow = 0;
            nextScene = SceneName.None;
        }

        // Update is called once per frame
        void Update()
        {
            sceneTimeNow += Time.deltaTime;

            if(nextScene!=SceneName.None) {
                if (sceneTimeNow > SceneTimeMax)
                {
                    SceneManager.LoadScene(GetSceneName(nextScene));
                }
            }
        }

        public void LoadScene(SceneName nextScene)
        {
            this.nextScene = nextScene;
            sceneTimeNow = 0;
        }

        private string GetSceneName(SceneName sceneName)
        {
            switch (sceneName)
            {
                case SceneName.None:
                    Debug.Log("SceneName : None");
                    return "MainScene";
                case SceneName.Main:
                    return "MainScene";
                default:
                    Debug.Log("SceneName : Other");
                    return "MainScene";
            }

        }
    }
}