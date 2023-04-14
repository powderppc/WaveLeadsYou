using PowderPPC.Object.Block;
using System.Collections;
using UnityEngine;

namespace PowderPPC.Object.Enemy
{
    public class StoneController : MonoBehaviour
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

        public bool isReverse;

        public float hitTimer;

        // Use this for initialization
        void Start()
        {
            hitTimer = 0;
        }

        // Update is called once per frame
        void Update()
        {
            hitTimer -= Time.deltaTime;
        }
    }
}