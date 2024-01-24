function BindCollectionEditor(editorGuid, parentGuid, collectionName, binder) {
    var handler = function (e) {
        console.log('collection change handler called; e:', e);
        console.log('parentGuid:', parentGuid, 'collection:', collectionName, 'editor:', editorGuid);
        if (e.action == "add") {
            var item = e.items[0];
            var collectionEditor = $('.view-model-collection-editor[id="' + editorGuid + '"]');
            collectionEditor.append(item.Editor);
            console.log('appended editor');
        }
    };
    binder(parentGuid, collectionName, handler);
}