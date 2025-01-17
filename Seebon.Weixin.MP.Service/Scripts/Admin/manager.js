﻿// JavaScript Document
//管理操作所需的所有函数
//全选取消按钮函数，调用样式如：
function checkAll() {
    if ($("#btncheckall").text() == "全选") {
        $("#btncheckall").text("取消");
        $(".checkall").attr("checked", true);
    } else {
        $("#btncheckall").text("全选");
        $(".checkall").attr("checked", false);
    }
}

$(function(){
	//关闭打开左栏目
	$("#sysBar").toggle(function(){
		$("#mainLeft").hide();
		$("#barImg").attr("src", "/Content/Admin/images/butOpen.gif");
	},function(){
		$("#mainLeft").show();
		$("#barImg").attr("src", "/Content/Admin/images/butClose.gif");
	});
	//页面加载完毕，显示第一个子菜单
	$(".left_menu").hide();
	$(".left_menu:eq(0)").show();
});
//后台主菜单控制函数
function tabs(tabNum){
	//设置点击后的切换样式
	$("#tabs ul li").removeClass("hover");
	$("#tabs ul li").eq(tabNum-1).addClass("hover");
	//根据参数决定显示子菜单
	$(".left_menu").hide();
	$(".left_menu").eq(tabNum).show();
}
//遮罩提示窗口
function jsmsg(w, h, msgtitle, msgbox, url,msgcss) {
    $("#msgdialog").remove();
    var cssname = "";
    switch (msgcss) {
        case "Success":
            cssname = "icon-01";
            break;
        case "Error":
            cssname = "icon-02";
            break;
        default:
            cssname = "icon-03";
            break;
    }
    var str = "<div id='msgdialog' title='" + msgtitle + "'><p class='" + cssname + "'>" + msgbox + "</p></div>";
    $("body").append(str);
    $("#msgdialog").dialog({
        //title: null,
        //show: null,
        bgiframe: true,
        autoOpen: false,
        width: w,
        //height: h,
        resizable: false,
        closeOnEscape: true,
        buttons: { "确定": function() { $(this).dialog("close"); } },
        modal: true
    });
    $("#msgdialog").dialog("open");
    if (url == "back") {
        sysMain.history.back(-1);
    } else if(url !="") {
        sysMain.location.href = url;
    }
}

//可以自动关闭的提示
function jsprint(msgtitle, url, msgcss) {
    $("#msgprint").remove();
    var cssname = "";
    switch (msgcss) {
        case "Success":
            cssname = "pcent correct";
            break;
        case "Error":
            cssname = "pcent disable";
            break;
        default:
            cssname = "pcent warning";
            break;
    }
    var str = "<div id=\"msgprint\" class=\"" + cssname + "\">" + msgtitle + "</div>";
    $("body").append(str);
    $("#msgprint").show();
    if (url == "back") {
        sysMain.history.back(-1);
    } else if (url != "") {
        sysMain.location.href = url;
    }
    //3秒后清除提示
    setTimeout(function() {
        $("#msgprint").fadeOut(500);
        //如果动画结束则删除节点
        if (!$("#msgprint").is(":animated")) {
            $("#msgprint").remove();
        }
    }, 3000);
}
//遮罩层上传
function ShowUploadDIV(thisObjID, inname) {
    $("#UploadHideDiv").css({ display: "block", height: $(document).height() });
    var yscroll = document.documentElement.scrollTop;
    $("#" + thisObjID).css("top", "180px");
    $("#" + thisObjID).css("display", "block");
    document.documentElement.scrollTop = 0;
    $("#hloadname").val(inname);
}

function closeDiv(thisObjID) {
    $("#UploadHideDiv").css("display", "none");
    $("#" + thisObjID).css("display", "none");
    $("#hloadname").val("");
}

function fromUpload() {
    $("#FromUpload").ajaxSubmit({
        beforeSubmit: function (formData, jqForm, options) {
            //隐藏上传按钮
            $(".files").hide();
            //显示LOADING图片
             $(".uploading").show();
        },
        success: function (data) {
            if (data == "error") {
                alert("出错了！");
            }
            else {
                var inname = $("#hloadname").val();
                closeDiv('UploadDiv');
                $("#" + inname).val(data);
                alert('上传成功！');
            }

        },
        error: function (data, status, e) {
            alert("上传失败，错误信息：" + e);
              $(".files").show();
              $(".uploading").hide();
        },
        url: "/Admin/Common/Upload",
        type: "post",
        dataType: "text",
        timeout: 600000
    });
}
//遮罩层上传