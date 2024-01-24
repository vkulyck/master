var ObservableViewModel = function ObservableViewModel(options) {
    this.model = options.model;
    this.view = $('#' + this.model.Guid);
    this.children = [];
    this.indent = "  ";
    var KendoWrapper;
    if ($.isArray(this.model))
        KendoWrapper = kendo.data.ObservableArray;
    else
        KendoWrapper = kendo.data.ObservableObject;
    this.observable = new KendoWrapper(this.model);
    this.parent = options.parent;
    this.root = null;
    this.observable.bind("change", function (e) {
        console.log('model changed; e:', e, 'plain model:', this.model);
    })
}

ObservableViewModel.prototype = new Array;

ObservableViewModel.prototype.getRoot = function () {
    if (this.root == null) {
        if (this.parent == null) {
            this.root = this;
        } else {
            this.root = this.parent.getRoot();
        }
    }
    return this.root;
}

ObservableViewModel.prototype.assignKeyValue = function (key, value) {
    this[key] = value;
    this.addChild(value);
}

ObservableViewModel.prototype.addChild = function (value) {
    this.children.push(value);
    if (this.parent != null)
        this.parent.addChild(value);
}

ObservableViewModel.prototype.recursive = function () {
    var self = this;
    if (self.parent != null)
        self.indent += parent.indent;
    var keys = Object.keys(self.model);
    $.each(keys, function (idx, key) {
        if (key.startsWith("_") || key == "parent" || key == "get" || key == "change")
            return;
        var value = self.model[key];
        //console.log(indent, 'read', key, 'from model:', value);
        if ($.isFunction(value) || $.isString(value)) {
            //console.log(indent, 'value is str/func');
            self[key] = value;
            //console.log(indent, 'creating simple str/func for:', key);
        }
        else if ($.isArray(value)) {
            //console.log(indent, 'value is array');
            var observable = new ObservableViewModel({ model: value, parent: self }).recursive();
            observable.length = value.length;
            self.assignKeyValue(key, observable);
            //console.log(indent, 'array filled for key:', key, 'array:', self[key]);
        } else if ($.isPlainObject(value)) {
            //console.log(indent, 'value is obj');
            //console.log(indent, 'creating sub-object for key:', key, 'object:', value);
            var observable = new ObservableViewModel({ model: value, parent: self }).recursive();
            self.assignKeyValue(key, observable);
        } else {
            //console.log(indent, 'value is other');
            //console.log(indent, 'creating simple property for:', key, 'value:', value);
            self[key] = value;
        }
    });
    console.log('binding guid:', this.model.Guid, 'to this:', this);
    return this;
}

ObservableViewModel.prototype.bindViews = function () {
    this.view = $('#' + this.Guid);
    kendo.bind(this.view, this.observable);
    $.each(this.children, function (idx, child) {
        child.bindViews();
    });
}

ObservableViewModel.prototype.update = function (child) {

}