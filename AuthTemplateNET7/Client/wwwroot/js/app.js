//added

var tf = window['tf'] = { // tf = template functions that are included

    focusElementById(id) {
        var el = document.getElementById(id);

        if (el) el.focus();
    },

    focusInputEl: function (el) {
        el.focus();
    },

    getItem: function (key) { //for CookieConsent component, but can be useful elsewhere
        return localStorage.getItem(key);
    },

    scrollToElementById: function (id) {
        document.getElementById(id).scrollIntoView();
    },

    //adds [target="_blank"] to any a elements that link to another hostname.
    //adds the external link icon
    setExternalLinks: function () {

        var anchorEls = document.querySelectorAll('a[href]:not([target])'); //has href, doesn't have the target set

        if (anchorEls.length < 1) return;

        const currHostName = new URL(window.location.href).hostname;

        anchorEls.forEach(el => {
            const href = el.getAttribute('href');

            if (!href || href === "#") return;

            try { //in case there's something in the href that isn't a valid url
                const targetHost = new URL(href).hostname;

                if (currHostName === targetHost) return; //it's a local link, let's blow this clambake

                el.setAttribute('target', '_blank');

                let small = document.createElement('small');
                small.classList = 'bi bi-box-arrow-up-right ms-1 seventy-pct';
                //<i class="bi bi-box-arrow-up-right"></i>

                el.appendChild(small);
            } catch { }
        });
    },

    setItem: function (key, obj) { //for CookieConsent component, but can be useful elsewhere
        localStorage.setItem(key, obj);
    }
};

//for input and textarea elements. Select's all text if the element has an 'select-all' attribute when the element gains focus
function selectAllOnFocus(e) {
    let el = e.target;
    if (el.hasAttribute('select-all')) {
        el.select();
    }
}

window.addEventListener('DOMContentLoaded', () => {
    document.querySelector('body').addEventListener('focus', function (e) {
        selectAllOnFocus(e);
    }, true);
});