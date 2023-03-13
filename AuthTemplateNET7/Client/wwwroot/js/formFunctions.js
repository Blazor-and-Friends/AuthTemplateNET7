//in case there's multiple checkboxes or other form components that need js, let's just have one file
window['ff'] = {
    setIndeterminateOnCheckbox(el, isIndeterminate) {
        el.indeterminate = isIndeterminate;
    },

    isLoaded() {
        return true;
    }
}