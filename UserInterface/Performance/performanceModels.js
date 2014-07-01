csapp.factory("$csPerformanceModels", ["$csShared", function ($csShared) {

    var performanceParam = function () {
        return {
            Product: { type: "enum", label: "Product",valueList:$csShared.enums.Products },
            Weightage: { type: "number", template: "percentage" },
            Param: { type: "text" },
        };
    };
    
    var init = function () {
        var models = {};

        models.PerformanceParam = {
            Table: 'PerformanceParam',
            Columns: performanceParam()
        };

        return models;
    };

    return {
        init: init
    };

}]);