////var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
////    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
////    return new (P || (P = Promise))(function (resolve, reject) {
////        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
////        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
////        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
////        step((generator = generator.apply(thisArg, _arguments || [])).next());
////    });
////};
////var __generator = (this && this.__generator) || function (thisArg, body) {
////    var _ = { label: 0, sent: function () { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
////    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function () { return this; }), g;
////    function verb(n) { return function (v) { return step([n, v]); }; }
////    function step(op) {
////        if (f) throw new TypeError("Generator is already executing.");
////        while (_) try {
////            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
////            if (y = 0, t) op = [op[0] & 2, t.value];
////            switch (op[0]) {
////                case 0: case 1: t = op; break;
////                case 4: _.label++; return { value: op[1], done: false };
////                case 5: _.label++; y = op[1]; op = [0]; continue;
////                case 7: op = _.ops.pop(); _.trys.pop(); continue;
////                default:
////                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
////                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
////                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
////                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
////                    if (t[2]) _.ops.pop();
////                    _.trys.pop(); continue;
////            }
////            op = body.call(thisArg, _);
////        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
////        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
////    }
////};

let placeholder = document.getElementById("placeholder");

/** Find all elements with 'caret' class and apply listeners for show inner elements. This method requeres 'caret' and 'active-table' classes. Only for HTMLTableElement */
function prepareLists() {
    const treeViewitems = document.getElementsByClassName("caret");

    for (var i = 0; i < treeViewitems.length; i++) {

        treeViewitems[i].addEventListener("click", function () {
            this.classList.toggle('caret-down');
            let clickedTr = this.closest('tr');

            // find all childs which lvl are greater by 1(one) for clicked row
            let clickedLevel = parseInt(clickedTr.dataset.level);
            let allowedLevel = clickedLevel + 1;

            if (this.classList.contains('caret-down')) {

                while (clickedTr != null && clickedTr.nextElementSibling != null) {

                    clickedTr = clickedTr.nextElementSibling;

                    let thisTrLevel = parseInt(clickedTr.dataset.level);

                    if (thisTrLevel == clickedLevel) {
                        return;
                    }

                    if (thisTrLevel > allowedLevel) {
                        continue;
                    }

                    expandNestedChilds(clickedLevel, clickedTr, allowedLevel);
                }

            }
            else {
                while (clickedTr != null && clickedTr.nextElementSibling != null) {
                    clickedTr = clickedTr.nextElementSibling;

                    let thisTrLevel = parseInt(clickedTr.dataset.level);

                    if (thisTrLevel == clickedLevel) {
                        return;
                    }

                    if (thisTrLevel >= allowedLevel) {
                        if (clickedTr.classList.contains('active-table')) {
                            clickedTr.classList.remove('active-table');
                        }
                    }
                }
            }
        });
    }
}

function expandNestedChilds(allowedLevelToExpand, element, elementLevel) {

    if (element == null)
        return;

    let localLevel = parseInt(element.dataset.level);

    //console.log("allowedLevelToExpand " + allowedLevelToExpand);
    //console.log("elementLevel " + elementLevel);
    //console.log("localLevel " + localLevel);
    //console.log('\n');

    if (localLevel == allowedLevelToExpand) {
        return;
    }

    if (element.classList.contains('parent') && element.children[0].children[0].classList.contains('caret-down')) {

        element.classList.add('active-table');

        expandNestedChilds(allowedLevelToExpand, element.nextElementSibling, elementLevel + 1);
    }
    else {
        if (localLevel == elementLevel) {
            element.classList.add('active-table');

            expandNestedChilds(allowedLevelToExpand, element.nextElementSibling, localLevel);
        } 
    }
}

var ready = function (action) {
    if (document.readyState != "loading")
        action();
    else
        document.addEventListener("DOMContentLoaded", action);
    //console.log("ready created");
};

