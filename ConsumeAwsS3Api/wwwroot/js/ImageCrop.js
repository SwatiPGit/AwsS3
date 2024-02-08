﻿$(document).ready(function () {
    var canvas;
    var context;
    var Originalcanvas;
    var Originalcontext;
    var imageCropWidth = "0";
    var imageCropHeight = "0";
    var cropPointX = "";
    var cropPointY = "";
    var originalImageName = "";
    var originalImageExtenion = "";
    var image;
    var prefsize;
    var btnEventName;

    getOriginalFileName();
    initCrop();

    function getOriginalFileName() {
        var imageName = $('#imgEmpPhoto').attr("alt");
        originalImageName = imageName.split(".")[0];
        originalImageExtenion = imageName.split(".")[1];
    }

    function initCrop() {

        $('#imgEmpPhoto').Jcrop({
            onChange: setCoordsAndImgSize,
            aspectRatio: 0, // 1 means will be same for height and weight
            onSelect: setCoordsAndImgSize,
            boxWidth: 400,
            boxHeight: 400
        }, function () { jcropApi = this });
    }
    function setCoordsAndImgSize(e) {

        imageCropWidth = e.w;
        imageCropHeight = e.h;

        cropPointX = e.x;
        cropPointY = e.y;

        $("#lblWidth").text(Math.round(imageCropWidth) + "px");
        $("#lblHeight").text(Math.round(imageCropHeight) + "px");
    }

    $("#btnClose").on("click", function (e) {
        e.preventDefault();
        $('#alert-NoCropwarning').hide();
    });
    $("#btnCrop").on("click", function (e) {
        e.preventDefault();
        cropImage();
    });
    function cropImage() {


        if (imageCropWidth == 0 && imageCropHeight == 0) {
            $('#alert-NoCropwarning').show();
            return;
        }
        if (imageCropWidth == 0 && imageCropHeight == 0) {

        }
        /*Show cropped image*/
        showCroppedImage();
        $('#divCanvas').show();
        $('#originalImage').hide();
        $('#btnCrop').hide();
        $('#btnBack').show();
        $('#btnSave').show();

    }


    function showCroppedImage() {
        var x1 = cropPointX;
        var y1 = cropPointY;

        var width = Math.round(imageCropWidth);
        var height = Math.round(imageCropHeight);

        canvas = $("#canvas")[0];
        context = canvas.getContext('2d');
        var img = new Image();
        img.onload = function () {
            canvas.height = height;
            canvas.width = width;
            context.drawImage(img, x1, y1, width, height, 0, 0, 400, 500);
        };
        img.src = $('#imgEmpPhoto').attr("src");

        Originalcanvas = $("#Originalcanvas")[0];
        Originalcontext = Originalcanvas.getContext('2d');
        var Originalimg = new Image();
        Originalimg.onload = function () {
            Originalcanvas.height = height;
            Originalcanvas.width = width;
            Originalcontext.drawImage(Originalimg, x1, y1, width, height, 0, 0, width, height);
        };
        Originalimg.src = $('#imgEmpPhoto').attr("src");
    }

    $("#btnBack").on("click", function (e) {

        e.preventDefault();
        $('#alert-danger').hide();
        $('#originalImage').show();
        $('#btnCrop').show();
        $('#btnBack').hide();
        $('#divCanvas').hide();
        $('#btnSave').hide();
    });

    function b64toBlob(b64Data, contentType, sliceSize) {
        contentType = contentType || '';
        sliceSize = sliceSize || 512;
        var byteCharacters = atob(b64Data);
        var byteArrays = [];
        for (var offset = 0; offset < byteCharacters.length; offset += sliceSize) {
            var slice = byteCharacters.slice(offset, offset + sliceSize);
            var byteNumbers = new Array(slice.length);
            for (var i = 0; i < slice.length; i++) {
                byteNumbers[i] = slice.charCodeAt(i);
            }
            var byteArray = new Uint8Array(byteNumbers);
            byteArrays.push(byteArray);
        }
        var blob = new Blob(byteArrays, { type: contentType }, { fileName: "ppp.JPG" });
        return blob;
    }

    function GetCurrentDate() {
        var tdate = new Date();
        var dd = tdate.getDate(); //yields day
        var MM = tdate.getMonth(); //yields month
        var yyyy = tdate.getFullYear(); //yields year
        var hour = tdate.getHours();
        var minute = tdate.getMinutes();
        var second = tdate.getSeconds();
        var currentDate = dd + "" + (MM + 1) + "" + yyyy + "" + hour + "" + minute + "" + second;
        return currentDate;
    }
    function SaveImage() {

        var fileUpload = $("#Originalcanvas")[0].toDataURL();
        var imageonly = fileUpload.split(';');

        var contenttype = imageonly[0].split(':')[1];
        var fileformat = contenttype.split('/')[1];
        var realdata = imageonly[1].split(',')[1];


        var blob = b64toBlob(realdata, contenttype);
        currentDate = GetCurrentDate();
        var Newfilename = originalImageName + "_thumb" + currentDate + "." + originalImageExtenion;
        var data = new FormData();
        data.append("UploadLocation", "\\images\\profiles\\"); ///location
        data.append("FileInitials", Newfilename);
        data.append(Newfilename, blob, Newfilename);
        data.append("originalImageName", originalImageName + "." + originalImageExtenion);


        $.ajax({
            type: "POST",
            url: `${location.protocol}//${window.location.host}/AwsS3FileDetails/SaveCroppedImage`,
            contentType: false,
            processData: false,
            data: data,
            success: function (path) {

                $('#divCropImage').hide();
                $('#alert-success').show();
            },
            error: function (jqXHR, exception, errorThrown) {
                $('#alert-danger').show();

            },
        });
    }
    $("#btnSave").on("click", function (e) {
        event.preventDefault();

        SaveImage();

    });
});