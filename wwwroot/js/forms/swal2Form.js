/**
 * Initializes a sweet alert 2 dropdown with AJAX-driven search capabilities.
 * This function is designed to simplify the setup of sweet alert 2 for scenarios
 * where data needs to be fetched dynamically from a server based on user input.
 *
 * @param {string} selector - The jQuery selector for the HTML element(s) to which sweet alert 2 should be applied.
 * Example: '#myDropdown', '.sweet alert 2 field'.
 * @param {string} title - Title of the alert form selector.
 * @param {Array<object>} inputs - An object containing additional configuration options for the sweet alert 2 instance.
 * These options are directly merged into the sweet alert 2 initialization object,
 * allowing for customization beyond the core AJAX and formatting.
 * Refer to the sweet alert 2 documentation for a comprehensive list of available options:
 * Example: `{ name: 'name', title: "Name", type: "text", required: true, value: "Empty", attributes: [{"key": "Value"}] }`.
 * @param {string} urlSubmit - The URL endpoint for the AJAX request that sweet alert 2 will use to submit results.
 * This URL should typically point to an API endpoint designed to submit JSON or form data.
 * @param {function(object): object} onSubmit - A callback function that takes the result of sweet alert 2 AJAX `response` object
 * Example: `function(res) { console.log(res); }`.
 * @param {function(object): object} onInit - A function that's run before initialing the alert takes click event of the alert as param
 * Example: `function(event) { console.log("Running before alert"); }`.
 * @param {string} placeholder - The text to display as a placeholder when no item is selected in the sweet alert 2 dropdown.
 * Example: 'Search for a user...', 'Select an option'.
 * @param {boolean} useMultiPart - A simple reference which represents the form as json object or form data
 * @throws {Error} If `selector` or `title` or `inputs` are null, empty, or contain only spaces.
 */
function handleSwal2Form(
  selector,
  title,
  inputs,
  urlSubmit,
  onSubmit,
  onInit,
  useMultiPart = false
) {
  if (isEmptyOrSpaces(selector)) throw Error("Selector not recognized");

  if (isEmptyOrSpaces(title)) throw Error("Title is required");

  if (inputs == undefined || inputs == null || inputs.length <= 0)
    throw Error("No inputs provided");

  var r;

  $(selector).on("click", function (event) {
    event.preventDefault();

    if (onInit != undefined && typeof onInit == "function") onInit(event);

    Swal.fire({
      title: title,
      target: $(selector)[0]?.parentNode,
      html: `
            <form id="swal2_form">
                <div class="d-flex flex-column justify-content-start align-items-start gap-5">
                    ${[...inputs]
                      .map((e) =>
                        typeof e == "object"
                          ? generateFormInputHTML(
                              e.name,
                              e.title,
                              e.type,
                              e.required,
                              e.value,
                              e.attributes
                            )
                          : e
                      )
                      .join("")}
                </div>
            </form>
        `,
      customClass: {
        title: "pt-0 pb-4",
        htmlContainer: "m-0 p-0 mh-600px min-h-fit",
        container: "m-0 p-5 min-w-sm-500px",
      },
      showCancelButton: true,
      showLoaderOnConfirm: true,
      preConfirm: async function (value) {
        if (!$("#swal2_form")[0].reportValidity()) {
          Swal.showValidationMessage("You need to write something!");
          return;
        }
        try {
          return await $.ajax({
            url: urlSubmit,
            method: "POST",
            xhrFields: {
              withCredentials: true,
            },
            processData: !useMultiPart,
            contentType: !useMultiPart ? "application/json" : false,
            headers: {
              Accept: "application/json",
            },
            data: !useMultiPart
              ? formDataToJson("#swal2_form")
              : new FormData(document.getElementById("swal2_form")),
            complete: (res) => {
              if (res.status != 200)
                Swal.showValidationMessage(
                  `Request failed: ${
                    res.responseJSON?.message ?? "Error occur"
                  }`
                );

              r = res.responseJSON;
              return res.responseJSON;
            },
          });
        } catch (error) {
          return;
        }
      },
      allowOutsideClick: () => !Swal.isLoading(),
    }).then(function (result) {
      if (result.isConfirmed) {
        Swal.fire({
          text: "Success",
          icon: "success",
          buttonsStyling: false,
          confirmButtonText: "Ok, got it!",
          customClass: {
            confirmButton: "btn fw-bold btn-primary",
          },
        }).then(function (res) {
          onSubmit(r);
        });
      }
    });
  });
}

