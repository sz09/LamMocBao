var ImageUtils = {
    BuildDOM: function (item, clasName, parentIdSelector, uploadedImageIdSelector) {
        var dom = document.createElement('div');
        dom.setAttribute('class', `${clasName} col-4 p-1`);
        dom.setAttribute('id', item.Id);
        var image = document.createElement('img');
        image.setAttribute('src', item.Url);
        dom.appendChild(image);
        var button = document.createElement('button');
        button.setAttribute('type', 'button');
        button.setAttribute('class', 'btn btn-sm');
        button.setAttribute('onclick', `ImageUtils.RemoveImage('${parentIdSelector}', '${item.Id}', '${uploadedImageIdSelector}')`);
        var span = document.createElement('span');
        span.setAttribute('aria-hidden', 'true');
        span.innerHTML = '&times;';
        button.appendChild(span);
        dom.appendChild(button);
        return dom;
    },
    RemoveImage: function (parentIdSelector, id, uploadedImageIdSelector) {
        $(`#${parentIdSelector} >#${id}`).remove();
        var val = $(`#${uploadedImageIdSelector}`).val() ?? '';
        var values = StringUtils.Split(val);
        values = values.filter((e) => e !== id && !!e);
        $(`#${uploadedImageIdSelector}`).val(StringUtils.Combine(values));
    }
}