/** Find all buttons which have attribute for creating modal */
ready(function () {

    const elems = document.querySelectorAll('[data-toggle="modal-toggler"]');

    for (var i = 0; i < elems.length; i++) {
        //console.log("buttons founded: " + i);
        elems[i].addEventListener('click', btnClick);
    }

    placeholder = document.getElementById("placeholder");
    prepareLists();
});

/** Attempts to create modal into element with Id == placeholder */
function btnClick() {
    if (document.readyState != "complete")
        return;
    placeholder = document.getElementById("placeholder");
    if (placeholder == undefined) {
        return;
    }
    const url = this.getAttribute("data-url");

    getDataWithXmlHttpRequest(url, placeholder);

};

/** xhr for bootstrap modal data */
function getDataWithXmlHttpRequest(url, placeholder) {
    const xhr = new XMLHttpRequest();
    xhr.onloadstart = function (e) {
    };
    xhr.onload = function (e) {
        tryCreateModal(placeholder, xhr.response);
    };
    xhr.onerror = function (e) {
        //console.log(e);
    };
    xhr.open('get', url);
    xhr.send();
}

function sortHidden() {
    var list = $('#Selected').children();
    for (var i = 0; i < list.length; i++) {
        list[i].id = 'Parameters[' + i + ']';
        list[i].name = 'Parameters[' + i + ']';
    }
}
function ControlDetVis() {
    var value = $('#AssetParameterTypeId').val();
    if (value == '2' || value === null) {
        $('#ContolDetails').fadeOut();
    }
    else {
        $('#ContolDetails').fadeIn();
    }
}
function AssetSortFade() {
    var value = $('#AssetSortId').val();
    if (value == '2') {
        $('.chld-fadable').fadeIn();
        $('.par-fadable').fadeOut();
    }
    else {
        $('.chld-fadable').fadeOut();
        $('.par-fadable').fadeIn();
    }
}

/** Creates bootstrap modal into giver placeholder. Also adds event listener for OnSave click and performs validation of sended data */
function tryCreateModal(placeholder, response) {
    placeholder.innerHTML = response;
    const div = placeholder.querySelector(".modal");
    const modal = new bootstrap.Modal(div);
    modal.show();
    prepareLists();
    $("#myInput1").on("keyup", function () {
        var value = $(this).val().toLowerCase();
        $("#SelectList1 option").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });
    $("#myInput2").on("keyup", function () {
        var value = $(this).val().toLowerCase();
        $("#SelectList2 option").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });
    $("#right").click(function () {
        var elem = $('#SelectList1').find(":selected");
        if (elem != null) {
            $('#SelectList1').find(":selected").remove();
            for (var i = 0; i < elem.length; i++) {
                $('#SelectList2').append(elem[i]);
                $('#Selected').append('<input id="prodId" name="prodId" type="hidden" value="' + elem[i].value + '">');
            }
            sortHidden();
        }
    });
    $("#left").click(function () {
        var elem = $('#SelectList2').find(":selected");
        if (elem != null) {
            $('#SelectList2').find(":selected").remove();
            $('#SelectList1').append(elem);
            $("#Selected>input[value='" + elem.val() + "']").remove();
            sortHidden();
        }
    });
    if ($('#EngineerEdit').length == 1) {
        $("#IsUser").click(function () {
            if ($("#IsUser").is(":checked")) {
                $('#logform').fadeIn();
            }
            else {
                //console.log(234);
                $('#logform').fadeOut();
                $('#Password').val('');
                $('#ConfirmPassword').val('');
            }
        });
        if ($("#IsUser").is(":checked")) {
            $('#logform').fadeIn();
        }
        else {
            $('#logform').fadeOut();
            $('#Password').val('');
            $('#ConfirmPassword').val('');
        }
    }
    /////////////////  JQuery working example for validate modal ///////////////
    //placeholder = $(placeholder);

    //placeholder.on('click', '[data-save="modal"]', function (event) {
    //event.preventDefault();

    //var form = $(this).parents('.modal').find('form');
    //var actionUrl = form.attr('action');
    //var dataToSend = form.serialize();

    //$.post(actionUrl, dataToSend).done(function (data) {
    //    var newBody = $('.modal-body', data);
    //    $(placeholder).find('.modal-body').replaceWith(newBody).clone();

    //    var isValid = newBody.find('[name="IsValid"]').val() == 'True';
    //    if (isValid) {
    //        $(placeholder).find('.modal').modal('hide');
    //    }
    //});
    //});
    //return;
    //////////////////  end of JQuery example  /////////////////////

    const btn = placeholder.querySelector('[data-save="modal"]');

    btn.addEventListener('click', function (event) {
        event.preventDefault();

        const modal = placeholder.querySelector('.modal');

        const form = modal.querySelector('form');
        const url = form.action;
        const dataToSend2 = new FormData(form);

        const xhr = new XMLHttpRequest();

        BlockModal();
        xhr.onload = function (e) {
            TryValidate(xhr.response);
            UnblockModal();
        }
        xhr.onerror = function (e) {
            UnblockModal();
        }

        xhr.open('post', url);
        xhr.send(dataToSend2);
    });

    //ControlDetVis();
    AssetSortFade();
}
function BlockModal() {
    if ($('#CoverModal').length == 1) {
        $('#CoverModal').removeClass('ModalBlockInvisible');
    }
}
function UnblockModal() {
    if ($('#CoverModal').length == 1) {
        $('#CoverModal').addClass('ModalBlockInvisible');
    }
}

