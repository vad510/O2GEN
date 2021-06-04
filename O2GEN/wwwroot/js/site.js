"use strict"
!(function prepareLists() {
    var treeViewitems = document.getElementsByClassName("caret");

    for (var i = 0; i < treeViewitems.length; i++) {
        treeViewitems[i].addEventListener("click", function () {
            this.parentElement.querySelector(".nested").classList.toggle("active");
            this.classList.toggle("caret-down");
        });
    }
})();

$(function () {
    var placeholder = $('#placeholder');

    $('button[data-toggle="modal-toggler"]')
        .click(function (e) {

            var url = $(this).data('url');
            $.get(url).done(function (data) {
                placeholder.html(data);
                placeholder.find('.modal').modal('show');
            })
    })
})

//window.addEventListener("DOMContentLoaded", function (event) {
//    prepareLists();
//})