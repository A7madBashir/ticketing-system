/**
 * Initializes a Select2 dropdown with AJAX-driven search capabilities.
 * This function is designed to simplify the setup of Select2 for scenarios
 * where data needs to be fetched dynamically from a server based on user input.
 *
 * @param {string} selector - The jQuery selector for the HTML <select> element(s) to which Select2 should be applied.
 * Example: '#myDropdown', '.select2-field'.
 * @param {object} options - An object containing additional configuration options for the Select2 instance.
 * These options are directly merged into the Select2 initialization object,
 * allowing for customization beyond the core AJAX and formatting.
 * Refer to the Select2 documentation for a comprehensive list of available options:
 * https://select2.org/configuration/options-settings
 * Example: `{ width: 'resolve', allowClear: true }`.
 * @param {string} url - The URL endpoint for the AJAX request that Select2 will use to fetch search results.
 * This URL should typically point to an API endpoint designed to return JSON data.
 * @param {function(object): object} queryPredicate - A function that takes the default Select2 AJAX `params` object
 * (containing `term`, `page`, etc.) and returns a modified object
 * of parameters to be sent to the server. This is where you can
 * add custom search parameters (e.g., filters, additional criteria)
 * that your backend API expects.
 * Example: `function(params) { return { search: params.term, type: 'users', page: params.page }; }`.
 * @param {function(object): object} processResults - A function that takes the raw AJAX response data from your server
 * and transforms it into the format expected by Select2 for its results.
 * It must return an object with a `results` array, and optionally
 * a `pagination` object with an `more` boolean.
 * Example: `function(data) { return { results: data.items.map(item => ({ id: item.id, text: item.name })), pagination: { more: data.has_more } }; }`.
 * @param {string} placeholder - The text to display as a placeholder when no item is selected in the Select2 dropdown.
 * Example: 'Search for a user...', 'Select an option'.
 * @param {function(object): string} [customFormat] - Optional. A function that takes a data object (representing a single result item)
 * and returns the HTML string or plain text to be displayed for each
 * result in the dropdown list. If not provided, the default `formatResult`
 * function (which checks for `data.name` or `data.text`) will be used.
 * This is useful for complex custom rendering of search results.
 * Example: `function(repo) { if (repo.loading) return repo.text; var markup = "<div class='select2-result-repository__title'>" + repo.full_name + "</div>"; return markup; }`.
 * @throws {Error} If `selector` or `url` are null, empty, or contain only spaces.
 */
function select2AjaxSearchHandleListener(
  selector,
  options,
  url,
  queryPredicate,
  processResults,
  placeholder,
  customFormat
) {
  // Assuming isEmptyOrSpaces and $ are defined elsewhere or imported
  if (isEmptyOrSpaces(selector)) throw Error("Selector not recognized");
  if (isEmptyOrSpaces(url)) throw Error("Url not recognized");

  $(selector).select2({
    ...options, // Spread additional custom options here
    ajax: {
      url: url,
      data: queryPredicate, // `queryPredicate` transforms default params to custom ones
      dataType: "json",
      processResults: processResults, // `processResults` transforms server response to Select2 format
      cache: true,
    },
    templateResult: customFormat ?? formatResult, // Use customFormat if provided, else default
    templateSelection: formatSelection, // Default selection format
    placeholder: placeholder,
    minimumInputLength: 0, // Allow search with no input (e.g., to show all on focus)
  });
}

/**
 * Default function to format an individual search result item for display in the dropdown list.
 * It attempts to display the 'name' property first, then falls back to 'text'.
 *
 * @param {object} data - The data object for a single result item returned from the server.
 * @returns {string} The text or HTML string to be displayed for the result item.
 */
function formatResult(data) {
  return data.name || data.text;
}

/**
 * Default function to format the selected item for display in the Select2 input field.
 * It attempts to display 'name', then 'text', then 'id' of the selected item.
 *
 * @param {object} data - The data object for the currently selected item.
 * @returns {string} The text or HTML string to be displayed for the selected item.
 */
function formatSelection(data) {
  return data.name || data.text || data.id;
}

// Dummy isEmptyOrSpaces function for completeness, assuming it exists elsewhere
function isEmptyOrSpaces(str) {
  return str === null || str.match(/^ *$/) !== null;
}
