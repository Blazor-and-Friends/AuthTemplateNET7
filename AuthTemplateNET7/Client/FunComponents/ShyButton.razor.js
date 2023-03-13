window['sb'] = {
    runAway: function (btnEl) {

        if (btnEl.style.position != 'fixed') btnEl.style.position = 'fixed';

        let h = window.innerHeight;
        let w = window.innerWidth;

        let top = Math.random() * h;
        var left = Math.random() * w;

        let bh = btnEl.offsetHeight;
        let bw = btnEl.offsetWidth;

        if (top + bh > h) top = top - bh;
        if (left + bw > w) left = left - bw;

        btnEl.style.top = top + 'px';
        btnEl.style.left = left + 'px';
    }
}

//for some reason this can't be found
//export function runAway() {
//    let h = window.innerHeight;
//    let w = window.innerWidth;

//    let top = Math.random() * h;
//    var left = Math.random() * w;

//    let btnEl = document.querySelector('.shy-button');

//    btnEl.style.top = top + 'px';
//    btnEl.style.left = left + 'px';
//}