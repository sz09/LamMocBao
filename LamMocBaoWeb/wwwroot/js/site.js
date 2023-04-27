// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function prepare() {
    if (!String.format) {
        String.format = function (format) {
            var args = Array.prototype.slice.call(arguments, 1);
            return format.replace(/{(\d+)}/g, function (match, number) {
                return typeof args[number] != 'undefined'
                    ? args[number]
                    : match
                    ;
            });
        };
    }
};
prepare();

var DialogBox = {
    Show: function (message, title) {
        if (!$('#from-server-dom').children('#main-modal').length) {
            $('#from-server-dom').append(message);
        }
        $('#main-modal').modal({ backdrop: 'static', keyboard: false, show: true });
        $('#main-modal').on('hidden.bs.modal', function () {
            $('#from-server-dom').empty();
        })
    },
    Confirm: function (messge, func, buttonConfirm, buttonNo ) {
        $.confirm({
            theme: 'dark',
            title: messge,
            buttons: {
                confirm: {
                    text: buttonConfirm,
                    action: func
                },
                cancel: {
                    text: buttonNo
                }
            }
        });
        //$('#' + id).modal({ backdrop: 'static', keyboard: false, show: true });
    }
}

var ImageSlider = {
    current: {},
    goToTimeout: null,
    ShowImage: function (identifier, index) {
        // Main div
        var slideImageElements = $(`[id^=${identifier}]`);
        var allImagePositions = slideImageElements.map(function (i, element) {
            return element.id.replace(identifier, '') * 1;
        }).toArray();
        slideImageElements.each((index, elem) => {
            $(elem).addClass('d-none')
        });
        if (allImagePositions.indexOf(index) > -1) {
            $(`#${identifier}${index}`).removeClass('d-none');
        }

        // Preview
        var previewImageElements = $(`[id^=preview-${identifier}]`);
        previewImageElements.each((index, elem) => {
            $(elem).removeClass('border-selected')
        });
        $('#preview-' + identifier + index).addClass('border-selected');
    },
    GoTo: function (identifier, increasePosition) {
        var me = this;
        if (me.goToTimeout) {
            clearTimeout(me.goToTimeout);
        }

        me.goToTimeout = setTimeout(function () {
            var slideImageElements = $(`[id^=${identifier}]`);
            var allImagePositions = slideImageElements.map(function (i, element) {
                return element.id.replace(identifier, '') * 1;
            }).toArray();
            var showingImage = $(`[id^=${identifier}]:not(.d-none)`);
            if (showingImage && showingImage[0]) {
                var currentId = showingImage[0].id;
                var currentIndex = currentId.replace(identifier, '') * 1;
                var goToIndex = currentIndex + increasePosition;

                if (!allImagePositions.includes(goToIndex)) {
                    if (increasePosition === 1) {
                        goToIndex = 0;
                    }
                    else {
                        goToIndex = allImagePositions[allImagePositions.length - 1];
                    }
                }
                $('.images-slider-content .preview-button').removeClass('border-selected');
                var next = slideImageElements[goToIndex];
                if (next) {
                    var nextId = next.id;
                    $(`[id^=preview-${nextId}]`).addClass('border-selected');
                    var targetPreview = $(`[id^=preview-${nextId}]`);
                    $('.images-slider').animate({ scrollLeft: goToIndex * targetPreview.width() - (targetPreview.width() / 2) }, 300);
                }

                $(`#${currentId}`).addClass('d-none');
                $(`#${identifier}${goToIndex}`).removeClass('d-none');
            }
            me.goToTimeout = null;
        }, 200)
    }
}

var Pagination = {
    GoToPage: function(page, formSelector = 'form'){
        $('#hidden-paging').val(page);
        $(formSelector).submit();
    }
}


var CustomerHelper = {
    Keys: {
        InterestedInProduct: 'InterestedInProduct',
        Seperator: ', ',
        InterestedButtonPrefix: 'interested-in-'
    },
    LocalStorageKey: 'InterestedInProduct',
    InterestedIn: function (id, e) {
        e.preventDefault();
        var interestedInProduct = CustomerHelper.GetInterestedInItems();
        var index = interestedInProduct.indexOf(id);
        if (index > -1) {
            interestedInProduct.splice(index, 1);
            $(`#${CustomerHelper.Keys.InterestedButtonPrefix}${id}`).addClass('btn-outline-link');
            $(`#${CustomerHelper.Keys.InterestedButtonPrefix}${id}`).removeClass('btn-outline-danger');
        }
        else {
            interestedInProduct.push(id);
            $(`#${CustomerHelper.Keys.InterestedButtonPrefix}${id}`).removeClass('btn-outline-link');
            $(`#${CustomerHelper.Keys.InterestedButtonPrefix}${id}`).addClass('btn-outline-danger');
        }

        localStorage.setItem(CustomerHelper.Keys.InterestedInProduct, interestedInProduct.join(CustomerHelper.Keys.Seperator));

    },
    DisplayInterestedIn: function () {
        var items = CustomerHelper.GetInterestedInItems();
        items.map(d => `#${CustomerHelper.Keys.InterestedButtonPrefix}${d}`).forEach(d => {
            $(d).removeClass('btn-outline-link');
            $(d).addClass('btn-outline-danger');
        })
        
    },
    GetInterestedInItems: function () {
        var value = localStorage.getItem(CustomerHelper.Keys.InterestedInProduct) ?? '';
        return value.split(CustomerHelper.Keys.Seperator).filter(d => !!d);
    }
}

var FormUtils = {
    SetFormValue: function (formId, obj) {
        var forms = $('#' + formId);
        var fields = Object.keys(obj);
        if (forms && forms[0]) {
            fields.forEach((key) => {
                if (typeof obj[key] !== "undefined") {
                    $('#' + formId + ` [name=${key}]`).val(obj[key]);
                }
            })
        }
    }
}

var LoaderService = {
    Id: '#main-loader',
    Show: function () {
        $(this.Id).addClass('show');
    },
    Hide: function () {
        $(this.Id).removeClass('show');
    },
    Register: function () {
        $('form').on('submit', function (e) {
            LoaderService.Show();
        });
        LoaderService.Hide();
    }
}

var StringUtils = {
    Splitter: "#@#@",
    Combine: function (src) {
        return src.join(this.Splitter);
    },
    Split: function (src) {
        return src.split(this.Splitter).filter((e) => !!e);
    }
}

//var CookieService = {
//    AddToList: function (key, value) {
//        var items = CookieService.GetList(key);
//        if (items) {
//            items.push(value);
//            CookieService.Set(key, items);
//        }
//        else {
//            CookieService.Set(key, [value]);
//        }
//    },
//    Set: function (key, value) {
//        await cookieStore.set(key, value);
//    },
//    Get: function () {
//        return await cookieStore.get(key).value;
//    },
//    GetList: function (key) {
//        var cookieFound = await cookieStore.get(key);
//        if (cookieFound.value) {
//            return cookieFound.value.split(',');
//        }
//        return null; 
//    }
//}