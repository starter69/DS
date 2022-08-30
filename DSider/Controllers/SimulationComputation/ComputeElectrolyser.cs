using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSider.Controllers.SimulationComputation
{
    public class ComputeElectrolyser
    {
        public ElectrolyserClass doCompute(RenewableClass renewableData, string PwrREDel, string PwrBatIn, string PwrBatOut, string PwrREAvail, string lastCapStart,
            string elDegrade, string convert, string Eff)
        {
            ElectrolyserClass resultData = new ElectrolyserClass();

            try
            {
                float PwrElInTot = float.Parse(PwrREDel) - float.Parse(PwrBatIn) + float.Parse(PwrBatOut);
                double power = 1 / 8760;
                double ElCap = float.Parse(lastCapStart) * (Math.Pow(double.Parse(elDegrade), power));
                float H2ElOut = float.Parse(convert) * float.Parse(Eff) * float.Parse(PwrREAvail);
                //
                //Compute H2ElOut for each renewable battery and gray and store in "H2ElOutForEachRenewables"
                //
                resultData.dateTime = renewableData.dateTime;
                resultData.ElCap = float.Parse(ElCap.ToString());
                resultData.H2ElOut = H2ElOut;
                resultData.PwrElInTot = PwrElInTot;
            }
            catch (Exception M)
            {
                string t = M.Message;
                throw;
            }
            return resultData;
        }
    }
    public class ElectrolyserClass
    {
        public DateTime dateTime { get; set; }
        public float PwrElInTot { get; set; }
        public float ElCap { get; set; }
        public float H2ElOut { get; set; }
        public float[] H2ElOutForEachRenewables { get; set; }
    }
}
