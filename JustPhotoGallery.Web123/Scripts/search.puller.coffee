# CoffeeScript
$(document).ready ->
    $("#navbar-search").focusin ->
        $(this).animate 
             width: "400px"
            , 500

    $("#navbar-search").focusout ->
        $(this).animate
            width: "160px"
            , 500