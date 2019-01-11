using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public class CountDown
    {
        public double Rate { get; set; }
        public double CoolDown { get; set; }
        public bool ReturnedToZero { get { return CoolDown <= 0; } }
        public bool StartCountOnce { get; set; }

        #region instance
        public CountDown()
        {
            CoolDown = 0;
            Rate = 1;
            StartCountOnce = false;
        }
        public CountDown(double rate, bool startCountOnce = false)
        {
            Rate = rate;
            CoolDown = 0;
            StartCountOnce = startCountOnce;
        }
        public void StartToCount()
        {
            if((StartCountOnce && ReturnedToZero) || !StartCountOnce)
            CoolDown = Rate;
        }
        public void Zero()
        {
            CoolDown = 0;
        }
        public void DecreaseTime()
        {
            if (CoolDown > 0)
                CoolDown -= Time.deltaTime;
        }
        #endregion

        #region static
        public static void DecreaseTime(CountDown model)
        {
            if (model.CoolDown > 0)
                model.CoolDown -= Time.deltaTime;
        }
        #endregion

    }
}