/** validate modal on errors before submit */
function TryValidate(response) {

    if (placeholder == null)
        throw new Error("Null reference exception");

    //console.log(response);
    if (response == 0) {
        window.location.reload(true);
        return;
    }
    //if (response.contains("html")) {
    //    document.innerHTML = response;
    //}
    const div = document.createElement('div');
    div.innerHTML = response;

    const newBody = div.querySelector('.modal-body');
    const oldBody = placeholder.querySelector('.modal-body');

    if (oldBody == null) {

        const modal = placeholder.querySelector('.modal');

        if (modal != null) {
            //console.log("Found existing modal");

            tryRemoveModals();
        }

        //console.log("No body of modal for placeholder is found");
        return;
    }

    oldBody.replaceWith(newBody);

    prepareLists();
    //const errors = placeholder.querySelectorAll(".field-validation-error");

    //if (errors.length > 0) {
    //    //console.log("we have errors");
    //}
    //else {
    //    //console.log("no errors");
    //    //tryRemoveModals();
    //    window.location.reload(true);
    //}
}

/** remove all bootstrap modals from screen */
function tryRemoveModals() {
    const modals = document.getElementsByClassName('modal');

    for (let i = 0; i < modals.length; i++) {
        modals[i].classList.remove('show');
        modals[i].setAttribute('aria-hidden', 'true');
        modals[i].setAttribute('style', 'display:none');
    }

    const modalOverlays = document.getElementsByClassName('modal-backdrop');

    for (let i = 0; i < modalOverlays.length; i++) {
        document.body.removeChild(modalOverlays[i]);
    }
}

$(document).on('show.bs.modal', '.modal', function (event) {
    var zIndex = 1040 + (10 * $('.modal:visible').length);
    $(this).css('z-index', zIndex);
    setTimeout(function () {
        $('.modal-backdrop').not('.modal-stack').css('z-index', zIndex - 1).addClass('modal-stack');
    }, 0);
});

//function getParents(el, parentSelector) {
//    if (parentSelector === undefined) {
//        parentSelector = document;
//    }
//    var parents = [];
//    var p = el.parentNode;
//    while (p !== parentSelector) {
//        var o = p;
//        parents.push(o);
//        p = o.parentNode;
//    }
//    parents.push(parentSelector);
//    return parents;
//}