///<reference path="jquery-1.3.2-vsdoc2.js" />
/*
Copyright (c) 2009 Mikael Söderström.
Contact: vimpyboy@msn.com

Feel free to use this script as long as you don't remove this comment.
*/

(function ($) {
    var isLoaded;
    var isClosed;

    $.fn.Ribbon = function (ribbonSettings) {
        var settings = $.extend({ theme: 'windows7' }, ribbonSettings || {});

        $('.ribbon1 a').each(function () {
            if ($(this).attr('accesskey')) {
                $(this).append('<div rel="accesskeyhelper" style="display: none; position: absolute; background-image: url(images/accessbg.png); background-repeat: none; width: 16px; padding: 0px; text-align: center; height: 17px; line-height: 17px; top: ' + $(this).offset().top + 'px; left: ' + ($(this).offset().left + $(this).width() - 15) + 'px;">' + $(this).attr('accesskey') + '</div>');
            }
        });

        $('head').append('<link href="/Ribbon/themes/' + settings.theme + '/ribbon.css" rel="stylesheet" type="text/css" />" />');

        if (!isLoaded) {
            SetupMenu(settings);
        }

        $(document).keydown(function (e) { ShowAccessKeys(e); });
        $(document).keyup(function (e) { HideAccessKeys(e); });

        function SetupMenu(settings) {
            $('.menu1 li a:first').addClass('active');
            $('.menu1 li ul').hide();
            $('.menu1 li a:first').parent().children('ul:first').show();
            $('.menu1 li a:first').parent().children('ul:first').addClass('submenu1');
            $('.menu1 li > a').click(function () { ShowSubMenu(this); });
            $('.orb1').click(function () { ShowMenu(); });
            $('.orb1 ul').hide();
            $('.orb1 ul ul').hide();
            $('.orb1 li ul li ul').show();
            $('.orb1 li li ul').each(function () { $(this).prepend('<div style="background-color: #EBF2F7; height: 25px; line-height: 25px; width: 292px; padding-left: 9px; border-bottom: 1px solid #CFDBEB;">' + $(this).parent().children('a:first').text() + '</div>'); });
            $('.orb1 li li a').each(function () { if ($(this).parent().children('ul').length > 0) { $(this).addClass('arrow') } });

            //$('.ribbon-list div').each(function() { $(this).parent().width($(this).parent().width()); });

            $('.ribbon-list1 div').click(function (e) {
                var elwidth = $(this).parent().width();
                var insideX = e.pageX > $(this).offset().left && e.pageX < $(this).offset().left + $(this).width();
                var insideY = e.pageY > $(this).offset().top && e.pageY < $(this).offset().top + $(this).height();

                $('.ribbon-list1 div ul').fadeOut('fast');

                if (insideX && insideY) {
                    $(this).attr('style', 'background-image: ' + $(this).css('background-image'));

                    $(this).parent().width(elwidth);

                    $(this).children('ul').width(elwidth - 4);
                    $(this).children('ul').fadeIn('fast');
                }
            });

            $('.ribbon-list1 div').parents().click(function (e) {
                var outsideX = true;//= e.pageX < $('.ribbon-list1 div ul:visible').parent().offset().left || e.pageX > $('.ribbon-list1 div ul:visible').parent().offset().left + $('.ribbon-list1 div ul:visible').parent().width();
                var outsideY = true;//= e.pageY < $('.ribbon-list1 div ul:visible').parent().offset().top || e.pageY > $('.ribbon-list1 div ul:visible').parent().offset().top + $('.ribbon-list1 div ul:visible').parent().height();

                if (outsideX || outsideY) {
                    $('.ribbon-list1 div ul:visible').each(function () {
                        $(this).fadeOut('fast');
                    });
                    $('.ribbon-list1 div').css('background-image', '');
                }
            });

            $('.orb1 li li a').mouseover(function () { ShowOrbChildren(this); });

            $('.menu1 li > a').dblclick(function () {
                $('ul .submenu1').animate({ height: "hide" });
                $('body').animate({ paddingTop: $(this).parent().parent().parent().parent().height() });
                isClosed = true;
            });
        }

        $('.ribbon1').parents().click(function (e) {
            var outsideX = e.pageX < $('.orb1 ul:first').offset().left || e.pageX > $('.orb1 ul:first').offset().left + $('.orb1 ul:first').width();
            var outsideY = e.pageY < $('.orb1 ul:first img:first').offset().top || e.pageY > $('.orb1 ul:first').offset().top + $('.orb1 ul:first').height();

            if (outsideX || outsideY)
                $('.orb1 ul').fadeOut('fast');
        });

        if (isLoaded) {
            $('.orb1 li:first ul:first img:first').remove();
            $('.orb1 li:first ul:first img:last').remove();
            $('.ribbon-list1 div img[src*="/images/arrow_down.png"]').remove();
        }

        $('.orb1 li:first ul:first').prepend('<img src="/ribbon/themes/' + settings.theme + '/images/menu_top.png" style="margin-left: -10px; margin-top: -22px;" />');
        $('.orb1 li:first ul:first').append('<img src="/ribbon/themes/' + settings.theme + '/images/menu_bottom.png" style="margin-left: -10px; margin-bottom: -20px;" />');

        $('.ribbon-list1 div').each(function () { if ($(this).children('ul').length > 0) { $(this).append('<img src="ribbon/themes/' + settings.theme + '/images/arrow_down.png" style="float: right; margin-top: 5px;" />') } });

        //Hack for IE 7.
        if (navigator.appVersion.indexOf('MSIE 6.0') > -1 || navigator.appVersion.indexOf('MSIE 7.0') > -1) {
            $('ul.menu1 li li div').css('width', '90px');
            $('ul.menu1').css('width', '500px');
            $('ul.menu1').css('float', 'left');
            $('ul.menu1 .submenu1 li div.ribbon-list1').css('width', '100px');
            $('ul.menu1 .submenu1 li div.ribbon-list1 div').css('width', '100px');
        }

        $('a[href="#' + window.location.hash + '"]').click();

        isLoaded = true;

        function ResetSubMenu() {
            $('.menu1 li a').removeClass('active');
            $('.menu1 ul').removeClass('submenu1');
            $('.menu1 li ul').hide();
        }

        function ShowSubMenu(e) {
            var isActive = $(e).next().css('display') == 'block';
            ResetSubMenu();

            $(e).addClass('active');
            $(e).parent().children('ul:first').addClass('submenu1');

            $(e).parent().children('ul:first').show();
            //$('body').css('padding-top', '120px');

            isClosed = false;
        }

        function ShowOrbChildren(e) {
            if (($(e).parent().children('ul').css('display') == 'none' || $(e).parent().children('ul').length == 0) && $(e).parent().parent().parent().parent().hasClass('orb1')) {
                $('.orb1 li li ul').fadeOut('fast');
                $(e).parent().children('ul').fadeIn('fast');
            }
        }

        function ShowMenu() {
            $('.orb1 ul').animate({ opacity: 'toggle' }, 'fast');
        }

        function ShowAccessKeys(e) {
            if (e.altKey) {
                $('div[rel="accesskeyhelper"]').each(function () { $(this).css('top', $(this).parent().offset().top).css('left', $(this).parent().offset().left - 20 + $(this).parent().width()); $(this).show(); });
            }
        }

        function HideAccessKeys(e) {
            $('div[rel="accesskeyhelper"]').hide();
        }
    }
})(jQuery);

(function ($) {
    var isLoaded;
    var isClosed;

    $.fn.Ribbon2 = function (ribbonSettings) {
        var settings = $.extend({ theme: 'windows7' }, ribbonSettings || {});

        $('.ribbon2 a').each(function () {
            if ($(this).attr('accesskey')) {
                $(this).append('<div rel="accesskeyhelper" style="display: none; position: absolute; background-image: url(images/accessbg.png); background-repeat: none; width: 16px; padding: 0px; text-align: center; height: 17px; line-height: 17px; top: ' + $(this).offset().top + 'px; left: ' + ($(this).offset().left + $(this).width() - 15) + 'px;">' + $(this).attr('accesskey') + '</div>');
            }
        });

        if (!isLoaded) {
            SetupMenu(settings);
        }

        $(document).keydown(function (e) { ShowAccessKeys(e); });
        $(document).keyup(function (e) { HideAccessKeys(e); });

        function SetupMenu(settings) {
            $('.menu2 li a:first').addClass('active');
            $('.menu2 li ul').hide();
            $('.menu2 li a:first').parent().children('ul:first').show();
            $('.menu2 li a:first').parent().children('ul:first').addClass('submenu2');
            $('.menu2 li > a').click(function () { ShowSubMenu(this); });
            $('.orb2').click(function () { ShowMenu(); });
            $('.orb2 ul').hide();
            $('.orb2 ul ul').hide();
            $('.orb2 li ul li ul').show();
            $('.orb2 li li ul').each(function () { $(this).prepend('<div style="background-color: #EBF2F7; height: 25px; line-height: 25px; width: 292px; padding-left: 9px; border-bottom: 1px solid #CFDBEB;">' + $(this).parent().children('a:first').text() + '</div>'); });
            $('.orb2 li li a').each(function () { if ($(this).parent().children('ul').length > 0) { $(this).addClass('arrow') } });

            //$('.ribbon-list div').each(function() { $(this).parent().width($(this).parent().width()); });

            $('.ribbon-list2 div').click(function (e) {
                var elwidth = $(this).parent().width();
                var insideX = e.pageX > $(this).offset().left && e.pageX < $(this).offset().left + $(this).width();
                var insideY = e.pageY > $(this).offset().top && e.pageY < $(this).offset().top + $(this).height();

                $('.ribbon-list2 div ul').fadeOut('fast');

                if (insideX && insideY) {
                    $(this).attr('style', 'background-image: ' + $(this).css('background-image'));

                    $(this).parent().width(elwidth);

                    $(this).children('ul').width(elwidth - 4);
                    $(this).children('ul').fadeIn('fast');
                }
            });

            $('.ribbon-list2 div').parents().click(function (e) {
                var outsideX = true;//= e.pageX < $('.ribbon-list2 div ul:visible').parent().offset().left || e.pageX > $('.ribbon-list2 div ul:visible').parent().offset().left + $('.ribbon-list2 div ul:visible').parent().width();
                var outsideY = true;//= e.pageY < $('.ribbon-list2 div ul:visible').parent().offset().top || e.pageY > $('.ribbon-list2 div ul:visible').parent().offset().top + $('.ribbon-list2 div ul:visible').parent().height();

                if (outsideX || outsideY) {
                    $('.ribbon-list2 div ul:visible').each(function () {
                        $(this).fadeOut('fast');
                    });
                    $('.ribbon-list2 div').css('background-image', '');
                }
            });

            $('.orb2 li li a').mouseover(function () { ShowOrbChildren(this); });

            $('.menu2 li > a').dblclick(function () {
                $('ul .submenu2').animate({ height: "hide" });
                $('body').animate({ paddingTop: $(this).parent().parent().parent().parent().height() });
                isClosed = true;
            });
        }

        $('.ribbon2').parents().click(function (e) {
            var outsideX = e.pageX < $('.orb2 ul:first').offset().left || e.pageX > $('.orb2 ul:first').offset().left + $('.orb2 ul:first').width();
            var outsideY = e.pageY < $('.orb2 ul:first img:first').offset().top || e.pageY > $('.orb2 ul:first').offset().top + $('.orb2 ul:first').height();

            if (outsideX || outsideY)
                $('.orb2 ul').fadeOut('fast');
        });

        if (isLoaded) {
            $('.orb2 li:first ul:first img:first').remove();
            $('.orb2 li:first ul:first img:last').remove();
            $('.ribbon-list2 div img[src*="/images/arrow_down.png"]').remove();
        }

        $('.orb2 li:first ul:first').prepend('<img src="/ribbon/themes/' + settings.theme + '/images/menu_top.png" style="margin-left: -10px; margin-top: -22px;" />');
        $('.orb2 li:first ul:first').append('<img src="/ribbon/themes/' + settings.theme + '/images/menu_bottom.png" style="margin-left: -10px; margin-bottom: -20px;" />');

        $('.ribbon-list2 div').each(function () { if ($(this).children('ul').length > 0) { $(this).append('<img src="/ribbon/themes/' + settings.theme + '/images/arrow_down.png" style="float: right; margin-top: 5px;" />') } });

        //Hack for IE 7.
        if (navigator.appVersion.indexOf('MSIE 6.0') > -1 || navigator.appVersion.indexOf('MSIE 7.0') > -1) {
            $('ul.menu2 li li div').css('width', '90px');
            $('ul.menu2').css('width', '500px');
            $('ul.menu2').css('float', 'left');
            $('ul.menu2 .submenu2 li div.ribbon-list2').css('width', '100px');
            $('ul.menu2 .submenu2 li div.ribbon-list2 div').css('width', '100px');
        }

        $('a[href="#' + window.location.hash + '"]').click();

        isLoaded = true;

        function ResetSubMenu() {
            $('.menu2 li a').removeClass('active');
            $('.menu2 ul').removeClass('submenu2');
            $('.menu2 li ul').hide();
        }

        function ShowSubMenu(e) {
            var isActive = $(e).next().css('display') == 'block';
            ResetSubMenu();

            $(e).addClass('active');
            $(e).parent().children('ul:first').addClass('submenu2');

            $(e).parent().children('ul:first').show();
            //$('body').css('padding-top', '120px');

            isClosed = false;
        }

        function ShowOrbChildren(e) {
            if (($(e).parent().children('ul').css('display') == 'none' || $(e).parent().children('ul').length == 0) && $(e).parent().parent().parent().parent().hasClass('orb2')) {
                $('.orb2 li li ul').fadeOut('fast');
                $(e).parent().children('ul').fadeIn('fast');
            }
        }

        function ShowMenu() {
            $('.orb2 ul').animate({ opacity: 'toggle' }, 'fast');
        }

        function ShowAccessKeys(e) {
            if (e.altKey) {
                $('div[rel="accesskeyhelper"]').each(function () { $(this).css('top', $(this).parent().offset().top).css('left', $(this).parent().offset().left - 20 + $(this).parent().width()); $(this).show(); });
            }
        }

        function HideAccessKeys(e) {
            $('div[rel="accesskeyhelper"]').hide();
        }
    }
})(jQuery);

