"use strict";

$(document).ready(function () {
    $("#start").click(function () {
        $.ajax({
            type: "POST",
            url: "Admin.asmx/StartCrawling",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                console.log(msg);
                setInterval(refreshDashboard, 5000);
            },
            error: function (e) {
                console.log(e);
            }
        });
    });

    $("#stop").click(function () {
        $.ajax({
            type: "POST",
            url: "Admin.asmx/StopCrawling",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                console.log(msg);
            },
            error: function (e) {
                console.log(e);
            }
        });
    });

    $("#clear").click(function () {
        $.ajax({
            type: "POST",
            url: "Admin.asmx/ClearIndex",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                console.log(msg);
            },
            error: function (e) {
                console.log(e);
            }
        });
    });

    $("#search").click(GetPageTitle);

    $("#url").keyup(function(event) {
        if(event.keyCode == 13) {
            GetPageTitle();
        }
    });


    function GetPageTitle() {
        $.ajax({
            type: "POST",
            url: "Admin.asmx/GetPageTitle",
            data: JSON.stringify({ url: $("#url").val().trim() }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                var pageTitle = document.createElement("p");
                pageTitle.innerHTML = msg.d;
                $("#results").append(pageTitle);
            },
            error: function (e) {
                console.log(e);
            }
        });
    }

    function refreshDashboard() {
        $.ajax({
            type: "POST",
            url: "Admin.asmx/RefreshDashboard",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                var stats = msg.d;
                console.log(stats[0]);
                emptyDashboard();

                var stateInfo = document.createElement("div");
                stateInfo.setAttribute("class", stats[0]);
                stateInfo.innerHTML = stats[0];
                $("#stateInfo").append(stateInfo);

                var CPUInfo = document.createElement("div");
                CPUInfo.innerHTML = "CPU Utilization%: " + stats[1];
                var RAMInfo = document.createElement("div");
                RAMInfo.innerHTML = "RAM available: " + stats[2];
                $("#counterInfo").append(CPUInfo);
                $("#counterInfo").append(RAMInfo);
                
                var crawlInfo = document.createElement("div");
                crawlInfo.innerHTML = stats[3];
                $("#crawledInfo").append(crawlInfo);

                var lastTenList = document.createElement("ul");
                var lastTenUrls = stats[4].split(",");
                lastTenUrls.forEach(function(item) {
                    var urlItem = document.createElement("li");
                    urlItem.innerHTML = item;
                    lastTenList.appendChild(urlItem);
                });
                $("#lastTenInfo").append(lastTenList);

                var queueInfo = document.createElement("div");
                queueInfo.innerHTML = stats[5];
                $("#queueInfo").append(queueInfo);

                var indexInfo = document.createElement("div");
                indexInfo.innerHTML = stats[6];
                $("#indexInfo").append(indexInfo);
                
                var errorList = document.createElement("ul");
                var errors = stats[7].split(",");
                errors.forEach(function (item) {
                    var errorItem = document.createElement("li");
                    errorItem.innerHTML = item;
                    errorList.appendChild(errorItem);
                });
                $("#errorInfo").append(errorList);
            },
            error: function (e) {
                console.log(e);
            }
        });
    }

    $("#refresh").click(function () {
        setInterval(refreshDashboard, 5000);
    });

    function emptyDashboard() {
        $("#stateInfo").empty();
        $("#counterInfo").empty();
        $("#crawledInfo").empty();
        $("#lastTenInfo").empty();
        $("#queueInfo").empty();
        $("#indexInfo").empty();
        $("#errorInfo").empty();
    }
});