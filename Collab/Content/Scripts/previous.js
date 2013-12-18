define(["jquery"], function($) {

  var init = function() {
    $(function() {
      $('.previousCollabBrowse').on('click', function() {
        $('.selectedCollab').removeClass('selectedCollab');
        $(this).addClass('selectedCollab');

        $.get('/Art/PreviousImage', { Id: $(this).find('span').text() }, function(response) {
          $('#selected').html(response);
        });
      });
    });
  };

  return {
    init: init
  };

});