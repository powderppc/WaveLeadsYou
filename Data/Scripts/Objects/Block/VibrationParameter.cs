namespace PowderPPC.Object.Block
{
    /// <summary>
    /// VibrationControllerクラスを生成する際に必要なパラメータを保持するクラス
    /// </summary>
    public class VibrationParameter
    {
        /// <summary>
        /// 振幅
        /// </summary>
        public float amplitude;

        /// <summary>
        /// 周期
        /// </summary>
        public float period;

        /// <summary>
        /// 波の伝導時間
        /// </summary>
        public float transmissionTime;

        /// <summary>
        /// デフォルトコンストラクタ（パラメータは標準値を設定）
        /// </summary>
        public VibrationParameter()
        {
            amplitude = 1.5f;
            period = 1.6f;
            transmissionTime = 0.1f;
        }
    }
}