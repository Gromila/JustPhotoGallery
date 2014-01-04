(function() {
  $(document).ready(function() {
    $("#navbar-search").focusin(function() {
      return $(this).animate({
        width: "400px"
      }, 500);
    });
    return $("#navbar-search").focusout(function() {
      return $(this).animate({
        width: "160px"
      }, 500);
    });
  });

}).call(this);
