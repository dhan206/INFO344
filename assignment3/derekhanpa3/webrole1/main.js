"use strict";

$(document).ready(function () {
    console.log("Loaded.")
    var keepRefreshing = true;
    refreshDashboard();
    setInterval(refreshDashboard, 2000); //auto refresh every 2 sec

    //Starts the crawler
    $("#start").click(function () {
        keepRefreshing = false;
        setTimeout(function () {
            $("#stateInfo").empty();
            var stateInfo = document.createElement("div");
            stateInfo.setAttribute("class", "Started");
            stateInfo.innerHTML = "The crawler has started";
            $("#stateInfo").append(stateInfo);
        }, 5000);
        $.ajax({
            type: "POST",
            url: "Admin.asmx/StartCrawling",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                keepRefreshing = true;
            },
            error: function (e) {
                console.log(e);
            }
        });
    });

    //Stops the crawler
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

    //Clears the dashboard, queues, and tables
    $("#clear").click(function () {
        keepRefreshing = false;
        document.getElementById("start").disabled = true;
        setTimeout(function () {
            emptyDashboard();
            var stateInfo = document.createElement("div");
            stateInfo.setAttribute("class", "Clearing");
            stateInfo.innerHTML = "Clearing all queues and tables. This will take a few minutes. Please wait...";
            $("#stateInfo").append(stateInfo);
        }, 5000);
        $.ajax({
            type: "POST",
            url: "Admin.asmx/ClearIndex",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                setTimeout(function () {
                    keepRefreshing = true;
                    document.getElementById("start").disabled = false;
                }, 120000);
            },
            error: function (e) {
                console.log(e);
            }
        });
    });

    //Sends the search
    $("#search").click(GetPageTitle);

    //Enter key as click
    $("#url").keyup(function(event) {
        if(event.keyCode == 13) {
            GetPageTitle();
        }
    });

    //retrives the page title with given url
    function GetPageTitle() {
        $.ajax({
            type: "POST",
            url: "Admin.asmx/GetPageTitle",
            data: JSON.stringify({ url: $("#url").val().trim() }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                $("#results").empty();
                var pageTitle = document.createElement("p");
                pageTitle.innerHTML = msg.d;
                $("#results").append(pageTitle);
            },
            error: function (e) {
                console.log(e);
            }
        });
    }

    //refreshes the dashboard
    function refreshDashboard() {
        if (keepRefreshing) {
            $.ajax({
                type: "POST",
                url: "Admin.asmx/RefreshDashboard",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    var stats = msg.d;
                    emptyDashboard();
                    console.log("Dashboard has been refreshed.");
                    var stateInfo = document.createElement("div");
                    stateInfo.setAttribute("class", stats[0]);
                    stateInfo.innerHTML = stats[0];
                    $("#stateInfo").append(stateInfo);
                    console.log(stats[0]);

                    var CPUInfo = document.createElement("div");
                    CPUInfo.innerHTML = "CPU Utilization: " + stats[1] + "%";
                    var RAMInfo = document.createElement("div");
                    RAMInfo.innerHTML = "RAM available: " + stats[2] + " MB";
                    $("#counterInfo").append(CPUInfo);
                    $("#counterInfo").append(RAMInfo);

                    var crawlInfo = document.createElement("div");
                    crawlInfo.innerHTML = stats[3];
                    $("#crawledInfo").append(crawlInfo);

                    var lastTenList = document.createElement("ul");
                    var lastTenUrls = stats[4].split(",");
                    lastTenUrls.forEach(function (item) {
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
                        if (item != "") {
                            var errorMessage = document.createElement("div");
                            var urlItem = document.createElement("div");
                            var message = item.split("*");
                            errorMessage.innerHTML = "Error: " + message[0];
                            urlItem.innerHTML = "URL: " + message[1];
                            errorItem.appendChild(urlItem);
                            errorItem.appendChild(errorMessage);
                            errorItem.appendChild(document.createElement("br"));
                        }
                        errorList.appendChild(errorItem);
                    });
                    $("#errorInfo").append(errorList);
                },
                error: function (e) {
                    console.log(e);
                }
            });
        }
    }

    //Clears the dashboard
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