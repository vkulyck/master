var True = true;
var False = false;

// Close modal dialogs when the user clicks outside
window.ActiveClosableModal = null;
$(document).on("click", ".k-overlay", function () {
    if (ActiveClosableModal != null) {
        ActiveClosableModal.close();
    }
});
// Set default toString format to ISO strings for simpler de/serialization
Date.prototype.toString = Date.prototype.toISOString;

function LoadParams(qparams) {
    var baseUrl = window.location.href.split('?')[0]
    var qstring = $.param(qparams);
    var url = baseUrl + "?" + qstring;
    window.location.href = url;
}

function SerializeForm($form) {
    var unindexed_array = $form.serializeArray();
    var indexed_array = {};

    $.map(unindexed_array, function (n, i) {
        indexed_array[n['name']] = n['value'];
    });

    return indexed_array;
}

function CreateFormValidator($form) {
    var validator = $form.kendoValidator({
        validate: function (e) {
            $("span.k-invalid-msg").hide();
            $("span.k-form-error").hide();
            $("span.field-validation-error").hide();
            let radioGroups = ['#MedicalInsurance', '#WorkStatus', '#CurrentOccupation', '#Job', '#MartialStatus', '#SexOrientation', '#ConversationalEnglish'];
            for (var i = 0; i < radioGroups.length; i++) {
                var ctrl = $(radioGroups[i]);
                var currentValue = ctrl.radioValue();
                if (typeof currentValue === 'undefined') {
                    ctrl.radioInvalidate();
                }
            }
        }
    }).data("kendoValidator");
    return validator;
}

function PostJson(formData, url, success, error) {
    $.post({
        url: url,
        data: JSON.stringify(formData),
        contentType: 'application/json',
        success: function (data) {
            if (success)
                success(data);
        },
        error: function (err) {
            if (error)
                error(err);
        }
    });
}