// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

class AdTimer {

    constructor(sec, sessionId, callback) {
        this._sessionId = sessionId;
        this._callback = callback;
        this._paused = true;
        if (sec < 5) {
            this._sec = 5;
        }
        else {
            this._sec = sec;
        }
    }

    start() {
        if (this._paused) {
            if (this._sec < 1) {
                this.ajax()
            }
            else {
                var context = this;
                this.i = setInterval(function () {
                    if (context._sec < 1) {
                        context.ajax();
                        clearInterval(context.i);
                    }
                    else {
                        context._sec--;
                        context._callback(context._sec);
                    }
                }, 1000);
                this._paused = false;
            }
        }
    }

    pause() {
        if (this._paused == false) {
            clearInterval(this.i);
            this._paused = true;
        }
    }

    ajax() {
        var context = this;
        $.ajax({
            url: "/api/adboard",
            type: "put",
            data: {
                "sessionId": this._sessionId,
                "action": 0,
            },
            success: context._callback
        })
    }
}