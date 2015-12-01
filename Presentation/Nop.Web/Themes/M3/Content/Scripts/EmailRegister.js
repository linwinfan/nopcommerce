var registerType = 'Mail';
var regs = /^[A-Za-z0-9]{6,30}$/;
var focusmsg = '请填写密码（6-30位数字或字母）';
var errormsg = '密码6-30位，支持“数字、字母”';
var mailStatus = true;
var nicknameStatus = true;
var pwdStatus = true;
var vpwdStatus = true;
var phoneStatus = true;
var codeStatus = false;
var agreementStatus = true;
var checkOK = true;

var isOK = true;
var smsSeconds = 60;
var intervaSMS;
var validateOnce = {
    Email: "",
    Exists: false
};

$(function () {
    var regStr = $('#hfRegisterToggle').val(); //注册方式
    var isOpen = $("#hfSMSIsOpen").val();
    if (regStr == 'Phone') {
        if (isOpen == "True") {
            $(".txtphone").show();
        }
    }
    //刷新页面时获取时间
    var time = $("#hfSeconds").val();
    if (time > 0) {
        smsSeconds = time;
        $("#btnSendSMS").attr("value", "请在(" + smsSeconds + ")秒后重新发送");
        intervaSMS = setInterval("CountDown()", 1000);
    }
    //点击回复触发
    $("#aUserAgreement").click(function () {
        $("#divUserAgreement").dialog(dialogOpts); //弹出‘用户协议’层  
    });

    //dialog层中项的设置
    var dialogOpts = {
        title: "用户注册协议",
        width: 700,
        height: 600,
        modal: true,
        buttons: {
            "同意": function () {
                $("#chkAgreement").attr("checked", "checked"); //选中同意
                $(this).dialog("close"); //关闭层
            },
            "取消": function () {
                $(this).dialog("close"); //关闭层
            }
        }
    };


    //注册按钮
    $("#btnEmailRegister").click(function () {
        $("#divRegTip").removeClass().html("");
        if (regStr == 'Phone') {
            if (!codeStatus && isOpen == "True") {
                ShowFailTip("手机效验码不正确");
                return;
            }
        }
        if (CheckRegister()) {
            $(".form-regi").submit();
        }
    });

    $("#btnSendSMS").click(function () {
        CheckPhone($("#phone"));
        var phone = $("#phone").val();
        if (phone == "") {
            ShowFailTip("请输入手机号码");
            return;
        }
        if (phoneStatus) {
            //发送短信
            $.ajax({
                url: $Maticsoft.BasePath + "Account/SendSMS",
                type: 'post',
                dataType: 'text',
                timeout: 10000,
                async: false,
                data: {
                    Action: "post", Phone: phone
                },
                success: function (resultData) {
                    if (resultData.split("|")[0] == "True") {
                        ShowSuccessTip("发送短信成功");
                        smsSeconds = 60;
                        $("#hfPhoneNumber").val(resultData.split("|")[1]);
                        $("#btnSendSMS").attr("value", "请在(" + smsSeconds + ")秒后重新发送");
                        intervaSMS = setInterval("CountDown()", 1000);
                    }
                    else {
                        $("#divPhoneTip").removeClass("msg msg-ok msg-naked").removeClass("msg msg-info").addClass("msg msg-err").html("<i class=\"msg-ico\"></i><p>服务器没有返回数据，可能服务器忙，请稍候再试！</p>");
                        mailStatus = false;
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    ShowServerBusyTip("服务器没有返回数据，可能服务器忙，请稍候再试！");
                    mailStatus = false;
                }

            });
        }
    });
    $("#checkCode").blur(function () {
        var code = $(this).val();
        if (code == "") {
            ShowFailTip("请输入手机效验码");
            codeStatus = false;
            return;
        }
        var phone = $("#phone").val();
        if (phone != $("#hfPhoneNumber").val()) {
            ShowFailTip("请输入一致的手机号码");
            codeStatus = false;
            return;
        }
        //验证注册邮箱是否存在
        $.ajax({
            url: $Maticsoft.BasePath + "Account/VerifiyCode",
            type: 'post',
            dataType: 'text',
            timeout: 10000,
            async: false,
            data: {
                Action: "post", SMSCode: code,Phone:phone
            },
            success: function (resultData) {

                if (resultData == "False") {
                    ShowFailTip("手机效验码不正确");
                } else {
                    $("#divVerifyCodeTip").removeClass("msg msg-err").removeClass("msg msg-info").addClass("msg msg-ok msg-naked").html("<i class=\"msg-ico\"></i><p>&nbsp;</p>");
                    codeStatus = true;
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                ShowServerBusyTip("服务器没有返回数据，可能服务器忙，请稍候再试！");
                mailStatus = false;
            }

        });
    });

    $("#email").focus(function () {
        $("#divEmailTip").removeClass("msg msg-ok msg-naked").removeClass("msg msg-err").addClass("msg msg-info").html("<i class=\"msg-ico\"></i><p>请填写有效的Email地址</p>");
    }).keypress(function (event) {
        if (event.which == 13) {
            $("#btnEmailRegister").trigger("click");
        }
    }).blur(function () {
        CheckEmail($(this));
    });

    $("#nickname").focus(function () {
        $("#divNicknameTip").removeClass("msg msg-ok msg-naked").removeClass("msg msg-err").addClass("msg msg-info").html("<i class=\"msg-ico\"></i><p>请填写昵称！</p>");
    }).keypress(function (event) {
        if (event.which == 13) {
            $("#btnEmailRegister").trigger("click");
        }
    }).blur(function () {
        CheckNickname($(this));
    });

    $("#pwd").focus(function () {
        $("#divPwdTip").removeClass("msg msg-ok msg-naked").removeClass("msg msg-err").addClass("msg msg-info").html("<i class=\"msg-ico\"></i><p>" + focusmsg + "</p>");
    }).keypress(function (event) {
        if (event.which == 13) {
            $("#btnEmailRegister").trigger("click");
        }
    }).blur(function () {
        CheckPwd($(this));
    });
    $("#vpwd").focus(function () {
        $("#divVPwdTip").removeClass("msg msg-ok msg-naked").removeClass("msg msg-err").addClass("msg msg-info").html("<i class=\"msg-ico\"></i><p>请再次填写密码，两次输入必须一致</p>");
    }).keypress(function (event) {
        if (event.which == 13) {
            $("#btnEmailRegister").trigger("click");
        }
    }).blur(function () {
        CheckVPwd($(this));
    });

    $("#phone").focus(function () {
        $("#divPhoneTip").removeClass("msg msg-ok msg-naked").removeClass("msg msg-err").addClass("msg msg-info").html("<i class=\"msg-ico\"></i><p>请填写手机号码</p>");
    }).keypress(function (event) {
        if (event.which == 13) {
            $("#btnEmailRegister").trigger("click");
        }
    }).blur(function () {
        CheckPhone($(this));
    });

    $("#chkAgreement").click(function () {
        CheckAgreement($(this));
    });
});

function CheckRegister() {
    var regStr = $('#hfRegisterToggle').val();
    var userNameStatus;
    if (regStr == "Phone") {
        CheckPhone($("#phone"));
        userNameStatus = phoneStatus;
    } else {
        CheckEmail($("#email"));
        userNameStatus = mailStatus;
    }
    CheckNickname($("#nickname"));
    CheckPwd($("#pwd"));
    CheckVPwd($("#vpwd"));
    CheckAgreement($("#chkAgreement"));
   
    if (!userNameStatus || !pwdStatus || !vpwdStatus || !nicknameStatus || !agreementStatus) {
        checkOK = false;
    } else {
        checkOK = true;
    }
    return checkOK;
}

//验证邮箱
function CheckEmail(obj) {
    var regs = /^[\w-]+(\.[\w-]+)*\@[A-Za-z0-9]+((\.|-|_)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$/;
    var emailval = obj.val();
    if (emailval != "") {
        if (!regs.test(emailval)) {
            ShowFailTip("请填写有效的Email地址");
            mailStatus = false;
        } else {
//验证注册邮箱是否存在
        $.ajax({
            url: $Maticsoft.BasePath + "Account/IsExistUserName",
            type: 'post',
            dataType: 'text',
            timeout: 10000,
            async: false,
            data: {
                Action: "post", userName: emailval
            },
            success: function (resultData) {
                if (resultData == "true") {
                    $("#divEmailTip").removeClass("msg msg-err").removeClass("msg msg-info").addClass("msg msg-ok msg-naked").html("<i class=\"msg-ico\"></i><p>&nbsp;</p>");
                    mailStatus = true;
                }
                else {
                    ShowFailTip("该Email已存在，请使用其他Email地址");
                    mailStatus = false;
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                ShowServerBusyTip("服务器没有返回数据，可能服务器忙，请稍候再试！");
                mailStatus = false;
            }

        });
        }
} else {
    ShowFailTip("请填写有效的Email地址");
        mailStatus = false;
    }
    return;
}
 
function CheckPhone(obj) {
    var regs = /^1([38][0-9]|4[57]|5[^4])\d{8}$/;
    var phoneval = obj.val();
    if (phoneval != "") {
        if (!regs.test(phoneval)) {
            ShowFailTip("请填写有效的手机号码");
            phoneStatus = false;
        } 
        else {
            //验证手机是否存在
            $.ajax({
                url: $Maticsoft.BasePath + "Account/IsExistUserName",
                type: 'post',
                dataType: 'text',
                timeout: 10000,
                async: false,
                data: {
                    Action: "post", userName: phoneval
                },
                success: function (resultData) {
                    if (resultData == "true") {
                        $("#divPhoneTip").removeClass("msg msg-err").removeClass("msg msg-info").addClass("msg msg-ok msg-naked").html("<i class=\"msg-ico\"></i><p>&nbsp;</p>");
                        phoneStatus = true;
                    }
                    else {
                        ShowFailTip("该手机号码已被注册，请使用其他手机号码注册");
                        phoneStatus = false;
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    ShowServerBusyTip("服务器没有返回数据，可能服务器忙，请稍候再试！");
                    phoneStatus = false;
                }

            });
        }
    } else {
        phoneStatus = false;
    }
    return;
}

//验证昵称
function CheckNickname(obj) {
    var i = 0;
    var niclnamevalue = obj.val();
    if (niclnamevalue.indexOf(";") > -1 || niclnamevalue.indexOf(",") > -1 || niclnamevalue.indexOf("'") > -1) {
        ShowFailTip('大神，请您手下留情！');
        $(this).val("");
        i++;
        if (i >= 3) {
            ShowFailTip('别玩了，这样有意思吗？');
        }
        nicknameStatus = false;
        return;
    }
    if (niclnamevalue != "") {
        //验证昵称是否存在
        $.ajax({
            url:  $Maticsoft.BasePath +"Account/IsExistNickName" ,
            type: 'post',
            dataType: 'text',
            timeout: 10000,
            async: false,
            data: {
                Action: "post",
                nickName: niclnamevalue
            },
            success: function (resultData) {
                if (resultData == "true") {
                    $("#divNicknameTip").removeClass("msg msg-err").removeClass("msg msg-info").addClass("msg msg-ok msg-naked").html("<i class=\"msg-ico\"></i><p>&nbsp;</p>");
                    nicknameStatus = true;
                } else {
                    ShowFailTip('该昵称已被其他用户抢先使用，换一个试试');
                    nicknameStatus = false;
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                ShowServerBusyTip("服务器没有返回数据，可能服务器忙，请稍候再试！");
                nicknameStatus = false;
            }
        });
    } else {
        ShowFailTip('请填写昵称！');
        nicknameStatus = false;
    }
    return;
}

//验证密码
function CheckPwd(obj) {
    var pwdval = obj.val();
    if (pwdval.length == 0) {
        ShowFailTip('请填写密码！');
        pwdStatus = false;
        return;
    }
    if (!regs.test(pwdval)) {
        ShowFailTip(errormsg);
        pwdStatus = false;
    } else {
        pwdStatus = true;
    }
}

//验证确认密码
function CheckVPwd(obj) {
    if (obj.val() != "") {
        if (obj.val() != $("#pwd").val()) {
            ShowFailTip("两次填写的不一致，请重新填写");
            vpwdStatus = false;
        } else {
            vpwdStatus = true;
        }
    } else {
        ShowFailTip("请填写确认密码");
        vpwdStatus = false;
    }
}

//验证协议
function CheckAgreement(obj) {
    if (obj.attr("checked")) {
        $("#divAgreementTip").removeClass("msg msg-err").removeClass("msg msg-info").html("");
        agreementStatus = true;
    } else {
        ShowFailTip("请先阅读并同意《用户服务协议》");
        agreementStatus = false;
    }
}



function CountDown() {
    if (smsSeconds < 0) {
        //                $("[id$='txtPhone']").removeAttr("disabled");
        isOK = true;
        $("#btnSendSMS").removeAttr("disabled");
        clearInterval(intervaSMS);
    }
    else {
        $("#btnSendSMS").attr("value", "请在(" + smsSeconds + ")秒后重新发送");
        $("#btnSendSMS").attr("disabled", "disabled");
        //                $("[id$='txtPhone']").attr("disabled", "disabled");
        isOK = false;
        smsSeconds--;
    }
}
 