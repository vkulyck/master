/*
 * This code is based on a sample from the following website: https://codepen.io/vsync/pen/frudD
*/

function ConfigureAutosizeTextarea(area) {
    area.one('focus.autoExpand', function () {
        var savedValue = this.value;
        this.value = '';
        this.baseScrollHeight = this.scrollHeight;
        this.value = savedValue;
    });
    function recompute() {
        var minRows = area.attr('data-min-rows') | 0, rows;
        this.rows = minRows;
        rows = Math.floor((this.scrollHeight - this.baseScrollHeight) / 16);
        this.rows = minRows + rows;
        var maxHeight = parseInt(area.css('max-height'));
        if (maxHeight < this.scrollHeight) {
            area.css('overflow-y', 'scroll');
            area.css('resize', 'none');
        } else {
            area.css('overflow-y', 'hidden');
            area.css('resize', null);
        }
    }
    area.on('input.autoExpand', function () {
        console.log('recomputing autosize');
        recompute.call(this);
    });
    area.on('change', function () {
        console.log('this thingy is changed, her is scroll height:', this.scrollHeight);
        recompute.call(this);
    });
}