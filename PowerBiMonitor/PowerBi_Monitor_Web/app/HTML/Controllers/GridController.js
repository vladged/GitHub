
//var app = angular.module('main', ['ui.grid', 'ui.grid.edit', 'ui.grid.pinning', 'ui.grid.resizeColumns', 'ngMaterial']);
var app = angular.module('main');
app.controller('GridCtrl', GridCtrl);

function GridCtrl($scope, $http, $interval, $timeout, $mdDialog, S_Tokens) {

    $scope.loading = false;
    //$scope.showgrid = true;
    //$scope.ReasonCodes = null;
    //this.GetGridData = GetGridData;
   
    //function GetSharedData() {
    //    $scope.RRauthToken = S_Tokens.getRRauthToken();
    //    $scope.access_token = S_Tokens.getaccess_token();
    //    $scope.api_config = {
    //        headers: {
    //            'Authorization': "Bearer " + $scope.RRauthToken

    //        }
    //    };
    //}
    $scope.DeleteCurrentReport = function (vw_WS_DS_REP) {
        //GetSharedData();
       
        var confirm = $mdDialog.confirm()
            .title('Are you sure you want to delete report ' + vw_WS_DS_REP.reports_name + '?')
            .textContent('If you say Yes, the report will be deleted permanently.')
            .ariaLabel('Report is about to be deleted.')
            //   .targetEvent(ev)
            .ok('Yes, delete the report.')
            .cancel('Cancel');

        $mdDialog.show(confirm).then(function () {
            var parameters = {
                access_token: $scope.access_token,
                ws_ds_rep: vw_WS_DS_REP
            };

            var str_parameters = JSON.stringify(parameters);

            $http.post("/api/Monitor/DeleteCurrentReport/", str_parameters, $scope.api_config)
                .then(function (response) {
                    window.alert(response.data);

                })
                .catch(function (errors) {
                    window.alert(errors.message);
                });
        }, function () {
            window.alert("You canceld deleting the report.");
        });
    };
    $scope.DeleteCurrentWorkspace = function (vw_WS_DS_REP) {
        //GetSharedData();

        var confirm = $mdDialog.confirm()
            .title('Are you sure you want to delete workspace ' + vw_WS_DS_REP.workspaces_name + '?')
            .textContent('If you say Yes, the workspace will be deleted permanently.')
            .ariaLabel('Workspace is about to be deleted.')
            //   .targetEvent(ev)
            .ok('Yes, delete the Workspace.')
            .cancel('Cancel');

        $mdDialog.show(confirm).then(function () {
            var parameters = {
                access_token: $scope.access_token,
                ws_ds_rep: vw_WS_DS_REP
            };

            var str_parameters = JSON.stringify(parameters);

            $http.post("/api/Monitor/DeleteCurrentWorkspace/", str_parameters, $scope.api_config)
                .then(function (response) {
                    window.alert(response.data);

                })
                .catch(function (errors) {
                    window.alert(errors.message);
                });
        }, function () {
            window.alert("You canceld deleting the Workspace.");
        });
    };
    $scope.DeleteCurrentDataset = function (vw_WS_DS_REP) {
        //GetSharedData();

        var confirm = $mdDialog.confirm()
            .title('Are you sure you want to delete Dataset ' + vw_WS_DS_REP.datasets_name + '?')
            .textContent('If you say Yes, the Dataset will be deleted permanently.')
            .ariaLabel('Dataset is about to be deleted.')
            //   .targetEvent(ev)
            .ok('Yes, delete the Dataset.')
            .cancel('Cancel');

        $mdDialog.show(confirm).then(function () {
            var parameters = {
                access_token: $scope.access_token,
                ws_ds_rep: vw_WS_DS_REP
            };

            var str_parameters = JSON.stringify(parameters);

            $http.post("/api/Monitor/DeleteCurrentDataset/", str_parameters, $scope.api_config)
                .then(function (response) {
                    window.alert(response.data);

                })
                .catch(function (errors) {
                    window.alert(errors.message);
                });
        }, function () {
            window.alert("You canceld deleting the Dataset.");
        });
    };
    //var vm = this;

    //vm.gridOptions = {
    //    exporterMenuCsv: false,
    //    enableGridMenu: true,
    //    enableFiltering: true,
    //   // treeRowHeaderAlwaysVisible: false,
    //    modifierKeysToMultiSelectCells: true,
    //    keyDownOverrides: [{ keyCode: 39, ctrlKey: true }],
    //    showGridFooter: true
    //};



    //vm.gridOptions.columnDefs = [
    //    { name: 'workspaces_name',  displayName: 'Workspaces Name', width: 150, pinnedLeft: true, enableCellEdit: false, type: "number", cellClass: 'textaligncenter' },
    //  //  { name: 'workspaces_id', displayName: 'workspaces_id', width: 120, pinnedLeft: false, enableCellEdit: false, cellClass: 'borderright' },
    //    { name: 'worksapce_state', displayName: 'Workspace State', type: 'number', width: 150, enableCellEdit: false, cellClass: 'textaligncenter' },
    //    { name: 'datasets_name',  displayName: 'Dataset Name', width: 120, enableCellEdit: false, cellClass: 'borderright' },
    //    { name: 'datasets_id', displayName: 'datasets_id', type: 'object', width: 150, enableCellEdit: false, cellClass: 'textaligncenter' },
    //    { name: 'datasets_configuredBy', displayName: 'Dataset Configured By', type: 'object', width: 120, enableCellEdit: false, cellClass: 'borderright' },
    //    { name: 'reports_name', displayName: 'Report Name', type: 'object', width: 160, enableCellEdit: false, cellClass: 'textaligncenter' },
    //    { name: 'reports_id', displayName: 'reports_id', type: 'object', width: 120, enableCellEdit: false, cellClass: 'borderright' }

    //];

    //vm.msg = {};
 

    //vm.gridOptions.onRegisterApi = function (gridApi) {
    //    vm.gridApi = gridApi;
    //    gridApi.cellNav.on.navigate($scope, function (newRowCol, oldRowCol) {
    //        // var rowCol = {row: newRowCol.row.index, col:newRowCol.col.colDef.name};
    //        // var msg = 'New RowCol is ' + angular.toJson(rowCol);
    //        // if(oldRowCol){
    //        //    rowCol = {row: oldRowCol.row.index, col:oldRowCol.col.colDef.name};
    //        //    msg += ' Old RowCol is ' + angular.toJson(rowCol);
    //        // }
    //       Console.log('navigation event');
    //    });
    //    gridApi.cellNav.on.viewPortKeyDown($scope, function (event, newRowCol) {
    //        var row = newRowCol.row,
    //            col = newRowCol.col;

    //        if (event.keyCode === 39) {
    //            vm.gridApi.cellNav.scrollToFocus(row.entity, vm.gridApi.grid.columns[vm.gridApi.grid.columns.length - 1]);
    //        }
    //    });
    //};




    ///////////////////////////////////////////////////////////////////////////////////////////////////////////


}

