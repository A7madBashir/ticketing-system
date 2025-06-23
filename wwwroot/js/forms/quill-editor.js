function initializeQuillEditor(editorElement, hiddenInput, initialValue = "") {
  if (typeof Quill === "undefined") {
    console.error("Quill library not found. Cannot initialize editor.");
    return null;
  }

  const quill = new Quill(editorElement, {
    theme: "snow",
    modules: {
      toolbar: [
        ["bold", "italic", "underline", "strike"], // toggled buttons
        ["blockquote", "code-block"],
        [{ header: 1 }, { header: 2 }], // custom button values
        [{ list: "ordered" }, { list: "bullet" }],
        [{ script: "sub" }, { script: "super" }], // superscript/subscript
        [{ indent: "-1" }, { indent: "+1" }], // outdent/indent
        [{ direction: "rtl" }], // text direction
        [{ size: ["small", false, "large", "huge"] }], // custom dropdown
        [{ header: [1, 2, 3, 4, 5, 6, false] }],
        [{ color: [] }, { background: [] }], // dropdown with defaults from theme
        [{ font: [] }],
        [{ align: [] }],
        ["clean"], // remove formatting button
      ],
    },
  });

  // Sync content from Quill to the hidden input on change
  quill.on("text-change", () => {
    hiddenInput.value = quill.root.innerHTML;
  });

  if (initialValue != undefined && initialValue != null && initialValue != "") {
    quill.root.innerHTML = initialValue;
  }

  return quill;
}
