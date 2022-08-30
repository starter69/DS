using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSider.Controllers.SimulationComputation
{
    public class ComputeBattery
    {
        public BatteryClass doCompute(RenewableClass renewableData, string capStart, string batCapDegrade,string lastBatCap, string lastPwrBatStore,
            string PwrBatIn, string LossBatIn, string LossBatStore, string LossBatOut, string PwrBatOut, string MaxBatDCharge)//lastBatCap = BatCap(t-1)
        {
            BatteryClass resultData = new BatteryClass();
            try
            {
                double power = 1 / 8760;
                double BatCap = float.Parse(lastBatCap) * Math.Pow(double.Parse(batCapDegrade), power);
                //
                float PwrBatStore = (float.Parse(PwrBatIn) * (1 - float.Parse(LossBatIn))) +
                    (float.Parse(lastPwrBatStore) * (1 - float.Parse(LossBatStore))) -
                    (float.Parse(PwrBatOut) / (1 - float.Parse(LossBatOut)));

                float PwrBatAvail = Math.Min((float.Parse(MaxBatDCharge) / 100) * float.Parse(lastBatCap), 
                    (float.Parse(lastPwrBatStore) * ((1 - float.Parse(LossBatStore)) / (1 - float.Parse(LossBatOut)))));
                //
                resultData.BatCap = float.Parse(BatCap.ToString());
                resultData.dateTime = renewableData.dateTime;
                resultData.PwrBatStore = PwrBatStore;
                resultData.PwrBatAvail = PwrBatAvail;
            }
            catch (Exception M)
            {
                string t = M.Message;
                throw;
            }
            return resultData;
        }
    }
    public class BatteryClass
    {
        public DateTime dateTime { get; set; }
        public float BatCap { get; set; }
        public float PwrBatStore { get; set; }
        public float PwrBatAvail { get; set; }
    }
}
