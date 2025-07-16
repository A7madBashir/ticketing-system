/**
 * datatable-initializer.js
 *
 * Provides a utility function to initialize jQuery DataTables with flexible options.
 */
class DataTableSSR {
  url = "";
  columns = [];
  columnDefs = [];
  #dt;

  /**
    * @param {Array<object>} columns - An array of column definitions for DataTables.
    Each object in the array corresponds to a column.
    Common properties:
    - `data`: The data property name from your data source (e.g., 'id', 'name').
    Can be a function for complex data access.
    - `title`: The header title for the column.
    - `name`: A unique name for the column (useful for server-side processing).
    - `visible`: (boolean) Whether the column is visible.
    - `orderable`: (boolean) Whether the column can be ordered.
    - `searchable`: (boolean) Whether the column can be searched.
    - `className`: CSS classes to apply to the cells in this column.
    - `render`: (function) A function to customize cell rendering.
    `function(data, type, row, meta) { return '...' }`
    Use this for 'column differences' where the data needs transformation
    or is not a direct property.
    and should modify `d` to add custom parameters.
    * @param {string} url - specify the endpoint of data table
    * @param {Array<object>} columnDefs - An array of column definitions for DataTables.
    Each object in the array corresponds to a column.
    Common properties:
    - `class`: CSS classes to apply to the cells in this column.
    - `targets`: function accepts (data,row,ful) and responsible to represents the data of the column
    by user modification.
   */
  constructor(url, columns, columnDefs) {
    this.url = url;
    this.columns = columns;
    this.columnDefs = columnDefs;
  }

  getDt() {
    return this.#dt;
  }

  getUrl() {
    return this.url;
  }

  reDraw() {
    if (this.#dt == null) throw Error("Data table is not defined");

    this.#dt.draw();
  }

  /**
   * Initializes a jQuery DataTables instance.
   *
   * @param {object} options - Configuration options for the DataTable.
   * @param {object} options.language - Configure elements custom text as language
   * `language`: {
          `processing`: "Loading users...",
          `search`: "Quick Search:",
          `lengthMenu`: "Show _MENU_ users per page",
          `info`: "Showing _START_ to _END_ of _TOTAL_ users",
          `infoEmpty`: "No users available",
          `infoFiltered`: "(filtered from _MAX_ total users)",
          `zeroRecords`: "No matching users found",
          `paginate`: {
              `first`: "First",
              `last`: "Last",
              `next`: "Next",
              `previous`: "Previous"
          },
          `buttons`: {
              `excel`: 'Export to Excel',
              `csv`: 'Export to CSV',
              `print`: 'Print Table',
              `copy`: 'Copy Table'
          }
      }
   * @param {Array<object>} options.buttons - Configure data table custom buttons with action handler
   * buttons: 
   * [
        {
            `text`: '<i class="bi bi-plus-circle"></i> Add New User',
            `className`: 'btn btn-success me-2',
            `action`: function (e, dt, node, config) {
                window.location.href = '/Users/Create'; // Navigate to a Create User page
            }
        },
        'excelHtml5',
        'csvHtml5',
        'print',
        {
            `extend`: 'copyHtml5',
            `text`: 'Copy',
            `exportOptions`: {
                `columns`: [0, 1, 2, 3, 4, 5] // Copy only specific columns (exclude actions)
            }
        }
    ],
   * @param {string} selector - The jQuery selector for the HTML table element (e.g., '#myTable', '.data-table').
   * @param {Array<Function>} [events] - Optional. An object where keys are DataTables event names (e.g., 'init', 'draw', 'click')
   * and values are callback functions. The functions will be bound to the DataTable instance.
   * Example: `[{ event:'init.dt', action: function() { console.log('DataTable initialized!'); }, { event: 'click', action: function(e) { console.log('Click event:', e); }} ]`
   * @returns {jQuery} The initialized DataTables API instance.
   */
  initDataTableObj(selector, options, initComplete, events) {
    if (!selector || isEmptyOrSpaces(selector))
      throw Error("HTML selector not recognized");

    if (window.DataTable == undefined)
      throw Error("Datatable is undefined, check existent");

    this.#dt = $(selector).DataTable({
      searchDelay: 500,
      responsive: true,
      processing: true,
      language: {
        zeroRecords: "no data exist",
      },
      serverSide: true,
      ordering: false,
      stateSave: true,
      paging: true,
      select: false,
      ...options,
      ajax: {
        url: this.url,
        data: (q) => {
          q.start = q.start / 10;
          Object.assign(q, this.#dataTableQueryParams(q));
        },
      },
      columns: this.columns,
      columnDefs: this.columnDefs,
      initComplete: initComplete,
    });

    if (events && events.length > 0) {
      [...events].forEach((e) => {
        this.#dt.on(e.event, e.action);
      });
    }
  }

