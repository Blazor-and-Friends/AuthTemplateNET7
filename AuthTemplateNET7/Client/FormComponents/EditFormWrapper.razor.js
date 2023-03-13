//since checkboxes need access to js and we don't want to load unneccessary js, call this from EditFormWrapper.razor
export function loadFormFunctions() {
    var src = 'js/formFunctions.js';
    var formFunctionsScriptEl = document.querySelector(`[src="${src}"]`);

    if (formFunctionsScriptEl) return; //we've already added it

    var scriptTag = document.createElement('script');
    scriptTag.setAttribute('src', src);
    scriptTag.setAttribute('type', 'text/javascript');
    document.head.appendChild(scriptTag);
}

export function focusFirstInput(formId) {

    let formEl = document.getElementById(formId);

    if (!formEl) return;

    var inputs = formEl.querySelectorAll('input, textarea, select');

    if (inputs) {
        inputs[0].focus();
    }
}