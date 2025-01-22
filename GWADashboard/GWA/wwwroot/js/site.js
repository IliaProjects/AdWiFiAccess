// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(function () {
    $(window).resize(function (e) {
        $(".height-as-parent").css('min-height', $(window).height());
        $(".sidebar").css('min-height', $(window).height());
    });

    $('.menu-toggler').click(function () {
        if ($('body').hasClass('page-sidebar-closed')) {
            Cookies.set('sidebar-closed', 'false', { expires: 365 * 10 });
        }
        else {
            Cookies.set('sidebar-closed', 'true', { expires: 365 * 10 });
        }
    });
    $(window).resize();
});

function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}


function ShowView(actionURI, id, menuId) {

    $tabTitle = $("#menu-item-" + menuId + " .title").text();
    $tab = '<li id="tab-opened-portlet-' + id + '" ><a id="a_portlet_' + id + '" href="#portlet_' + id + '" data-toggle="tab" aria-expanded="false" class="a-tab-opened-portlet">' + $tabTitle + '<span class="badge tab-badge" onclick="closeBadgeClick(this)" >x</span> </a></li>';
    $('#tab-opened-portlets').append($tab);

    var $element = $('<div class="tab-pane" id="portlet_' + id + '"></div>');
    $("#all-portlets").empty();
    $("#all-portlets").append($element);

    $('#a_portlet_' + id).click();

    $('#portlet_' + id).load(actionURI, null,
        function (response, status, xhr) {
            if (status == "error") {
                var msg = "Ошибка в результате выполнения запроса.<br/>HTTP ";
                toastr["error"](msg + xhr.status + " " + xhr.statusText);
            }
            else {

                $('#portlet_' + id + ' .portlet-header-icon').attr('class', $("#menu-item-" + menuId + ' i').attr('class'));

                $(window).resize();
            }
        }
    );
}