  customSearchDataTable(selector) {
    if (!selector || isEmptyOrSpaces(selector))
      throw Error("HTML selector not recognized");

    if (this.#dt == undefined || this.#dt == null)
      throw Error("DataTable is not defined");

    const filterSearch = document.querySelector(selector);
    filterSearch.addEventListener("keyup", (e) => {
      this.#dt.search(e.target.value).draw();
    });
  }

  customFilterDataTable(selector) {
    if (!selector || isEmptyOrSpaces(selector))
      throw Error("HTML selector not recognized");

    if (this.#dt == undefined || this.#dt == null)
      throw Error("DataTable is not defined");

    const filter = document.querySelector(selector);

    filter.on("change", (e) => {
      this.#dt.draw();
    });
  }

  deleteRowsBtnEvents(btnSelectors, customLink = null, sequence = 0) {
    if (!btnSelectors || isEmptyOrSpaces(btnSelectors))
      throw Error("HTML selector not recognized");

    if (this.#dt == undefined || this.#dt == null)
      throw Error("Data table is not defined");

    const deleteButtons = document.querySelectorAll(btnSelectors);

    deleteButtons.forEach((d) => {
      d.addEventListener("click", (e) => {
        e.preventDefault();

        let elementId = d.id;
        // Select parent row
        const parent = e.target.closest("tr");

        // Get element name
        const elementName = parent.querySelectorAll("td")[sequence].innerText;

        // SweetAlert2 pop up --- official docs reference: https://sweetalert2.github.io/
        Swal.fire({
          text: "Are you sure delete " + elementName + " ?",
          icon: "warning",
          showCancelButton: true,
          buttonsStyling: false,
          confirmButtonText: "Yes Delete",
          cancelButtonText: "No Cancel",
          customClass: {
            confirmButton: "btn fw-bold btn-danger",
            cancelButton: "btn fw-bold btn-active-light-primary",
          },
        }).then((result) => {
          if (result.value) {
            // Simulate delete request -- for demo purpose only
            Swal.fire({
              text: "Deleting " + elementName,
              icon: "info",
              buttonsStyling: false,
              showConfirmButton: false,
              timer: 2000,
            }).then(() => {
              $.ajax({
                url: isEmptyOrSpaces(customLink)
                  ? `${this.url}/${elementId}`
                  : `${customLink}/${elementId}`,
                type: "Delete",
                headers: {
                  RequestVerificationToken: $(
                    "[name=__RequestVerificationToken]"
                  ).val(),
                },
                complete: (result) => {
                  let err = result.responseJSON?.message || "";
                  //Add Sweet Alert
                  if (result.status == 200) {
                    Swal.fire({
                      title: "You deleted " + elementName + "!.",
                      text: err,
                      icon: "success",
                      buttonsStyling: false,
                      confirmButtonText: "Ok",
                      customClass: {
                        confirmButton: "btn fw-bold btn-primary",
                      },
                    }).then(() => {
                      // delete row data from server and re-draw datatable
                      this.#dt.draw();
                    });
                  } else {
                    Swal.fire({
                      title: "Failed to delete " + elementName + "!.",
                      text: err,
                      icon: "error",
                      buttonsStyling: false,
                      confirmButtonText: "Ok",
                      customClass: {
                        confirmButton: "btn fw-bold btn-primary",
                      },
                    }).then(() => {
                      // delete row data from server and re-draw datatable
                      this.#dt.draw();
                    });
                  }
                },
              });
            });
          }
        });
      });
    });
  }

  initialTooltip() {
    var tooltipTriggerList = [].slice.call(
      document.querySelectorAll('[data-bs-toggle="tooltip"]')
    );
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
      return new bootstrap.Tooltip(tooltipTriggerEl);
    });
  }

  generateEditLinkDataTableRow(link, element = null, attributes = null) {
    if (isEmptyOrSpaces(link)) throw Error("Link is not provided");

    return `
      <a href="${link}" title="Edit" data-kt-docs-table-filter='edit_row' class="btn btn-sm btn-light-primary cursor-pointer px-3" data-bs-toggle="tooltip"
        ${attributes && [...attributes].join(" ")}
      >
      ${isEmptyOrSpaces(element) ? "Edit" : element}
    </a>
    `;
  }

  generateDeleteLinkDataTableRow(id, element = null, attributes = null) {
    if (isEmptyOrSpaces(id)) throw Error("Id is not provided");

    return `
      <a title="Delete" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-boundary="window"
      id="${id}" class="btn btn-sm btn-light-danger cursor-pointer px-3" 
        data-kt-docs-table-filter="delete_row" 
        ${attributes && [...attributes].join(" ")}
      >
        ${isEmptyOrSpaces(element) ? "Delete" : element}
      </a>
    `;
  }

  generateDetailsLinkDataTableRow(link, element = null, attributes = null) {
    if (isEmptyOrSpaces(link)) throw Error("Link is not provided");

    return `
      <a href="${link}" title="Details" class="btn btn-sm btn-light-info cursor-pointer px-3" data-bs-toggle="tooltip"
        ${attributes && [...attributes].join(" ")}
      >
      ${isEmptyOrSpaces(element) ? "Details" : element}
    </a>
    `;
  }

  generateUsersLinkDataTableRow(link, element = null, attributes = null) {
    if (isEmptyOrSpaces(link)) throw Error("Link is not provided");

    return `
      <a href="${link}" title="Details" class="btn btn-sm btn-light-secondary cursor-pointer px-3" data-bs-toggle="tooltip"
        ${attributes && [...attributes].join(" ")}
      >
      ${isEmptyOrSpaces(element) ? "Users" : element}
    </a>
    `;
  }

  generateCustomLinkDataTableRow(
    link,
    element = null,
    attributes = null,
    customClass = null
  ) {
    if (isEmptyOrSpaces(link)) throw Error("Link is not provided");
    if (isEmptyOrSpaces(element)) throw Error("Element is not provided");

    return `
    <a href="${link}" class="menu-link cursor-pointer px-3 ${
      customClass || ""
    }" ${attributes && [...attributes].join(" ")} data-bs-toggle="tooltip"
    >
    ${element}
  </a>
  `;
  }

  customizeDataTableQueryParams() {
    return {};
  }

  #dataTableQueryParams(q) {
    let userParams = this.customizeDataTableQueryParams();

    if (userParams != undefined) Object.assign(q, userParams);
  }
}
