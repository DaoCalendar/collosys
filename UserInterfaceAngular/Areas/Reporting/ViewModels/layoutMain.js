/// <reference path="../plugins/ng-grid-reorderable.js" />
/// <reference path="../ng-grid-1.0.0.debug.js" />
var filterArray = new Array();
var typeOfView;
var plugins = {};
function userController($scope) {
    var self = this;	
    $('body').layout({
        applyDemoStyles: true,
        center__onresize: function (x, ui) {
            // may be called EITHER from layout-pane.onresize OR tabs.show
            plugins.ngGridLayoutPlugin.updateGridLayout();
        }
    });
	plugins.ngGridLayoutPlugin = new ngGridLayoutPlugin();
    $scope.mySelections = [];
    $scope.mySelections2 = [];
    $scope.filters = [];
    $scope.myData = [];
    $scope.filterOptions = {
        filterText: "",
        useExternalFilter: false,
    };
    $scope.totalServerItems = 0;
    $scope.pagingOptions = {
        pageSizes: [5, 10, 15], //page Sizes
        pageSize: 5, //Size of Paging data
        currentPage: 1 //what page they are currently on
    };
  
    $scope.$watch('pagingOptions', function () {$scope.getData(typeOfView); }, true);
    $scope.$watch('filterOptions', function () {$scope.getData(typeOfView); }, true);

    $scope.gridOptions = {
		data: 'myData',
//		jqueryUITheme: false,
//		jqueryUIDraggable: false,
//        selectedItems: $scope.mySelections,
//        showSelectionCheckbox: true,
//        multiSelect: true,
//        showColumnMenu: true,
//        enableCellSelection: false,
//        enableCellEditOnFocus: false,
//		plugins: [plugins.ngGridLayoutPlugin],
//        showGroupPanel: true,
        enablePaging: true,
        showFilter: true,
        showFooter: true,
        totalServerItems: 'totalServerItems',
        filterOptions: $scope.filterOptions,
        pagingOptions: $scope.pagingOptions,
        columnDefs:'myDefs'//,
};
    $scope.myData = '';    
    
    $scope.isNumber = function()
    {
    return false;
    }

    $scope.changeTable = function()    {
    //debugger;
    console.log($scope.test.value);
     var sampleFilterObject = 
         {
             modelName: typeOfView,
             propertyName: $scope.test.propertyName,// $scope.Columns.split('|')[1],
             propertytype: $scope.test.type,
             operatortype: $scope.test.operatortype,
             val1:$scope.test.value1,
             val2:$scope.test.value2
         };

        $scope.filters.push(sampleFilterObject);
        // filterArray.push(sampleFilterObject);



    //alert(val);
    //\alert($scope.selectname1 + ":" + $scope.selectname2);
    // return false;
    //alert($scope.selectname1);
    }

    $scope.remove = function()
    {
    debugger;
    alert($scope.filter);

    }

     $scope.getData = function (type,filters) {
     $scope.myData = '';  

     var query = "";
     typeOfView = type;

     if ($scope.pagingOptions.pageSize && $scope.pagingOptions.currentPage && $scope.filterabc) query = "?pageSize=" + $scope.pagingOptions.pageSize + "&currentPage=" + $scope.pagingOptions.currentPage + "&filter=" + filters  ;//$scope.Columns + ":" + $scope.filterabc;
     else if ($scope.pagingOptions.pageSize && $scope.pagingOptions.currentPage) query = "?pageSize=" + $scope.pagingOptions.pageSize + "&currentPage=" + $scope.pagingOptions.currentPage + "&filter=";

     function getData() {
                        $.getJSON('../../api/' + type + 'Wrapper' + query).success(function (data){
                        $scope.myData = data.DataList;
                        $scope.myDefs = data.Columns;
                        $scope.gridOptions = data.GridOptions[0];
                        debugger;
                        $scope.totalServerItems = data.totalRecords;
                        if(!$scope.$$phase) {
                        $scope.$apply();

                        }}
                        );
                     };
                 getData();

    };

  $scope.searchType = [];
  $scope.test = {type:'',propertyName:'',value1:'',value2:'',operatortype:'',operatortypetext:''};
  $scope.selectSearchType = function(op){
  //Reset Test to blank
  $scope.test = {type:'',propertyName:'',value1:'',value2:'',operatortype:'',operatortypetext:''};
  //Reset Test to blank
  //debugger;
  $scope.searchType = op;
  $scope.test.propertyName = op.field;
  $scope.test.type = op.type;
  if(op.type == "text") $scope.test.operatortype = "like";
  };
  
  $scope.resetFilter = function()
  {
  for(var i = $scope.filters.length; i > 0; i--) $scope.filters.pop();
  }
     $scope.applyFilter = function()
     {
     //alert($scope.filterabc);
     //alert($scope.Columns);
    
         //var sampleFilterObject

// Write your logic to set Filter Object, Refer Filter class in C# 
//            for(var i = $scope.filters.length; i > 0; i--)
//                       {
//                         $scope.filters.pop();
//                        }

//         var sampleFilterObject = 
//         {
//             modelName: "Employee",
//             propertyName: "EmployeeSalary",// $scope.Columns.split('|')[1],
//             propertytype: "number",
//             operatortype: "5",
//             operatortypetext: "in between",
//             val1:"100000",
//             val2:"120000"
//         };

        // $scope.filters.push(sampleFilterObject);
//         filterArray.push(sampleFilterObject);

//         filterArray[1] = {
//            modelName: "Employee",
//             propertyName: "EmployeeAddress",// $scope.Columns.split('|')[1],
//             propertytype: "text",
//             operatortype: "0",
//             operatortypetext: "like",
//             val1:"Pu"
//         };

        // $scope.filters.push(sampleFilterObject);
//          filterArray[2] = {
//            modelName: "Employee",
//             propertyName: "EmployeeName",// $scope.Columns.split('|')[1],
//             propertytype: "text",
//             operatortype: "0",
//             operatortypetext: "like",
//             val1:"Emp"
//         };

//         $scope.filters.push(filterArray[0]);
//         $scope.filters.push(filterArray[1]);
//         $scope.filters.push(filterArray[2]);
         var jsonSerialized = JSON.stringify($scope.filters);

    $.ajax({
        type: "POST",
        url: "../../api/" + typeOfView + "Wrapper/Filter",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: jsonSerialized,
        success: function (result) {
           // console.log(result); //log to the console to see whether it worked
           $scope.myData = result.DataList;
                        $scope.myDefs = eval(result.Columns);
                        $scope.totalServerItems = result.totalRecords;
                        if(!$scope.$$phase) {
                        $scope.$apply();
                        }
        },
        error: function (error) {
            alert("There was an error posting the data to the server: " + error.responseText);
        }
    });
    }
};