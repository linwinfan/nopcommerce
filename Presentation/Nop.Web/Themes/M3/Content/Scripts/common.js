function autoLoadJs(jsFile) { var oHead = document.getElementsByTagName('HEAD').item(0); var oScript = document.createElement("script"); oScript.type = "text/javascript"; oScript.src = jsFile + '&t=' + Math.random(); oHead.appendChild(oScript); }
(function (jQuery) {
    var _popupObj; var _param = {}; var _isShowMask; var _isHideSelects; var _maskObj; var _hidedSelects; jQuery.fn.setObjToCenter = function (isShowMask, minTopDist, isHideSelects) {
        var maskObjId = 'ql_common_mask'; var options; if ('object' === typeof (arguments[0])) { options = arguments[0]; } else { options = { isShowMask: isShowMask, minTopDist: minTopDist, isHideSelects: isHideSelects }; }
        if (true === options.isShowMask && 'undefined' == typeof (options.isHideSelects)) { options.isHideSelects = true; }
        if (false === _isIE6()) { options.isHideSelects = false; }
        options = jQuery.extend({}, jQuery.fn.setObjToCenter.defaults, options); _popupObj = this; _param = options; _isShowMask = options.isShowMask; _isHideSelects = options.isHideSelects; var m_left = parseInt(_getTotalWidth() / 2 - this.outerWidth() / 2) + jQuery(document).scrollLeft(); var m_top = parseInt(_getTotalHeight() / 2 - this.outerHeight() / 2) + jQuery(document).scrollTop(); if (options.minTopDist > 0) { var mix_top = jQuery(document).scrollTop() + options.minTopDist; if (m_top < mix_top) { m_top = mix_top; } }
        if (options.isHideSelects) { _hidedSelects = $('select').not($('select', this)); _hidedSelects.hide(); }
        this.css({ left: m_left, top: m_top }); if (true === options.isShowMask) { jQuery('#' + maskObjId).remove(); _maskObj = jQuery('<div style="display:none"></div>'); _maskObj.attr('id', maskObjId); _maskObj.css({ 'position': "absolute", 'background': "#000", 'top': "0px", 'left': "0px", 'width': "100%", 'zIndex': "9997", 'filter': "alpha(opacity=30)", '-moz-opacity': "0.3", 'opacity': "0.3" }); _maskObj.css("width", document.body.scrollWidth).css("height", document.body.scrollHeight); _maskObj.appendTo('body').show(); }
        return this;
    }; jQuery(window).resize(function () { if (!_popupObj) return false; if (_popupObj.css('display') != 'none') { _popupObj.setObjToCenter(_param); } }); jQuery.fn.setObjToCenter.defaults = { isShowMask: false, minTopDist: 0, isHideSelects: false }; jQuery.fn.setObjToCenter_hide = function () {
        this.hide(); if (_isShowMask) { _maskObj.hide(); }
        if (_isHideSelects && 'undefined' != typeof (_hidedSelects)) { _hidedSelects.show(); } 
    }; function _getTotalWidth() { if (jQuery.browser.msie) { return document.compatMode == "CSS1Compat" ? document.documentElement.clientWidth : document.body.clientWidth; } else { return self.innerWidth; }; }; function _getTotalHeight() { if (jQuery.browser.msie) { return document.compatMode == "CSS1Compat" ? document.documentElement.clientHeight : document.body.clientHeight; } else { return self.innerHeight; }; }; function _isIE6() { return jQuery.browser.msie && (jQuery.browser.version == "6.0") && !jQuery.support.style; } 
})(jQuery); (function (jQuery) {
    var _getTotalWidth = function () { if (jQuery.browser.msie) { return document.compatMode == "CSS1Compat" ? document.documentElement.clientWidth : document.body.clientWidth; } else { return self.innerWidth; }; }; var _getTotalHeight = function () { if (jQuery.browser.msie) { return document.compatMode == "CSS1Compat" ? document.documentElement.clientHeight : document.body.clientHeight; } else { return self.innerHeight; }; }; var _popupObj; var _param = {}; jQuery.fn.setObjToCenter = function (isShowMask, maskObjId, minTopDist) {
        var options; if ('object' === typeof (arguments[0])) { options = arguments[0]; } else { options = { isShowMask: isShowMask, maskObjId: maskObjId, minTopDist: minTopDist }; }
        options = jQuery.extend({}, jQuery.fn.setObjToCenter.defaults, options); _popupObj = this; _param = options; var m_left = parseInt(_getTotalWidth() / 2 - this.outerWidth() / 2) + jQuery(document).scrollLeft(); var m_top = parseInt(_getTotalHeight() / 2 - this.outerHeight() / 2) + jQuery(document).scrollTop(); if (options.minTopDist > 0) { var mix_top = jQuery(document).scrollTop() + options.minTopDist; if (m_top < mix_top) { m_top = mix_top; } }
        this.css({ left: m_left, top: m_top }); if (true === options.isShowMask) { jQuery('#' + options.maskObjId).remove(); var maskObj = jQuery('<div style="display:none"></div>'); maskObj.css({ 'position': "absolute", 'background': "#000", 'top': "0px", 'left': "0px", 'width': "100%", 'zIndex': "9997", 'filter': "alpha(opacity=30)", '-moz-opacity': "0.7", 'opacity': "0.7" }); maskObj.attr('id', options.maskObjId); maskObj.css("width", document.body.scrollWidth).css("height", document.body.scrollHeight); maskObj.appendTo('body').show(); }
        return this;
    }; jQuery(window).resize(function () { if (!_popupObj) return false; if (_popupObj.css('display') != 'none') { _popupObj.setObjToCenter(_param); } }); jQuery.fn.setObjToCenter.defaults = { isShowMask: false, maskObjId: 'ql_default_mask', minTopDist: 0 };
})(jQuery); (function (jQuery) {
    var isMoving = false; var relativeTop = 0; var relativeLeft = 0; if ('undefined' == typeof (jQuery.fn.setObjToCenter)) { alert('jQuery.fn.setObjToCenter is not defined'); return false; }
    jQuery.qlAlert = function (options) { options = jQuery.extend({}, jQuery.qlAlert.defaults, options); var mask_id = 'ql_alert_mask'; var qlalert_id = 'ql_alert'; jQuery('#' + mask_id).remove(); jQuery('#' + qlalert_id).remove(); var str = ''; str += '<div id="' + qlalert_id + '" class="ql_common_alert" style="display:none">'; str += '<h2 class="ql_common_alert_title"><span class="ql_common_alert_close alert_close"></span>' + options.title + '</h2>'; str += '<div class="ql_common_alert_content">'; str += '<p>' + options.content + '</p>'; str += '<input type="button" class="ql_common_alert_button alert_close" value="' + options.btnText + '" hidefocus="true" />'; str += '</div>'; str += '</div>'; str += '<div class="qi_common_bg">&nbsp;</div>'; var alertObj = jQuery(str); alertObj.find('.ql_common_alert_title').mousedown(function (e) { isMoving = true; relativeTop = e.clientY - alertObj.offset().top; relativeLeft = e.clientX - alertObj.offset().left; }).mouseup(function () { isMoving = false; }); jQuery('body').mousemove(function (e) { if (isMoving) { alertObj.css({ left: e.clientX - relativeLeft, top: e.clientY - relativeTop }); }; }); alertObj.find('.alert_close').click(function () { if (options.isShowMask) jQuery('#' + mask_id).hide(); alertObj.hide(); if (options.callback) options.callback(); }); alertObj.appendTo('body').setObjToCenter(options.isShowMask, mask_id).show(); alertObj.find('.ql_common_alert_button').focus(); return alertObj; }; jQuery.qlAlert.defaults = { content: '提示文字', callback: function () { }, isShowMask: false, title: '温馨提示', btnText: '确定' }; window.qlAlert = function (content, callback, isShowMask, title, btnText) {
        var options; if ('object' === typeof (arguments[0])) { options = arguments[0]; } else { options = { content: content, callback: callback, isShowMask: isShowMask, title: title, btnText: btnText }; }
        jQuery.qlAlert(options);
    } 
})(jQuery); (function (jQuery) {
    var isMoving = false; var relativeTop = 0; var relativeLeft = 0; if ('undefined' == typeof (jQuery.fn.setObjToCenter)) { alert('jQuery.fn.setObjToCenter is not defined'); return false; }
    jQuery.qlConfirm = function (options) { options = jQuery.extend({}, jQuery.qlConfirm.defaults, options); var mask_id = 'ql_confirm_mask'; var qlconfirm_id = 'ql_confirm'; jQuery('#' + mask_id).remove(); jQuery('#' + qlconfirm_id).remove(); var str = ''; str += '<div id="' + qlconfirm_id + '" class="ql_common_confirm" style="display:none">'; str += '<h2 class="ql_common_confirm_title"><span class="ql_common_confirm_close confirm_close"></span>' + options.title + '</h2>'; str += '<div class="ql_common_confirm_content">'; str += '<p>' + options.content + '</p>'; str += '<input type="button" class="ql_common_confirm_button button_left" value="' + options.btnTextL + '" hidefocus="true" />'; str += '<input type="button" class="ql_common_confirm_button button_right" value="' + options.btnTextR + '" hidefocus="true" />'; str += '</div>'; str += '<div class="qi_common_bg">&nbsp;</div>'; str += '</div>'; var confirmObj = jQuery(str); confirmObj.find('.ql_common_confirm_title').mousedown(function (e) { isMoving = true; relativeTop = e.clientY - confirmObj.offset().top; relativeLeft = e.clientX - confirmObj.offset().left; }).mouseup(function () { isMoving = false; }); jQuery('body').mousemove(function (e) { if (isMoving) { confirmObj.css({ left: e.clientX - relativeLeft, top: e.clientY - relativeTop }); }; }); confirmObj.find('.button_left').click(function () { if (options.isShowMask) jQuery('#' + mask_id).hide(); confirmObj.hide(); if (options.callbackL) options.callbackL(); }); confirmObj.find('.button_right').click(function () { if (options.isShowMask) jQuery('#' + mask_id).hide(); confirmObj.hide(); if (options.callbackR) options.callbackR(); }); confirmObj.find('.confirm_close').click(function () { if (options.isShowMask) jQuery('#' + mask_id).hide(); confirmObj.hide(); if ('undefined' === typeof (options.callbackClose)) { options.callbackClose(); } else if (options.callbackR) { options.callbackR(); } }); confirmObj.appendTo('body').setObjToCenter(options.isShowMask, mask_id).show(); confirmObj.find('.ql_common_confirm_button').focus(); return confirmObj; }; jQuery.qlConfirm.defaults = { content: '提示文字', callbackL: function () { }, callbackR: function () { }, callbackClose: function () { }, isShowMask: false, title: '温馨提示', btnTextL: '确定', btnTextR: '取消' }; window.qlConfirm = function (content, isShowMask, callbackL, callbackR, title, btnTextL, btnTextR, callbackClose) {
        var options; if ('object' === typeof (arguments[0])) { options = arguments[0]; } else { options = { content: content, callbackL: callbackL, callbackR: callbackR, callbackClose: callbackClose, isShowMask: isShowMask, title: title, btnTextL: btnTextL, btnTextR: btnTextR }; }
        jQuery.qlConfirm(options);
    }
})(jQuery);


//判断是否含有禁用词
function ContainsDisWords(desc) {
    var isContain = false;
    $.ajax({
        url: $Maticsoft.BasePath + "Partial/ContainsDisWords",
        type: 'post', dataType: 'text', timeout: 10000,
        async: false,
        data: { Desc: desc },
        success: function (resultData) {
            if (resultData == "True") {
                isContain = true;
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            ShowFailTip("操作失败：" + errorThrown);
        }
    });
    return isContain;
}