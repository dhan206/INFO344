'use strict'; //always

$(document).ready(function () {
    //ajax calls after each key up while a user types in the input field
    $("#input").keyup(function () {
        $.ajax({
            type: "POST",
            url: "getQuerySuggestions.asmx/SearchTrie",
            data: JSON.stringify({ prefix: $("#input").val().toLowerCase().trim() }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                //empties the results for new results
                $("#results").empty();
                //adds each result to the suggestion box, ensures 10 results are displayed.
                for (var i = 0; i < msg.d.length; i++) {
                    var suggestion = document.createElement("div");
                    suggestion.innerHTML = msg.d[i]
                    $("#results").append(suggestion);
                }
            },
            error: function (msg) {
                console.log(msg);
            }
        });
    });
});