class FormSubmitHandler {
  constructor(options) {
    this.options = {
      formSelector: null,
      method: null,
      submitButtonSelector: null,
      apiEndpoint: null,
      redirectUrl: null,
      clearFormOnSuccess: true,
      onBeforeSubmit: (form) => true,
      onSuccess: (responseData) => {
        console.log("Form submitted successfully:", responseData);
      },
      onError: (errorData) => {
        console.error("Form submission error:", errorData);
      },
      beforeSubmitData: null,
      formDataType: "json",
    };

    Object.assign(this.options, options);

    if (
      !this.options.formSelector ||
      !this.options.submitButtonSelector ||
      !this.options.apiEndpoint
    ) {
      console.error("FormSubmitHandler: Missing required options.");
      return;
    }

    this.form = document.querySelector(this.options.formSelector);
    this.submitButton = document.querySelector(this.options.submitButtonSelector);

    if (!this.form || !this.submitButton) {
      console.error("FormSubmitHandler: Form or submit button not found.");
      return;
    }

    if (typeof Swal === "undefined") {
      console.error("FormSubmitHandler: SweetAlert2 library (Swal) not found.");
      return;
    }

    this.init();
  }

  init() {
    this.form.addEventListener("submit", this.handleSubmit.bind(this));
  }

  handleSubmit(event) {
    event.preventDefault();

    if (
      typeof this.options.onBeforeSubmit === "function" &&
      !this.options.onBeforeSubmit(this.form)
    ) {
      console.log("Form submission prevented by onBeforeSubmit callback.");
      return;
    }

    this.showLoadingIndicator();

    if (!this.form.checkValidity()) {
      this.form.reportValidity();
      this.hideLoadingIndicator();
      return;
    }

    this.submitForm();
  }

  submitForm() {
    const url = this.options.apiEndpoint;
    const method = this.options.method || "POST";
    let body;
    let headers = {};

    const formData = new FormData(this.form);
    let data = {};
    formData.forEach((value, key) => {
      data[key] = value;
    });

    if (typeof this.options.beforeSubmitData === "function") {
      const modified = this.options.beforeSubmitData(data);
      if (modified && typeof modified === "object") {
        data = modified;
      }
    }

    // Step 3: Build request body
    if (this.options.formDataType === "json") {
      body = JSON.stringify(data);
      headers["Content-Type"] = "application/json";
    } else {
      const finalFormData = new FormData();
      for (const key in data) {
        finalFormData.append(key, data[key]);
      }
      body = finalFormData;
    }

    // Step 4: Submit
    fetch(url, {
      method: method,
      headers: headers,
      body: body,
    })
      .then((response) => {
        if (!response.ok) {
          return response.json().then((err) => Promise.reject(err));
        }
        return response.json().catch(() => ({}));
      })
      .then((data) => {
        this.hideLoadingIndicator();
        this.displaySuccessAlert("Operation completed successfully!", "Success!");

        if (this.options.clearFormOnSuccess) {
          this.form.reset();
        }

        if (typeof this.options.onSuccess === "function") {
          this.options.onSuccess(data);
        }

        if (this.options.redirectUrl) {
          setTimeout(() => {
            window.location.href = this.options.redirectUrl;
          }, 1500);
        }
      })
      .catch((error) => {
        this.hideLoadingIndicator();
        let errorMessage = "An unexpected error occurred.";
        let errorTitle = "Error!";

        if (typeof error === "object" && error !== null) {
          if (error.title) errorTitle = error.title;
          if (error.detail) errorMessage = error.detail;
          else if (error.errors) {
            errorMessage = Object.values(error.errors).flat().join("<br/>");
            if (!errorTitle || errorTitle === "Error!") errorTitle = "Validation Errors!";
          } else if (error.message) {
            errorMessage = error.message;
          } else if (error.status) {
            errorMessage = `Server responded with status: ${error.status}`;
          }
        } else if (typeof error === "string") {
          errorMessage = error;
        }

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

  displaySuccessAlert(text, title = "Success!") {
    Swal.fire({
      icon: "success",
      title: title,
      text: text,
      showConfirmButton: false,
      timer: 2500,
    });
  }

  displayErrorAlert(text, title = "Error!") {
    Swal.fire({
      icon: "error",
      title: title,
      html: text,
      confirmButtonText: "Ok",
    });
  }
}
