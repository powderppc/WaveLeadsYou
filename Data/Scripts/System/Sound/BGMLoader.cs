using UnityEngine;

namespace PowderPPC.System.Sound
{
    public class BGMLoader : MonoBehaviour
    {
        [SerializeField] SoundManager.BGM bgm;
        [SerializeField] float volume;

        void Start()
        {
            SoundManager.Instance.SoundBGMImmediate(bgm);
        }


    }
}