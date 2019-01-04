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
        public bool Flag { get; set; }

        public CountDown()
        {
            CoolDown = 0;
            Rate = 1;
            Flag = false;
        }

        public CountDown(double rate)
        {
            Rate = rate;
            CoolDown = 0;
            Flag = false;
        }

        public void StartToCount()
        {
            CoolDown = Rate;
        }

        public void F(bool f)
        {
            Flag = f;
        }

        public static void DecreaseTime(CountDown model)
        {
            if (model.CoolDown > 0)
                model.CoolDown -= Time.deltaTime;
        }
    }
}