(function ($) {
    var isLoaded;
    var isClosed;

    $.fn.Ribbon3 = function (ribbonSettings) {
        var settings = $.extend({ theme: 'windows7' }, ribbonSettings || {});

        $('.ribbon3 a').each(function () {
            if ($(this).attr('accesskey')) {
                $(this).append('<div rel="accesskeyhelper" style="display: none; position: absolute; background-image: url(images/accessbg.png); background-repeat: none; width: 16px; padding: 0px; text-align: center; height: 17px; line-height: 17px; top: ' + $(this).offset().top + 'px; left: ' + ($(this).offset().left + $(this).width() - 15) + 'px;">' + $(this).attr('accesskey') + '</div>');
            }
        });

        if (!isLoaded) {
            SetupMenu(settings);
        }

        $(document).keydown(function (e) { ShowAccessKeys(e); });
        $(document).keyup(function (e) { HideAccessKeys(e); });

        function SetupMenu(settings) {
            $('.menu3 li a:first').addClass('active');
            $('.menu3 li ul').hide();
            $('.menu3 li a:first').parent().children('ul:first').show();
            $('.menu3 li a:first').parent().children('ul:first').addClass('submenu3');
            $('.menu3 li > a').click(function () { ShowSubMenu(this); });
            $('.orb3').click(function () { ShowMenu(); });
            $('.orb3 ul').hide();
            $('.orb3 ul ul').hide();
            $('.orb3 li ul li ul').show();
            $('.orb3 li li ul').each(function () { $(this).prepend('<div style="background-color: #EBF2F7; height: 25px; line-height: 25px; width: 292px; padding-left: 9px; border-bottom: 1px solid #CFDBEB;">' + $(this).parent().children('a:first').text() + '</div>'); });
            $('.orb3 li li a').each(function () { if ($(this).parent().children('ul').length > 0) { $(this).addClass('arrow') } });

            //$('.ribbon-list div').each(function() { $(this).parent().width($(this).parent().width()); });

            /*$('.ribbon-list3 div').click(function (e) {
                if ($(this).parent('div').length > 0) {
                    var elwidth = $(this).parent().width();
                    var insideX = e.pageX > $(this).offset().left && e.pageX < $(this).offset().left + $(this).width();
                    var insideY = e.pageY > $(this).offset().top && e.pageY < $(this).offset().top + $(this).height();

                    /*var divclass = document.getElementById("PayReports").getElementsByClassName("ribbon-list3");
                    for (var i = 0; i < divclass.length; i++) {

                        if (divclass[i].id != $(this)[0].parentElement.id) {
                            /*divclass[i].children[0].children[1].fadeOut('fast');
                        }
                    }اینجا

                    if (e.srcElement.childElementCount ==1) {
                        $('.ribbon-list3 div ul').fadeOut('fast');
                    }

                    if (insideX && insideY) {
                        $('.ribbon-list3 div ul').fadeOut('fast');
                        $(this).attr('style', 'z-index:100000;background-image: ' + $(this).css('background-image'));
                        $(this).parent().width(20);
                        $(this).children('ul').width(150 + 4);
                        $(this).children('ul').fadeIn('fast');
                    }
                }
            });*/
            $('.menu3 li ul li div').click(function (e) {
                /*if ($(this).children('div').children('div').children('ul').length > 0) {*/
                aaa = e;

                if (e.target != undefined && e.target.tagName != "LI" && e.target.parentNode.className != "ribbon-sublist3") {
                    $('.ribbon-list3 div ul').fadeOut('fast');
                    $(this).children('div').children('div').children('ul').width(154);
                    $(this).children('div').children('div').children('ul').fadeIn('fast');
                }
            });

            $('.ribbon-list3 div').parents().click(function (e) {
                if ($('.ribbon-list3 div ul:visible').parent().position() != undefined) {
                    var outsideX = e.pageX < $('.ribbon-list3 div ul:visible').parent().position().left || e.pageX > $('.ribbon-list3 div ul:visible').parent().position().left + $('.ribbon-list3 div ul:visible').parent().width();
                    var outsideY = e.pageY < $('.ribbon-list3 div ul:visible').parent().position().top || e.pageY > $('.ribbon-list3 div ul:visible').parent().position().top + $('.ribbon-list3 div ul:visible').parent().height();

                    if (outsideX || outsideY) {
                        $('.ribbon-list3 div ul:visible').each(function () {
                            if (e.target.childElementCount == 1) {
                                $(this).fadeOut('fast');
                            }
                        });
                        $('.ribbon-list3 div').css('background-image', '');
                    }
                }
            });

            $('.orb3 li li a').mouseover(function () { ShowOrbChildren(this); });

            $('.menu3 li > a').dblclick(function () {
                $('ul .submenu3').animate({ height: "hide" });
                $('body').animate({ paddingTop: $(this).parent().parent().parent().parent().height() });
                isClosed = true;
            });
        }

        $('.ribbon3').parents().click(function (e) {
            var outsideX = e.pageX < $('.orb3 ul:first').offset().left || e.pageX > $('.orb3 ul:first').offset().left + $('.orb3 ul:first').width();
            var outsideY = e.pageY < $('.orb3 ul:first img:first').offset().top || e.pageY > $('.orb3 ul:first').offset().top + $('.orb3 ul:first').height();

            if (outsideX || outsideY)
                $('.orb3 ul').fadeOut('fast');
        });

        if (isLoaded) {
            $('.orb3 li:first ul:first img:first').remove();
            $('.orb3 li:first ul:first img:last').remove();
            $('.ribbon-list3 div img[src*="/images/arrow_down.png"]').remove();
        }

        $('.orb3 li:first ul:first').prepend('<img src="/ribbon/themes/' + settings.theme + '/images/menu_top.png" style="margin-left: -10px; margin-top: -22px;" />');
        $('.orb3 li:first ul:first').append('<img src="/ribbon/themes/' + settings.theme + '/images/menu_bottom.png" style="margin-left: -10px; margin-bottom: -20px;" />');

        $('.ribbon-list3 div').each(function () { if ($(this).children('ul').length > 0 && $(this).parent('div').length > 0) { $(this).append('<img src="/ribbon/themes/' + settings.theme + '/images/arrow_down.png" style="float: right; margin-top: 5px;" />') } });
        $('.ribbon-sublist3 div').each(function () { if ($(this).children('ul').length > 0) { $(this).append('<img src="/ribbon/themes/' + settings.theme + '/images/arrow.png" style="float: left; margin-top: 5px;" />') } });

        $('.ribbon-sublist3 div').click(function (e) {
            var elwidth = $(this).parent().width();
            var insideX = e.pageX > $(this).offset().left && e.pageX < $(this).offset().left + $(this).width();
            var insideY = e.pageY > $(this).offset().top && e.pageY < $(this).offset().top + $(this).height();

            $('.ribbon-sublist3 div ul').fadeOut('fast');

            if (insideX && insideY) {
                $(this).attr('style', 'z-index:100000;background-image: ' + $(this).css('background-image'));
                $(this).parent().width(150);
                $(this).children('ul').width(150 + 4);
                $(this).children('ul').css("right", $(this).parent().width() + 2);
                $(this).children('ul').css("margin-top", "-19px");
                $(this).children('ul').fadeIn('fast');
            }
        });



        $('.ribbon-list3 div ul li').hover(function (e) { if ($(this).children('div').length == 0 && $(this).parent('ul').parent('div').parent('li').attr('class') != "ribbon-sublist3") { $('.ribbon-sublist3 div ul').fadeOut('fast'); } });

        //Hack for IE 7.
        if (navigator.appVersion.indexOf('MSIE 6.0') > -1 || navigator.appVersion.indexOf('MSIE 7.0') > -1) {
            $('ul.menu3 li li div').css('width', '90px');
            $('ul.menu3').css('width', '500px');
            $('ul.menu3').css('float', 'left');
            $('ul.menu3 .submenu3 li div.ribbon-list3').css('width', '100px');
            $('ul.menu3 .submenu3 li div.ribbon-list3 div').css('width', '100px');
        }

        $('a[href="#' + window.location.hash + '"]').click();

        isLoaded = true;

        function ResetSubMenu() {
            $('.menu3 li a').removeClass('active');
            $('.menu3 ul').removeClass('submenu3');
            $('.menu3 li ul').hide();
        }

        function ShowSubMenu(e) {
            var isActive = $(e).next().css('display') == 'block';
            ResetSubMenu();

            $(e).addClass('active');
            $(e).parent().children('ul:first').addClass('submenu3');

            $(e).parent().children('ul:first').show();
            //$('body').css('padding-top', '120px');

            isClosed = false;
        }

        function ShowOrbChildren(e) {
            if (($(e).parent().children('ul').css('display') == 'none' || $(e).parent().children('ul').length == 0) && $(e).parent().parent().parent().parent().hasClass('orb3')) {
                $('.orb3 li li ul').fadeOut('fast');
                $(e).parent().children('ul').fadeIn('fast');
            }
        }

        function ShowMenu() {
            $('.orb3 ul').animate({ opacity: 'toggle' }, 'fast');
        }

        function ShowAccessKeys(e) {
            if (e.altKey) {
                $('div[rel="accesskeyhelper"]').each(function () { $(this).css('top', $(this).parent().offset().top).css('left', $(this).parent().offset().left - 20 + $(this).parent().width()); $(this).show(); });
            }
        }

        function HideAccessKeys(e) {
            $('div[rel="accesskeyhelper"]').hide();
        }
    }
})(jQuery);


/**/
(function ($) {
    var isLoaded;
    var isClosed;

    $.fn.Ribbon4 = function (ribbonSettings) {
        var settings = $.extend({ theme: 'windows7' }, ribbonSettings || {});

        $('.ribbon4 a').each(function () {
            if ($(this).attr('accesskey')) {
                $(this).append('<div rel="accesskeyhelper" style="display: none; position: absolute; background-image: url(images/accessbg.png); background-repeat: none; width: 16px; padding: 0px; text-align: center; height: 17px; line-height: 17px; top: ' + $(this).offset().top + 'px; left: ' + ($(this).offset().left + $(this).width() - 15) + 'px;">' + $(this).attr('accesskey') + '</div>');
            }
        });

        if (!isLoaded) {
            SetupMenu(settings);
        }

        $(document).keydown(function (e) { ShowAccessKeys(e); });
        $(document).keyup(function (e) { HideAccessKeys(e); });

        function SetupMenu(settings) {
            $('.menu4 li a:first').addClass('active');
            $('.menu4 li ul').hide();
            $('.menu4 li a:first').parent().children('ul:first').show();
            $('.menu4 li a:first').parent().children('ul:first').addClass('submenu4');
            $('.menu4 li > a').click(function () { ShowSubMenu(this); });
            $('.orb4').click(function () { ShowMenu(); });
            $('.orb4 ul').hide();
            $('.orb4 ul ul').hide();
            $('.orb4 li ul li ul').show();
            $('.orb4 li li ul').each(function () { $(this).prepend('<div style="background-color: #EBF2F7; height: 25px; line-height: 25px; width: 292px; padding-left: 9px; border-bottom: 1px solid #CFDBEB;">' + $(this).parent().children('a:first').text() + '</div>'); });
            $('.orb4 li li a').each(function () { if ($(this).parent().children('ul').length > 0) { $(this).addClass('arrow') } });

            //$('.ribbon-list div').each(function() { $(this).parent().width($(this).parent().width()); });

            /*$('.ribbon-list3 div').click(function (e) {
                if ($(this).parent('div').length > 0) {
                    var elwidth = $(this).parent().width();
                    var insideX = e.pageX > $(this).offset().left && e.pageX < $(this).offset().left + $(this).width();
                    var insideY = e.pageY > $(this).offset().top && e.pageY < $(this).offset().top + $(this).height();

                    /*var divclass = document.getElementById("PayReports").getElementsByClassName("ribbon-list3");
                    for (var i = 0; i < divclass.length; i++) {

                        if (divclass[i].id != $(this)[0].parentElement.id) {
                            /*divclass[i].children[0].children[1].fadeOut('fast');
                        }
                    }اینجا

                    if (e.srcElement.childElementCount ==1) {
                        $('.ribbon-list3 div ul').fadeOut('fast');
                    }

                    if (insideX && insideY) {
                        $('.ribbon-list3 div ul').fadeOut('fast');
                        $(this).attr('style', 'z-index:100000;background-image: ' + $(this).css('background-image'));
                        $(this).parent().width(20);
                        $(this).children('ul').width(150 + 4);
                        $(this).children('ul').fadeIn('fast');
                    }
                }
            });*/
            $('.menu4 li ul li div').click(function (e) {
                /*if ($(this).children('div').children('div').children('ul').length > 0) {*/
                aaa = e;

                if (e.target != undefined && e.target.tagName != "LI" && e.target.parentNode.className != "ribbon-sublist4") {
                    $('.ribbon-list4 div ul').fadeOut('fast');
                    $(this).children('div').children('div').children('ul').width(154);
                    $(this).children('div').children('div').children('ul').fadeIn('fast');
                }
            });

            $('.ribbon-list4 div').parents().click(function (e) {

                var outsideX = true;//= e.pageX < $('.ribbon-list4 div ul:visible').parent().offset().left || e.pageX > $('.ribbon-list4 div ul:visible').parent().offset().left + $('.ribbon-list4 div ul:visible').parent().width();
                var outsideY = true;//= e.pageY < $('.ribbon-list4 div ul:visible').parent().offset().top || e.pageY > $('.ribbon-list4 div ul:visible').parent().offset().top + $('.ribbon-list4 div ul:visible').parent().height();

                if (outsideX || outsideY) {
                    $('.ribbon-list4 div ul:visible').each(function () {
                        if (e.srcElement.childElementCount == 1) {
                            $(this).fadeOut('fast');
                        }
                    });
                    $('.ribbon-list4 div').css('background-image', '');
                }
            });

            $('.orb4 li li a').mouseover(function () { ShowOrbChildren(this); });

            $('.menu4 li > a').dblclick(function () {
                $('ul .submenu4').animate({ height: "hide" });
                $('body').animate({ paddingTop: $(this).parent().parent().parent().parent().height() });
                isClosed = true;
            });
        }

        $('.ribbon4').parents().click(function (e) {
            var outsideX = e.pageX < $('.orb4 ul:first').offset().left || e.pageX > $('.orb4 ul:first').offset().left + $('.orb4 ul:first').width();
            var outsideY = e.pageY < $('.orb4 ul:first img:first').offset().top || e.pageY > $('.orb4 ul:first').offset().top + $('.orb4 ul:first').height();

            if (outsideX || outsideY)
                $('.orb4 ul').fadeOut('fast');
        });

        if (isLoaded) {
            $('.orb4 li:first ul:first img:first').remove();
            $('.orb4 li:first ul:first img:last').remove();
            $('.ribbon-list4 div img[src*="/images/arrow_down.png"]').remove();
        }

        $('.orb4 li:first ul:first').prepend('<img src="/ribbon/themes/' + settings.theme + '/images/menu_top.png" style="margin-left: -10px; margin-top: -22px;" />');
        $('.orb4 li:first ul:first').append('<img src="/ribbon/themes/' + settings.theme + '/images/menu_bottom.png" style="margin-left: -10px; margin-bottom: -20px;" />');

        $('.ribbon-list4 div').each(function () { if ($(this).children('ul').length > 0 && $(this).parent('div').length > 0) { $(this).append('<img src="/ribbon/themes/' + settings.theme + '/images/arrow_down.png" style="float: right; margin-top: 5px;" />') } });
        $('.ribbon-sublist4 div').each(function () { if ($(this).children('ul').length > 0) { $(this).append('<img src="/ribbon/themes/' + settings.theme + '/images/arrow.png" style="float: left; margin-top: 5px;" />') } });

        $('.ribbon-sublist4 div').click(function (e) {
            var elwidth = $(this).parent().width();
            var insideX = e.pageX > $(this).offset().left && e.pageX < $(this).offset().left + $(this).width();
            var insideY = e.pageY > $(this).offset().top && e.pageY < $(this).offset().top + $(this).height();

            $('.ribbon-sublist4 div ul').fadeOut('fast');

            if (insideX && insideY) {
                $(this).attr('style', 'z-index:100000;background-image: ' + $(this).css('background-image'));
                $(this).parent().width(150);
                $(this).children('ul').width(150 + 4);
                $(this).children('ul').css("right", $(this).parent().width() + 2);
                $(this).children('ul').css("margin-top", "-19px");
                $(this).children('ul').fadeIn('fast');
            }
        });



        $('.ribbon-list4 div ul li').hover(function (e) { if ($(this).children('div').length == 0 && $(this).parent('ul').parent('div').parent('li').attr('class') != "ribbon-sublist4") { $('.ribbon-sublist4 div ul').fadeOut('fast'); } });

        //Hack for IE 7.
        if (navigator.appVersion.indexOf('MSIE 6.0') > -1 || navigator.appVersion.indexOf('MSIE 7.0') > -1) {
            $('ul.menu4 li li div').css('width', '90px');
            $('ul.menu4').css('width', '500px');
            $('ul.menu4').css('float', 'left');
            $('ul.menu4 .submenu4 li div.ribbon-list4').css('width', '100px');
            $('ul.menu4 .submenu4 li div.ribbon-list4 div').css('width', '100px');
        }

        $('a[href="#' + window.location.hash + '"]').click();

        isLoaded = true;

        function ResetSubMenu() {
            $('.menu4 li a').removeClass('active');
            $('.menu4 ul').removeClass('submenu4');
            $('.menu4 li ul').hide();
        }

        function ShowSubMenu(e) {
            var isActive = $(e).next().css('display') == 'block';
            ResetSubMenu();

            $(e).addClass('active');
            $(e).parent().children('ul:first').addClass('submenu4');

            $(e).parent().children('ul:first').show();
            //$('body').css('padding-top', '120px');

            isClosed = false;
        }

        function ShowOrbChildren(e) {
            if (($(e).parent().children('ul').css('display') == 'none' || $(e).parent().children('ul').length == 0) && $(e).parent().parent().parent().parent().hasClass('orb4')) {
                $('.orb4 li li ul').fadeOut('fast');
                $(e).parent().children('ul').fadeIn('fast');
            }
        }

        function ShowMenu() {
            $('.orb4 ul').animate({ opacity: 'toggle' }, 'fast');
        }

        function ShowAccessKeys(e) {
            if (e.altKey) {
                $('div[rel="accesskeyhelper"]').each(function () { $(this).css('top', $(this).parent().offset().top).css('left', $(this).parent().offset().left - 20 + $(this).parent().width()); $(this).show(); });
            }
        }

        function HideAccessKeys(e) {
            $('div[rel="accesskeyhelper"]').hide();
        }
    }
})(jQuery);

