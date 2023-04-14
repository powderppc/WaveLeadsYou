using UnityEngine;

namespace PowderPPC.System.Sound
{

    /// <summary>
    /// Todo 後で統合したい
    /// </summary>
    public abstract class VolumeChanger : MonoBehaviour
    {

        [SerializeField] float volume;

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
                var c = mySprite.color;
                mySprite.color = selectedColor;
            }
            else
            {
                mySprite.color = defaultColor;
            }
        }

    }
}