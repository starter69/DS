using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSider.Models
{
    public class PlottingData
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string subProjectID { get; set; }
        public string timestep { get; set; }
        public string TimeUTC { get; set; }
        public double pwr_turbine_out { get; set; }
        public double pwr_turbine_avail { get; set; }
        public double pwr_solar_out { get; set; }
        public double pwr_solar_avail { get; set; }
        public double pwr_elec_loadbalancer_excess { get; set; }
        public double pwr_elec_loadbalancer_to_electrolyzer { get; set; }
        public double pwr_battery_in { get; set; }
        public double pwr_battery_out { get; set; }
        public double pwr_battery_storage { get; set; }
        public double pwr_battery_stored { get; set; }
        public double pwr_battery_excess { get; set; }
        public double pwr_battery_cap { get; set; }
        public double pwr_grid_in { get; set; }
        public double pwr_grid_avail { get; set; }
        public double pwr_grid_sold_total { get; set; }
        public double pwr_grid_sold { get; set; }
        public double pwr_grid_excess { get; set; }
        public double pwr_electrolyzer_in { get; set; }
        public double H2_electrolyzer_out { get; set; }
        public double pwr_cap_electrolyzer { get; set; }
        public double H2_electrolyzer_out_re { get; set; }
        public double H2_electrolyzer_out_bat { get; set; }
        public double H2_electrolyzer_out_grid { get; set; }
        public double H2_loadbalancer_total_demand { get; set; }
        public double H2_loadbalancer_total_avail { get; set; }
        public double H2_loadbalancer_excess { get; set; }
        public double H2_greenstore_in { get; set; }
        public double H2_greenstore_out { get; set; }
        public double H2_greenstore_storage { get; set; }
        public double H2_greenstore_stored { get; set; }
        public double H2_greenstore_excess { get; set; }
        public double H2_greenstore_cap { get; set; }
        public double H2_externalsupply_in { get; set; }
        public double H2_externalsupply_out { get; set; }
        public double H2_externalsupply_total_sold { get; set; }
        public double H2_externalsupply_sold { get; set; }
        public double H2_externalsupply_excess { get; set; }
        public double H2_externalsupply_cap { get; set; }
        public double H2_industry_demand { get; set; }
        public double H2_industry_supplied { get; set; }
        public double H2_mobility_demand { get; set; }
        public double H2_mobility_supplied { get; set; }
    }
}