/**/
(function ($) {
    var isLoaded;
    var isClosed;

    $.fn.Ribbon5 = function (ribbonSettings) {
        var settings = $.extend({ theme: 'windows7' }, ribbonSettings || {});

        $('.ribbon5 a').each(function () {
            if ($(this).attr('accesskey')) {
                $(this).append('<div rel="accesskeyhelper" style="display: none; position: absolute; background-image: url(images/accessbg.png); background-repeat: none; width: 16px; padding: 0px; text-align: center; height: 17px; line-height: 17px; top: ' + $(this).offset().top + 'px; left: ' + ($(this).offset().left + $(this).width() - 15) + 'px;">' + $(this).attr('accesskey') + '</div>');
            }
        });

        if (!isLoaded) {
            SetupMenu(settings);
        }

        $(document).keydown(function (e) { ShowAccessKeys(e); });
        $(document).keyup(function (e) { HideAccessKeys(e); });

        function SetupMenu(settings) {
            $('.menu5 li a:first').addClass('active');
            $('.menu5 li ul').hide();
            $('.menu5 li a:first').parent().children('ul:first').show();
            $('.menu5 li a:first').parent().children('ul:first').addClass('submenu5');
            $('.menu5 li > a').click(function () { ShowSubMenu(this); });
            $('.orb5').click(function () { ShowMenu(); });
            $('.orb5 ul').hide();
            $('.orb5 ul ul').hide();
            $('.orb5 li ul li ul').show();
            $('.orb5 li li ul').each(function () { $(this).prepend('<div style="background-color: #EBF2F7; height: 25px; line-height: 25px; width: 292px; padding-left: 9px; border-bottom: 1px solid #CFDBEB;">' + $(this).parent().children('a:first').text() + '</div>'); });
            $('.orb5 li li a').each(function () { if ($(this).parent().children('ul').length > 0) { $(this).addClass('arrow') } });

            //$('.ribbon-list div').each(function() { $(this).parent().width($(this).parent().width()); });

            /*$('.ribbon-list3 div').click(function (e) {
                if ($(this).parent('div').length > 0) {
                    var elwidth = $(this).parent().width();
                    var insideX = e.pageX > $(this).offset().left && e.pageX < $(this).offset().left + $(this).width();
                    var insideY = e.pageY > $(this).offset().top && e.pageY < $(this).offset().top + $(this).height();

                    /*var divclass = document.getElementById("PayReports").getElementsByClassName("ribbon-list3");
                    for (var i = 0; i < divclass.length; i++) {

                        if (divclass[i].id != $(this)[0].parentElement.id) {
                            /*divclass[i].children[0].children[1].fadeOut('fast');
                        }
                    }اینجا

                    if (e.srcElement.childElementCount ==1) {
                        $('.ribbon-list3 div ul').fadeOut('fast');
                    }

                    if (insideX && insideY) {
                        $('.ribbon-list3 div ul').fadeOut('fast');
                        $(this).attr('style', 'z-index:100000;background-image: ' + $(this).css('background-image'));
                        $(this).parent().width(20);
                        $(this).children('ul').width(150 + 4);
                        $(this).children('ul').fadeIn('fast');
                    }
                }
            });*/
            $('.menu5 li ul li div').click(function (e) {
                /*if ($(this).children('div').children('div').children('ul').length > 0) {*/
                aaa = e;

                if (e.target != undefined && e.target.tagName != "LI" && e.target.parentNode.className != "ribbon-sublist5") {
                    $('.ribbon-list5 div ul').fadeOut('fast');
                    $(this).children('div').children('div').children('ul').width(154);
                    $(this).children('div').children('div').children('ul').fadeIn('fast');
                }
            });

            $('.ribbon-list5 div').parents().click(function (e) {

                var outsideX = true;//= e.pageX < $('.ribbon-list5 div ul:visible').parent().offset().left || e.pageX > $('.ribbon-list5 div ul:visible').parent().offset().left + $('.ribbon-list5 div ul:visible').parent().width();
                var outsideY = true;//= e.pageY < $('.ribbon-list5 div ul:visible').parent().offset().top || e.pageY > $('.ribbon-list5 div ul:visible').parent().offset().top + $('.ribbon-list5 div ul:visible').parent().height();

                if (outsideX || outsideY) {
                    $('.ribbon-list5 div ul:visible').each(function () {
                        if (e.srcElement.childElementCount == 1) {
                            $(this).fadeOut('fast');
                        }
                    });
                    $('.ribbon-list5 div').css('background-image', '');
                }
            });

            $('.orb5 li li a').mouseover(function () { ShowOrbChildren(this); });

            $('.menu5 li > a').dblclick(function () {
                $('ul .submenu5').animate({ height: "hide" });
                $('body').animate({ paddingTop: $(this).parent().parent().parent().parent().height() });
                isClosed = true;
            });
        }

        $('.ribbon5').parents().click(function (e) {
            var outsideX = e.pageX < $('.orb5 ul:first').offset().left || e.pageX > $('.orb5 ul:first').offset().left + $('.orb5 ul:first').width();
            var outsideY = e.pageY < $('.orb5 ul:first img:first').offset().top || e.pageY > $('.orb5 ul:first').offset().top + $('.orb5 ul:first').height();

            if (outsideX || outsideY)
                $('.orb5 ul').fadeOut('fast');
        });

        if (isLoaded) {
            $('.orb5 li:first ul:first img:first').remove();
            $('.orb5 li:first ul:first img:last').remove();
            $('.ribbon-list5 div img[src*="/images/arrow_down.png"]').remove();
        }

        $('.orb5 li:first ul:first').prepend('<img src="/ribbon/themes/' + settings.theme + '/images/menu_top.png" style="margin-left: -10px; margin-top: -22px;" />');
        $('.orb5 li:first ul:first').append('<img src="/ribbon/themes/' + settings.theme + '/images/menu_bottom.png" style="margin-left: -10px; margin-bottom: -20px;" />');

        $('.ribbon-list5 div').each(function () { if ($(this).children('ul').length > 0 && $(this).parent('div').length > 0) { $(this).append('<img src="/ribbon/themes/' + settings.theme + '/images/arrow_down.png" style="float: right; margin-top: 5px;" />') } });
        $('.ribbon-sublist5 div').each(function () { if ($(this).children('ul').length > 0) { $(this).append('<img src="/ribbon/themes/' + settings.theme + '/images/arrow.png" style="float: left; margin-top: 5px;" />') } });

        $('.ribbon-sublist5 div').click(function (e) {
            var elwidth = $(this).parent().width();
            var insideX = e.pageX > $(this).offset().left && e.pageX < $(this).offset().left + $(this).width();
            var insideY = e.pageY > $(this).offset().top && e.pageY < $(this).offset().top + $(this).height();

            $('.ribbon-sublist5 div ul').fadeOut('fast');

            if (insideX && insideY) {
                $(this).attr('style', 'z-index:100000;background-image: ' + $(this).css('background-image'));
                $(this).parent().width(150);
                $(this).children('ul').width(150 + 5);
                $(this).children('ul').css("right", $(this).parent().width() + 2);
                $(this).children('ul').css("margin-top", "-19px");
                $(this).children('ul').fadeIn('fast');
            }
        });



        $('.ribbon-list5 div ul li').hover(function (e) { if ($(this).children('div').length == 0 && $(this).parent('ul').parent('div').parent('li').attr('class') != "ribbon-sublist5") { $('.ribbon-sublist5 div ul').fadeOut('fast'); } });

        //Hack for IE 7.
        if (navigator.appVersion.indexOf('MSIE 6.0') > -1 || navigator.appVersion.indexOf('MSIE 7.0') > -1) {
            $('ul.menu5 li li div').css('width', '90px');
            $('ul.menu5').css('width', '500px');
            $('ul.menu5').css('float', 'left');
            $('ul.menu5 .submenu5 li div.ribbon-list5').css('width', '100px');
            $('ul.menu5 .submenu5 li div.ribbon-list5 div').css('width', '100px');
        }

        $('a[href="#' + window.location.hash + '"]').click();

        isLoaded = true;

        function ResetSubMenu() {
            $('.menu5 li a').removeClass('active');
            $('.menu5 ul').removeClass('submenu5');
            $('.menu5 li ul').hide();
        }

        function ShowSubMenu(e) {
            var isActive = $(e).next().css('display') == 'block';
            ResetSubMenu();

            $(e).addClass('active');
            $(e).parent().children('ul:first').addClass('submenu5');

            $(e).parent().children('ul:first').show();
            //$('body').css('padding-top', '120px');

            isClosed = false;
        }

        function ShowOrbChildren(e) {
            if (($(e).parent().children('ul').css('display') == 'none' || $(e).parent().children('ul').length == 0) && $(e).parent().parent().parent().parent().hasClass('orb5')) {
                $('.orb5 li li ul').fadeOut('fast');
                $(e).parent().children('ul').fadeIn('fast');
            }
        }

        function ShowMenu() {
            $('.orb5 ul').animate({ opacity: 'toggle' }, 'fast');
        }

        function ShowAccessKeys(e) {
            if (e.altKey) {
                $('div[rel="accesskeyhelper"]').each(function () { $(this).css('top', $(this).parent().offset().top).css('left', $(this).parent().offset().left - 20 + $(this).parent().width()); $(this).show(); });
            }
        }

        function HideAccessKeys(e) {
            $('div[rel="accesskeyhelper"]').hide();
        }
    }
})(jQuery);


