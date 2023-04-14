using PowderPPC.System.SaveData;
using System.Collections.Generic;
using UnityEngine;

namespace PowderPPC.System.Sound
{
    public class VolumeChangerManager : MonoBehaviour
    {
        [SerializeField] List<SEVolumeChanger> seList;
        [SerializeField] List<BGMVolumeChanger> bgmList;

        /// <summary>
        /// 選択状態にあるブロック（BGM）
        /// </summary>
        int bgmSelectedIndex;

        /// <summary>
        /// 選択状態にあるブロック（SE）
        /// </summary>
        int seSelectedIndex;

        void Start()
        {
            //セーブデータから取得
            bgmSelectedIndex = SaveManager.Instance.SoundSaveData.BGMIndex;// VolumeManagerStatic.selectedBGMIndex;
            seSelectedIndex = SaveManager.Instance.SoundSaveData.SEIndex;// VolumeManagerStatic.selectedSEIndex;

            bgmList[bgmSelectedIndex].ChangeVolume();
            seList[seSelectedIndex].ChangeVolume();
        }


        /// <summary>
        /// Select状態未満を光らせる
        /// </summary>
        /// <param name="child"></param>
        public void ChangeSelectBGM(int index)
        {
            for (var i = 0; i < bgmList.Count; ++i)
            {
                if (i < index)
                {
                    bgmList[i].ResetIsSelected();
                }
                else if (i > index)
                {
                    bgmList[i].SetSelected();
                }
                else
                {
                    SaveManager.Instance.SoundSaveData.BGMIndex = i;
                    SaveManager.Instance.SoundSaveData.Save();
                    //VolumeManagerStatic.selectedBGMIndex = i;
                }
            }
        }
        /// <summary>
        /// Select状態未満を光らせる
        /// </summary>
        /// <param name="index"></param>
        public void ChangeSelectSE(int index)
        {
            for (var i = 0; i < seList.Count; ++i)
            {
                if (i < index)
                {
                    seList[i].ResetIsSelected();
                }
                else if (i > index)
                {
                    seList[i].SetSelected();
                }
                else
                {
                    SaveManager.Instance.SoundSaveData.SEIndex = i;
                    SaveManager.Instance.SoundSaveData.Save();
                    //VolumeManagerStatic.selectedSEIndex = i;
                }
            }
        }
    }
}