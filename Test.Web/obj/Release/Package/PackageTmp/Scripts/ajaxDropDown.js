viewModel = {
    collection: ko.observableArray()
};

$(function () {
    var changeSelectAjax = function () {
        var $select = $(this);
        var tempCollection = [];

        var options = {
            url: $select.attr("ajax-action"),
            data: { id: $select.val() },
            dataType: 'json',
            method: 'POST'
        };

        if ($select.val() == "-1") {
            viewModel.collection.removeAll();
        } else {
            $.ajax(options).done(function (data) {
                viewModel.collection.removeAll();
                $(data).each(function(index, item) {
                   viewModel.collection.push(item);
                });
            }).error(function() {
                alert("Error");
            });
        }
    };

    var changeSelect = function() {
        var $select = $(this);
        var $target = $($select.attr("target"));
        if ($select.val() == "-1") {
            $target.prop("disabled", true);
        } else {
            $target.prop("disabled", false);
        }
    }


    var ajaxFormSubmit = function() {
        var $form = $(this);

        var datas = "";

        $("select").each(function(index, item) {
            datas += $(item).val() + ",";
        });

        var options = {
            url: $form.attr('ajax-action'),
            type: $form.attr('ajax-method'),
            data: { answers: datas }
        };

        $.ajax(options).done(function (data) {
            var $target = $($form.attr("ajax-target"));
            $target.replaceWith(data);
        });

        return false;
    };

    $('#addQuestion').click(function () {
        $('.hiddenForm').toggle('fast');
    });

    $("form[ajax-form='true']").submit(ajaxFormSubmit);

    ko.applyBindings(viewModel);
    $("select[ajax-select='true']").change(changeSelectAjax);
    $("select").change(changeSelect);

    $("select").prop("disabled", true);
    $("button").prop("disabled", true);
    $("#studentSelect").prop("disabled", false);
});