/*****/
(function ($) {
    var isLoaded;
    var isClosed;

    $.fn.Ribbon9 = function (ribbonSettings) {
        var settings = $.extend({ theme: 'windows7' }, ribbonSettings || {});

        $('.ribbon9 a').each(function () {
            if ($(this).attr('accesskey')) {
                $(this).append('<div rel="accesskeyhelper" style="display: none; position: absolute; background-image: url(images/accessbg.png); background-repeat: none; width: 16px; padding: 0px; text-align: center; height: 17px; line-height: 17px; top: ' + $(this).offset().top + 'px; left: ' + ($(this).offset().left + $(this).width() - 15) + 'px;">' + $(this).attr('accesskey') + '</div>');
            }
        });

        if (!isLoaded) {
            SetupMenu(settings);
        }

        $(document).keydown(function (e) { ShowAccessKeys(e); });
        $(document).keyup(function (e) { HideAccessKeys(e); });

        function SetupMenu(settings) {
            $('.menu9 li a:first').addClass('active');
            $('.menu9 li ul').hide();
            $('.menu9 li a:first').parent().children('ul:first').show();
            $('.menu9 li a:first').parent().children('ul:first').addClass('submenu9');
            $('.menu9 li > a').click(function () { ShowSubMenu(this); });
            $('.orb9').click(function () { ShowMenu(); });
            $('.orb9 ul').hide();
            $('.orb9 ul ul').hide();
            $('.orb9 li ul li ul').show();
            $('.orb9 li li ul').each(function () { $(this).prepend('<div style="background-color: #EBF2F7; height: 25px; line-height: 25px; width: 292px; padding-left: 9px; border-bottom: 1px solid #CFDBEB;">' + $(this).parent().children('a:first').text() + '</div>'); });
            $('.orb9 li li a').each(function () { if ($(this).parent().children('ul').length > 0) { $(this).addClass('arrow') } });

            //$('.ribbon-list div').each(function() { $(this).parent().width($(this).parent().width()); });

            /*$('.ribbon-list3 div').click(function (e) {
                if ($(this).parent('div').length > 0) {
                    var elwidth = $(this).parent().width();
                    var insideX = e.pageX > $(this).offset().left && e.pageX < $(this).offset().left + $(this).width();
                    var insideY = e.pageY > $(this).offset().top && e.pageY < $(this).offset().top + $(this).height();

                    /*var divclass = document.getElementById("PayReports").getElementsByClassName("ribbon-list3");
                    for (var i = 0; i < divclass.length; i++) {

                        if (divclass[i].id != $(this)[0].parentElement.id) {
                            /*divclass[i].children[0].children[1].fadeOut('fast');
                        }
                    }اینجا

                    if (e.srcElement.childElementCount ==1) {
                        $('.ribbon-list3 div ul').fadeOut('fast');
                    }

                    if (insideX && insideY) {
                        $('.ribbon-list3 div ul').fadeOut('fast');
                        $(this).attr('style', 'z-index:100000;background-image: ' + $(this).css('background-image'));
                        $(this).parent().width(20);
                        $(this).children('ul').width(150 + 4);
                        $(this).children('ul').fadeIn('fast');
                    }
                }
            });*/
            $('.menu9 li ul li div').click(function (e) {
                /*if ($(this).children('div').children('div').children('ul').length > 0) {*/
                aaa = e;

                if (e.target != undefined && e.target.tagName != "LI" && e.target.parentNode.className != "ribbon-sublist9") {
                    $('.ribbon-list9 div ul').fadeOut('fast');
                    $(this).children('div').children('div').children('ul').width(154);
                    $(this).children('div').children('div').children('ul').fadeIn('fast');
                }
            });

            $('.ribbon-list9 div').parents().click(function (e) {

                var outsideX;//= e.pageX < $('.ribbon-list4 div ul:visible').parent().offset().left || e.pageX > $('.ribbon-list4 div ul:visible').parent().offset().left + $('.ribbon-list4 div ul:visible').parent().width();
                var outsideY;//= e.pageY < $('.ribbon-list4 div ul:visible').parent().offset().top || e.pageY > $('.ribbon-list4 div ul:visible').parent().offset().top + $('.ribbon-list4 div ul:visible').parent().height();

                if (outsideX || outsideY) {
                    $('.ribbon-list9 div ul:visible').each(function () {
                        if (e.srcElement.childElementCount == 1) {
                            $(this).fadeOut('fast');
                        }
                    });
                    $('.ribbon-list9 div').css('background-image', '');
                }
            });

            $('.orb9 li li a').mouseover(function () { ShowOrbChildren(this); });

            $('.menu9 li > a').dblclick(function () {
                $('ul .submenu9').animate({ height: "hide" });
                $('body').animate({ paddingTop: $(this).parent().parent().parent().parent().height() });
                isClosed = true;
            });
        }

        $('.ribbon9').parents().click(function (e) {
            var outsideX = e.pageX < $('.orb9 ul:first').offset().left || e.pageX > $('.orb9 ul:first').offset().left + $('.orb9 ul:first').width();
            var outsideY = e.pageY < $('.orb9 ul:first img:first').offset().top || e.pageY > $('.orb9 ul:first').offset().top + $('.orb9 ul:first').height();

            if (outsideX || outsideY)
                $('.orb9 ul').fadeOut('fast');
        });

        if (isLoaded) {
            $('.orb9 li:first ul:first img:first').remove();
            $('.orb9 li:first ul:first img:last').remove();
            $('.ribbon-list9 div img[src*="/images/arrow_down.png"]').remove();
        }

        $('.orb9 li:first ul:first').prepend('<img src="/ribbon/themes/' + settings.theme + '/images/menu_top.png" style="margin-left: -10px; margin-top: -22px;" />');
        $('.orb9 li:first ul:first').append('<img src="/ribbon/themes/' + settings.theme + '/images/menu_bottom.png" style="margin-left: -10px; margin-bottom: -20px;" />');

        $('.ribbon-list9 div').each(function () { if ($(this).children('ul').length > 0 && $(this).parent('div').length > 0) { $(this).append('<img src="/ribbon/themes/' + settings.theme + '/images/arrow_down.png" style="float: right; margin-top: 5px;" />') } });
        $('.ribbon-sublist9 div').each(function () { if ($(this).children('ul').length > 0) { $(this).append('<img src="/ribbon/themes/' + settings.theme + '/images/arrow.png" style="float: left; margin-top: 5px;" />') } });

        $('.ribbon-sublist9 div').click(function (e) {
            var elwidth = $(this).parent().width();
            var insideX = e.pageX > $(this).offset().left && e.pageX < $(this).offset().left + $(this).width();
            var insideY = e.pageY > $(this).offset().top && e.pageY < $(this).offset().top + $(this).height();

            $('.ribbon-sublist9 div ul').fadeOut('fast');

            if (insideX && insideY) {
                $(this).attr('style', 'z-index:100000;background-image: ' + $(this).css('background-image'));
                $(this).parent().width(150);
                $(this).children('ul').width(150 + 4);
                $(this).children('ul').css("right", $(this).parent().width() + 2);
                $(this).children('ul').css("margin-top", "-19px");
                $(this).children('ul').fadeIn('fast');
            }
        });



        $('.ribbon-list9 div ul li').hover(function (e) { if ($(this).children('div').length == 0 && $(this).parent('ul').parent('div').parent('li').attr('class') != "ribbon-sublist9") { $('.ribbon-sublist9 div ul').fadeOut('fast'); } });

        //Hack for IE 7.
        if (navigator.appVersion.indexOf('MSIE 6.0') > -1 || navigator.appVersion.indexOf('MSIE 7.0') > -1) {
            $('ul.menu9 li li div').css('width', '90px');
            $('ul.menu9').css('width', '500px');
            $('ul.menu9').css('float', 'left');
            $('ul.menu9 .submenu9 li div.ribbon-list9').css('width', '100px');
            $('ul.menu9 .submenu9 li div.ribbon-list9 div').css('width', '100px');
        }

        $('a[href="#' + window.location.hash + '"]').click();

        isLoaded = true;

        function ResetSubMenu() {
            $('.menu9 li a').removeClass('active');
            $('.menu9 ul').removeClass('submenu9');
            $('.menu9 li ul').hide();
        }

        function ShowSubMenu(e) {
            var isActive = $(e).next().css('display') == 'block';
            ResetSubMenu();

            $(e).addClass('active');
            $(e).parent().children('ul:first').addClass('submenu9');

            $(e).parent().children('ul:first').show();
            //$('body').css('padding-top', '120px');

            isClosed = false;
        }

        function ShowOrbChildren(e) {
            if (($(e).parent().children('ul').css('display') == 'none' || $(e).parent().children('ul').length == 0) && $(e).parent().parent().parent().parent().hasClass('orb9')) {
                $('.orb9 li li ul').fadeOut('fast');
                $(e).parent().children('ul').fadeIn('fast');
            }
        }

        function ShowMenu() {
            $('.orb9 ul').animate({ opacity: 'toggle' }, 'fast');
        }

        function ShowAccessKeys(e) {
            if (e.altKey) {
                $('div[rel="accesskeyhelper"]').each(function () { $(this).css('top', $(this).parent().offset().top).css('left', $(this).parent().offset().left - 20 + $(this).parent().width()); $(this).show(); });
            }
        }

        function HideAccessKeys(e) {
            $('div[rel="accesskeyhelper"]').hide();
        }
    }
})(jQuery);
/******/
(function ($) {
    var isLoaded;
    var isClosed;

    $.fn.Ribbon12 = function (ribbonSettings) {
        var settings = $.extend({ theme: 'windows7' }, ribbonSettings || {});

        $('.ribbon12 a').each(function () {
            if ($(this).attr('accesskey')) {
                $(this).append('<div rel="accesskeyhelper" style="display: none; position: absolute; background-image: url(images/accessbg.png); background-repeat: none; width: 16px; padding: 0px; text-align: center; height: 17px; line-height: 17px; top: ' + $(this).offset().top + 'px; left: ' + ($(this).offset().left + $(this).width() - 15) + 'px;">' + $(this).attr('accesskey') + '</div>');
            }
        });

        if (!isLoaded) {
            SetupMenu(settings);
        }

        $(document).keydown(function (e) { ShowAccessKeys(e); });
        $(document).keyup(function (e) { HideAccessKeys(e); });

        function SetupMenu(settings) {
            $('.menu12 li a:first').addClass('active');
            $('.menu12 li ul').hide();
            $('.menu12 li a:first').parent().children('ul:first').show();
            $('.menu12 li a:first').parent().children('ul:first').addClass('submenu12');
            $('.menu12 li > a').click(function () { ShowSubMenu(this); });
            $('.orb12').click(function () { ShowMenu(); });
            $('.orb12 ul').hide();
            $('.orb12 ul ul').hide();
            $('.orb12 li ul li ul').show();
            $('.orb12 li li ul').each(function () { $(this).prepend('<div style="background-color: #EBF2F7; height: 25px; line-height: 25px; width: 292px; padding-left: 9px; border-bottom: 1px solid #CFDBEB;">' + $(this).parent().children('a:first').text() + '</div>'); });
            $('.orb12 li li a').each(function () { if ($(this).parent().children('ul').length > 0) { $(this).addClass('arrow') } });

            //$('.ribbon-list div').each(function() { $(this).parent().width($(this).parent().width()); });

            /*$('.ribbon-list3 div').click(function (e) {
                if ($(this).parent('div').length > 0) {
                    var elwidth = $(this).parent().width();
                    var insideX = e.pageX > $(this).offset().left && e.pageX < $(this).offset().left + $(this).width();
                    var insideY = e.pageY > $(this).offset().top && e.pageY < $(this).offset().top + $(this).height();

                    /*var divclass = document.getElementById("PayReports").getElementsByClassName("ribbon-list3");
                    for (var i = 0; i < divclass.length; i++) {

                        if (divclass[i].id != $(this)[0].parentElement.id) {
                            /*divclass[i].children[0].children[1].fadeOut('fast');
                        }
                    }اینجا

                    if (e.srcElement.childElementCount ==1) {
                        $('.ribbon-list3 div ul').fadeOut('fast');
                    }

                    if (insideX && insideY) {
                        $('.ribbon-list3 div ul').fadeOut('fast');
                        $(this).attr('style', 'z-index:100000;background-image: ' + $(this).css('background-image'));
                        $(this).parent().width(20);
                        $(this).children('ul').width(150 + 4);
                        $(this).children('ul').fadeIn('fast');
                    }
                }
            });*/
            $('.menu12 li ul li div').click(function (e) {
                /*if ($(this).children('div').children('div').children('ul').length > 0) {*/
                aaa = e;

                if (e.target != undefined && e.target.tagName != "LI" && e.target.parentNode.className != "ribbon-sublist12") {
                    $('.ribbon-list12 div ul').fadeOut('fast');
                    $(this).children('div').children('div').children('ul').width(154);
                    $(this).children('div').children('div').children('ul').fadeIn('fast');
                }
            });

            $('.ribbon-list12 div').parents().click(function (e) {

                var outsideX;//= e.pageX < $('.ribbon-list4 div ul:visible').parent().offset().left || e.pageX > $('.ribbon-list4 div ul:visible').parent().offset().left + $('.ribbon-list4 div ul:visible').parent().width();
                var outsideY;//= e.pageY < $('.ribbon-list4 div ul:visible').parent().offset().top || e.pageY > $('.ribbon-list4 div ul:visible').parent().offset().top + $('.ribbon-list4 div ul:visible').parent().height();

                if (outsideX || outsideY) {
                    $('.ribbon-list12 div ul:visible').each(function () {
                        if (e.srcElement.childElementCount == 1) {
                            $(this).fadeOut('fast');
                        }
                    });
                    $('.ribbon-list12 div').css('background-image', '');
                }
            });

            $('.orb12 li li a').mouseover(function () { ShowOrbChildren(this); });

            $('.menu12 li > a').dblclick(function () {
                $('ul .submenu12').animate({ height: "hide" });
                $('body').animate({ paddingTop: $(this).parent().parent().parent().parent().height() });
                isClosed = true;
            });
        }

        $('.ribbon12').parents().click(function (e) {
            var outsideX = e.pageX < $('.orb12 ul:first').offset().left || e.pageX > $('.orb12 ul:first').offset().left + $('.orb12 ul:first').width();
            var outsideY = e.pageY < $('.orb12 ul:first img:first').offset().top || e.pageY > $('.orb12 ul:first').offset().top + $('.orb12 ul:first').height();

            if (outsideX || outsideY)
                $('.orb12 ul').fadeOut('fast');
        });

        if (isLoaded) {
            $('.orb12 li:first ul:first img:first').remove();
            $('.orb12 li:first ul:first img:last').remove();
            $('.ribbon-list12 div img[src*="/images/arrow_down.png"]').remove();
        }

        $('.orb12 li:first ul:first').prepend('<img src="/ribbon/themes/' + settings.theme + '/images/menu_top.png" style="margin-left: -10px; margin-top: -22px;" />');
        $('.orb12 li:first ul:first').append('<img src="/ribbon/themes/' + settings.theme + '/images/menu_bottom.png" style="margin-left: -10px; margin-bottom: -20px;" />');

        $('.ribbon-list12 div').each(function () { if ($(this).children('ul').length > 0 && $(this).parent('div').length > 0) { $(this).append('<img src="/ribbon/themes/' + settings.theme + '/images/arrow_down.png" style="float: right; margin-top: 5px;" />') } });
        $('.ribbon-sublist12 div').each(function () { if ($(this).children('ul').length > 0) { $(this).append('<img src="/ribbon/themes/' + settings.theme + '/images/arrow.png" style="float: left; margin-top: 5px;" />') } });

        $('.ribbon-sublist12 div').click(function (e) {
            var elwidth = $(this).parent().width();
            var insideX = e.pageX > $(this).offset().left && e.pageX < $(this).offset().left + $(this).width();
            var insideY = e.pageY > $(this).offset().top && e.pageY < $(this).offset().top + $(this).height();

            $('.ribbon-sublist12 div ul').fadeOut('fast');

            if (insideX && insideY) {
                $(this).attr('style', 'z-index:100000;background-image: ' + $(this).css('background-image'));
                $(this).parent().width(150);
                $(this).children('ul').width(150 + 4);
                $(this).children('ul').css("right", $(this).parent().width() + 2);
                $(this).children('ul').css("margin-top", "-19px");
                $(this).children('ul').fadeIn('fast');
            }
        });



        $('.ribbon-list12 div ul li').hover(function (e) { if ($(this).children('div').length == 0 && $(this).parent('ul').parent('div').parent('li').attr('class') != "ribbon-sublist12") { $('.ribbon-sublist12 div ul').fadeOut('fast'); } });

        //Hack for IE 7.
        if (navigator.appVersion.indexOf('MSIE 6.0') > -1 || navigator.appVersion.indexOf('MSIE 7.0') > -1) {
            $('ul.menu12 li li div').css('width', '90px');
            $('ul.menu12').css('width', '500px');
            $('ul.menu12').css('float', 'left');
            $('ul.menu12 .submenu12 li div.ribbon-list12').css('width', '100px');
            $('ul.menu12 .submenu12 li div.ribbon-list12 div').css('width', '100px');
        }

        $('a[href="#' + window.location.hash + '"]').click();

        isLoaded = true;

        function ResetSubMenu() {
            $('.menu12 li a').removeClass('active');
            $('.menu12 ul').removeClass('submenu12');
            $('.menu12 li ul').hide();
        }

        function ShowSubMenu(e) {
            var isActive = $(e).next().css('display') == 'block';
            ResetSubMenu();

            $(e).addClass('active');
            $(e).parent().children('ul:first').addClass('submenu12');

            $(e).parent().children('ul:first').show();
            //$('body').css('padding-top', '120px');

            isClosed = false;
        }

        function ShowOrbChildren(e) {
            if (($(e).parent().children('ul').css('display') == 'none' || $(e).parent().children('ul').length == 0) && $(e).parent().parent().parent().parent().hasClass('orb12')) {
                $('.orb12 li li ul').fadeOut('fast');
                $(e).parent().children('ul').fadeIn('fast');
            }
        }

        function ShowMenu() {
            $('.orb12 ul').animate({ opacity: 'toggle' }, 'fast');
        }

        function ShowAccessKeys(e) {
            if (e.altKey) {
                $('div[rel="accesskeyhelper"]').each(function () { $(this).css('top', $(this).parent().offset().top).css('left', $(this).parent().offset().left - 20 + $(this).parent().width()); $(this).show(); });
            }
        }

        function HideAccessKeys(e) {
            $('div[rel="accesskeyhelper"]').hide();
        }
    }
})(jQuery);

/******************/
(function ($) {
    var isLoaded;
    var isClosed;

    $.fn.Ribbon13 = function (ribbonSettings) {
        var settings = $.extend({ theme: 'windows7' }, ribbonSettings || {});

        $('.ribbon13 a').each(function () {
            if ($(this).attr('accesskey')) {
                $(this).append('<div rel="accesskeyhelper" style="display: none; position: absolute; background-image: url(images/accessbg.png); background-repeat: none; width: 16px; padding: 0px; text-align: center; height: 17px; line-height: 17px; top: ' + $(this).offset().top + 'px; left: ' + ($(this).offset().left + $(this).width() - 15) + 'px;">' + $(this).attr('accesskey') + '</div>');
            }
        });

        if (!isLoaded) {
            SetupMenu(settings);
        }

        $(document).keydown(function (e) { ShowAccessKeys(e); });
        $(document).keyup(function (e) { HideAccessKeys(e); });

        function SetupMenu(settings) {
            $('.menu13 li a:first').addClass('active');
            $('.menu13 li ul').hide();
            $('.menu13 li a:first').parent().children('ul:first').show();
            $('.menu13 li a:first').parent().children('ul:first').addClass('submenu13');
            $('.menu13 li > a').click(function () { ShowSubMenu(this); });
            $('.orb13').click(function () { ShowMenu(); });
            $('.orb13 ul').hide();
            $('.orb13 ul ul').hide();
            $('.orb13 li ul li ul').show();
            $('.orb13 li li ul').each(function () { $(this).prepend('<div style="background-color: #EBF2F7; height: 25px; line-height: 25px; width: 292px; padding-left: 9px; border-bottom: 1px solid #CFDBEB;">' + $(this).parent().children('a:first').text() + '</div>'); });
            $('.orb13 li li a').each(function () { if ($(this).parent().children('ul').length > 0) { $(this).addClass('arrow') } });

            //$('.ribbon-list div').each(function() { $(this).parent().width($(this).parent().width()); });

            /*$('.ribbon-list3 div').click(function (e) {
                if ($(this).parent('div').length > 0) {
                    var elwidth = $(this).parent().width();
                    var insideX = e.pageX > $(this).offset().left && e.pageX < $(this).offset().left + $(this).width();
                    var insideY = e.pageY > $(this).offset().top && e.pageY < $(this).offset().top + $(this).height();

                    /*var divclass = document.getElementById("PayReports").getElementsByClassName("ribbon-list3");
                    for (var i = 0; i < divclass.length; i++) {

                        if (divclass[i].id != $(this)[0].parentElement.id) {
                            /*divclass[i].children[0].children[1].fadeOut('fast');
                        }
                    }اینجا

                    if (e.srcElement.childElementCount ==1) {
                        $('.ribbon-list3 div ul').fadeOut('fast');
                    }

                    if (insideX && insideY) {
                        $('.ribbon-list3 div ul').fadeOut('fast');
                        $(this).attr('style', 'z-index:100000;background-image: ' + $(this).css('background-image'));
                        $(this).parent().width(20);
                        $(this).children('ul').width(150 + 4);
                        $(this).children('ul').fadeIn('fast');
                    }
                }
            });*/
            $('.menu13 li ul li div').click(function (e) {
                /*if ($(this).children('div').children('div').children('ul').length > 0) {*/
                aaa = e;

                if (e.target != undefined && e.target.tagName != "LI" && e.target.parentNode.className != "ribbon-sublist13") {
                    $('.ribbon-list13 div ul').fadeOut('fast');
                    $(this).children('div').children('div').children('ul').width(154);
                    $(this).children('div').children('div').children('ul').fadeIn('fast');
                }
            });

            $('.ribbon-list13 div').parents().click(function (e) {

                var outsideX;//= e.pageX < $('.ribbon-list4 div ul:visible').parent().offset().left || e.pageX > $('.ribbon-list4 div ul:visible').parent().offset().left + $('.ribbon-list4 div ul:visible').parent().width();
                var outsideY;//= e.pageY < $('.ribbon-list4 div ul:visible').parent().offset().top || e.pageY > $('.ribbon-list4 div ul:visible').parent().offset().top + $('.ribbon-list4 div ul:visible').parent().height();

                if (outsideX || outsideY) {
                    $('.ribbon-list13 div ul:visible').each(function () {
                        if (e.srcElement.childElementCount == 1) {
                            $(this).fadeOut('fast');
                        }
                    });
                    $('.ribbon-list13 div').css('background-image', '');
                }
            });

            $('.orb13 li li a').mouseover(function () { ShowOrbChildren(this); });

            $('.menu13 li > a').dblclick(function () {
                $('ul .submenu13').animate({ height: "hide" });
                $('body').animate({ paddingTop: $(this).parent().parent().parent().parent().height() });
                isClosed = true;
            });
        }

        $('.ribbon13').parents().click(function (e) {
            var outsideX = e.pageX < $('.orb13 ul:first').offset().left || e.pageX > $('.orb13 ul:first').offset().left + $('.orb13 ul:first').width();
            var outsideY = e.pageY < $('.orb13 ul:first img:first').offset().top || e.pageY > $('.orb13 ul:first').offset().top + $('.orb13 ul:first').height();

            if (outsideX || outsideY)
                $('.orb13 ul').fadeOut('fast');
        });

        if (isLoaded) {
            $('.orb13 li:first ul:first img:first').remove();
            $('.orb13 li:first ul:first img:last').remove();
            $('.ribbon-list13 div img[src*="/images/arrow_down.png"]').remove();
        }

        $('.orb13 li:first ul:first').prepend('<img src="/ribbon/themes/' + settings.theme + '/images/menu_top.png" style="margin-left: -10px; margin-top: -22px;" />');
        $('.orb13 li:first ul:first').append('<img src="/ribbon/themes/' + settings.theme + '/images/menu_bottom.png" style="margin-left: -10px; margin-bottom: -20px;" />');

        $('.ribbon-list13 div').each(function () { if ($(this).children('ul').length > 0 && $(this).parent('div').length > 0) { $(this).append('<img src="/ribbon/themes/' + settings.theme + '/images/arrow_down.png" style="float: right; margin-top: 5px;" />') } });
        $('.ribbon-sublist13 div').each(function () { if ($(this).children('ul').length > 0) { $(this).append('<img src="/ribbon/themes/' + settings.theme + '/images/arrow.png" style="float: left; margin-top: 5px;" />') } });

        $('.ribbon-sublist13 div').click(function (e) {
            var elwidth = $(this).parent().width();
            var insideX = e.pageX > $(this).offset().left && e.pageX < $(this).offset().left + $(this).width();
            var insideY = e.pageY > $(this).offset().top && e.pageY < $(this).offset().top + $(this).height();

            $('.ribbon-sublist13 div ul').fadeOut('fast');

            if (insideX && insideY) {
                $(this).attr('style', 'z-index:100000;background-image: ' + $(this).css('background-image'));
                $(this).parent().width(150);
                $(this).children('ul').width(150 + 4);
                $(this).children('ul').css("right", $(this).parent().width() + 2);
                $(this).children('ul').css("margin-top", "-19px");
                $(this).children('ul').fadeIn('fast');
            }
        });



        $('.ribbon-list13 div ul li').hover(function (e) { if ($(this).children('div').length == 0 && $(this).parent('ul').parent('div').parent('li').attr('class') != "ribbon-sublist13") { $('.ribbon-sublist13 div ul').fadeOut('fast'); } });

        //Hack for IE 7.
        if (navigator.appVersion.indexOf('MSIE 6.0') > -1 || navigator.appVersion.indexOf('MSIE 7.0') > -1) {
            $('ul.menu13 li li div').css('width', '90px');
            $('ul.menu13').css('width', '500px');
            $('ul.menu13').css('float', 'left');
            $('ul.menu13 .submenu13 li div.ribbon-list13').css('width', '100px');
            $('ul.menu13 .submenu13 li div.ribbon-list13 div').css('width', '100px');
        }

        $('a[href="#' + window.location.hash + '"]').click();

        isLoaded = true;

        function ResetSubMenu() {
            $('.menu13 li a').removeClass('active');
            $('.menu13 ul').removeClass('submenu13');
            $('.menu13 li ul').hide();
        }

        function ShowSubMenu(e) {
            var isActive = $(e).next().css('display') == 'block';
            ResetSubMenu();

            $(e).addClass('active');
            $(e).parent().children('ul:first').addClass('submenu13');

            $(e).parent().children('ul:first').show();
            //$('body').css('padding-top', '120px');

            isClosed = false;
        }

        function ShowOrbChildren(e) {
            if (($(e).parent().children('ul').css('display') == 'none' || $(e).parent().children('ul').length == 0) && $(e).parent().parent().parent().parent().hasClass('orb13')) {
                $('.orb13 li li ul').fadeOut('fast');
                $(e).parent().children('ul').fadeIn('fast');
            }
        }

        function ShowMenu() {
            $('.orb13 ul').animate({ opacity: 'toggle' }, 'fast');
        }

        function ShowAccessKeys(e) {
            if (e.altKey) {
                $('div[rel="accesskeyhelper"]').each(function () { $(this).css('top', $(this).parent().offset().top).css('left', $(this).parent().offset().left - 20 + $(this).parent().width()); $(this).show(); });
            }
        }

        function HideAccessKeys(e) {
            $('div[rel="accesskeyhelper"]').hide();
        }
    }
})(jQuery);

