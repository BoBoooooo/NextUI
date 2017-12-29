 
$(function () {
    $('#file_upload').uploadifive({
        'auto': false,
        'uploadScript': '/FileUpload/Upload',
        'fileObjName': 'fileData',
        'buttonText': '选择文件',
        'queueID': 'fileQueue',
        'fileType': '*',
        'multi': true,
        'fileSizeLimit': '500MB',
        'uploadLimit': 10,
        'queueSizeLimit': 10,
        'removeCompleted': true,
        'onUploadComplete': function (file, data) {
            refreshFileUploadGrid();

        },
        onCancel: function (file) {

        },
        onFallback: function () {

        },
        onUpload: function (file) {

            $('#file_upload').data('uploadifive').settings.formData = { 'folder': 'Upload', 'guid': $("#info_AttachmentID").val() };
        }
    });

});

//绑定附件列表
function ShowUpFiles(guid, show_div) {


    $.ajax({
        type: 'GET',
        url: '/FileUpload/GetAttachmentHtml?guid=' + guid,
        //async: false, //同步
        //dataType: 'json',
        success: function (json) {
            $("#" + show_div + "").html(json);
        },
        error: function (xhr, status, error) {
            $.messager.alert("提示", "操作失败"); //xhr.responseText
        }
    });
}

//删除附件id：当前附件ID
var attachguid = "";//用来记录附件组的ID，方便更新
function deleteAttach(id) {

    $.messager.confirm("删除确认", "您确定要删除该附件吗？", function (deleteAction) {
        if (deleteAction) {
            $.ajax({
                type: 'POST',
                url: '/FileUpload/Delete?id=' + id,
                async: false,
                success: function (msg) {
                    //ShowUpFiles(attachguid, "div_files");//更新列表
                    refreshFileUploadGrid();
                },
                error: function (xhr, status, error) {
                    $.messager.alert("提示", "操作失败"); //xhr.responseText
                }
            });
        }
    });
}

function newGuid() {
    var guid = "";
    for (var i = 1; i <= 32; i++) {
        var n = Math.floor(Math.random() * 16.0).toString(16);
        guid += n;
        if ((i == 8) || (i == 12) || (i == 16) || (i == 20))
            guid += "-";
    }
    return guid;
}
