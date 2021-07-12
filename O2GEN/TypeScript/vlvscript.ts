////// for avoid .js file to auto wrap __awaiter and __generator 
////// need to target es2017(in tsconfig.json) ?
////// all comments in here would not come into .js file

/////*
//// * this function gets callback as a parameter and firing it whenever page is loaded
//// */
////let ready = (action) => {
////    if (document.readyState != "loading")
////        action();
////    else
////        document.addEventListener("DOMContentLoaded", action);
////    console.log("ready created");
////}

/////*
//// * callback for page loaded. For now this function adds event 'onclick' 
//// * for every button with specified attribute
//// */
////ready(() => {
////    let elems = document.querySelectorAll('[data-toggle="modal-toggler"]')

////    for (var i = 0; i < elems.length; i++) {
////        console.log("buttons founded: " + i)
////        elems[i].addEventListener('click', btnClick);
////    }
////});

/////*
////  'onclick function for button with attibute 'modal-toggler'
////   attempts to fetch data from server (as partial view) and place it onto
////   #placeholder div element.
//// */
////function btnClick(this: HTMLButtonElement) {

////    // in case
////    if (document.readyState != "complete")
////        return;

////    var placeholder = document.getElementById("placeholder");

////    if (placeholder == undefined) {
////        return;
////    }

////    var url: string = this.getAttribute("data-url");

////    // this self.fetch - determines if browser supports Promise.
////    // IE does not.
////    if (self.fetch) {
        
////        // is this call actualy async?
////        getDataWithFetch(url).then(response => {
////            tryCreateModal(placeholder, response);
////        });
////    }
////    else {
////        // and this?
////        getDataWithXmlHttpRequest(url, tryCreateModal, placeholder);
////    }
////}

/////*
//// * fetching data from server and returning Promise as text
//// * should use credentials: 'include' options in future
//// * look for options here https://developer.mozilla.org/ru/docs/Web/API/Fetch_API/Using_Fetch
//// */
////async function getDataWithFetch(url: string) {

////    const response = await fetch(url);

////    if (response.ok)
////        return await response.text();
////}

/////*
//// This method used for creates xmlHttpRequest for Internet Explorer.
//// params:
////    url: where to make request
////    callback: function which accepts HtmlElement and response to insert into that HtmlElement. This one calls whenever xhr got response
////    placeholder: element in which response should be pasted
//// */
////function getDataWithXmlHttpRequest(url: string, callback: (placeholder: HTMLElement, text: string) => void, placeholder: HTMLElement) {

////    var xhr = new XMLHttpRequest();

////    xhr.onloadstart = function (e) {
////        xhr.responseType = 'json';
////    }

////    xhr.onload = function (e) {
////        callback(placeholder, xhr.responseText);
////    }

////    xhr.onerror = function (e) {
////        console.log(e);
////    }

////    xhr.open('get', url);
////    xhr.send();
////}

/////*
//// Creates modal for provided htmlElement (here I use <div> with #id == placeholder)
//// and response as planty HTML for insert into placeholder.
//// */
////function tryCreateModal(placeholder: HTMLElement, response: string) {

////    placeholder.innerHTML = response;

////    let div: HTMLDivElement = placeholder.querySelector(".modal");

////    let modal = new bootstrap.Modal(div);
////    modal.show();
////}
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