/******/
(function ($) {
    var isLoaded;
    var isClosed;

    $.fn.Ribbon14 = function (ribbonSettings) {
        var settings = $.extend({ theme: 'windows7' }, ribbonSettings || {});

        $('.ribbon14 a').each(function () {
            if ($(this).attr('accesskey')) {
                $(this).append('<div rel="accesskeyhelper" style="display: none; position: absolute; background-image: url(images/accessbg.png); background-repeat: none; width: 16px; padding: 0px; text-align: center; height: 17px; line-height: 17px; top: ' + $(this).offset().top + 'px; left: ' + ($(this).offset().left + $(this).width() - 15) + 'px;">' + $(this).attr('accesskey') + '</div>');
            }
        });

        if (!isLoaded) {
            SetupMenu(settings);
        }

        $(document).keydown(function (e) { ShowAccessKeys(e); });
        $(document).keyup(function (e) { HideAccessKeys(e); });

        function SetupMenu(settings) {
            $('.menu14 li a:first').addClass('active');
            $('.menu14 li ul').hide();
            $('.menu14 li a:first').parent().children('ul:first').show();
            $('.menu14 li a:first').parent().children('ul:first').addClass('submenu14');
            $('.menu14 li > a').click(function () { ShowSubMenu(this); });
            $('.orb14').click(function () { ShowMenu(); });
            $('.orb14 ul').hide();
            $('.orb14 ul ul').hide();
            $('.orb14 li ul li ul').show();
            $('.orb14 li li ul').each(function () { $(this).prepend('<div style="background-color: #EBF2F7; height: 25px; line-height: 25px; width: 292px; padding-left: 9px; border-bottom: 1px solid #CFDBEB;">' + $(this).parent().children('a:first').text() + '</div>'); });
            $('.orb14 li li a').each(function () { if ($(this).parent().children('ul').length > 0) { $(this).addClass('arrow') } });

            //$('.ribbon-list div').each(function() { $(this).parent().width($(this).parent().width()); });

            /*$('.ribbon-list3 div').click(function (e) {
                if ($(this).parent('div').length > 0) {
                    var elwidth = $(this).parent().width();
                    var insideX = e.pageX > $(this).offset().left && e.pageX < $(this).offset().left + $(this).width();
                    var insideY = e.pageY > $(this).offset().top && e.pageY < $(this).offset().top + $(this).height();

                    /*var divclass = document.getElementById("PayReports").getElementsByClassName("ribbon-list3");
                    for (var i = 0; i < divclass.length; i++) {

                        if (divclass[i].id != $(this)[0].parentElement.id) {
                            /*divclass[i].children[0].children[1].fadeOut('fast');
                        }
                    }اینجا

                    if (e.srcElement.childElementCount ==1) {
                        $('.ribbon-list3 div ul').fadeOut('fast');
                    }

                    if (insideX && insideY) {
                        $('.ribbon-list3 div ul').fadeOut('fast');
                        $(this).attr('style', 'z-index:100000;background-image: ' + $(this).css('background-image'));
                        $(this).parent().width(20);
                        $(this).children('ul').width(150 + 4);
                        $(this).children('ul').fadeIn('fast');
                    }
                }
            });*/
            $('.menu14 li ul li div').click(function (e) {
                /*if ($(this).children('div').children('div').children('ul').length > 0) {*/
                aaa = e;

                if (e.target != undefined && e.target.tagName != "LI" && e.target.parentNode.className != "ribbon-sublist14") {
                    $('.ribbon-list14 div ul').fadeOut('fast');
                    $(this).children('div').children('div').children('ul').width(154);
                    $(this).children('div').children('div').children('ul').fadeIn('fast');
                }
            });

            $('.ribbon-list14 div').parents().click(function (e) {

                var outsideX;//= e.pageX < $('.ribbon-list4 div ul:visible').parent().offset().left || e.pageX > $('.ribbon-list4 div ul:visible').parent().offset().left + $('.ribbon-list4 div ul:visible').parent().width();
                var outsideY;//= e.pageY < $('.ribbon-list4 div ul:visible').parent().offset().top || e.pageY > $('.ribbon-list4 div ul:visible').parent().offset().top + $('.ribbon-list4 div ul:visible').parent().height();

                if (outsideX || outsideY) {
                    $('.ribbon-list14 div ul:visible').each(function () {
                        if (e.srcElement.childElementCount == 1) {
                            $(this).fadeOut('fast');
                        }
                    });
                    $('.ribbon-list14 div').css('background-image', '');
                }
            });

            $('.orb14 li li a').mouseover(function () { ShowOrbChildren(this); });

            $('.menu14 li > a').dblclick(function () {
                $('ul .submenu14').animate({ height: "hide" });
                $('body').animate({ paddingTop: $(this).parent().parent().parent().parent().height() });
                isClosed = true;
            });
        }

        $('.ribbon14').parents().click(function (e) {
            var outsideX = e.pageX < $('.orb14 ul:first').offset().left || e.pageX > $('.orb14 ul:first').offset().left + $('.orb14 ul:first').width();
            var outsideY = e.pageY < $('.orb14 ul:first img:first').offset().top || e.pageY > $('.orb14 ul:first').offset().top + $('.orb14 ul:first').height();

            if (outsideX || outsideY)
                $('.orb14 ul').fadeOut('fast');
        });

        if (isLoaded) {
            $('.orb14 li:first ul:first img:first').remove();
            $('.orb14 li:first ul:first img:last').remove();
            $('.ribbon-list14 div img[src*="/images/arrow_down.png"]').remove();
        }

        $('.orb14 li:first ul:first').prepend('<img src="/ribbon/themes/' + settings.theme + '/images/menu_top.png" style="margin-left: -10px; margin-top: -22px;" />');
        $('.orb14 li:first ul:first').append('<img src="/ribbon/themes/' + settings.theme + '/images/menu_bottom.png" style="margin-left: -10px; margin-bottom: -20px;" />');

        $('.ribbon-list14 div').each(function () { if ($(this).children('ul').length > 0 && $(this).parent('div').length > 0) { $(this).append('<img src="/ribbon/themes/' + settings.theme + '/images/arrow_down.png" style="float: right; margin-top: 5px;" />') } });
        $('.ribbon-sublist14 div').each(function () { if ($(this).children('ul').length > 0) { $(this).append('<img src="/ribbon/themes/' + settings.theme + '/images/arrow.png" style="float: left; margin-top: 5px;" />') } });

        $('.ribbon-sublist14 div').click(function (e) {
            var elwidth = $(this).parent().width();
            var insideX = e.pageX > $(this).offset().left && e.pageX < $(this).offset().left + $(this).width();
            var insideY = e.pageY > $(this).offset().top && e.pageY < $(this).offset().top + $(this).height();

            $('.ribbon-sublist14 div ul').fadeOut('fast');

            if (insideX && insideY) {
                $(this).attr('style', 'z-index:100000;background-image: ' + $(this).css('background-image'));
                $(this).parent().width(150);
                $(this).children('ul').width(150 + 4);
                $(this).children('ul').css("right", $(this).parent().width() + 2);
                $(this).children('ul').css("margin-top", "-19px");
                $(this).children('ul').fadeIn('fast');
            }
        });



        $('.ribbon-list14 div ul li').hover(function (e) { if ($(this).children('div').length == 0 && $(this).parent('ul').parent('div').parent('li').attr('class') != "ribbon-sublist14") { $('.ribbon-sublist14 div ul').fadeOut('fast'); } });

        //Hack for IE 7.
        if (navigator.appVersion.indexOf('MSIE 6.0') > -1 || navigator.appVersion.indexOf('MSIE 7.0') > -1) {
            $('ul.menu14 li li div').css('width', '90px');
            $('ul.menu14').css('width', '500px');
            $('ul.menu14').css('float', 'left');
            $('ul.menu14 .submenu14 li div.ribbon-list14').css('width', '100px');
            $('ul.menu14 .submenu14 li div.ribbon-list14 div').css('width', '100px');
        }

        $('a[href="#' + window.location.hash + '"]').click();

        isLoaded = true;

        function ResetSubMenu() {
            $('.menu14 li a').removeClass('active');
            $('.menu14 ul').removeClass('submenu14');
            $('.menu14 li ul').hide();
        }

        function ShowSubMenu(e) {
            var isActive = $(e).next().css('display') == 'block';
            ResetSubMenu();

            $(e).addClass('active');
            $(e).parent().children('ul:first').addClass('submenu14');

            $(e).parent().children('ul:first').show();
            //$('body').css('padding-top', '120px');

            isClosed = false;
        }

        function ShowOrbChildren(e) {
            if (($(e).parent().children('ul').css('display') == 'none' || $(e).parent().children('ul').length == 0) && $(e).parent().parent().parent().parent().hasClass('orb14')) {
                $('.orb14 li li ul').fadeOut('fast');
                $(e).parent().children('ul').fadeIn('fast');
            }
        }

        function ShowMenu() {
            $('.orb14 ul').animate({ opacity: 'toggle' }, 'fast');
        }

        function ShowAccessKeys(e) {
            if (e.altKey) {
                $('div[rel="accesskeyhelper"]').each(function () { $(this).css('top', $(this).parent().offset().top).css('left', $(this).parent().offset().left - 20 + $(this).parent().width()); $(this).show(); });
            }
        }

        function HideAccessKeys(e) {
            $('div[rel="accesskeyhelper"]').hide();
        }
    }
})(jQuery);

