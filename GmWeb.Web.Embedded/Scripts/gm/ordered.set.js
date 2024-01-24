var OrderedSet = function OrderedSet(items, selector) {
    var self = this;
    self.set = {};
    self.collection = [];
    self.selector = selector;
    $.each(items, function (idx, item) {
        var id = self.selector(item);
        self.set[id] = item;
        self.collection.push(item);
    });
    self.addRange(items);
}

OrderedSet.prototype.add = function (item) {
    var self = this;
    var id = self.selector(item);
    if (id in self.set)
        return;
    self.set[id] = item;
    self.collection.push(item);
    console.log('added item (id=' + id + '):', item);
}

OrderedSet.prototype.addRange = function (items) {
    var self = this;
    $.each(items, function (idx, item) {
        self.add(item);
    });
};

OrderedSet.prototype.remove = function (item) {
    var self = this;
    var id = self.selector(item);
    if (!(id in self.set))
        return;
    delete self.set[id];
    self.collection.splice($.inArray(item, self.collection), 1);
    console.log('removed item (id=' + id + '):', item);
}

OrderedSet.prototype.removeRange = function (items) {
    var self = this;
    $.each(items, function (idx, item) {
        self.remove(item);
    });
};

OrderedSet.prototype.clear = function () {
    var self = this;
    self.set = {};
    self.collection = [];
}
OrderedSet.prototype.lookupItem = function (id) {
    return this.set[id];
}
OrderedSet.prototype.getItem = function (idx) {
    return this.collection[idx];
}
OrderedSet.prototype.length = function () {
    return this.collection.length;
}