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
        public bool ReturnedToZero { get; set; }

        public CountDown()
        {
            CoolDown = 0;
            Rate = 1;
            ReturnedToZero = false;
        }

        public CountDown(double rate)
        {
            Rate = rate;
            CoolDown = 0;
            ReturnedToZero = false;
        }

        public void StartToCount()
        {
            CoolDown = Rate;
            ReturnedToZero = false;
        }

        public static void DecreaseTime(CountDown model)
        {
            if (model.CoolDown > 0)
            {
                model.CoolDown -= Time.deltaTime;
                if (model.CoolDown <= 0)
                    model.ReturnedToZero = true;
            }
            else
                model.ReturnedToZero = false;
        }
    }
}