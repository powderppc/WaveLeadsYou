using UnityEngine;

namespace PowderPPC.Object.Block
{
    public class VibrationController
    {
        /// <summary>
        /// 振幅
        /// </summary>
        private float amplitude;

        /// <summary>
        /// 周期
        /// </summary>
        private float period;

        /// <summary>
        /// 波の伝導時間
        /// </summary>
        private float transmissionTime;

        private bool isTransmissioned;

        /// <summary>
        /// 逆位相かどうか
        /// </summary>
        private bool isReverse;

        private float timer_max => period;

        private float _timer;
        private float timer
        {
            get { return _timer; }
            set
            {
                if (value > timer_max)
                {
                    _timer = timer_max;
                }
                else if (timer < 0)
                {
                    _timer = 0;
                }
                else
                {
                    _timer = value;
                }
            }
        }

        private float period_rate { get { return timer / timer_max; } }

        /// <summary>
        /// どの方向から波が発生したか
        /// </summary>
        public enum ForceFrom
        {
            /// <summary>
            /// 中央。左右に伝える
            /// </summary>
            Center,
            /// <summary>
            /// 左から。右に伝える
            /// </summary>
            Left,
            /// <summary>
            /// 右から。左に伝える
            /// </summary>
            Right,
        }

        public ForceFrom MyForce { get; private set; }

        /// <summary>
        /// 現在の振れ幅
        /// </summary>
        public float AmplitudeNow => amplitude * Mathf.Sin(period_rate * Mathf.PI * 2 * (isReverse ? 1 : -1));

        /// <summary>
        /// 振動が終了しているかどうか
        /// Trueの場合は除外なり何らかの処理をする
        /// </summary>
        public bool IsEndVibration => period_rate == 1;

        /// <summary>
        /// 隣接ブロックに伝導可能か
        /// 未実施かつ伝導時間になったらTrue
        /// </summary>
        public bool CanTransmission => !isTransmissioned && timer > transmissionTime;

        /// <summary>
        /// 伝導可能な場合に、相手に伝える超過時間
        /// </summary>
        public float TransmissionOverTime => CanTransmission ? timer - transmissionTime : 0;

        /// <summary>
        /// 伝導処理が終わった後の自信の後処理
        /// </summary>
        public void DoAfterTransmission()
        {
            isTransmissioned = true;
        }

        /// <summary>
        /// コンストラクタ（初期生成）
        /// </summary>
        /// <param name="amplitude"></param>
        /// <param name="period"></param>
        /// <param name="transmissionTime"></param>
        /// <param name="force"></param>
        public VibrationController(float amplitude, float period, float transmissionTime, bool isReverse = false, ForceFrom force = ForceFrom.Center)
        {
            this.amplitude = amplitude;
            this.period = period;
            this.transmissionTime = transmissionTime;
            this.isReverse = isReverse;

            MyForce = force;
            Init();
        }



        /// <summary>
        /// コンストラクタ（伝導した値を引き継ぐ）
        /// </summary>
        /// <param name="vibration"></param>
        public VibrationController(VibrationController vibration, ForceFrom force)
        {
            amplitude = vibration.amplitude;
            period = vibration.period;
            transmissionTime= vibration.transmissionTime;
            isReverse= vibration.isReverse;
            MyForce = force;

            Init(vibration.TransmissionOverTime);
        }


        private void Init(float overTime = 0)
        {
            isTransmissioned = false;
            timer = overTime;
        }
   


        /// <summary>
        /// 更新処理
        /// </summary>
        /// <param name="deltaTime">timde.DeltaTime</param>
        public void MyUpdate(float deltaTime)
        {
            timer += deltaTime;
        }



    }
}