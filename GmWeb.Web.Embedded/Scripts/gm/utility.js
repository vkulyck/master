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