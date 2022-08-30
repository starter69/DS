using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DSider.Controllers.SimulationComputation
{
    public class ComputeRenewablePower
    {
        public List<RenewableClass> doCompute(DataTable dtRenewablePower, string dataSourceColoumn, string pwrRECap, string takeOfPercent, string TransLoss,
            string componentName)
        {
            dataSourceColoumn = dataSourceColoumn.TrimStart().TrimEnd();
            List<RenewableClass> resultList = new List<RenewableClass>();
            try
            {
                foreach (DataRow row in dtRenewablePower.Rows)
                {
                    DateTime mDate = DateTime.Parse(row[0].ToString());
                    float LF = float.Parse(row[dataSourceColoumn].ToString());
                    resultList.Add(new RenewableClass()
                    {
                        dateTime = mDate,
                        pwrREOut = float.Parse(pwrRECap) * LF * float.Parse(takeOfPercent),
                        pwrREAvail = float.Parse(pwrRECap) * LF * float.Parse(takeOfPercent) * (1 - float.Parse(TransLoss)),
                        name = componentName
                    });
                }
            }
            catch (Exception M)
            {
                string t = M.Message;
            }
            return resultList;
        }
    }
    public class RenewableClass
    {
        public DateTime dateTime { get; set; }
        public float pwrREOut { get; set; }
        public float pwrREAvail { get; set; }
        public string name { get; set; }
        public float electrolyzerDemand10Percent { get; set; }
    }
}