/******************/
(function ($) {
    var isLoaded;
    var isClosed;

    $.fn.Ribbon15 = function (ribbonSettings) {
        var settings = $.extend({ theme: 'windows7' }, ribbonSettings || {});

        $('.ribbon15 a').each(function () {
            if ($(this).attr('accesskey')) {
                $(this).append('<div rel="accesskeyhelper" style="display: none; position: absolute; background-image: url(images/accessbg.png); background-repeat: none; width: 16px; padding: 0px; text-align: center; height: 17px; line-height: 17px; top: ' + $(this).offset().top + 'px; left: ' + ($(this).offset().left + $(this).width() - 15) + 'px;">' + $(this).attr('accesskey') + '</div>');
            }
        });

        if (!isLoaded) {
            SetupMenu(settings);
        }

        $(document).keydown(function (e) { ShowAccessKeys(e); });
        $(document).keyup(function (e) { HideAccessKeys(e); });

        function SetupMenu(settings) {
            $('.menu15 li a:first').addClass('active');
            $('.menu15 li ul').hide();
            $('.menu15 li a:first').parent().children('ul:first').show();
            $('.menu15 li a:first').parent().children('ul:first').addClass('submenu15');
            $('.menu15 li > a').click(function () { ShowSubMenu(this); });
            $('.orb15').click(function () { ShowMenu(); });
            $('.orb15 ul').hide();
            $('.orb15 ul ul').hide();
            $('.orb15 li ul li ul').show();
            $('.orb15 li li ul').each(function () { $(this).prepend('<div style="background-color: #EBF2F7; height: 25px; line-height: 25px; width: 292px; padding-left: 9px; border-bottom: 1px solid #CFDBEB;">' + $(this).parent().children('a:first').text() + '</div>'); });
            $('.orb15 li li a').each(function () { if ($(this).parent().children('ul').length > 0) { $(this).addClass('arrow') } });

            //$('.ribbon-list div').each(function() { $(this).parent().width($(this).parent().width()); });

            /*$('.ribbon-list3 div').click(function (e) {
                if ($(this).parent('div').length > 0) {
                    var elwidth = $(this).parent().width();
                    var insideX = e.pageX > $(this).offset().left && e.pageX < $(this).offset().left + $(this).width();
                    var insideY = e.pageY > $(this).offset().top && e.pageY < $(this).offset().top + $(this).height();

                    /*var divclass = document.getElementById("PayReports").getElementsByClassName("ribbon-list3");
                    for (var i = 0; i < divclass.length; i++) {

                        if (divclass[i].id != $(this)[0].parentElement.id) {
                            /*divclass[i].children[0].children[1].fadeOut('fast');
                        }
                    }اینجا

                    if (e.srcElement.childElementCount ==1) {
                        $('.ribbon-list3 div ul').fadeOut('fast');
                    }

                    if (insideX && insideY) {
                        $('.ribbon-list3 div ul').fadeOut('fast');
                        $(this).attr('style', 'z-index:100000;background-image: ' + $(this).css('background-image'));
                        $(this).parent().width(20);
                        $(this).children('ul').width(150 + 4);
                        $(this).children('ul').fadeIn('fast');
                    }
                }
            });*/
            $('.menu15 li ul li div').click(function (e) {
                /*if ($(this).children('div').children('div').children('ul').length > 0) {*/
                aaa = e;

                if (e.target != undefined && e.target.tagName != "LI" && e.target.parentNode.className != "ribbon-sublist15") {
                    $('.ribbon-list15 div ul').fadeOut('fast');
                    $(this).children('div').children('div').children('ul').width(154);
                    $(this).children('div').children('div').children('ul').fadeIn('fast');
                }
            });

            $('.ribbon-list15 div').parents().click(function (e) {

                var outsideX;//= e.pageX < $('.ribbon-list4 div ul:visible').parent().offset().left || e.pageX > $('.ribbon-list4 div ul:visible').parent().offset().left + $('.ribbon-list4 div ul:visible').parent().width();
                var outsideY;//= e.pageY < $('.ribbon-list4 div ul:visible').parent().offset().top || e.pageY > $('.ribbon-list4 div ul:visible').parent().offset().top + $('.ribbon-list4 div ul:visible').parent().height();

                if (outsideX || outsideY) {
                    $('.ribbon-list15 div ul:visible').each(function () {
                        if (e.srcElement.childElementCount == 1) {
                            $(this).fadeOut('fast');
                        }
                    });
                    $('.ribbon-list15 div').css('background-image', '');
                }
            });

            $('.orb15 li li a').mouseover(function () { ShowOrbChildren(this); });

            $('.menu15 li > a').dblclick(function () {
                $('ul .submenu15').animate({ height: "hide" });
                $('body').animate({ paddingTop: $(this).parent().parent().parent().parent().height() });
                isClosed = true;
            });
        }

        $('.ribbon15').parents().click(function (e) {
            var outsideX = e.pageX < $('.orb15 ul:first').offset().left || e.pageX > $('.orb15 ul:first').offset().left + $('.orb15 ul:first').width();
            var outsideY = e.pageY < $('.orb15 ul:first img:first').offset().top || e.pageY > $('.orb15 ul:first').offset().top + $('.orb15 ul:first').height();

            if (outsideX || outsideY)
                $('.orb15 ul').fadeOut('fast');
        });

        if (isLoaded) {
            $('.orb15 li:first ul:first img:first').remove();
            $('.orb15 li:first ul:first img:last').remove();
            $('.ribbon-list15 div img[src*="/images/arrow_down.png"]').remove();
        }

        $('.orb15 li:first ul:first').prepend('<img src="/ribbon/themes/' + settings.theme + '/images/menu_top.png" style="margin-left: -10px; margin-top: -22px;" />');
        $('.orb15 li:first ul:first').append('<img src="/ribbon/themes/' + settings.theme + '/images/menu_bottom.png" style="margin-left: -10px; margin-bottom: -20px;" />');

        $('.ribbon-list15 div').each(function () { if ($(this).children('ul').length > 0 && $(this).parent('div').length > 0) { $(this).append('<img src="/ribbon/themes/' + settings.theme + '/images/arrow_down.png" style="float: right; margin-top: 5px;" />') } });
        $('.ribbon-sublist15 div').each(function () { if ($(this).children('ul').length > 0) { $(this).append('<img src="/ribbon/themes/' + settings.theme + '/images/arrow.png" style="float: left; margin-top: 5px;" />') } });

        $('.ribbon-sublist15 div').click(function (e) {
            var elwidth = $(this).parent().width();
            var insideX = e.pageX > $(this).offset().left && e.pageX < $(this).offset().left + $(this).width();
            var insideY = e.pageY > $(this).offset().top && e.pageY < $(this).offset().top + $(this).height();

            $('.ribbon-sublist15 div ul').fadeOut('fast');

            if (insideX && insideY) {
                $(this).attr('style', 'z-index:100000;background-image: ' + $(this).css('background-image'));
                $(this).parent().width(150);
                $(this).children('ul').width(150 + 4);
                $(this).children('ul').css("right", $(this).parent().width() + 2);
                $(this).children('ul').css("margin-top", "-19px");
                $(this).children('ul').fadeIn('fast');
            }
        });



        $('.ribbon-list15 div ul li').hover(function (e) { if ($(this).children('div').length == 0 && $(this).parent('ul').parent('div').parent('li').attr('class') != "ribbon-sublist15") { $('.ribbon-sublist15 div ul').fadeOut('fast'); } });

        //Hack for IE 7.
        if (navigator.appVersion.indexOf('MSIE 6.0') > -1 || navigator.appVersion.indexOf('MSIE 7.0') > -1) {
            $('ul.menu15 li li div').css('width', '90px');
            $('ul.menu15').css('width', '500px');
            $('ul.menu15').css('float', 'left');
            $('ul.menu15 .submenu15 li div.ribbon-list15').css('width', '100px');
            $('ul.menu15 .submenu15 li div.ribbon-list15 div').css('width', '100px');
        }

        $('a[href="#' + window.location.hash + '"]').click();

        isLoaded = true;

        function ResetSubMenu() {
            $('.menu15 li a').removeClass('active');
            $('.menu15 ul').removeClass('submenu15');
            $('.menu15 li ul').hide();
        }

        function ShowSubMenu(e) {
            var isActive = $(e).next().css('display') == 'block';
            ResetSubMenu();

            $(e).addClass('active');
            $(e).parent().children('ul:first').addClass('submenu15');

            $(e).parent().children('ul:first').show();
            //$('body').css('padding-top', '120px');

            isClosed = false;
        }

        function ShowOrbChildren(e) {
            if (($(e).parent().children('ul').css('display') == 'none' || $(e).parent().children('ul').length == 0) && $(e).parent().parent().parent().parent().hasClass('orb15')) {
                $('.orb15 li li ul').fadeOut('fast');
                $(e).parent().children('ul').fadeIn('fast');
            }
        }

        function ShowMenu() {
            $('.orb15 ul').animate({ opacity: 'toggle' }, 'fast');
        }

        function ShowAccessKeys(e) {
            if (e.altKey) {
                $('div[rel="accesskeyhelper"]').each(function () { $(this).css('top', $(this).parent().offset().top).css('left', $(this).parent().offset().left - 20 + $(this).parent().width()); $(this).show(); });
            }
        }

        function HideAccessKeys(e) {
            $('div[rel="accesskeyhelper"]').hide();
        }
    }
})(jQuery);
/******************16*/
(function ($) {
    var isLoaded;
    var isClosed;

    $.fn.Ribbon16 = function (ribbonSettings) {
        var settings = $.extend({ theme: 'windows7' }, ribbonSettings || {});

        $('.ribbon16 a').each(function () {
            if ($(this).attr('accesskey')) {
                $(this).append('<div rel="accesskeyhelper" style="display: none; position: absolute; background-image: url(images/accessbg.png); background-repeat: none; width: 16px; padding: 0px; text-align: center; height: 17px; line-height: 17px; top: ' + $(this).offset().top + 'px; left: ' + ($(this).offset().left + $(this).width() - 15) + 'px;">' + $(this).attr('accesskey') + '</div>');
            }
        });

        if (!isLoaded) {
            SetupMenu(settings);
        }

        $(document).keydown(function (e) { ShowAccessKeys(e); });
        $(document).keyup(function (e) { HideAccessKeys(e); });

        function SetupMenu(settings) {
            $('.menu16 li a:first').addClass('active');
            $('.menu16 li ul').hide();
            $('.menu16 li a:first').parent().children('ul:first').show();
            $('.menu16 li a:first').parent().children('ul:first').addClass('submenu16');
            $('.menu16 li > a').click(function () { ShowSubMenu(this); });
            $('.orb16').click(function () { ShowMenu(); });
            $('.orb16 ul').hide();
            $('.orb16 ul ul').hide();
            $('.orb16 li ul li ul').show();
            $('.orb16 li li ul').each(function () { $(this).prepend('<div style="background-color: #EBF2F7; height: 25px; line-height: 25px; width: 292px; padding-left: 9px; border-bottom: 1px solid #CFDBEB;">' + $(this).parent().children('a:first').text() + '</div>'); });
            $('.orb16 li li a').each(function () { if ($(this).parent().children('ul').length > 0) { $(this).addClass('arrow') } });

            //$('.ribbon-list div').each(function() { $(this).parent().width($(this).parent().width()); });

            /*$('.ribbon-list3 div').click(function (e) {
                if ($(this).parent('div').length > 0) {
                    var elwidth = $(this).parent().width();
                    var insideX = e.pageX > $(this).offset().left && e.pageX < $(this).offset().left + $(this).width();
                    var insideY = e.pageY > $(this).offset().top && e.pageY < $(this).offset().top + $(this).height();

                    /*var divclass = document.getElementById("PayReports").getElementsByClassName("ribbon-list3");
                    for (var i = 0; i < divclass.length; i++) {

                        if (divclass[i].id != $(this)[0].parentElement.id) {
                            /*divclass[i].children[0].children[1].fadeOut('fast');
                        }
                    }اینجا

                    if (e.srcElement.childElementCount ==1) {
                        $('.ribbon-list3 div ul').fadeOut('fast');
                    }

                    if (insideX && insideY) {
                        $('.ribbon-list3 div ul').fadeOut('fast');
                        $(this).attr('style', 'z-index:100000;background-image: ' + $(this).css('background-image'));
                        $(this).parent().width(20);
                        $(this).children('ul').width(150 + 4);
                        $(this).children('ul').fadeIn('fast');
                    }
                }
            });*/
            $('.menu16 li ul li div').click(function (e) {
                /*if ($(this).children('div').children('div').children('ul').length > 0) {*/
                aaa = e;

                if (e.target != undefined && e.target.tagName != "LI" && e.target.parentNode.className != "ribbon-sublist16") {
                    $('.ribbon-list16 div ul').fadeOut('fast');
                    $(this).children('div').children('div').children('ul').width(154);
                    $(this).children('div').children('div').children('ul').fadeIn('fast');
                }
            });

            $('.ribbon-list16 div').parents().click(function (e) {

                var outsideX;//= e.pageX < $('.ribbon-list4 div ul:visible').parent().offset().left || e.pageX > $('.ribbon-list4 div ul:visible').parent().offset().left + $('.ribbon-list4 div ul:visible').parent().width();
                var outsideY;//= e.pageY < $('.ribbon-list4 div ul:visible').parent().offset().top || e.pageY > $('.ribbon-list4 div ul:visible').parent().offset().top + $('.ribbon-list4 div ul:visible').parent().height();

                if (outsideX || outsideY) {
                    $('.ribbon-list16 div ul:visible').each(function () {
                        if (e.srcElement.childElementCount == 1) {
                            $(this).fadeOut('fast');
                        }
                    });
                    $('.ribbon-list16 div').css('background-image', '');
                }
            });

            $('.orb16 li li a').mouseover(function () { ShowOrbChildren(this); });

            $('.menu16 li > a').dblclick(function () {
                $('ul .submenu16').animate({ height: "hide" });
                $('body').animate({ paddingTop: $(this).parent().parent().parent().parent().height() });
                isClosed = true;
            });
        }

        $('.ribbon16').parents().click(function (e) {
            var outsideX = e.pageX < $('.orb16 ul:first').offset().left || e.pageX > $('.orb16 ul:first').offset().left + $('.orb16 ul:first').width();
            var outsideY = e.pageY < $('.orb16 ul:first img:first').offset().top || e.pageY > $('.orb16 ul:first').offset().top + $('.orb16 ul:first').height();

            if (outsideX || outsideY)
                $('.orb16 ul').fadeOut('fast');
        });

        if (isLoaded) {
            $('.orb16 li:first ul:first img:first').remove();
            $('.orb16 li:first ul:first img:last').remove();
            $('.ribbon-list16 div img[src*="/images/arrow_down.png"]').remove();
        }

        $('.orb16 li:first ul:first').prepend('<img src="/ribbon/themes/' + settings.theme + '/images/menu_top.png" style="margin-left: -10px; margin-top: -22px;" />');
        $('.orb16 li:first ul:first').append('<img src="/ribbon/themes/' + settings.theme + '/images/menu_bottom.png" style="margin-left: -10px; margin-bottom: -20px;" />');

        $('.ribbon-list16 div').each(function () { if ($(this).children('ul').length > 0 && $(this).parent('div').length > 0) { $(this).append('<img src="/ribbon/themes/' + settings.theme + '/images/arrow_down.png" style="float: right; margin-top: 5px;" />') } });
        $('.ribbon-sublist16 div').each(function () { if ($(this).children('ul').length > 0) { $(this).append('<img src="/ribbon/themes/' + settings.theme + '/images/arrow.png" style="float: left; margin-top: 5px;" />') } });

        $('.ribbon-sublist16 div').click(function (e) {
            var elwidth = $(this).parent().width();
            var insideX = e.pageX > $(this).offset().left && e.pageX < $(this).offset().left + $(this).width();
            var insideY = e.pageY > $(this).offset().top && e.pageY < $(this).offset().top + $(this).height();

            $('.ribbon-sublist16 div ul').fadeOut('fast');

            if (insideX && insideY) {
                $(this).attr('style', 'z-index:100000;background-image: ' + $(this).css('background-image'));
                $(this).parent().width(150);
                $(this).children('ul').width(150 + 4);
                $(this).children('ul').css("right", $(this).parent().width() + 2);
                $(this).children('ul').css("margin-top", "-19px");
                $(this).children('ul').fadeIn('fast');
            }
        });



        $('.ribbon-list16 div ul li').hover(function (e) { if ($(this).children('div').length == 0 && $(this).parent('ul').parent('div').parent('li').attr('class') != "ribbon-sublist16") { $('.ribbon-sublist16 div ul').fadeOut('fast'); } });

        //Hack for IE 7.
        if (navigator.appVersion.indexOf('MSIE 6.0') > -1 || navigator.appVersion.indexOf('MSIE 7.0') > -1) {
            $('ul.menu16 li li div').css('width', '90px');
            $('ul.menu16').css('width', '500px');
            $('ul.menu16').css('float', 'left');
            $('ul.menu16 .submenu16 li div.ribbon-list16').css('width', '100px');
            $('ul.menu16 .submenu16 li div.ribbon-list16 div').css('width', '100px');
        }

        $('a[href="#' + window.location.hash + '"]').click();

        isLoaded = true;

        function ResetSubMenu() {
            $('.menu16 li a').removeClass('active');
            $('.menu16 ul').removeClass('submenu16');
            $('.menu16 li ul').hide();
        }

        function ShowSubMenu(e) {
            var isActive = $(e).next().css('display') == 'block';
            ResetSubMenu();

            $(e).addClass('active');
            $(e).parent().children('ul:first').addClass('submenu16');

            $(e).parent().children('ul:first').show();
            //$('body').css('padding-top', '120px');

            isClosed = false;
        }

        function ShowOrbChildren(e) {
            if (($(e).parent().children('ul').css('display') == 'none' || $(e).parent().children('ul').length == 0) && $(e).parent().parent().parent().parent().hasClass('orb16')) {
                $('.orb16 li li ul').fadeOut('fast');
                $(e).parent().children('ul').fadeIn('fast');
            }
        }

        function ShowMenu() {
            $('.orb16 ul').animate({ opacity: 'toggle' }, 'fast');
        }

        function ShowAccessKeys(e) {
            if (e.altKey) {
                $('div[rel="accesskeyhelper"]').each(function () { $(this).css('top', $(this).parent().offset().top).css('left', $(this).parent().offset().left - 20 + $(this).parent().width()); $(this).show(); });
            }
        }

        function HideAccessKeys(e) {
            $('div[rel="accesskeyhelper"]').hide();
        }
    }
})(jQuery);
/*****17*/
(function ($) {
    var isLoaded;
    var isClosed;

    $.fn.Ribbon17 = function (ribbonSettings) {
        var settings = $.extend({ theme: 'windows7' }, ribbonSettings || {});

        $('.ribbon17 a').each(function () {
            if ($(this).attr('accesskey')) {
                $(this).append('<div rel="accesskeyhelper" style="display: none; position: absolute; background-image: url(images/accessbg.png); background-repeat: none; width: 16px; padding: 0px; text-align: center; height: 17px; line-height: 17px; top: ' + $(this).offset().top + 'px; left: ' + ($(this).offset().left + $(this).width() - 15) + 'px;">' + $(this).attr('accesskey') + '</div>');
            }
        });

        if (!isLoaded) {
            SetupMenu(settings);
        }

        $(document).keydown(function (e) { ShowAccessKeys(e); });
        $(document).keyup(function (e) { HideAccessKeys(e); });

        function SetupMenu(settings) {
            $('.menu17 li a:first').addClass('active');
            $('.menu17 li ul').hide();
            $('.menu17 li a:first').parent().children('ul:first').show();
            $('.menu17 li a:first').parent().children('ul:first').addClass('submenu17');
            $('.menu17 li > a').click(function () { ShowSubMenu(this); });
            $('.orb17').click(function () { ShowMenu(); });
            $('.orb17 ul').hide();
            $('.orb17 ul ul').hide();
            $('.orb17 li ul li ul').show();
            $('.orb17 li li ul').each(function () { $(this).prepend('<div style="background-color: #EBF2F7; height: 25px; line-height: 25px; width: 292px; padding-left: 9px; border-bottom: 1px solid #CFDBEB;">' + $(this).parent().children('a:first').text() + '</div>'); });
            $('.orb17 li li a').each(function () { if ($(this).parent().children('ul').length > 0) { $(this).addClass('arrow') } });

            //$('.ribbon-list div').each(function() { $(this).parent().width($(this).parent().width()); });

            /*$('.ribbon-list3 div').click(function (e) {
                if ($(this).parent('div').length > 0) {
                    var elwidth = $(this).parent().width();
                    var insideX = e.pageX > $(this).offset().left && e.pageX < $(this).offset().left + $(this).width();
                    var insideY = e.pageY > $(this).offset().top && e.pageY < $(this).offset().top + $(this).height();

                    /*var divclass = document.getElementById("PayReports").getElementsByClassName("ribbon-list3");
                    for (var i = 0; i < divclass.length; i++) {

                        if (divclass[i].id != $(this)[0].parentElement.id) {
                            /*divclass[i].children[0].children[1].fadeOut('fast');
                        }
                    }اینجا

                    if (e.srcElement.childElementCount ==1) {
                        $('.ribbon-list3 div ul').fadeOut('fast');
                    }

                    if (insideX && insideY) {
                        $('.ribbon-list3 div ul').fadeOut('fast');
                        $(this).attr('style', 'z-index:100000;background-image: ' + $(this).css('background-image'));
                        $(this).parent().width(20);
                        $(this).children('ul').width(150 + 4);
                        $(this).children('ul').fadeIn('fast');
                    }
                }
            });*/
            $('.menu17 li ul li div').click(function (e) {
                /*if ($(this).children('div').children('div').children('ul').length > 0) {*/
                aaa = e;

                if (e.target != undefined && e.target.tagName != "LI" && e.target.parentNode.className != "ribbon-sublist17") {
                    $('.ribbon-list17 div ul').fadeOut('fast');
                    $(this).children('div').children('div').children('ul').width(154);
                    $(this).children('div').children('div').children('ul').fadeIn('fast');
                }
            });

            $('.ribbon-list17 div').parents().click(function (e) {

                var outsideX;//= e.pageX < $('.ribbon-list4 div ul:visible').parent().offset().left || e.pageX > $('.ribbon-list4 div ul:visible').parent().offset().left + $('.ribbon-list4 div ul:visible').parent().width();
                var outsideY;//= e.pageY < $('.ribbon-list4 div ul:visible').parent().offset().top || e.pageY > $('.ribbon-list4 div ul:visible').parent().offset().top + $('.ribbon-list4 div ul:visible').parent().height();

                if (outsideX || outsideY) {
                    $('.ribbon-list17 div ul:visible').each(function () {
                        if (e.srcElement.childElementCount == 1) {
                            $(this).fadeOut('fast');
                        }
                    });
                    $('.ribbon-list17 div').css('background-image', '');
                }
            });

            $('.orb17 li li a').mouseover(function () { ShowOrbChildren(this); });

            $('.menu17 li > a').dblclick(function () {
                $('ul .submenu17').animate({ height: "hide" });
                $('body').animate({ paddingTop: $(this).parent().parent().parent().parent().height() });
                isClosed = true;
            });
        }

        $('.ribbon17').parents().click(function (e) {
            var outsideX = e.pageX < $('.orb17 ul:first').offset().left || e.pageX > $('.orb17 ul:first').offset().left + $('.orb17 ul:first').width();
            var outsideY = e.pageY < $('.orb17 ul:first img:first').offset().top || e.pageY > $('.orb17 ul:first').offset().top + $('.orb17 ul:first').height();

            if (outsideX || outsideY)
                $('.orb17 ul').fadeOut('fast');
        });

        if (isLoaded) {
            $('.orb17 li:first ul:first img:first').remove();
            $('.orb17 li:first ul:first img:last').remove();
            $('.ribbon-list17 div img[src*="/images/arrow_down.png"]').remove();
        }

        $('.orb17 li:first ul:first').prepend('<img src="/ribbon/themes/' + settings.theme + '/images/menu_top.png" style="margin-left: -10px; margin-top: -22px;" />');
        $('.orb17 li:first ul:first').append('<img src="/ribbon/themes/' + settings.theme + '/images/menu_bottom.png" style="margin-left: -10px; margin-bottom: -20px;" />');

        $('.ribbon-list17 div').each(function () { if ($(this).children('ul').length > 0 && $(this).parent('div').length > 0) { $(this).append('<img src="/ribbon/themes/' + settings.theme + '/images/arrow_down.png" style="float: right; margin-top: 5px;" />') } });
        $('.ribbon-sublist17 div').each(function () { if ($(this).children('ul').length > 0) { $(this).append('<img src="/ribbon/themes/' + settings.theme + '/images/arrow.png" style="float: left; margin-top: 5px;" />') } });

        $('.ribbon-sublist17 div').click(function (e) {
            var elwidth = $(this).parent().width();
            var insideX = e.pageX > $(this).offset().left && e.pageX < $(this).offset().left + $(this).width();
            var insideY = e.pageY > $(this).offset().top && e.pageY < $(this).offset().top + $(this).height();

            $('.ribbon-sublist17 div ul').fadeOut('fast');

            if (insideX && insideY) {
                $(this).attr('style', 'z-index:100000;background-image: ' + $(this).css('background-image'));
                $(this).parent().width(150);
                $(this).children('ul').width(150 + 4);
                $(this).children('ul').css("right", $(this).parent().width() + 2);
                $(this).children('ul').css("margin-top", "-19px");
                $(this).children('ul').fadeIn('fast');
            }
        });



        $('.ribbon-list17 div ul li').hover(function (e) { if ($(this).children('div').length == 0 && $(this).parent('ul').parent('div').parent('li').attr('class') != "ribbon-sublist17") { $('.ribbon-sublist17 div ul').fadeOut('fast'); } });

        //Hack for IE 7.
        if (navigator.appVersion.indexOf('MSIE 6.0') > -1 || navigator.appVersion.indexOf('MSIE 7.0') > -1) {
            $('ul.menu17 li li div').css('width', '90px');
            $('ul.menu17').css('width', '500px');
            $('ul.menu17').css('float', 'left');
            $('ul.menu17 .submenu17 li div.ribbon-list17').css('width', '100px');
            $('ul.menu17 .submenu17 li div.ribbon-list17 div').css('width', '100px');
        }

        $('a[href="#' + window.location.hash + '"]').click();

        isLoaded = true;

        function ResetSubMenu() {
            $('.menu17 li a').removeClass('active');
            $('.menu17 ul').removeClass('submenu17');
            $('.menu17 li ul').hide();
        }

        function ShowSubMenu(e) {
            var isActive = $(e).next().css('display') == 'block';
            ResetSubMenu();

            $(e).addClass('active');
            $(e).parent().children('ul:first').addClass('submenu17');

            $(e).parent().children('ul:first').show();
            //$('body').css('padding-top', '120px');

            isClosed = false;
        }

        function ShowOrbChildren(e) {
            if (($(e).parent().children('ul').css('display') == 'none' || $(e).parent().children('ul').length == 0) && $(e).parent().parent().parent().parent().hasClass('orb17')) {
                $('.orb17 li li ul').fadeOut('fast');
                $(e).parent().children('ul').fadeIn('fast');
            }
        }

        function ShowMenu() {
            $('.orb17 ul').animate({ opacity: 'toggle' }, 'fast');
        }

        function ShowAccessKeys(e) {
            if (e.altKey) {
                $('div[rel="accesskeyhelper"]').each(function () { $(this).css('top', $(this).parent().offset().top).css('left', $(this).parent().offset().left - 20 + $(this).parent().width()); $(this).show(); });
            }
        }

        function HideAccessKeys(e) {
            $('div[rel="accesskeyhelper"]').hide();
        }
    }
})(jQuery);
/*****18*/
(function ($) {
    var isLoaded;
    var isClosed;

    $.fn.Ribbon18 = function (ribbonSettings) {
        var settings = $.extend({ theme: 'windows7' }, ribbonSettings || {});

        $('.ribbon18 a').each(function () {
            if ($(this).attr('accesskey')) {
                $(this).append('<div rel="accesskeyhelper" style="display: none; position: absolute; background-image: url(images/accessbg.png); background-repeat: none; width: 16px; padding: 0px; text-align: center; height: 17px; line-height: 17px; top: ' + $(this).offset().top + 'px; left: ' + ($(this).offset().left + $(this).width() - 15) + 'px;">' + $(this).attr('accesskey') + '</div>');
            }
        });

        if (!isLoaded) {
            SetupMenu(settings);
        }

        $(document).keydown(function (e) { ShowAccessKeys(e); });
        $(document).keyup(function (e) { HideAccessKeys(e); });

        function SetupMenu(settings) {
            $('.menu18 li a:first').addClass('active');
            $('.menu18 li ul').hide();
            $('.menu18 li a:first').parent().children('ul:first').show();
            $('.menu18 li a:first').parent().children('ul:first').addClass('submenu18');
            $('.menu18 li > a').click(function () { ShowSubMenu(this); });
            $('.orb18').click(function () { ShowMenu(); });
            $('.orb18 ul').hide();
            $('.orb18 ul ul').hide();
            $('.orb18 li ul li ul').show();
            $('.orb18 li li ul').each(function () { $(this).prepend('<div style="background-color: #EBF2F7; height: 25px; line-height: 25px; width: 292px; padding-left: 9px; border-bottom: 1px solid #CFDBEB;">' + $(this).parent().children('a:first').text() + '</div>'); });
            $('.orb18 li li a').each(function () { if ($(this).parent().children('ul').length > 0) { $(this).addClass('arrow') } });

            //$('.ribbon-list div').each(function() { $(this).parent().width($(this).parent().width()); });

            /*$('.ribbon-list3 div').click(function (e) {
                if ($(this).parent('div').length > 0) {
                    var elwidth = $(this).parent().width();
                    var insideX = e.pageX > $(this).offset().left && e.pageX < $(this).offset().left + $(this).width();
                    var insideY = e.pageY > $(this).offset().top && e.pageY < $(this).offset().top + $(this).height();

                    /*var divclass = document.getElementById("PayReports").getElementsByClassName("ribbon-list3");
                    for (var i = 0; i < divclass.length; i++) {

                        if (divclass[i].id != $(this)[0].parentElement.id) {
                            /*divclass[i].children[0].children[1].fadeOut('fast');
                        }
                    }اینجا

                    if (e.srcElement.childElementCount ==1) {
                        $('.ribbon-list3 div ul').fadeOut('fast');
                    }

                    if (insideX && insideY) {
                        $('.ribbon-list3 div ul').fadeOut('fast');
                        $(this).attr('style', 'z-index:100000;background-image: ' + $(this).css('background-image'));
                        $(this).parent().width(20);
                        $(this).children('ul').width(150 + 4);
                        $(this).children('ul').fadeIn('fast');
                    }
                }
            });*/
            $('.menu18 li ul li div').click(function (e) {
                /*if ($(this).children('div').children('div').children('ul').length > 0) {*/
                aaa = e;

                if (e.target != undefined && e.target.tagName != "LI" && e.target.parentNode.className != "ribbon-sublist18") {
                    $('.ribbon-list18 div ul').fadeOut('fast');
                    $(this).children('div').children('div').children('ul').width(154);
                    $(this).children('div').children('div').children('ul').fadeIn('fast');
                }
            });

            $('.ribbon-list18 div').parents().click(function (e) {

                var outsideX;//= e.pageX < $('.ribbon-list4 div ul:visible').parent().offset().left || e.pageX > $('.ribbon-list4 div ul:visible').parent().offset().left + $('.ribbon-list4 div ul:visible').parent().width();
                var outsideY;//= e.pageY < $('.ribbon-list4 div ul:visible').parent().offset().top || e.pageY > $('.ribbon-list4 div ul:visible').parent().offset().top + $('.ribbon-list4 div ul:visible').parent().height();

                if (outsideX || outsideY) {
                    $('.ribbon-list18 div ul:visible').each(function () {
                        if (e.srcElement.childElementCount == 1) {
                            $(this).fadeOut('fast');
                        }
                    });
                    $('.ribbon-list18 div').css('background-image', '');
                }
            });

            $('.orb18 li li a').mouseover(function () { ShowOrbChildren(this); });

            $('.menu18 li > a').dblclick(function () {
                $('ul .submenu18').animate({ height: "hide" });
                $('body').animate({ paddingTop: $(this).parent().parent().parent().parent().height() });
                isClosed = true;
            });
        }

        $('.ribbon18').parents().click(function (e) {
            var outsideX = e.pageX < $('.orb18 ul:first').offset().left || e.pageX > $('.orb18 ul:first').offset().left + $('.orb18 ul:first').width();
            var outsideY = e.pageY < $('.orb18 ul:first img:first').offset().top || e.pageY > $('.orb18 ul:first').offset().top + $('.orb18 ul:first').height();

            if (outsideX || outsideY)
                $('.orb18 ul').fadeOut('fast');
        });

        if (isLoaded) {
            $('.orb18 li:first ul:first img:first').remove();
            $('.orb18 li:first ul:first img:last').remove();
            $('.ribbon-list18 div img[src*="/images/arrow_down.png"]').remove();
        }

        $('.orb18 li:first ul:first').prepend('<img src="/ribbon/themes/' + settings.theme + '/images/menu_top.png" style="margin-left: -10px; margin-top: -22px;" />');
        $('.orb18 li:first ul:first').append('<img src="/ribbon/themes/' + settings.theme + '/images/menu_bottom.png" style="margin-left: -10px; margin-bottom: -20px;" />');

        $('.ribbon-list18 div').each(function () { if ($(this).children('ul').length > 0 && $(this).parent('div').length > 0) { $(this).append('<img src="/ribbon/themes/' + settings.theme + '/images/arrow_down.png" style="float: right; margin-top: 5px;" />') } });
        $('.ribbon-sublist18 div').each(function () { if ($(this).children('ul').length > 0) { $(this).append('<img src="/ribbon/themes/' + settings.theme + '/images/arrow.png" style="float: left; margin-top: 5px;" />') } });

        $('.ribbon-sublist18 div').click(function (e) {
            var elwidth = $(this).parent().width();
            var insideX = e.pageX > $(this).offset().left && e.pageX < $(this).offset().left + $(this).width();
            var insideY = e.pageY > $(this).offset().top && e.pageY < $(this).offset().top + $(this).height();

            $('.ribbon-sublist18 div ul').fadeOut('fast');

            if (insideX && insideY) {
                $(this).attr('style', 'z-index:100000;background-image: ' + $(this).css('background-image'));
                $(this).parent().width(150);
                $(this).children('ul').width(150 + 4);
                $(this).children('ul').css("right", $(this).parent().width() + 2);
                $(this).children('ul').css("margin-top", "-19px");
                $(this).children('ul').fadeIn('fast');
            }
        });



        $('.ribbon-list18 div ul li').hover(function (e) { if ($(this).children('div').length == 0 && $(this).parent('ul').parent('div').parent('li').attr('class') != "ribbon-sublist18") { $('.ribbon-sublist18 div ul').fadeOut('fast'); } });

        //Hack for IE 7.
        if (navigator.appVersion.indexOf('MSIE 6.0') > -1 || navigator.appVersion.indexOf('MSIE 7.0') > -1) {
            $('ul.menu18 li li div').css('width', '90px');
            $('ul.menu18').css('width', '500px');
            $('ul.menu18').css('float', 'left');
            $('ul.menu18 .submenu18 li div.ribbon-list18').css('width', '100px');
            $('ul.menu18 .submenu18 li div.ribbon-list18 div').css('width', '100px');
        }

        $('a[href="#' + window.location.hash + '"]').click();

        isLoaded = true;

        function ResetSubMenu() {
            $('.menu18 li a').removeClass('active');
            $('.menu18 ul').removeClass('submenu18');
            $('.menu18 li ul').hide();
        }

        function ShowSubMenu(e) {
            var isActive = $(e).next().css('display') == 'block';
            ResetSubMenu();

            $(e).addClass('active');
            $(e).parent().children('ul:first').addClass('submenu18');

            $(e).parent().children('ul:first').show();
            //$('body').css('padding-top', '120px');

            isClosed = false;
        }

        function ShowOrbChildren(e) {
            if (($(e).parent().children('ul').css('display') == 'none' || $(e).parent().children('ul').length == 0) && $(e).parent().parent().parent().parent().hasClass('orb18')) {
                $('.orb18 li li ul').fadeOut('fast');
                $(e).parent().children('ul').fadeIn('fast');
            }
        }

        function ShowMenu() {
            $('.orb18 ul').animate({ opacity: 'toggle' }, 'fast');
        }

        function ShowAccessKeys(e) {
            if (e.altKey) {
                $('div[rel="accesskeyhelper"]').each(function () { $(this).css('top', $(this).parent().offset().top).css('left', $(this).parent().offset().left - 20 + $(this).parent().width()); $(this).show(); });
            }
        }

        function HideAccessKeys(e) {
            $('div[rel="accesskeyhelper"]').hide();
        }
    }
})(jQuery);

