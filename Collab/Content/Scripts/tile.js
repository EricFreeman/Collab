define(["jquery", "jquery.form"], function ($) {
  var baseSize = 64,
      speed = 200,
      mouseEvents = { 'enter': 10, 'leave': 0, 'down': -10, 'up': 10 };

  function init() {

    $(function () {
      $('.tile').on('click', function() {
        var tile = $(this);
        $.get('Art/Upload', { x: tile.data('x'), y: tile.data('y') }, function (response) {
          $('#artUpload').html(response);

          var options = {  
            success: function(result) {
              var imgUrl;
              $('#artUpload').html(result);
              imgUrl = $('#successMessage').data('imageurl');
              if(imgUrl != undefined)
                tile.find('img').attr('src', '/Content/Images/Current/' + imgUrl);
            }
          };
          $('form').ajaxForm(options);
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