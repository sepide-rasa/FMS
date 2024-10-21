///<reference path="jquery-1.3.2-vsdoc2.js" />
///<reference path="jquery.ribbon.js" />

$(document).ready(function() {
    $('#mainContainer').Ribbon({ theme: 'windows7' });

    $('ul.ribbon-theme li').click(function() { $().Ribbon({ theme: $(this).attr('class').substring(7) }); });
});