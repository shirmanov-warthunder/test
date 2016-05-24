$(function () {
    $('#addQuestion').click(function() {
        $('.hiddenForm').toggle('fast');
    });

    $("form[ajax-form='true']").submit(ajaxFormSubmit);
});