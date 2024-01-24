var HANDLER_INITIALIZATION_COMPLETE = false;
function LoadHandlers() {
    if (HANDLER_INITIALIZATION_COMPLETE) return;
    _LoadHandlers();
    HANDLER_INITIALIZATION_COMPLETE = true;
}

function LoadKendoHandlers() {
    $.fn.kwidget = function () {
        var input = $(this);
        if (!input.is("input") && !input.hasClass('k-grid') && !input.hasClass('k-content')) return;
        var widget = null;
        for (var key in input.data())
            if (key.match(/^kendo/))
                widget = input.data(key);
        return widget;
    };
    $.fn.krefresh = function () {
        var widget = this.kwidget();
        if (widget == null) return;
        if (widget.dataSource == null) return;
        widget.dataSource.read();
        if (widget.refresh == null) return;
        widget.refresh();
    };
    $.fn.kclear = function () {
        var widget = this.kwidget();
        if (widget == null) return;
        widget.value(null);
    };
    $.fn.kdisable = function () {
        var widget = this.kwidget();
        widget.enable(false);
    }
    $.fn.kenable = function () {
        var widget = this.kwidget();
        widget.enable(true);
    }
    $.fn.khide = function () {
        var widget = this.kwidget();
        if (widget == null)
            this.hide();
        else
            this.closest(".k-widget").hide();
    };
    $.fn.kshow = function () {
        var widget = this.kwidget();
        if (widget == null)
            this.show();
        else
            this.closest(".k-widget").show();
    };
    $.fn.khasValue = function () {
        var widget = this.kwidget();
        return widget.value() != "" && widget.value() != null;
    }
    $.fn.kdata = function () {
        var grid = this.kwidget();
        if (grid.options.name == 'Grid') {
            return grid.dataSource.data().toJSON();
        }
        return null;
    }
    $.fn.kdate = function () {
        var widget = this.kwidget();
        var value = widget.value();
        if (value == null)
            return null;
        return value.toUTCString();
    };
    $.fn.kvalue = function (val) {
        var widget = this.kwidget();
        if (widget == null) return null;
        if (val != null) {
            widget.value(val);
            widget.trigger('change');
            return null;
        }
        var value = widget.value();
        if (widget.options.name == 'DatePicker') {
            value = this.val();
            if (value == '' || value == null)
                return null;
            value = moment(value, ["MM/DD/YYYY", "MM-DD-YYYY"]).format('MM-DD-YYYY');
        } else if (widget.options.name == 'DropDownList') {
            if (widget.select() == -1)
                return null;
        }
        return value;
    };
    $.fn.khighlight = function () {
        $(this).closest('span.k-widget').addClass('highlight');
    };
    $.fn.krelax = function () {
        $(this).closest('span.k-widget').removeClass('highlight');
    };
    var kendo = window.kendo,
        ui = kendo.ui,
        Widget = ui.Widget
    ;

    kendo.data.ObservableArray.prototype.remove = function (item) {
        var index = this.indexOf(item);
        if (index !== -1)
            this.splice(index, 1);
    };
    kendo.data.ObservableArray.prototype.clear = function () {
        this.splice(0, this.length);
    };
    Array.prototype.remove = function (item) {
        var index = this.indexOf(item);
        if (index !== -1)
            this.splice(index, 1);
    };
    Array.prototype.clear = function () {
        this.splice(0, this.length);
    };
}

function LoadJqueryHandlers() {
    $.isString = function (s) {
        return typeof (s) == "string";
    }
    $.isEmptyOrWhitespace = function (s) {
        return s == null || /^\s*$/.test(s);
    };
    jQuery.fn.outerHTML = function (s) {
        return s
            ? this.before(s).remove()
            : jQuery("<p>").append(this.eq(0).clone()).html();
    };
}

function LoadPrototypeExtensions() {

}

function _LoadHandlers() {
    LoadKendoHandlers();
}

LoadJqueryHandlers();