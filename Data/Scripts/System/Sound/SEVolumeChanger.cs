using UnityEngine;

namespace PowderPPC.System.Sound
{
    public class SEVolumeChanger : MonoBehaviour
    {
        [SerializeField] float volume;
        [SerializeField] int index;

        bool isSelected;

        SpriteRenderer mySprite;

        [SerializeField] Color selectedColor;
        Color defaultColor;

        [SerializeField] VolumeChangerManager volumeChangerManager;


        private void Start()
        {
            mySprite = GetComponent<SpriteRenderer>();
            defaultColor = mySprite.color;
        }

        public void ChangeVolume()
        {
            SoundManager.Instance.SetSEVolume(volume);

            isSelected = true;
            volumeChangerManager.ChangeSelectSE(index);

        }

        /// <summary>
        /// 選択状態にする（光らせる目的で使用）
        /// </summary>
        public void SetSelected()
        {
            isSelected = true;
        }

        public void ResetIsSelected()
        {
            isSelected = false;
        }

        private void Update()
        {
            if (isSelected)
            {
                mySprite.color = selectedColor;
            }
            else
            {
                mySprite.color = defaultColor;
            }
        }

    }
}