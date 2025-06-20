// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

﻿// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Block ui handler
var block_ui_target_element = document.querySelector("body .page");
var blockUI = new KTBlockUI(block_ui_target_element, {
  message: `<div class="blockui-message" style="z-index:9999"><span class="spinner-border text-primary"></span> Loading...</div>`,
});
function toggleBlockUI() {
  blockUI.isBlocked() ? blockUI.release() : blockUI.block();
}

function isEmptyOrSpaces(str) {
  return str === undefined || str === null || str.trim() === "";
}

function formatDateTime(dateTime) {
  if (isEmptyOrSpaces(dateTime)) return "";

  return moment(dateTime).format("YYYY-MM-DD HH:MM A").toLocaleString();
}

function successSwal2(title, message, cb = () => {}, options) {
  Swal.fire({
    title: title,
    text: message,
    icon: "success",
    buttonsStyling: false,
    ...options,
    confirmButtonText: "Ok",
    customClass: {
      confirmButton: "btn fw-bold btn-primary",
      cancelButton: "btn fw-bold btn-danger",
    },
  }).then((result) => result.isConfirmed && cb());
}

function infoSwal2(title, message, cb = () => {}, options) {
  Swal.fire({
    title: title,
    text: message,
    icon: "info",
    buttonsStyling: false,
    ...options,
    confirmButtonText: "Ok",
    customClass: {
      confirmButton: "btn fw-bold btn-primary",
      cancelButton: "btn fw-bold btn-danger",
    },
  }).then((result) => result.isConfirmed && cb());
}

function errorSwal2(title, message, cb = () => {}, options) {
  Swal.fire({
    title: title,
    text: message,
    icon: "error",
    buttonsStyling: false,
    ...options,
    confirmButtonText: "Ok",
    customClass: {
      confirmButton: "btn fw-bold btn-primary",
      cancelButton: "btn fw-bold btn-danger",
    },
  }).then((result) => result.isConfirmed && cb());
}

var formDataToJson = (formSelector) => {
  var object = {};
  $(formSelector)
    .serializeArray()
    .forEach((o) => {
      object[o.name] = $(`${formSelector} [name='${o.name}']`).val();
    });
  return JSON.stringify(object);
};

var formDataToJsonWithNullValues = (formSelector) => {
  var object = {};
  $(formSelector)
    .serializeArray()
    .forEach((o) => {
      let v = $(`${formSelector} [name='${o.name}']`).val();
      object[o.name] = isEmptyOrSpaces(v) ? null : v;
    });
  return JSON.stringify(object);
};
