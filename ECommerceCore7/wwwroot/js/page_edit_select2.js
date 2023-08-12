$(document).ready(function () {

$("#Tags").select2({
    dir: "rtl",
    language: "fa",
    tags: true,
    //tokenSeparators: [',', ' '],
    createTag: function (tag) {
        return {
            id: tag.term,
            text: tag.term,
            // add indicator:
            isNew: true
        }
    }
}).on('select2:select',
    function (e) {
        if (e.params.data.isNew) {

            var selectElement = $(this);
            $.ajax({
                url: saveTagActionUrl,
                data: { tag: e.params.data.text },
                type: "GET"
            }).done(function (result) {
                if (result != null) {
                    if (result.ID > 0) {
                        selectElement.find('[data-select2-tag="true"]').replaceWith(
                            '<option selected value="' +
                            result.ID +
                            '">' +
                            e.params.data.text +
                            '</option>');
                    } else {
                        return null;
                    }
                }
            });
        } else {
            // <Workaround>:
            // When editing tags, some new items are not correctly added to the end of the tags list,
            // following code is a workaround to that,
            // taken code taken from https://stackoverflow.com/questions/31431197/select2-how-to-prevent-tags-sorting
            var element = e.params.data.element;
            console.log('element: ' + element);
            var $element = $(element);
            $element.detach();
            $(this).append($element);
            $(this).trigger("change");
            // </Workaround>
        }
    })
    // <Activate cursor in same input again>
    .on('select2:close', function (e) {
        var select2SearchField = $(this).parent().find('.select2-search__field'),
            setfocus = setTimeout(function () {
                select2SearchField.focus();
            }, 100);
    });
                // </Activate cursor in same input again>
});