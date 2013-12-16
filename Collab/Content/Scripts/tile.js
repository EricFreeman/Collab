define(["jquery", "jquery.form"], function ($) {
  var baseSize = 64,
      speed = 200,
      mouseEvents = { 'enter': 10, 'leave': 0, 'down': -10, 'up': 10 };

  function init() {

    $(function () {
      var initializeUploader = function(response, options) {
        $('#artUpload').html(response);
        $('form').ajaxForm(options);
      };

      $('.tile').on('click', function() {
        var tile = $(this);
        $.get('Art/Upload', { x: tile.data('x'), y: tile.data('y') }, function (response) {

          var options = {
            success: function(result) {
              var imgUrl, successMessage;
              initializeUploader(result, options);

              successMessage = $('#successMessage');
              imgUrl = successMessage.data('imageurl');

              if (!!imgUrl) {
                tile.find('img').attr('src', '/Content/Images/Current/' + imgUrl);
                successMessage.fadeOut(4000);
              }
            }
          };

          initializeUploader(response, options);
        });
      });
      
      $.each(mouseEvents, function (key, value) {
        var size = baseSize + value + 'px',
            move = value / 2 * -1 + 'px',
            event = 'mouse' + key;

        $('.tile').on(event, function () {
          $(this).find('img').animate({
            'width': size,
            'height': size,
            'left': move,
            'top': move
          }, speed, function() {
            $(this).css({
              'z-index' : value > 0 ? '1' : '0'
            });
          }).css({
            'z-index': '1'
          });
        });
      });
    });
  }
  
  return {
    init: init
  };
});