var ajaxFormSubmit = function () {
    var $form = $(this);

    var options = {
        url: $form.attr('ajax-action'),
        type: $form.attr('ajax-method'),
        data: $form.serialize()
    };

    $.ajax(options).done(function (data) {
        var $target = $($form.attr("ajax-target"));
        $target.replaceWith(data);
    });

    return false;
};