/******19*/
(function ($) {
    var isLoaded;
    var isClosed;

    $.fn.Ribbon19 = function (ribbonSettings) {
        var settings = $.extend({ theme: 'windows7' }, ribbonSettings || {});

        $('.ribbon19 a').each(function () {
            if ($(this).attr('accesskey')) {
                $(this).append('<div rel="accesskeyhelper" style="display: none; position: absolute; background-image: url(images/accessbg.png); background-repeat: none; width: 16px; padding: 0px; text-align: center; height: 17px; line-height: 17px; top: ' + $(this).offset().top + 'px; left: ' + ($(this).offset().left + $(this).width() - 15) + 'px;">' + $(this).attr('accesskey') + '</div>');
            }
        });

        if (!isLoaded) {
            SetupMenu(settings);
        }

        $(document).keydown(function (e) { ShowAccessKeys(e); });
        $(document).keyup(function (e) { HideAccessKeys(e); });

        function SetupMenu(settings) {
            $('.menu19 li a:first').addClass('active');
            $('.menu19 li ul').hide();
            $('.menu19 li a:first').parent().children('ul:first').show();
            $('.menu19 li a:first').parent().children('ul:first').addClass('submenu19');
            $('.menu19 li > a').click(function () { ShowSubMenu(this); });
            $('.orb19').click(function () { ShowMenu(); });
            $('.orb19 ul').hide();
            $('.orb19 ul ul').hide();
            $('.orb19 li ul li ul').show();
            $('.orb19 li li ul').each(function () { $(this).prepend('<div style="background-color: #EBF2F7; height: 25px; line-height: 25px; width: 292px; padding-left: 9px; border-bottom: 1px solid #CFDBEB;">' + $(this).parent().children('a:first').text() + '</div>'); });
            $('.orb19 li li a').each(function () { if ($(this).parent().children('ul').length > 0) { $(this).addClass('arrow') } });

            //$('.ribbon-list div').each(function() { $(this).parent().width($(this).parent().width()); });

            /*$('.ribbon-list3 div').click(function (e) {
                if ($(this).parent('div').length > 0) {
                    var elwidth = $(this).parent().width();
                    var insideX = e.pageX > $(this).offset().left && e.pageX < $(this).offset().left + $(this).width();
                    var insideY = e.pageY > $(this).offset().top && e.pageY < $(this).offset().top + $(this).height();

                    /*var divclass = document.getElementById("PayReports").getElementsByClassName("ribbon-list3");
                    for (var i = 0; i < divclass.length; i++) {

                        if (divclass[i].id != $(this)[0].parentElement.id) {
                            /*divclass[i].children[0].children[1].fadeOut('fast');
                        }
                    }اینجا

                    if (e.srcElement.childElementCount ==1) {
                        $('.ribbon-list3 div ul').fadeOut('fast');
                    }

                    if (insideX && insideY) {
                        $('.ribbon-list3 div ul').fadeOut('fast');
                        $(this).attr('style', 'z-index:100000;background-image: ' + $(this).css('background-image'));
                        $(this).parent().width(20);
                        $(this).children('ul').width(150 + 4);
                        $(this).children('ul').fadeIn('fast');
                    }
                }
            });*/
            $('.menu19 li ul li div').click(function (e) {
                /*if ($(this).children('div').children('div').children('ul').length > 0) {*/
                aaa = e;

                if (e.target != undefined && e.target.tagName != "LI" && e.target.parentNode.className != "ribbon-sublist19") {
                    $('.ribbon-list19 div ul').fadeOut('fast');
                    $(this).children('div').children('div').children('ul').width(154);
                    $(this).children('div').children('div').children('ul').fadeIn('fast');
                }
            });

            $('.ribbon-list19 div').parents().click(function (e) {

                var outsideX;//= e.pageX < $('.ribbon-list4 div ul:visible').parent().offset().left || e.pageX > $('.ribbon-list4 div ul:visible').parent().offset().left + $('.ribbon-list4 div ul:visible').parent().width();
                var outsideY;//= e.pageY < $('.ribbon-list4 div ul:visible').parent().offset().top || e.pageY > $('.ribbon-list4 div ul:visible').parent().offset().top + $('.ribbon-list4 div ul:visible').parent().height();

                if (outsideX || outsideY) {
                    $('.ribbon-list19 div ul:visible').each(function () {
                        if (e.srcElement.childElementCount == 1) {
                            $(this).fadeOut('fast');
                        }
                    });
                    $('.ribbon-list19 div').css('background-image', '');
                }
            });

            $('.orb19 li li a').mouseover(function () { ShowOrbChildren(this); });

            $('.menu19 li > a').dblclick(function () {
                $('ul .submenu19').animate({ height: "hide" });
                $('body').animate({ paddingTop: $(this).parent().parent().parent().parent().height() });
                isClosed = true;
            });
        }

        $('.ribbon19').parents().click(function (e) {
            var outsideX = e.pageX < $('.orb19 ul:first').offset().left || e.pageX > $('.orb19 ul:first').offset().left + $('.orb19 ul:first').width();
            var outsideY = e.pageY < $('.orb19 ul:first img:first').offset().top || e.pageY > $('.orb19 ul:first').offset().top + $('.orb19 ul:first').height();

            if (outsideX || outsideY)
                $('.orb19 ul').fadeOut('fast');
        });

        if (isLoaded) {
            $('.orb19 li:first ul:first img:first').remove();
            $('.orb19 li:first ul:first img:last').remove();
            $('.ribbon-list19 div img[src*="/images/arrow_down.png"]').remove();
        }

        $('.orb19 li:first ul:first').prepend('<img src="/ribbon/themes/' + settings.theme + '/images/menu_top.png" style="margin-left: -10px; margin-top: -22px;" />');
        $('.orb19 li:first ul:first').append('<img src="/ribbon/themes/' + settings.theme + '/images/menu_bottom.png" style="margin-left: -10px; margin-bottom: -20px;" />');

        $('.ribbon-list19 div').each(function () { if ($(this).children('ul').length > 0 && $(this).parent('div').length > 0) { $(this).append('<img src="/ribbon/themes/' + settings.theme + '/images/arrow_down.png" style="float: right; margin-top: 5px;" />') } });
        $('.ribbon-sublist19 div').each(function () { if ($(this).children('ul').length > 0) { $(this).append('<img src="/ribbon/themes/' + settings.theme + '/images/arrow.png" style="float: left; margin-top: 5px;" />') } });

        $('.ribbon-sublist19 div').click(function (e) {
            var elwidth = $(this).parent().width();
            var insideX = e.pageX > $(this).offset().left && e.pageX < $(this).offset().left + $(this).width();
            var insideY = e.pageY > $(this).offset().top && e.pageY < $(this).offset().top + $(this).height();

            $('.ribbon-sublist19 div ul').fadeOut('fast');

            if (insideX && insideY) {
                $(this).attr('style', 'z-index:100000;background-image: ' + $(this).css('background-image'));
                $(this).parent().width(150);
                $(this).children('ul').width(150 + 4);
                $(this).children('ul').css("right", $(this).parent().width() + 2);
                $(this).children('ul').css("margin-top", "-19px");
                $(this).children('ul').fadeIn('fast');
            }
        });



        $('.ribbon-list19 div ul li').hover(function (e) { if ($(this).children('div').length == 0 && $(this).parent('ul').parent('div').parent('li').attr('class') != "ribbon-sublist19") { $('.ribbon-sublist19 div ul').fadeOut('fast'); } });

        //Hack for IE 7.
        if (navigator.appVersion.indexOf('MSIE 6.0') > -1 || navigator.appVersion.indexOf('MSIE 7.0') > -1) {
            $('ul.menu19 li li div').css('width', '90px');
            $('ul.menu19').css('width', '500px');
            $('ul.menu19').css('float', 'left');
            $('ul.menu19 .submenu19 li div.ribbon-list19').css('width', '100px');
            $('ul.menu19 .submenu19 li div.ribbon-list19 div').css('width', '100px');
        }

        $('a[href="#' + window.location.hash + '"]').click();

        isLoaded = true;

        function ResetSubMenu() {
            $('.menu19 li a').removeClass('active');
            $('.menu19 ul').removeClass('submenu19');
            $('.menu19 li ul').hide();
        }

        function ShowSubMenu(e) {
            var isActive = $(e).next().css('display') == 'block';
            ResetSubMenu();

            $(e).addClass('active');
            $(e).parent().children('ul:first').addClass('submenu19');

            $(e).parent().children('ul:first').show();
            //$('body').css('padding-top', '120px');

            isClosed = false;
        }

        function ShowOrbChildren(e) {
            if (($(e).parent().children('ul').css('display') == 'none' || $(e).parent().children('ul').length == 0) && $(e).parent().parent().parent().parent().hasClass('orb19')) {
                $('.orb19 li li ul').fadeOut('fast');
                $(e).parent().children('ul').fadeIn('fast');
            }
        }

        function ShowMenu() {
            $('.orb19 ul').animate({ opacity: 'toggle' }, 'fast');
        }

        function ShowAccessKeys(e) {
            if (e.altKey) {
                $('div[rel="accesskeyhelper"]').each(function () { $(this).css('top', $(this).parent().offset().top).css('left', $(this).parent().offset().left - 20 + $(this).parent().width()); $(this).show(); });
            }
        }

        function HideAccessKeys(e) {
            $('div[rel="accesskeyhelper"]').hide();
        }
    }
})(jQuery);