function generateFormInputHTML(
  name,
  title,
  type = "text",
  required = false,
  value = "",
  attributes = []
) {
  if (isEmptyOrSpaces(name)) throw Error("Input name attribute is required");

  if (type != "hidden" && isEmptyOrSpaces(title))
    throw Error("Title name attribute is required");

  return `
        <div class="form-group text-start p-0 w-100">
            <label for="${name}" class="form-label text-start 
                ${required && "required"}">
                ${title}
            </label>
            <input id="${name}" type="${type}" name="${name}" ${
    required ? "required" : ""
  } placeholder="${title}" ${attributes.join(
    " "
  )} value="${value}" step="any" class="form-control">
        </div>   
    `;
}

function handleSwal2FormEdit(
  table,
  innerSelector,
  title,
  inputs,
  urlObj,
  onSubmit,
  onInit,
  useMultiPart = false
) {
  if (isEmptyOrSpaces(table)) throw Error("Table not recognized");

  if (isEmptyOrSpaces(innerSelector))
    throw Error("Inner selector not recognized");

  if (isEmptyOrSpaces(title)) throw Error("Title is required");

  if (inputs == undefined || inputs == null || inputs.length <= 0)
    throw Error("No inputs provided");

  $(table).on("click", innerSelector, function (e) {
    e.preventDefault();
    // Select parent row
    const parent = e.target.closest("tr");
    // Get area name
    const eleId = $(e.target).attr("item-id");
    const row = parent.querySelectorAll("td");

    if (onInit != undefined && typeof onInit == "function") onInit(row);

    Swal.fire({
      title: title,
      html: `
            <form id="swal2_update_form">
                <div class="d-flex flex-column justify-content-start align-items-start gap-5">
                    <input type="hidden" name="id" value="${eleId}" />
                    ${[...inputs]
                      .map((t) =>
                        typeof t == "object"
                          ? generateFormInputHTML(
                              t.name,
                              t.title,
                              t.type,
                              t.required,
                              row[t.sequence]?.innerText,
                              t.attributes
                            )
                          : t
                      )
                      .join("")}
                </div>
            </form>
      `,
      showCancelButton: true,
      customClass: {
        title: "pt-0 pb-4",
        htmlContainer: "m-0 p-0 mh-600px min-h-fit",
        container: "m-0 p-5 min-w-sm-500px",
      },
      cancelButtonText: "cancel",
      showLoaderOnConfirm: true,
      preConfirm: async function (value) {
        if (!$("#swal2_update_form")[0].reportValidity()) {
          Swal.showValidationMessage("You need to write something!");
          return;
        }

        try {
          await $.ajax({
            url: `${urlObj.url}${urlObj.appendId ? `/${eleId}` : ""}`,
            method: "PUT",
            processData: !useMultiPart,
            contentType: !useMultiPart ? "application/json" : false,
            headers: {
              Accept: "application/json",
            },
            data: !useMultiPart
              ? formDataToJson("#swal2_update_form")
              : new FormData(document.getElementById("swal2_update_form")),
            success: function (event) {
              Swal.fire({
                text: "Success",
                icon: "success",
                buttonsStyling: false,
                confirmButtonText: "Ok, got it!",
                customClass: {
                  confirmButton: "btn fw-bold btn-primary",
                },
              }).then(function () {
                onSubmit();
              });
              return;
            },
            error: function (error) {
              return Swal.showValidationMessage(
                `Request failed: ${
                  error.responseJSON?.errors
                    ? Object.values(error.responseJSON?.errors).join(" ")
                    : "please try again!"
                }`
              );
            },
          }).then((res) => res.responseJSON);
        } catch (err) {
          return;
        }
      },
      allowOutsideClick: () => !Swal.isLoading(),
    }).then(function (result) {
      //console.log(result)
      $("#areas-table").DataTable().draw();
    });
  });
}

/**
 * Simple reusable sweet alert 2 function represents a success alert
 * @param {string} message - Simple message to appear for the success alert
 * @returns SweetAlert2 Object
 */
function releaseSuccessAlert(message = "") {
  return Swal.fire({
    icon: "success",
    title: "success",
    message: message,
  });
}

/**
 * Simple reusable sweet alert 2 function represents a error alert
 * @param {string} message - Simple message to appear for the error alert
 * @returns SweetAlert2 Object
 */
function releaseErrorAlert(message = "") {
  return Swal.fire({
    icon: "error",
    title: "Error",
    message: message,
  });
}
