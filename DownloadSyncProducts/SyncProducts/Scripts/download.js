var prodUrls = null;
var imgUrls = null;
var filePaths = null;
var prodFilesDwn = 0;
var imgFilesDwn = 0;
var prodFilesSaved = 0;

$(document).ready(function () {
    
    prodUrls = $('.prodUrlTD');
    imgUrls = $('.imgUrlTD');

    //download files btn
    $('#spanDwnFiles').click(function () {

        //product files
        ClearFileSaveDir(1);
        DownloadUrl(0, 1);

        //image files
        //ClearFileSaveDir(2);
        //DownloadUrl(0, 2);
    });

    //save files btn
    $('#spanSaveFiles').click(function () {
        TruncateProductImportTable();
        filePaths = $('.filePathTD');
        SaveFile(0);
    });

    //set panel heights
    $('#downloadTop, #downloadBottom, #pnlLogTop, #pnlLogBottom').css('height', ($(window).height() - 460) / 2);

});

function TruncateProductImportTable() {
    $.ajax({
        method: "GET",
        url: "/home/TruncateProductImportTable"
    })
    .done(function (data) {
        $('#pnlDwn2').html(data.ErrForUser + $('#pnlDwn2').html());
    });
}

function SaveFile(fileIndex) {

    console.log('filePaths');
    console.log(filePaths);
    console.log('');

    var path = $(filePaths[fileIndex]).text();

    console.log('Path: ' + path);
    console.log('FileIndex: ' + fileIndex);

    

    $.ajax({
        method: "GET",
        url: "/home/SaveProductsFromFileToDB",
        data: { filePath: path }
    })
    .done(function (data) {
        if (data.ErrForUser != null) {
            console.log('data.ErrForUser: ' + data.ErrForUser);
            $('#pnlDwn2').html(data.ErrForUser + $('#pnlDwn2').html());
        }
        ShowProgress($('#spanSaveFilesProg'), 2);

        if (fileIndex + 1 <= filePaths.length-1) {
            SaveFile(fileIndex + 1);
        }
    });
    
}

function DownloadUrl(urlIndex, urlType) {
    var url = urlType == 1 ? $(prodUrls[urlIndex]).text() : $(imgUrls[urlIndex]).text();

    if(url){
        $.ajax({
            method: "POST",
            url: "/home/downloadFileFromUrl",
            data: { url: url, urlType: urlType }
        })
        .done(function (data) {
            $('#pnlDwn1').html(data.ErrForUser + $('#pnlDwn1').html());
            $('#pnlFileDownloads').html('<div class="filePathTD">' + data.ReturnObj + '</div>' + $('#pnlFileDownloads').html());
            ShowProgress($('#spanDwnFilesProg'), 1);

            if (urlIndex + 1 <= prodUrls.length-1) {
                DownloadUrl(urlIndex + 1, urlType);
            }
        });
    }
}

function ClearFileSaveDir(urlType) {
    $.ajax({
        method: "GET",
        url: "/home/ClearFileSaveDir",
        data: { urlType: urlType }
    })
    .done(function (data) {
        $('#pnlDwn1').html(data.ErrForUser + $('#pnlDwn1').html());
    });
}

function ShowProgress(bar, progType) {
    var pct = 0;

    if (progType == 1) {
        prodFilesDwn += 1;
        pct = (prodFilesDwn / prodUrls.length) * 100;
    }
    else if (progType == 2) {
        prodFilesSaved += 1;
        pct = (prodFilesSaved / filePaths.length) * 100;
    }
    console.log('pct: ' + pct);
    bar.css('width', pct + '%');
    bar.text(pct + '%');
}