/******20*/
(function ($) {
    var isLoaded;
    var isClosed;

    $.fn.Ribbon20 = function (ribbonSettings) {
        var settings = $.extend({ theme: 'windows7' }, ribbonSettings || {});

        $('.ribbon20 a').each(function () {
            if ($(this).attr('accesskey')) {
                $(this).append('<div rel="accesskeyhelper" style="display: none; position: absolute; background-image: url(images/accessbg.png); background-repeat: none; width: 16px; padding: 0px; text-align: center; height: 17px; line-height: 17px; top: ' + $(this).offset().top + 'px; left: ' + ($(this).offset().left + $(this).width() - 15) + 'px;">' + $(this).attr('accesskey') + '</div>');
            }
        });

        if (!isLoaded) {
            SetupMenu(settings);
        }

        $(document).keydown(function (e) { ShowAccessKeys(e); });
        $(document).keyup(function (e) { HideAccessKeys(e); });

        function SetupMenu(settings) {
            $('.menu20 li a:first').addClass('active');
            $('.menu20 li ul').hide();
            $('.menu20 li a:first').parent().children('ul:first').show();
            $('.menu20 li a:first').parent().children('ul:first').addClass('submenu20');
            $('.menu20 li > a').click(function () { ShowSubMenu(this); });
            $('.orb20').click(function () { ShowMenu(); });
            $('.orb20 ul').hide();
            $('.orb20 ul ul').hide();
            $('.orb20 li ul li ul').show();
            $('.orb20 li li ul').each(function () { $(this).prepend('<div style="background-color: #EBF2F7; height: 25px; line-height: 25px; width: 292px; padding-left: 9px; border-bottom: 1px solid #CFDBEB;">' + $(this).parent().children('a:first').text() + '</div>'); });
            $('.orb120 li li a').each(function () { if ($(this).parent().children('ul').length > 0) { $(this).addClass('arrow') } });

            //$('.ribbon-list div').each(function() { $(this).parent().width($(this).parent().width()); });

            /*$('.ribbon-list3 div').click(function (e) {
                if ($(this).parent('div').length > 0) {
                    var elwidth = $(this).parent().width();
                    var insideX = e.pageX > $(this).offset().left && e.pageX < $(this).offset().left + $(this).width();
                    var insideY = e.pageY > $(this).offset().top && e.pageY < $(this).offset().top + $(this).height();

                    /*var divclass = document.getElementById("PayReports").getElementsByClassName("ribbon-list3");
                    for (var i = 0; i < divclass.length; i++) {

                        if (divclass[i].id != $(this)[0].parentElement.id) {
                            /*divclass[i].children[0].children[1].fadeOut('fast');
                        }
                    }اینجا

                    if (e.srcElement.childElementCount ==1) {
                        $('.ribbon-list3 div ul').fadeOut('fast');
                    }

                    if (insideX && insideY) {
                        $('.ribbon-list3 div ul').fadeOut('fast');
                        $(this).attr('style', 'z-index:100000;background-image: ' + $(this).css('background-image'));
                        $(this).parent().width(20);
                        $(this).children('ul').width(150 + 4);
                        $(this).children('ul').fadeIn('fast');
                    }
                }
            });*/
            $('.menu20 li ul li div').click(function (e) {
                /*if ($(this).children('div').children('div').children('ul').length > 0) {*/
                aaa = e;

                if (e.target != undefined && e.target.tagName != "LI" && e.target.parentNode.className != "ribbon-sublist20") {
                    $('.ribbon-list20 div ul').fadeOut('fast');
                    $(this).children('div').children('div').children('ul').width(154);
                    $(this).children('div').children('div').children('ul').fadeIn('fast');
                }
            });

            $('.ribbon-list20 div').parents().click(function (e) {

                var outsideX;//= e.pageX < $('.ribbon-list4 div ul:visible').parent().offset().left || e.pageX > $('.ribbon-list4 div ul:visible').parent().offset().left + $('.ribbon-list4 div ul:visible').parent().width();
                var outsideY;//= e.pageY < $('.ribbon-list4 div ul:visible').parent().offset().top || e.pageY > $('.ribbon-list4 div ul:visible').parent().offset().top + $('.ribbon-list4 div ul:visible').parent().height();

                if (outsideX || outsideY) {
                    $('.ribbon-list20 div ul:visible').each(function () {
                        if (e.srcElement.childElementCount == 1) {
                            $(this).fadeOut('fast');
                        }
                    });
                    $('.ribbon-list20 div').css('background-image', '');
                }
            });

            $('.orb20 li li a').mouseover(function () { ShowOrbChildren(this); });

            $('.menu20 li > a').dblclick(function () {
                $('ul .submenu20').animate({ height: "hide" });
                $('body').animate({ paddingTop: $(this).parent().parent().parent().parent().height() });
                isClosed = true;
            });
        }

        $('.ribbon20').parents().click(function (e) {
            var outsideX = e.pageX < $('.orb20 ul:first').offset().left || e.pageX > $('.orb20 ul:first').offset().left + $('.orb20 ul:first').width();
            var outsideY = e.pageY < $('.orb20 ul:first img:first').offset().top || e.pageY > $('.orb20 ul:first').offset().top + $('.orb20 ul:first').height();

            if (outsideX || outsideY)
                $('.orb20 ul').fadeOut('fast');
        });

        if (isLoaded) {
            $('.orb20 li:first ul:first img:first').remove();
            $('.orb20 li:first ul:first img:last').remove();
            $('.ribbon-list20 div img[src*="/images/arrow_down.png"]').remove();
        }

        $('.orb20 li:first ul:first').prepend('<img src="/ribbon/themes/' + settings.theme + '/images/menu_top.png" style="margin-left: -10px; margin-top: -22px;" />');
        $('.orb20 li:first ul:first').append('<img src="/ribbon/themes/' + settings.theme + '/images/menu_bottom.png" style="margin-left: -10px; margin-bottom: -20px;" />');

        $('.ribbon-list20 div').each(function () { if ($(this).children('ul').length > 0 && $(this).parent('div').length > 0) { $(this).append('<img src="/ribbon/themes/' + settings.theme + '/images/arrow_down.png" style="float: right; margin-top: 5px;" />') } });
        $('.ribbon-sublist20 div').each(function () { if ($(this).children('ul').length > 0) { $(this).append('<img src="/ribbon/themes/' + settings.theme + '/images/arrow.png" style="float: left; margin-top: 5px;" />') } });

        $('.ribbon-sublist20 div').click(function (e) {
            var elwidth = $(this).parent().width();
            var insideX = e.pageX > $(this).offset().left && e.pageX < $(this).offset().left + $(this).width();
            var insideY = e.pageY > $(this).offset().top && e.pageY < $(this).offset().top + $(this).height();

            $('.ribbon-sublist20 div ul').fadeOut('fast');

            if (insideX && insideY) {
                $(this).attr('style', 'z-index:100000;background-image: ' + $(this).css('background-image'));
                $(this).parent().width(150);
                $(this).children('ul').width(150 + 4);
                $(this).children('ul').css("right", $(this).parent().width() + 2);
                $(this).children('ul').css("margin-top", "-19px");
                $(this).children('ul').fadeIn('fast');
            }
        });



        $('.ribbon-list20 div ul li').hover(function (e) { if ($(this).children('div').length == 0 && $(this).parent('ul').parent('div').parent('li').attr('class') != "ribbon-sublist20") { $('.ribbon-sublist20 div ul').fadeOut('fast'); } });

        //Hack for IE 7.
        if (navigator.appVersion.indexOf('MSIE 6.0') > -1 || navigator.appVersion.indexOf('MSIE 7.0') > -1) {
            $('ul.menu20 li li div').css('width', '90px');
            $('ul.menu20').css('width', '500px');
            $('ul.menu20').css('float', 'left');
            $('ul.menu20 .submenu20 li div.ribbon-list20').css('width', '100px');
            $('ul.menu20 .submenu20 li div.ribbon-list20 div').css('width', '100px');
        }

        $('a[href="#' + window.location.hash + '"]').click();

        isLoaded = true;

        function ResetSubMenu() {
            $('.menu20 li a').removeClass('active');
            $('.menu20 ul').removeClass('submenu20');
            $('.menu20 li ul').hide();
        }

        function ShowSubMenu(e) {
            var isActive = $(e).next().css('display') == 'block';
            ResetSubMenu();

            $(e).addClass('active');
            $(e).parent().children('ul:first').addClass('submenu20');

            $(e).parent().children('ul:first').show();
            //$('body').css('padding-top', '120px');

            isClosed = false;
        }

        function ShowOrbChildren(e) {
            if (($(e).parent().children('ul').css('display') == 'none' || $(e).parent().children('ul').length == 0) && $(e).parent().parent().parent().parent().hasClass('orb20')) {
                $('.orb20 li li ul').fadeOut('fast');
                $(e).parent().children('ul').fadeIn('fast');
            }
        }

        function ShowMenu() {
            $('.orb20 ul').animate({ opacity: 'toggle' }, 'fast');
        }

        function ShowAccessKeys(e) {
            if (e.altKey) {
                $('div[rel="accesskeyhelper"]').each(function () { $(this).css('top', $(this).parent().offset().top).css('left', $(this).parent().offset().left - 20 + $(this).parent().width()); $(this).show(); });
            }
        }

        function HideAccessKeys(e) {
            $('div[rel="accesskeyhelper"]').hide();
        }
    }
})(jQuery);
