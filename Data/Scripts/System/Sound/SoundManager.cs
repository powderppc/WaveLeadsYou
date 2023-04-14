using KanKikuchi.AudioManager;
using PowderPPC.System.SaveData;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace PowderPPC.System.Sound
{
    /// <summary>
    /// 音量の調整と何を再生するかを全部管理する
    /// </summary>
    public class SoundManager : SingletonMonoBehaviour<SoundManager>
    {

        float _masterVolume;
        float _MasterVolume
        {
            get { return _masterVolume; }
            set
            {
                if (value < 0) _masterVolume = 0;
                else if (value > 1) _masterVolume = 1;
                else _masterVolume = value;
            }
        }
        float _bgmVolume;
        float _BGMVolume
        {
            get { return _bgmVolume; }
            set
            {
                if (value < 0) _bgmVolume = 0;
                else if (value > 1) _bgmVolume = 1;
                else _bgmVolume = value;
            }
        }
        float _seVolume;
        float _SEVolume
        {
            get { return _seVolume; }
            set
            {
                if (value < 0) _seVolume = 0;
                else if (value > 1) _seVolume = 1;
                else _seVolume = value;
            }
        }

        float _switchingBGMVolume;
        float SwitchingBGMVolume
        {
            get { return _switchingBGMVolume; }
            set
            {
                if (value < 0) _switchingBGMVolume = 0;
                else if (value > 1) _switchingBGMVolume = 1;
                else _switchingBGMVolume = value;
            }
        }
        readonly float switchingDurationDefault = 0.2f;
        float switchingDuration;

        float CurrentBGMVolumeRate;
        float nextBGMVolumeRate;

        public float BGMVolume { get { return _MasterVolume * _BGMVolume * SwitchingBGMVolume * CurrentBGMVolumeRate; } }
        public float SEVolume { get { return _MasterVolume * _SEVolume; } }

        public enum BGM
        {
            None,
            W1,
            W2,
            W3,
            W4,
            W5,
            W6,
            W7,
            W8,
        }

        BGM currentBGM;
        BGM nextBGM;

        /// <summary>
        /// BGMの流し方
        /// </summary>
        enum BGMState
        {
            /// <summary>
            /// ゆっくり
            /// </summary>
            PlayNormal,
            /// <summary>
            /// 即時反映
            /// </summary>
            PlayImmediate,

        }
        BGMState state;


        // Start is called before the first frame update
        void Start()
        {

            //セーブデータからロード
            //SetMasterVolume(SaveManager.Instance.SoundSaveData.MasterVolume);
            //SetBGMVolume(SaveManager.Instance.SoundSaveData.BGMVolume);
            //SetSEVolume(SaveManager.Instance.SoundSaveData.SEVolume);

            //SetMasterVolume(1);
            //SetBGMVolume(1);
            //SetSEVolume(1);

            //if (!BGMManager.Instance.IsPlaying() && SceneManager.GetActiveScene().name == "MainScene")
            //{
            //    currentBGM = BGM.W1;
            //    currentBGM = BGM.W1;
            //    SwitchingBGMVolume = 1.0f;
            //    CurrentBGMVolumeRate = 1.0f;
            //    nextBGMVolumeRate = 1.0f;
            //    switchingDuration = switchingDurationDefault;
            //}
            //else
            //{
            //    currentBGM = BGM.None;
            //    nextBGM = BGM.None;
            //    SwitchingBGMVolume = 0f;
            //    CurrentBGMVolumeRate = 1.0f;
            //    nextBGMVolumeRate = 1.0f;
            //    switchingDuration = switchingDurationDefault;
            //    state = BGMState.PlayNormal;
            //}
            currentBGM = VolumeManagerStatic.currentBGM;
            nextBGM = VolumeManagerStatic.currentBGM;
            SwitchingBGMVolume = 1;// BGMManagerStatic.currentVolume;
            CurrentBGMVolumeRate = 1.0f;
            nextBGMVolumeRate = 1.0f;
            switchingDuration = switchingDurationDefault;
            state = BGMState.PlayImmediate;

            var se = SaveManager.Instance.SoundSaveData.SEVolume;// VolumeManagerStatic.currentSEVolume;
            var bgm = SaveManager.Instance.SoundSaveData.BGMVolume;// VolumeManagerStatic.currentBGMVolume;

            SetMasterVolume(1);
            SetBGMVolume(bgm);
            SetSEVolume(se);
        }

        void Update()
        {
            switch (state)
            {
                case BGMState.PlayNormal:
                    SwitchingBGM();
                    break;
                default:
                    break;
            }
        }

        void SwitchingBGM()
        {
            //現在と次の状態が同一の場合は音量を上げる
            if (currentBGM == nextBGM)
            {
                //既に最大の場合は何もしない
                if (SwitchingBGMVolume == 1)
                    return;

                //BGMが何もない場合は何もしない
                if (currentBGM == BGM.None)
                    return;

                //再生中のBGMの音量を上げる
                SwitchingBGMVolume += Time.deltaTime / switchingDuration;
                BGMManager.Instance.ChangeBaseVolume(BGMVolume);

                //BGMManagerStatic.currentVolume = SwitchingBGMVolume;
            }
            else
            {
                //再生中のBGMの音量を下げる
                SwitchingBGMVolume -= Time.deltaTime / switchingDuration;
                BGMManager.Instance.ChangeBaseVolume(BGMVolume);

                //BGMManagerStatic.currentVolume = SwitchingBGMVolume;
                //無音になったらBGMを切り替える
                if (SwitchingBGMVolume == 0)
                {
                    SwitchBGM();
                }
            }
        }



        void SwitchBGM()
        {
            //既に再生中なら何もしない
            if (BGMManager.Instance.IsPlaying()) return;

            BGMManager.Instance.Stop();
            currentBGM = nextBGM;
            CurrentBGMVolumeRate = nextBGMVolumeRate;

            VolumeManagerStatic.currentBGM = currentBGM;

            switch (currentBGM)
            {
                case BGM.None:
                    break;
                case BGM.W1:
                    BGMManager.Instance.Play(BGMPath.W1, BGMVolume);
                    break;
                case BGM.W2:
                    BGMManager.Instance.Play(BGMPath.W2, BGMVolume);
                    break;
                case BGM.W3:
                    BGMManager.Instance.Play(BGMPath.W3, BGMVolume);
                    break;
                case BGM.W4:
                    BGMManager.Instance.Play(BGMPath.W4, BGMVolume);
                    break;
                case BGM.W5:
                    BGMManager.Instance.Play(BGMPath.W5, BGMVolume);
                    break;
                case BGM.W6:
                    BGMManager.Instance.Play(BGMPath.W6, BGMVolume);
                    break;
                case BGM.W7:
                    BGMManager.Instance.Play(BGMPath.W7, BGMVolume);
                    break;
                case BGM.W8:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// BGMを即切り替える（違うものがなってる場合は止める
        /// 同じものが流れてる場合はそのまま）
        /// </summary>
        /// <param name="bgm"></param>
        public void SoundBGMImmediate(BGM bgm)
        {
            nextBGM = bgm;
            nextBGMVolumeRate = 1;
            switchingDuration = switchingDurationDefault;

            {
                if (currentBGM == nextBGM)
                {
                    CurrentBGMVolumeRate = nextBGMVolumeRate;
                    SwitchingBGMVolume = 1;
                    BGMManager.Instance.ChangeBaseVolume(BGMVolume);
                    return;
                }

                BGMManager.Instance.Stop();

                currentBGM = nextBGM;
                CurrentBGMVolumeRate = nextBGMVolumeRate;
                SwitchingBGMVolume = 1;
                VolumeManagerStatic.currentBGM = currentBGM;

                switch (currentBGM)
                {
                    case BGM.None:
                        break;
                    case BGM.W1:
                        BGMManager.Instance.Play(BGMPath.W1, BGMVolume);
                        break;
                    case BGM.W2:
                        BGMManager.Instance.Play(BGMPath.W2, BGMVolume);
                        break;
                    case BGM.W3:
                        BGMManager.Instance.Play(BGMPath.W3, BGMVolume);
                        break;
                    case BGM.W4:
                        BGMManager.Instance.Play(BGMPath.W4, BGMVolume);
                        break;
                    case BGM.W5:
                        BGMManager.Instance.Play(BGMPath.W5, BGMVolume);
                        break;
                    case BGM.W6:
                        BGMManager.Instance.Play(BGMPath.W6, BGMVolume);
                        break;
                    case BGM.W7:
                        BGMManager.Instance.Play(BGMPath.W7, BGMVolume);
                        break;
                    case BGM.W8:
                        BGMManager.Instance.Play(BGMPath.W8, BGMVolume);
                        break;
                    default:
                        break;
                }
                BGMManager.Instance.ChangeBaseVolume(BGMVolume);

            }
        }

        public void SoundBGM(BGM bgm, float volume)
        {
            nextBGM = bgm;
            nextBGMVolumeRate = volume;
            switchingDuration = switchingDurationDefault;
        }

        public void SoundBGM(BGM bgm, float volume, float switchingDuration)
        {
            nextBGM = bgm;
            nextBGMVolumeRate = volume;
            this.switchingDuration = switchingDuration;
        }

        public void StopBGM()
        {
            SoundBGM(BGM.None, 1, 3);
            //BGMManager.Instance.FadeOut(3);
        }

        public bool IsSoundSE()
        {
            return SEManager.Instance.IsPlaying();
        }

        public void SoundJump()
        {
            SEManager.Instance.Play(SEPath.JUMP, SEVolume);
        }

        public void SoundHitPlayer()
        {
            SEManager.Instance.Play(SEPath.HIT_PLAYER, SEVolume);
        }

        public void SoundHipdrop()
        {
            SEManager.Instance.Play(SEPath.HIP_DROP, SEVolume);
        }

        public void SoundToSuperHipdrop()
        {
            SEManager.Instance.Play(SEPath.SUPER_HIPDROP, SEVolume);
        }

        public void SoundHitBlock()
        {
            SEManager.Instance.Play(SEPath.HIT_BLOCK, SEVolume);
        }
        public void SoundDoneHipdrop()
        {
            SEManager.Instance.Play(SEPath.HIT_BLOCK, SEVolume);
        }

        public void SoundChargeGun()
        {
            SEManager.Instance.Play(SEPath.CHARGR_GUN, SEVolume);
        }

        public void SoundShotGun()
        {
            SEManager.Instance.Play(SEPath.HIT_BLOCK, SEVolume);
        }

        public void SoundExplodeBomb()
        {
            SEManager.Instance.Play(SEPath.EXPLOSION, SEVolume);
        }

        public void SoundUnlockGoal()
        {
            SEManager.Instance.Play(SEPath.UNLOCK_GOAL, SEVolume);
        }

        public void SoundGoal()
        {
            SEManager.Instance.Play(SEPath.GOAL, SEVolume);
        }

        public void SoundGoalFailed()
        {
            SEManager.Instance.Play(SEPath.GOAL_FAILED, SEVolume);
        }

        public void SoundFadeout()
        {
            SEManager.Instance.Play(SEPath.FADE_OUT, SEVolume, 1f, 3f);
        }

        //public void SoundError()
        //{
        //    SEManager.Instance.Play(SEPath.SETTING_BUTTON, SEVolume);
        //}


        //public void SoundCollision()
        //{
        //    SEManager.Instance.Play(SEPath.COLLISION, SEVolume * 0.7f);

        //}



        public void SetMasterVolume(float value)
        {
            _MasterVolume = value;
            BGMManager.Instance.ChangeBaseVolume(BGMVolume);
        }
        public void SetBGMVolume(float value)
        {
            _BGMVolume = value;
            BGMManager.Instance.ChangeBaseVolume(BGMVolume);
            //VolumeManagerStatic.currentBGMVolume = value;
            SaveBGMVolume();
        }
        public void SetSEVolume(float value)
        {
            _SEVolume = value;
            SaveSEVolume();
            //VolumeManagerStatic.currentSEVolume = value;
        }

        //public void SaveMasterVolume()
        //{
        //    SaveManager.Instance.SoundSaveData.MasterVolume = _MasterVolume;
        //    SaveManager.Instance.SoundSaveData.Save();
        //}
        public void SaveBGMVolume()
        {
            SaveManager.Instance.SoundSaveData.BGMVolume = _BGMVolume;
            SaveManager.Instance.SoundSaveData.Save();
        }
        public void SaveSEVolume()
        {
            SaveManager.Instance.SoundSaveData.SEVolume = _SEVolume;
            SaveManager.Instance.SoundSaveData.Save();
        }
    }

}
