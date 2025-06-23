/**
 * Reusable Form Submission Handler for Metronic-themed forms.
 * Handles pure HTML5 form validation, AJAX submission, Metronic loading indicators,
 * and displaying success/error messages using SweetAlert2.
 *
 * Requires Metronic's global JS bundles (plugins.bundle.js, scripts.bundle.js)
 * for indicator functionality.
 * Requires SweetAlert2 for beautiful alerts.
 */
class FormSubmitHandler {
  constructor(options) {
    // Default options
    this.options = {
      formSelector: null, // Required: CSS selector for the form (e.g., '#myFormId')
      method: null, // method of the form generally post method
      submitButtonSelector: null, // Required: CSS selector for the submit button
      apiEndpoint: null, // Required: URL for API submission (e.g., '/api/users')
      redirectUrl: null, // Optional: URL to redirect to on successful submission
      clearFormOnSuccess: true, // Optional: Whether to clear form fields after successful submission
      // Callbacks
      onBeforeSubmit: () => true, // Optional: Callback before submission. Return false to prevent submission.
      onSuccess: (responseData) => {
        console.log("Form submitted successfully:", responseData);
      }, // Optional: Callback on success
      onError: (errorData) => {
        console.error("Form submission error:", errorData);
      }, // Optional: Callback on error
      formDataType: "json", // Optional: 'json' or 'formData'. How to send data.
    };

    // Merge user-provided options
    Object.assign(this.options, options);

    // Ensure required options are provided
    if (
      !this.options.formSelector ||
      !this.options.submitButtonSelector ||
      !this.options.apiEndpoint
    ) {
      console.error(
        "FormSubmitHandler: Missing required options (formSelector, submitButtonSelector, apiEndpoint)."
      );
      return;
    }

    this.form = document.querySelector(this.options.formSelector);
    this.submitButton = document.querySelector(
      this.options.submitButtonSelector
    );

    if (!this.form) {
      console.error(
        `FormSubmitHandler: Form not found with selector: ${this.options.formSelector}`
      );
      return;
    }
    if (!this.submitButton) {
      console.error(
        `FormSubmitHandler: Submit button not found with selector: ${this.options.submitButtonSelector}`
      );
      return;
    }

    // Check if SweetAlert2 is available
    if (typeof Swal === "undefined") {
      console.error(
        "FormSubmitHandler: SweetAlert2 library (Swal) not found. Please include it."
      );
      return;
    }

    this.init();
  }

  init() {
    this.form.addEventListener("submit", this.handleSubmit.bind(this));
  }

  reInit() {
    Object.assign(this.options,new FormSubmitHandler(this.options));
  }

  handleSubmit(event) {
    event.preventDefault(); // Prevent default form submission

    // Execute onBeforeSubmit callback
    if (
      typeof this.options.onBeforeSubmit === "function" &&
      !this.options.onBeforeSubmit()
    ) {
      console.log("Form submission prevented by onBeforeSubmit callback.");
      return;
    }

    this.showLoadingIndicator();

    // Use pure HTML5 form validation
    if (!this.form.checkValidity()) {
      this.form.reportValidity(); // This shows the native browser validation messages
      this.hideLoadingIndicator();
      // No SweetAlert2 here for client-side validation errors, as browser messages are immediate.
      return;
    }

    this.submitForm();
  }

  submitForm() {
    const url = this.options.apiEndpoint;
    const method = this.options.method || "POST"; // Default to POST if not specified
    let body;
    let headers = {};

    if (this.options.formDataType === "json") {
      const formData = new FormData(this.form);
      const data = {};
      // Convert FormData to plain object for JSON serialization
      formData.forEach((value, key) => {
        // This simple conversion works for flat forms.
        // For nested objects or arrays, more complex logic is needed.
        data[key] = value;
      });
      body = JSON.stringify(data);
      headers["Content-Type"] = "application/json";
    } else {
      // formData
      body = new FormData(this.form);
      // No Content-Type header needed for FormData; browser sets it correctly with boundary
    }

    fetch(url, {
      method: method,
      headers: headers,
      body: body,
    })
      .then((response) => {
        if (!response.ok) {
          // If response is not OK (e.g., 400, 500), try to read error details
          return response.json().then((err) => Promise.reject(err)); // Reject with error object
        }
        // Always try to parse JSON, even if response is empty (e.g., 204 No Content)
        return response.json().catch(() => ({})); // Return empty object if no JSON to parse
      })
      .then((data) => {
        this.hideLoadingIndicator();
        this.displaySuccessAlert(
          "Operation completed successfully!",
          "Success!"
        );

        if (this.options.clearFormOnSuccess) {
          this.form.reset();
        }
        if (typeof this.options.onSuccess === "function") {
          this.options.onSuccess(data);
        }
        if (this.options.redirectUrl) {
          setTimeout(() => {
            window.location.href = this.options.redirectUrl;
          }, 1500); // Small delay for message
        }
      })
      .catch((error) => {
        this.hideLoadingIndicator();
        let errorMessage = "An unexpected error occurred.";
        let errorTitle = "Error!";

        if (typeof error === "object" && error !== null) {
          // Common error response patterns from ASP.NET Core APIs (ProblemDetails or custom)
          if (error.title) errorTitle = error.title;
          if (error.detail) errorMessage = error.detail;
          else if (error.errors) {
            // For validation errors from .NET Core
            errorMessage = Object.values(error.errors).flat().join("<br/>");
            if (!errorTitle || errorTitle === "Error!")
              errorTitle = "Validation Errors!";
          } else if (error.message)
            errorMessage = error.message; // Generic message
          else if (error.status)
            errorMessage = `Server responded with status: ${error.status}`; // For non-JSON errors
        } else if (typeof error === "string") {
          errorMessage = error;
        }
        console.log(error);
        this.displayErrorAlert(errorMessage, errorTitle);
        if (typeof this.options.onError === "function") {
          this.options.onError(error);
        }
      });
  }

  showLoadingIndicator() {
    this.submitButton.setAttribute("data-kt-indicator", "on");
    this.submitButton.disabled = true;
  }

  hideLoadingIndicator() {
    this.submitButton.removeAttribute("data-kt-indicator");
    this.submitButton.disabled = false;
  }

  /**
   * Displays a SweetAlert2 success message.
   * @param {string} text - The main message text.
   * @param {string} title - The title of the alert.
   */
  displaySuccessAlert(text, title = "Success!") {
    Swal.fire({
      icon: "success",
      title: title,
      text: text,
      showConfirmButton: false,
      timer: 2500, // Auto-close after 2.5 seconds
    });
  }

  /**
   * Displays a SweetAlert2 error message.
   * @param {string} text - The main message text. Can contain HTML (e.g., <br/>).
   * @param {string} title - The title of the alert.
   */
  displayErrorAlert(text, title = "Error!") {
    Swal.fire({
      icon: "error",
      title: title,
      html: text, // Use html to render <br/> or other HTML tags
      confirmButtonText: "Ok",
    });
  }
}
