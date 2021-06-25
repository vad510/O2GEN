// for avoid .js file to auto wrap __awaiter and __generator 
// need to target es2017(in tsconfig.json) ?
// all comments in here would not come into .js file

/*
 * this function gets callback as a parameter and firing it whenever page is loaded
 */
let ready = (action) => {
    if (document.readyState != "loading")
        action();
    else
        document.addEventListener("DOMContentLoaded", action);
    console.log("ready created");
}

/*
 * callback for page loaded. For now this function adds event 'onclick' 
 * for every button with specified attribute
 */
ready(() => {
    let elems = document.querySelectorAll('[data-toggle="modal-toggler"]')

    for (var i = 0; i < elems.length; i++) {
        console.log("buttons founded: " + i)
        elems[i].addEventListener('click', btnClick);
    }
});

/*
  'onclick function for button with attibute 'modal-toggler'
   attempts to fetch data from server (as partial view) and place it onto
   #placeholder div element.
 */
function btnClick(this: HTMLButtonElement) {

    // in case
    if (document.readyState != "complete")
        return;

    var placeholder = document.getElementById("placeholder");

    if (placeholder == undefined) {
        return;
    }

    var url: string = this.getAttribute("data-url");

    // this self.fetch - determines if browser supports Promise.
    // IE does not.
    if (self.fetch) {

        // is this call actualy async?
        getDataWithFetch(url).then(response => {
            tryCreateModal(placeholder, response);
        });
    }
    else {
        // and this?
        getDataWithXmlHttpRequest(url, tryCreateModal, placeholder);
    }
}

/*
 * fetching data from server and returning Promise as text
 * should use credentials: 'include' options in future
 * look for options here https://developer.mozilla.org/ru/docs/Web/API/Fetch_API/Using_Fetch
 */
async function getDataWithFetch(url: string) {

    const response = await fetch(url);

    if (response.ok)
        return await response.text();
}

/*
 This method used for creates xmlHttpRequest for Internet Explorer.
 params:
    url: where to make request
    callback: function which accepts HtmlElement and response to insert into that HtmlElement. This one calls whenever xhr got response
    placeholder: element in which response should be pasted
 */
function getDataWithXmlHttpRequest(url: string, callback: (placeholder: HTMLElement, text: string) => void, placeholder: HTMLElement) {

    var xhr = new XMLHttpRequest();

    xhr.onloadstart = function (e) {
        xhr.responseType = 'json';
    }

    xhr.onload = function (e) {
        callback(placeholder, xhr.responseText);
    }

    xhr.onerror = function (e) {
        console.log(e);
    }

    xhr.open('get', url);
    xhr.send();
}

/*
 Creates modal for provided htmlElement (here I use <div> with #id == placeholder)
 and response as planty HTML for insert into placeholder.
 */
function tryCreateModal(placeholder: HTMLElement, response: string) {

    placeholder.innerHTML = response;

    let div: HTMLDivElement = placeholder.querySelector(".modal");

    let modal = new bootstrap.Modal(div);
    modal.show();
}

