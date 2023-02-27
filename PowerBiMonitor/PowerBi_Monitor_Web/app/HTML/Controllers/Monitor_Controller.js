
(function () {
    'use strict';
    // angular.module('main', ['ngTouch', 'ui.grid', 'ui.grid.edit', 'ui.grid.selection', 'ui.grid.cellNav',  'ui.grid.pinning', 'ui.grid.resizeColumns', 'ui.grid.grouping', 'ngMaterial', 'ngAnimate'])
    angular.module('main', ['ngMaterial', 'ngAnimate'])
        .config(function ($mdThemingProvider) {
            $mdThemingProvider.theme('default')
                .primaryPalette('indigo')
                .accentPalette('green');

        });

})();

var app = angular.module("main");
app.controller('ReceiptController', ReceiptController);


function ReceiptController($scope, $mdDialog, $mdMedia, $http, $templateCache, $q, S_Tokens) {
    //var self = this;

    $scope.isDisabled = false;

    $scope.authentication_code = null;
    $scope.access_token = null;

    $scope.ReportContainer = null;

    $scope.selectedBrand = null;

    $scope.ReportreportId = "CA0F037F-247C-47F2-9455-E9E01612535F"; //PbiMonitor_Web_Direct



    // $scope.DownloadDisabled = true;
    $scope.loading = false;

    $scope.EmbedReport = EmbedReport;


    $scope.authentication_code = document.getElementById('authorizationCode').value;
    $scope.IfPowerBiAdmin = document.getElementById('IfPowerBiAdmin').value;
    $scope.ShowAdminTab = false;
    if ($scope.IfPowerBiAdmin === '1') {
        $scope.ShowAdminTab = true;
    }

    if ($scope.authentication_code !== null) {

        generateEmbedToken()
            .then(function () {
                //   window.alert();
                EmbedReport('reportContainer', $scope.ReportreportId);

                //GetAllBookmarksForUser();
                //GetAllSharedBookmarksForUser();

            },
            function (response) {
                window.alert(response);
            });

    }
    //let promise = new Promise(function (resolve, reject) {
    //    setTimeout(() => resolve("done!"), 1000);
    //});



    $scope.GetPbiData = function () {

        var parameters = {
            accessToken: $scope.access_token
        };
        $scope.loading = true;
        var str_parameters = JSON.stringify(parameters);
        //$scope.access_token =
        $http.post("/api/Monitor/GetPbiData", str_parameters, $scope.api_config)//?AADAuthorityUri=", str_parameters)
            .then(function (response) {
                $scope.loading = false;
                this.status = response.status;
                //   window.alert(response.data);
                $scope.RefreshReport();
            })
            .catch(function (errors) {
                $scope.loading = false;
                window.alert(errors.message);
            });

    };

    $scope.ClearAllData = function () {

        var parameters = {
            accessToken: $scope.access_token
        };

        var str_parameters = JSON.stringify(parameters);
        //$scope.access_token =
        $http.post("/api/Monitor/ClearAllData", str_parameters, $scope.api_config)//?AADAuthorityUri=", str_parameters)
            .then(function (response) {
                $scope.loading = false;
                this.status = response.status;
                //window.alert(response.data);
                $scope.RefreshReport();
            })
            .catch(function (errors) {
                $scope.loading = false;
                window.alert(errors.message);
            });

    };
    $scope.RefreshReport = function () {

        $scope.report.refresh()
            .then(function (result) {
                $scope.loading = false;

                //window.alert("Reloaded");
            })
            .catch(function (errors) {
                $scope.loading = true;
                window.alert("Refresh Error");
            });

    };
    $scope.GetUserActivitiesLast30Days = function () {

        var parameters = {
            accessToken: $scope.access_token
        };
        $scope.loading = true;
        var str_parameters = JSON.stringify(parameters);
        //$scope.access_token =
        $http.post("/api/Monitor/GetUserActivitiesLast30Days", str_parameters, $scope.api_config)//?AADAuthorityUri=", str_parameters)
            .then(function (response) {
                $scope.loading = false;
                this.status = response.status;
                //   window.alert(response.data);
                $scope.RefreshReport();
            })
            .catch(function (errors) {
                $scope.loading = false;
                window.alert(errors.message);
            });

    };
    $scope.GetAllAdUsers = function () {

        var parameters = {
            accessToken: $scope.GraphApiAccessToken
        };
        $scope.loading = true;
        var str_parameters = JSON.stringify(parameters);
        //$scope.access_token =
        $http.post("/api/Monitor/GetAllAdUsers", str_parameters, $scope.api_config)//?AADAuthorityUri=", str_parameters)
            .then(function (response) {
                $scope.loading = false;
                this.status = response.status;
                //   window.alert(response.data);
                $scope.RefreshReport();
            })
            .catch(function (errors) {
                $scope.loading = false;
                window.alert(errors.message);
            });

    };

    $scope.GetScheduleHistory = function () {

        var parameters = {
            accessToken: $scope.access_token
        };
        $scope.loading = true;
        var str_parameters = JSON.stringify(parameters);
        //$scope.access_token =
        $http.post("/api/Monitor/GetScheduleHistory", str_parameters, $scope.api_config)//?AADAuthorityUri=", str_parameters)
            .then(function (response) {
                $scope.loading = false;
                this.status = response.status;
                //   window.alert(response.data);
                $scope.RefreshReport();
            })
            .catch(function (errors) {
                $scope.loading = false;
                window.alert(errors.message);
            });

    };
    $scope.GetPbiMonitorData = function (filters) {
        //GetSharedData();
        $scope.loading = true;
        $http.post('/api/Monitor/GetGridData/', filters, $scope.api_config)
            .then(function (response) {
                var data = response.data;
                $scope.PbiMonitorData = data;
            }, function (response) {
                $scope.status = response.status;
            }).finally(function () {
                // called no matter success or failure
                $scope.loading = false;
            });

    };
    function EmbedReport(ReportContainerId, ReportId) {

        if ($scope.access_token === null) {
            return;
        }

        var models = window['powerbi-client'].models;
        var config = {
            type: 'report',
            tokenType: models.TokenType.Aad, //models.TokenType.Embed,
            accessToken: $scope.access_token,//.$$state.value,
            embedUrl: "https://app.powerbi.com/reportEmbed?reportId=" + ReportId,
            id: ReportId,
            permissions: models.Permissions.All /*gives maximum permissions*/,
            //viewMode: models.ViewMode.Edit,
            settings: {
                filterPaneEnabled: false,
                navContentPaneEnabled: true
            }
        };

        var reportContainer = document.getElementById(ReportContainerId);
        powerbi.reset(reportContainer);
        var report = powerbi.embed(reportContainer, config);
        $scope.IsCannedReport = true;
        $scope.report = report;
        report.off("loaded");
        report.on("loaded", function () {
            //   setTokenExpirationListener($scope.PowerBiExpiresOn,55);

        });
        report.off("rendered");
        report.on("rendered", function () {


        });
        report.off("dataSelected");

        // Report.on will add an event listener.
        report.on("dataSelected", function (event) {

            var data = event.detail;
            //if (data.visual.type === "slicer") {
            //    var str_parameters = JSON.stringify(event.detail.dataPoints);
            //    $http.post("/api/Receipts/EventDataSelected/", str_parameters, $scope.api_config)
            //        .then(function (response) {
            //            var result = response.data;

            //        })
            //        .catch(function (errors) {
            //            window.alert(errors.message);
            //        });
            //}
        });
        report.off("error");
        report.on('error', function (errorObject) {
            var err = errorObject.detail;
            window.alert(`Error occurred: ${err.message}. Detailed message: ${err.detailedMessage}`);

        });

        // Report.off removes a given event listener if it exists.
        report.off("pageChanged");

        // Report.on will add an event listener.
        report.on("pageChanged", function (event) {
            var page = event.detail.newPage;
        });

    }
    //   var ClientSecret_hid_element = document.getElementById('ClientSecret');
    ///////////////////////////////////////////////////////////////////
    function generateEmbedToken() {
        return new Promise(function (resolve, reject) {
            var parameters = {
                //   AADAuthorityUri: $scope.AADAuthorityUri,
                authorizationCode: $scope.authentication_code,
                IfPowerBiAdmin: $scope.IfPowerBiAdmin
            };

            var str_parameters = JSON.stringify(parameters);
            //$scope.access_token =
            $http.post("/api/PowerBiInit/GetAccessTokenAsyn", str_parameters)//?AADAuthorityUri=", str_parameters)
                .then(function (response) {
                    this.status = response.status;
                    if (response.data.ErrCode !== "0") {
                        if (response.data.ErrCode === "1") {
                            var instructions = " <div style='font-size: large'>" +
                                "<h2 style='color: #2e6c80;'>What to do if the Session is expired:</h2>" +
                                "<p>Close the browser. <br/> Open a new browser instance. <br/>Navigate to the same URL.</p>" +
                                "</div>";
                            document.body.innerHTML = '<b>' + response.data.ErrMessage + '</b>' + instructions;
                        }
                        if (response.data.ErrCode === "2") {
                            var instructions1 = " <div style='font-size: large'>" +
                                "<h2 style='color: #2e6c80;'>How to request the group membership:</h2>" +
                              
                                "</div>";
                            document.body.innerHTML = '<b>' + response.data.ErrMessage + '</b>' + instructions1;
                        }
                        //   window.alert(response.data.ErrMessage);


                        //window.alert(response.data.ErrMessage);
                        reject(response.data.ErrMessage);
                    }

                    $scope.access_token = response.data.PowerBiAccessToken;
                    $scope.RRauthToken = response.data.WebApiAccessToken;
                    $scope.GraphApiAccessToken = response.data.GraphApiAccessToken;
                    $scope.PowerBiExpiresOn = response.data.PowerBiExpiresOn;
                    $scope.userObjectID = response.data.userObjectID;

                    S_Tokens.setRRauthToken($scope.RRauthToken);
                    S_Tokens.setaccess_token($scope.access_token);
                    //sessionStorage.setItem('RRauthToken', tokens[1]);

                    $scope.api_config = {
                        headers: {
                            'Authorization': "Bearer " + $scope.RRauthToken
                            //'Accept': 'application/json;odata=verbose',

                        }
                    };

                    $http.post("/api/Admin/GetUserFromDB/", null, $scope.api_config)
                        //$http.get("/api/Receipts/getUserName/")
                        .then(function (response) {
                            $scope.LoggedInUser = response.data;
                        });
                    resolve("done!");

                },
                function (response) {
                    //  $scope.directors = response.data || 'Request failed';
                    // window.alert(response);
                    reject(response);
                });

        });
    }
    function generateEmbedTokenByRefreshToken() {
        return new Promise(function (resolve, reject) {
            var parameters = {
                userObjectID: $scope.userObjectID,
                // WebApiRefreshToken: $scope.WebApiRefreshToken
            };

            var str_parameters = JSON.stringify(parameters);
            //$scope.access_token =
            $http.post("/api/PowerBiInit/GetAccessTokenByRefreshToken", str_parameters, $scope.api_config)//?AADAuthorityUri=", str_parameters)
                .then(function (response) {
                    this.status = response.status;
                    if (response.data.ErrCode !== "0") {
                        window.alert(response.data.ErrMessage);

                        //window.alert(response.data.ErrMessage);
                        reject(response.data.ErrMessage);
                    }

                    $scope.access_token = response.data.PowerBiAccessToken;
                    //   $scope.RRauthToken = response.data.WebApiAccessToken;
                    $scope.PowerBiExpiresOn = response.data.PowerBiExpiresOn;

                    S_Tokens.setaccess_token($scope.access_token);
                    //sessionStorage.setItem('RRauthToken', tokens[1]);

                    $scope.api_config = {
                        headers: {
                            'Authorization': "Bearer " + $scope.RRauthToken
                            //'Accept': 'application/json;odata=verbose',

                        }
                    };

                    resolve("done!");

                },
                function (response) {
                    reject(response);
                });

        });
    }
    /////////////////////////////////////////////////////////////////////////////////////////


    function setTokenExpirationListener(tokenExpiration, minutesToRefresh = 2) {
        // get current time
        var currentTime = Date.now();
        var expiration = Date.parse(tokenExpiration);
        var safetyInterval = minutesToRefresh * 60 * 1000;

        // time until token refresh in milliseconds
        var timeout = expiration - currentTime - safetyInterval;

        // if token already expired, generate new token and set the access token
        if (timeout <= 0) {
            console.log("Updating Report Embed Token");
            updateToken();
        } // set timeout so minutesToRefresh minutes before token expires, token will be updated
        else {
            console.log("Report Embed Token will be updated in " + timeout + " milliseconds.");
            setTimeout(function () {
                updateToken();
            }, timeout);
        }
    }

    function updateToken() {
        // Generate new EmbedToken
        generateEmbedTokenByRefreshToken()
            .then(function () {

                var report = $scope.report;

                // Set AccessToken
                report.setAccessToken($scope.access_token)
                    .then(function () {
                        // Set token expiration listener
                        // result.expiration is in ISO format
                        setTokenExpirationListener($scope.PowerBiExpiresOn, 55 /*minutes before expiration*/);
                    });
            });
    }






}


