$(function () {
    var utils = new function () { };

    var viewModel = new function () {
        var self = this;

        self.markers = ko.observableArray();

        self.markerLeft = ko.observable(50);
        self.markerTop = ko.observable(50);

        self.mapSource = ko.observable();
        self.mapCoordinatesObj = ko.observableArray([{
            north: 52.40907821,
            east: 0.27900871,
            south: 52.39401453,
            west: 0.24536697
        }]);
        // This computed function unwraps the JSON object...
        self.mapCoordinates = ko.computed(function () {
            return self.mapCoordinatesObj()[0];
        });

        self.mapDimensions = {
            height: 847,
            width: 1233
        };

        self.objectLeft = function (longitude) {
            var left = self.mapDimensions.width * (longitude - self.mapCoordinates().west) / (self.mapCoordinates().east - self.mapCoordinates().west);
            return Math.max(Math.min(self.mapDimensions.width, left), 0);
        };
        self.objectTop = function (latitude) {
            var top = self.mapDimensions.height * (self.mapCoordinates().north - latitude) / (self.mapCoordinates().north - self.mapCoordinates().south);
            return Math.max(Math.min(self.mapDimensions.height, top), 0);
        };

        self.displayName = function (marker) {
            var lastSeen = new Date(marker.LastSeen);
            return marker.Name + " (" + lastSeen.getHours() + ":" + lastSeen.getMinutes() + ")";
        }
    };

    ko.applyBindings(viewModel);

    $.get("/api/map/source").success(viewModel.mapSource);
    setInterval(function () {
        $.getJSON("/api/aprsdata/stations").success(viewModel.markers);
    }, 30000);
    $.getJSON("/api/aprsdata/stations").success(viewModel.markers);
});