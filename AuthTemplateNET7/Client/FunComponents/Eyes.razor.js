let ex = 0;
let ey = 0;
let ae;
let le;
let re;
let x0, y0, tid, realx, realy;

function moveall() {
    let rx = realx + 40;
    let ry = realy + 40;
    rx += (ex - rx) * 0.1;
    ry += (ey - ry) * 0.1;
    realx = rx - 40;
    realy = ry - 40;
    x0 = Math.round(realx);
    y0 = Math.round(realy) - 20;
    moveeye();

    tid = setTimeout(function () {
        moveall();
    }, 100);
}

function moveeye() {
    let dy = ey - y0 - 20;
    let dx1 = ex - x0 - 20;
    let dx2 = ex - x0 - 60;
    let r = Math.sqrt(dx1 * dx1 + dy * dy);
    if (r < 20) r = 20;
    dx1 = dx1 * 10 / r + x0 + 10;
    let dy1 = dy * 10 / r + y0 + 10;
    r = Math.sqrt(dx2 * dx2 + dy * dy);
    if (r < 20) r = 20;
    dx2 = dx2 * 10 / r + x0 + 50;

    ae.style.left = x0 + 'px';
    ae.style.top = y0 + 'px';

    le.style.left = dx1 + 'px';
    le.style.top = dy1 + 'px';

    re.style.left = dx2 + 'px';
    re.style.top = dy1 + 'px';
}

export function setHandlers() {
    ae = document.getElementById('eyeballs');
    le = document.getElementById('lefteye');
    re = document.getElementById('righteye');

    //var offset = $('body').offset();
    let offset = document.body.getBoundingClientRect();
    console.log(offset);
    y0 = offset.top;
    x0 = offset.left;

    realx = x0 + 0.1;
    realy = y0 + 0.1;
    moveall();
}

export function clearEyes() {
    if (tid) clearTimeout(tid);
    window.removeEventListener('mousemove', moveeye);
}

document.addEventListener('mousemove', function (e) {
    ex = e.pageX;
    ey = e.pageY;

    moveeye();
});

