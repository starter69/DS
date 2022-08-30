var splittedUrl = window.location.href.split("/");
var Tariana = angular.module('Tariana', []);
Tariana.controller("DashboardMitigationController", function ($scope) {
    jQuery('.projectNameContainer').text('"' + splittedUrl[splittedUrl.length - 3].replace('%20', ' ') + '"')
    //Back to simulation page
    $scope.backToSimulation = function () {
        window.location.href = '/Home/Simulation?project=' + splittedUrl[splittedUrl.length - 4] + '&name=' + splittedUrl[splittedUrl.length - 3] + '&folder=' + splittedUrl[splittedUrl.length - 2] + '&type=' + splittedUrl[splittedUrl.length - 1];
    }
    fetchDataPlot();
});
var allDataPlotting = [];
//Get plot data for current sub project
function fetchDataPlot() {
    $.ajax({
        url: '/api/WebAPI_PlottingData/carbonMitigaionDashboardPlot/' + splittedUrl[splittedUrl.length - 4],
        type: "GET",
        contentType: 'application/json',
        success: function (response) {
            if (response.length > 0) {
                allDataPlotting = response[0];
                console.log(allDataPlotting);
                plotChart1();
                plotChart2();
            }
        },
        error: function (response) {
        },
        failure: function (response) {
        }
    });

}
//Plotting plot 1
function plotChart1() {
    var xAxis = allDataPlotting.xAxis;
    //var yAxisMerged = [];
    //if (allDataPlotting.reductionAchieve != null)
    //    yAxisMerged = yAxisMerged.concat(allDataPlotting.reductionAchieve);
    //if (allDataPlotting.reductionAchieve != null)
    //    yAxisMerged = yAxisMerged.concat(allDataPlotting.reductionAchieve);
    //if (allDataPlotting.reductionTarget != null)
    //    yAxisMerged = yAxisMerged.concat(allDataPlotting.reductionTarget);
    //
    var chart = new Highcharts.Chart({
        chart: {
            renderTo: 'containerPlotDash1',
            zoomType: 'xy',
        },
        title: {
            text: 'Emissions'
        },
        yAxis: {
            title: {
                text: 'tCO2e'
            },
            //min: findMin(yAxisMerged),
            //max: findMax(yAxisMerged),
        },
        xAxis: {
            title: {
                text: 'Time (Years)'
            },
            categories: xAxis,
            labels: {
                rotation: 90,
                style: { fontSize: "14px" }
            },
        },

        legend: {
            align: 'center',
            verticalAlign: 'bottom',
            x: 0,
            y: 0
        },

        plotOptions: {
            series: {
                label: {
                    connectorAllowed: false
                },
                pointStart: 2022,
            },
        },

        series: [{
            name: 'Target',
            'marker': {
                'enabled': false
            },
            point: {
                events: {
                    click: function () {
                        computeTable(this.category);
                        computeTableCumulative(this.category);

                    }
                }
            },
            data: allDataPlotting.reductionTarget //67.5, 94.5, 121.5, 148.5, 175.5, 202.5, 290.25, 378, 465.75, 553.5, 641.25, 823.5, 1005.75, 1188, 1370.25, 1552.5, 1620, 1687.5, 1755, 1822.5, 1890, 1937.25, 1984.5, 2031.75, 2079, 2112.75, 2146.5, 2180.25, 2214]
        }, {
            name: 'Achieved',
            'marker': {
                'enabled': false
            },
            point: {
                events: {
                    click: function () {
                        computeTable(this.category);
                        computeTableCumulative(this.category);
                    }
                }
            },
            data: allDataPlotting.reductionAchieve //[67.5, 87.75, 108, 128.25, 148.5, 168.75, 249.75, 330.75, 411.75, 492.75, 573.75, 742.5, 911.25, 1080, 1248.75, 1417.5, 1532.25, 1647, 1761.75, 1876.5, 1991.25, 2072.25, 2153.25, 2234.25, 2315.25, 2382.75, 2450.25, 2517.75, 2585.25]
        }, {
            name: 'BaseLine',
            'marker': {
                'enabled': false
            },
            point: {
                events: {
                    click: function () {
                        computeTable(this.category);
                        computeTableCumulative(this.category);
                    }
                }
            },
            data: allDataPlotting.reductionBaseLine// [67.5, 101.25, 135, 168.75, 236.25, 303.75, 371.25, 523.125, 675, 826.875, 1080, 1333.125, 1586.25, 2092.5, 2598.75, 3105, 3358.125, 3611.25, 3864.375, 4050, 4235.625, 4421.25, 4505.625, 4590, 4674.375, 4708.125, 4741.875, 4775.625, 4809.375]
        }],
    });
}
//Plotting plot 2
function plotChart2() {
    var xAxis = allDataPlotting.xAxis;

    var chart = new Highcharts.Chart({
        chart: {
            renderTo: 'containerPlotDash4',
            zoomType: 'xy',
        },
        title: {
            text: 'Capex',
        },

        xAxis: {
            title: {
                text: 'Time (Years)'
            },
            categories: xAxis,
            labels: {
                rotation: 90,
                style: { fontSize: "14px" }
            },
        },
        yAxis: {
            title: {
                text: '$'
            }
        },
        tooltip: {
            valueSuffix: ''
        },
        legend: {
            align: 'center',
            verticalAlign: 'bottom',
            x: 0,
            y: 0
        },
        series: [{
            name: 'Budget',
            'marker': {
                'enabled': false
            },
            data: allDataPlotting.capexBudget// [10.63125, 10.63125, 10.63125, 10.63125, 10.63125, 40.5, 40.5, 40.5, 40.5, 40.5, 130.359375, 130.359375, 130.359375, 130.359375, 130.359375, 269.83125, 269.83125, 269.83125, 269.83125, 269.83125, 336.403125, 336.403125, 336.403125, 336.403125, 336.403125, 336.403125, 356.90625, 356.90625, 356.90625]
        }, {
            name: 'Deployed',
            'marker': {
                'enabled': false
            },
            data: allDataPlotting.capexDeploy// [10.125, 13.1625, 16.2, 19.2375, 22.275, 25.3125, 37.4625, 49.6125, 61.7625, 73.9125, 86.0625, 111.375, 136.6875, 162, 187.3125, 212.625, 229.8375, 247.05, 264.2625, 281.475, 298.6875, 310.8375, 322.9875, 335.1375, 347.2875, 357.4125, 367.5375, 377.6625, 387.7875]
        }, {
            name: 'BaseLine',
            'marker': {
                'enabled': false
            },
            data: allDataPlotting.capexBaseLine// [1.35, 2.025, 2.7, 3.375, 4.725, 6.075, 7.425, 10.4625, 13.5, 16.5375, 21.6, 26.6625, 31.725, 41.85, 51.975, 62.1, 67.1625, 72.225, 77.2875, 81, 84.7125, 88.425, 90.1125, 91.8, 93.4875, 94.1625, 94.8375, 95.5125, 96.1875]
        }],
        plotOptions: {
            series: {
                label: {
                    connectorAllowed: false
                },
                pointStart: 2022
            }
        },

    });



}
//Create Cumulative table
function computeTableCumulative(Year) {
    var sub = Year - 2022;
    var tCo2Reduce_Scope1_Cum = 0;
    var tCo2Reduce_Scope2_Cum = 0;
    var tCo2Reduce_Scope3_Cum = 0;
    var totalTargetScope1_Cum = 0;
    var totalTargetScope2_Cum = 0;
    var totalTargetScope3_Cum = 0;
    var totalBaseLineScope1_Cum = 0;
    var totalBaseLineScope2_Cum = 0;
    var totalBaseLineScope3_Cum = 0;
    var totalAchieveScope1_Cum = 0;
    var totalAchieveScope2_Cum = 0;
    var totalAchieveScope3_Cum = 0;
    var grandTotalAchiveForAllScopes = 0;
    var grandTotalBaseLineForAllScopes = 0;

    //
    for (var i = 0; i <= sub; i++) {
        tCo2Reduce_Scope1_Cum += allDataPlotting.scope1BaseLine[i] - allDataPlotting.scope1Achieve[i];
        tCo2Reduce_Scope2_Cum += allDataPlotting.scope2BaseLine[i] - allDataPlotting.scope2Achieve[i];
        tCo2Reduce_Scope3_Cum += allDataPlotting.scope2BaseLine[i] - allDataPlotting.scope3Achieve[i];
        //
        totalTargetScope1_Cum += allDataPlotting.scope1TargetReduction[i];
        totalTargetScope2_Cum += allDataPlotting.scope2TargetReduction[i];
        totalTargetScope3_Cum += allDataPlotting.scope3TargetReduction[i];
        //
        totalBaseLineScope1_Cum += allDataPlotting.scope1BaseLine[i];
        totalBaseLineScope2_Cum += allDataPlotting.scope2BaseLine[i];
        totalBaseLineScope3_Cum += allDataPlotting.scope3BaseLine[i];
        //
        totalAchieveScope1_Cum += allDataPlotting.scope1Achieve[i];
        totalAchieveScope2_Cum += allDataPlotting.scope2Achieve[i];
        totalAchieveScope3_Cum += allDataPlotting.scope3Achieve[i];
        //
        grandTotalAchiveForAllScopes += allDataPlotting.reductionAchieve[i];
        grandTotalBaseLineForAllScopes += allDataPlotting.reductionBaseLine[i];
    }
    jQuery('#TargetCumScope1').text(((totalAchieveScope1_Cum / totalTargetScope1_Cum).toFixed(2) * 100).toFixed(2) + '%');
    jQuery('#TargetCumScope2').text(((totalAchieveScope2_Cum / totalTargetScope2_Cum).toFixed(2) * 100).toFixed(2) + '%');
    jQuery('#TargetCumScope3').text(((totalAchieveScope3_Cum / totalTargetScope3_Cum).toFixed(2) * 100).toFixed(2) + '%');
    jQuery('#TargetCumScopeTotal').text((((totalAchieveScope1_Cum + totalAchieveScope2_Cum + totalAchieveScope3_Cum) /
        (totalTargetScope1_Cum + totalTargetScope2_Cum + totalTargetScope3_Cum)).toFixed(2) * 100).toFixed(2) + '%');
    //
    jQuery('#BaseCumScope1').text(((totalAchieveScope1_Cum / totalBaseLineScope1_Cum).toFixed(2) * 100).toFixed(2) + '%');
    jQuery('#BaseCumScope2').text(((totalAchieveScope2_Cum / totalBaseLineScope2_Cum).toFixed(2) * 100).toFixed(2) + '%');
    jQuery('#BaseCumScope3').text(((totalAchieveScope3_Cum / totalBaseLineScope3_Cum).toFixed(2) * 100).toFixed(2) + '%');
    jQuery('#BaseCumScopeTotal').text((((grandTotalAchiveForAllScopes) /
        (grandTotalBaseLineForAllScopes)).toFixed(2) * 100).toFixed(2) + '%');

    //
    jQuery('#tCo2Reduce_Scope1_Cum').text(addComma(tCo2Reduce_Scope1_Cum));
    jQuery('#tCo2Reduce_Scope2_Cum').text(addComma(tCo2Reduce_Scope2_Cum));
    jQuery('#tCo2Reduce_Scope3_Cum').text(addComma(tCo2Reduce_Scope3_Cum));
    jQuery('#tCo2Reduce_Total_Cum').text(addComma(tCo2Reduce_Scope1_Cum + tCo2Reduce_Scope2_Cum + tCo2Reduce_Scope3_Cum));
}
function computeTable(Year) {
    jQuery('#tdYearTitle').text(Year);
    //allDataPlotting
    var sub = Year - 2022;
    var tCo2Reduce_Scope1 = 0;
    var tCo2Reduce_Scope2 = 0;
    var tCo2Reduce_Scope3 = 0;
    //
    var totalTargetScope1 = 0;
    var totalTargetScope2 = 0;
    var totalTargetScope3 = 0;
    var totalBaseLineScope1 = 0;
    var totalBaseLineScope2 = 0;
    var totalBaseLineScope3 = 0;
    var totalAchieveScope1 = 0;
    var totalAchieveScope2 = 0;
    var totalAchieveScope3 = 0;

    //for (var i = 0; i <= sub; i++) {
    tCo2Reduce_Scope1 += allDataPlotting.scope1BaseLine[sub] - allDataPlotting.scope1Achieve[sub];
    tCo2Reduce_Scope2 += allDataPlotting.scope2BaseLine[sub] - allDataPlotting.scope2Achieve[sub];
    tCo2Reduce_Scope3 += allDataPlotting.scope2BaseLine[sub] - allDataPlotting.scope3Achieve[sub];
    //
    totalTargetScope1 += allDataPlotting.scope1TargetReduction[sub];
    totalTargetScope2 += allDataPlotting.scope2TargetReduction[sub];
    totalTargetScope3 += allDataPlotting.scope3TargetReduction[sub];
    //
    totalBaseLineScope1 += allDataPlotting.scope1BaseLine[sub];
    totalBaseLineScope2 += allDataPlotting.scope2BaseLine[sub];
    totalBaseLineScope3 += allDataPlotting.scope3BaseLine[sub];
    //
    totalAchieveScope1 += allDataPlotting.scope1Achieve[sub];
    totalAchieveScope2 += allDataPlotting.scope2Achieve[sub];
    totalAchieveScope3 += allDataPlotting.scope3Achieve[sub];
    //}
    jQuery('#TargetScope1').text(((totalAchieveScope1 / totalTargetScope1).toFixed(2) * 100).toFixed(2) + '%');
    jQuery('#TargetScope2').text(((totalAchieveScope2 / totalTargetScope2).toFixed(2) * 100).toFixed(2) + '%');
    jQuery('#TargetScope3').text(((totalAchieveScope3 / totalTargetScope3).toFixed(2) * 100).toFixed(2) + '%');
    jQuery('#TargetScopeTotal').text((((totalAchieveScope1 + totalAchieveScope2 + totalAchieveScope3) /
        (totalTargetScope1 + totalTargetScope2 + totalTargetScope3)).toFixed(2) * 100).toFixed(2) + '%');
    //
    jQuery('#BaseScope1').text(((totalAchieveScope1 / totalBaseLineScope1) * 100).toFixed(2) + '%');
    jQuery('#BaseScope2').text(((totalAchieveScope2 / totalBaseLineScope2) * 100).toFixed(2) + '%');
    jQuery('#BaseScope3').text(((totalAchieveScope3 / totalBaseLineScope3).toFixed(2) * 100).toFixed(2) + '%');
    jQuery('#BaseScopeTotal').text((((totalAchieveScope1 + totalAchieveScope2 + totalAchieveScope3) /
        (totalBaseLineScope1 + totalBaseLineScope2 + totalBaseLineScope3)).toFixed(2) * 100).toFixed(2) + '%');
    //
    jQuery('#tCo2Reduce_Scope1').text(addComma(tCo2Reduce_Scope1));
    jQuery('#tCo2Reduce_Scope2').text(addComma(tCo2Reduce_Scope2));
    jQuery('#tCo2Reduce_Scope3').text(addComma(tCo2Reduce_Scope3));
    jQuery('#tCo2Reduce_Total').text(addComma(tCo2Reduce_Scope1 + tCo2Reduce_Scope2 + tCo2Reduce_Scope3));
    //
    jQuery('#totalBaseLeftTable').text((((allDataPlotting.reductionAchieve[sub]) / (allDataPlotting.reductionBaseLine[sub])) * 100).toFixed(2) + '%');
    jQuery('#totalTargetLeftTable').text((((allDataPlotting.reductionAchieve[sub]) / (allDataPlotting.reductionTarget[sub])) * 100).toFixed(2) + '%');

}
function addComma(number) {
    return number.toString().replace(/,/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}
function findMax(num) {
    return Math.max.apply(null, num);
}
function findMin(num) {
    return Math.min.apply(null, num);
}