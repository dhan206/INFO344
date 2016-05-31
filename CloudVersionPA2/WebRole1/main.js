'use strict'; //always

$(document).ready(function () {

    window.query_cache = {};
    window.player_cache = {};

    function populatePlayer(results) {
        if (JSON.stringify(results) != "false") {
            player_cache[$("#input").val().toLowerCase().trim()] = results;
            var panelDiv = document.createElement('div');
            panelDiv.setAttribute('class', 'panel panel-default');
            var panelHead = document.createElement('div');
            panelHead.setAttribute('class', 'panel-heading');
            panelHead.innerText = results['name'] + '\'s NBA Stats for 2015 -- 2016';
            var panelTable = createTable(results);
            panelDiv.appendChild(panelHead);
            panelDiv.appendChild(panelTable);
            $('#player').append(panelDiv);
        }
    }

    function populateResults(results) {
        results.forEach(function (item) { 
            var pageResult = document.createElement('div');
            var pageTitle = document.createElement('h3');
            var pageLink = document.createElement('a');
            pageLink.setAttribute('href', item.URL);
            pageLink.setAttribute('alt', 'page link for ' + item.Title);
            pageLink.setAttribute('target', '_blank');
            pageLink.innerText = item.Title;
            var pageDate = document.createElement('p');
            if (item.Date != "") {
                pageDate.innerText = 'Date: ' + item.Date;
            } else {
                pageDate.innerHTML = 'Date: not specified';
            }
            pageTitle.appendChild(pageLink);
            pageResult.appendChild(pageTitle);
            pageResult.appendChild(pageDate);
            $("#pages").append(pageResult);
        })
    }

    function createTable(results) {
        var panelTable = document.createElement('table');
        panelTable.setAttribute('class', 'table');
        var panelRow = document.createElement('tr');
        var playerRow = document.createElement('tr');

        var tableTEAM = document.createElement('th');
        tableTEAM.innerText = 'TEAM';
        panelRow.appendChild(tableTEAM);
        var playerTEAM = document.createElement('td');
        playerTEAM.innerText = results['team'];
        playerRow.appendChild(playerTEAM);

        var tableGP = document.createElement('th');
        tableGP.innerText = 'GP';
        panelRow.appendChild(tableGP);
        var playerGP = document.createElement('td');
        playerGP.innerText = results['games_played'];
        playerRow.appendChild(playerGP);

        var tableMIN = document.createElement('th');
        tableMIN.innerText = 'MIN';
        panelRow.appendChild(tableMIN);
        var playerMIN = document.createElement('td');
        playerMIN.innerText = results['minutes'];
        playerRow.appendChild(playerMIN);

        var tablePPG = document.createElement('th');
        tablePPG.innerText = 'PPG';
        panelRow.appendChild(tablePPG);
        var playerPPG = document.createElement('td');
        playerPPG.innerText = results['points_per_game'];
        playerRow.appendChild(playerPPG);

        var tableFGM = document.createElement('th');
        tableFGM.innerText = 'FGM';
        panelRow.appendChild(tableFGM);
        var playerFGM = document.createElement('td');
        playerFGM.innerText = results['FG_made'];
        playerRow.appendChild(playerFGM);

        //var tableFGA = document.createElement('th');
        //tableFGA.innerText = 'FGA';
        //panelRow.appendChild(tableFGA);
        //var playerFGA = document.createElement('td');
        //playerFGA.innerText = results['FG_attempted'];
        //playerRow.appendChild(playerFGA);

        //var tableFGP = document.createElement('th');
        //tableFGP.innerText = 'FG%';
        //panelRow.appendChild(tableFGP);
        //var playerFGP = document.createElement('td');
        //playerFGP.innerText = results['FG_percentage'];
        //playerRow.appendChild(playerFGP);

        var table3PM = document.createElement('th');
        table3PM.innerText = '3PM';
        panelRow.appendChild(table3PM);
        var player3PM = document.createElement('td');
        player3PM.innerText = results['3PT_made'];
        playerRow.appendChild(player3PM);

        //var table3PA = document.createElement('th');
        //table3PA.innerText = '3PA';
        //panelRow.appendChild(table3PA);
        //var player3PA = document.createElement('td');
        //player3PA.innerText = results['3PT_attempted'];
        //playerRow.appendChild(player3PA);

        //var table3PP = document.createElement('th');
        //table3PP.innerText = '3P%';
        //panelRow.appendChild(table3PP);
        //var player3PP = document.createElement('td');
        //player3PP.innerText = results['3PT_percentage'];
        //playerRow.appendChild(player3PP);

        //var tableFTM = document.createElement('th');
        //tableFTM.innerText = 'FTM';
        //panelRow.appendChild(tableFTM);
        //var playerFTM = document.createElement('td');
        //playerFTM.innerText = results['FT_made'];
        //playerRow.appendChild(playerFTM);

        //var tableFTA = document.createElement('th');
        //tableFTA.innerText = 'FTA';
        //panelRow.appendChild(tableFTA);
        //var playerFTA = document.createElement('td');
        //playerFTA.innerText = results['FT_attempted'];
        //playerRow.appendChild(playerFTA);

        var tableFTP = document.createElement('th');
        tableFTP.innerText = 'FT%';
        panelRow.appendChild(tableFTP);
        var playerFTP = document.createElement('td');
        playerFTP.innerText = results['FT_percentage'];
        playerRow.appendChild(playerFTP);

        //var tableOREB = document.createElement('th');
        //tableOREB.innerText = 'OREB';
        //panelRow.appendChild(tableOREB);
        //var playerOREB = document.createElement('td');
        //playerOREB.innerText = results['offensive_rebounds'];
        //playerRow.appendChild(playerOREB);

        //var tableDREB = document.createElement('th');
        //tableDREB.innerText = 'DREB';
        //panelRow.appendChild(tableDREB);
        //var playerDREB = document.createElement('td');
        //playerDREB.innerText = results['defensive_rebounds'];
        //playerRow.appendChild(playerDREB);

        var tableREB = document.createElement('th');
        tableREB.innerText = 'REB';
        panelRow.appendChild(tableREB);
        var playerREB = document.createElement('td');
        playerREB.innerText = results['total_rebounds'];
        playerRow.appendChild(playerREB);

        var tableAST = document.createElement('th');
        tableAST.innerText = 'AST';
        panelRow.appendChild(tableAST);
        var playerAST = document.createElement('td');
        playerAST.innerText = results['assists'];
        playerRow.appendChild(playerAST);

        var tableSTL = document.createElement('th');
        tableSTL.innerText = 'STL';
        panelRow.appendChild(tableSTL);
        var playerSTL = document.createElement('td');
        playerSTL.innerText = results['steals'];
        playerRow.appendChild(playerSTL);

        var tableBLK = document.createElement('th');
        tableBLK.innerText = 'BLK';
        panelRow.appendChild(tableBLK);
        var playerBLK = document.createElement('td');
        playerBLK.innerText = results['blocks'];
        playerRow.appendChild(playerBLK);

        var tablePF = document.createElement('th');
        tablePF.innerText = 'FOULS';
        panelRow.appendChild(tablePF);
        var playerPF = document.createElement('td');
        playerPF.innerText = results['personal_fouls'];
        playerRow.appendChild(playerPF);

        var tableTO = document.createElement('th');
        tableTO.innerText = 'TO';
        panelRow.appendChild(tableTO);
        var playerTO = document.createElement('td');
        playerTO.innerText = results['turnovers'];
        playerRow.appendChild(playerTO);

        panelTable.appendChild(panelRow);
        panelTable.appendChild(playerRow);
        return panelTable;
    }

    $('#input').typeahead({
        minLength: 1,
        highlight: true,
        hint: true
    }, {
        limit: 10,
        async: true,
        source: function (query, syncResults, asyncResults) {
            if (query_cache[query] !== undefined) {
                syncResults(query_cache[query]);
                return;
            }
            return $.ajax({
                type: 'POST',
                url: "WebService1.asmx/SearchTrie",
                data: JSON.stringify({ prefix: query.toLowerCase().trim() }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    query_cache[query] = msg.d;
                    return asyncResults(msg.d);
                },
                error: function (msg) {
                    console.log(msg);
                }
            });
        }
    })

    $("#input").keydown(function (event) {
        if (event.keyCode == 13) {
            $("#player").empty();
            $("#pages").empty();
            search();
        }
    });

    $("#submit").click(function () {
        $("#player").empty();
        search();
    })

    function search() {
        var userInput = $("#input").val().toLowerCase().trim();
        $.ajax({
            crossDomain: true,
            url: 'http://dhanwebcrawler.cloudapp.net/Admin.asmx/GetPage',
            dataType: 'jsonp',
            contentType: 'application/json; charset=utf-8',
            data: { query: userInput },
            success: populateResults
        });

        if (player_cache[userInput] !== undefined) {
            populatePlayer(player_cache[userInput]);
        } else {
            $.ajax({
                crossDomain: true,
                url: 'http://ec2-52-40-84-72.us-west-2.compute.amazonaws.com/index.php',
                dataType: 'jsonp',
                contentType: 'application/json; charset=utf-8',
                data: { name: userInput },
                success: populatePlayer
            })
        }
    }
});