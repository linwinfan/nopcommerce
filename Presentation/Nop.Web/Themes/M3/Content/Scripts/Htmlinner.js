//品牌库下广告商品特效
$(function () {
    $(".all_cate_list li").hover(function () {
        $(this).css('z-index', '99');
        $(this).children('.i_e').addClass('on');
        $(this).children('.i_t').show();
    }, function () {
        $(this).css('z-index', '0');
        $(this).children('.i_e').removeClass('on');
        $(this).children('.i_t').hide();
    });
    $("#banr ul li div").hover(function () {
        $(this).siblings().css("opacity", 0.3);
    }, function () {
        $("#banr ul li div").css("opacity", 1);
    });
});
//滑动到我的百买下的特效
$(function () {
    $('.my-mro').hover(function () {
        $('.my-mropopup').stop(true, true).fadeIn();
    }, function () {
        $('.my-mropopup').stop(true, true).fadeOut();
    });
});

$(function () {
    $('#login_sub').die('click').live('click', function (e) {
        var regStr = $('#hfRegisterToggle').val(); //注册方式
        var nickname = $('#head_nickname').val();
        var password = $('#head_password').val();
        var tipStr = '';
        if (regStr == "Phone") {
            tipStr = "手机号码";
        } else {
            tipStr = "邮箱地址";
        }
        if (nickname.length <= 0 ) {
            ShowFailTip("请输入"+tipStr);
            return false;
        } else {
            if (regStr == "Phone") { //手机号码登录
                var regs = /^(1(([35][0-9])|(47)|[8][0126789]))\d{8}$/;
                if (!regs.test(nickname)) {
                    ShowFailTip("请填写有效的手机号码");
                    return false;
                }
            } else { //邮箱登录
                var regsEmail = /^[\w-]+(\.[\w-]+)*\@[A-Za-z0-9]+((\.|-|_)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$/;
                if (!regsEmail.test(nickname)) {
                    ShowFailTip("请填写有效的Email地址");
                    return false;
                }
            }
        }
        if (password.length <= 0 || password == '请输入密码') {
            ShowFailTip("请输入密码");
            return false;
        }
        $.post("/Account/AjaxLogin", { UserName: nickname, UserPwd: password }, function (data, textStatus) {
            if (data.split('|')[0] != "1") {
                ShowFailTip("用户名或密码错误"); //这里没有处理完整
            } else {
                $('.login-reg').load('/Partial/Login');
            }
        });
    });
    var AjaxLoginGetUserInfo = function (pointer) {
        $.ajax({
            type: "get",
            dataType: "text",
            url: "/Partial/Header",
            data: { pointer: pointer },
            success: function (data) {
                $(".headers").html(data);
            }
        });
    };
});

    $(function () {
        var num = 0;
        var timer = null;
        var oSliders = $('.l_event');
        var oSlider = $('.l_number>li');
        var oLevent = $('.l_event>li');
        var oLenght = oSlider.length;

        oSliders.hover(function () {
            clearInterval(timer);
        }, function () {
            timer = setInterval(function () {
                if (num == oLenght) {
                    num = 0;
                }
                oLevent.eq(num).stop(false, true).fadeIn().siblings().stop(false, true).fadeOut();
                oSlider.eq(num).addClass('slide_number_active').siblings().removeClass('slide_number_active');
                num++;
            }, 4000);
        }).trigger('mouseleave');
        oSlider.hover(function () {
            clearInterval(timer);
            $(this).addClass('slide_number_active').siblings().removeClass('slide_number_active');
            var aIndex = $(this).index();
            $('.l_event>li').eq(aIndex).stop(false, true).fadeIn().siblings().stop(false, true).fadeOut();
        });
    });



