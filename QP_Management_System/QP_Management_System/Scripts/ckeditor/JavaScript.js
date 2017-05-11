$(function () {
    $('#test').keyup(function () {
        var pos = 0;
        if (this.selectionStart) {
            pos = this.selectionStart;
        } else if (document.selection) {
            this.focus();

            var r = document.selection.createRange();
            if (r == null) {
                pos = 0;
            } else {

                var re = this.createTextRange(),
                rc = re.duplicate();
                re.moveToBookmark(r.getBookmark());
                rc.setEndPoint('EndToStart', re);

                pos = rc.text.length;
            }
        }
        $('#c').html(this.value.substr(0, pos).split("\n").length);
    });
});