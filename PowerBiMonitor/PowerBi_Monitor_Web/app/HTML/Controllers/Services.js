var app = angular.module("main");

app.service('S_Tokens', function () {
    var access_token = null;
    var RRauthToken = null;
    this.setaccess_token = function (value) { access_token = value };
    this.getaccess_token = function () { return access_token };
    this.setRRauthToken = function (value) { RRauthToken = value };
    this.getRRauthToken = function () { return RRauthToken };
});


