define(["jquery", "jquery.form"], function ($) {
  var baseSize = 64,
      speed = 200,
      mouseEvents = { 'enter': 10, 'leave': 0, 'down': -10, 'up': 10 };

  function init() {

    $(function () {
      var initializeUploader = function(response, options) {
        $('#artUpload').html(response);
        $('form').ajaxForm(options);
        $('#cancel').on('click', function () {
          $('#uploader').fadeOut(250);
        });
        $('#File').change(function () {
          var path = $(this).val();
          var editPath = path.substring(path.indexOf("fakepath\\") + 9);
          $('#path').val(editPath); 
        });
      };

      $('.tile').on('click', function () {
        var tile = $(this);
        if (!$(tile).find('img').hasClass('unassigned')) {
          $('#uploader').fadeOut(250);
          return;
        }

        $.get('/Art/Upload', { x: tile.data('x'), y: tile.data('y') }, function (response) {
          var options = {
            success: function(result) {
              var imgUrl, successMessage; 
              initializeUploader(result, options);

              successMessage = $('#successMessage');
              imgUrl = successMessage.data('imageurl');

              if (!!imgUrl) {
                tile.find('img').attr('src', imgUrl).removeClass('unassigned').unbind();
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

        $('.unassigned').on(event, function () {
          $(this).animate({
            'width': size,
            'height': size,
            'left': value < 0 ? 0 : move,
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