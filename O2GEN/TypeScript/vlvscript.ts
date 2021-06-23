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
}

/*
 * callback for page loaded. For now this function adds event 'onclick' 
 * for every button with specified attribute
 */
ready(() => {
    let elems = document.querySelectorAll('[data-toggle="modal-toggler"]')

    for (var i = 0; i < elems.length; i++) {
        elems[i].addEventListener('click', btnClick);
    }
});

/*
 * 'onclick function for button with attibute 'modal-toggler'
 * attempts to fetch data from server (as partial view) and place it onto
 * #placeholder div element.
 */
function btnClick(this: HTMLButtonElement) {

    // in case
    if (document.readyState != "complete")
        return;

    var placeholder = document.getElementById("placeholder");

    if (placeholder == undefined) {
        console.log("could not find element with id=placeholder");
        return;
    }

    var url: string = this.getAttribute("data-url");

    getData(url)
        .then(response => {

            if (response == null)
                return;

            //console.log(response);
            placeholder.innerHTML = response;

            let div: HTMLDivElement = placeholder.querySelector(".modal");
            let modal = new bootstrap.Modal(div);
            modal.show();
        })
        .catch(error => {
            console.log(error);
        })
}

/*
 * fetching data from server and returning Promise as text
 * should use credentials: 'include' options in future
 * look for options here https://developer.mozilla.org/ru/docs/Web/API/Fetch_API/Using_Fetch
 */
async function getData(url: string) {

    // helper to determine if fetch request is allowed in current browser
    if (self.fetch) {

        // here we also can get error 
        // so possible wrap with try/catch
        // also have no support in IE
        const response = await fetch(url);

        if (response.ok)
            return await response.text();
    }
    else {
        // for IE
        var xhr = new XMLHttpRequest();

        xhr.onload = function (e) {
            return this.responseText;
        }

        xhr.onerror = function (e) {
            console.log(e);
        }

        xhr.open('get', url);
        xhr.send();
    }

    return null;
}

