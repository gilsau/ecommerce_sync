var prodUrls = null;
var logoUrls = null;
var filePaths = null;
var prodFilesDwn = 0;
var imgFilesDwn = 0;
var prodFilesSaved = 0;

$(document).ready(function () {

    console.log('Url.baseUrl(): ' + Url.baseUrl());

    prodUrls = $('.prodUrlTD');
    logoUrls = $('.logoUrlTD img');
    
    //download files btn
    $('#spanDwnFiles').click(function () {

        //product files
        InitializeDownloads();
    });

    //save files btn
    $('#spanSaveFiles').click(function () {

        //get files to save
        $.ajax({
            method: "GET",
            url: Url.baseUrl() + "/home/GetDownloadedFiles"
        })
        .done(function (data) {
            filePaths = data;
            
            var htmlFD = '';
            var iCnt = 0;
            for (var d in data) {
                htmlFD += '<div style="border-bottom:solid 1px #eee;padding:2px 0px 2px 0px;">' + iCnt + ') ' + data[d] + '</div>';
                iCnt++;
            }
            $('#pnlFileDownloads').html(htmlFD);

            //start saving files
            if (data.length > 0) {
                InitializeDatabase();
            }
        });
    });

    //set panel heights
    $('#downloadTop, #downloadBottom').css('height', ($(window).height() - 300) / 2);

    $('#pnlLogTop, #pnlLogBottom').css('height', ($(window).height() - 200) / 2);
});

function CleanUpDatabase() {
    $.ajax({
        method: "GET",
        url: Url.baseUrl() + "/home/truncatetable",
        data: { table: 'ProductImport' }
    })
        .done(function (data) {
            $('#pnlDwn2').html(data.ErrForUser + '<hr/>' + $('#pnlDwn2').html());
            PopulateProductImportTable();
        });
}

function PopulateProductImportTable() {
    $.ajax({
        method: "GET",
        url: Url.baseUrl() + "/home/PopulateProductImportTable"
    })
        .done(function (data) {
            $('#pnlDwn2').html(data.ErrForUser + '<hr/>' + $('#pnlDwn2').html());
            CreateCategoriesTable();
        });
}

function CreateCategoriesTable() {
    $.ajax({
        method: "GET",
        url: Url.baseUrl() + "/home/CreateCategoriesTable"
    })
        .done(function (data) {
            $('#pnlDwn2').html(data.ErrForUser + '<hr/>' + $('#pnlDwn2').html());
        });
}

function InitializeDatabase() {
    $.ajax({
        method: "GET",
        url: Url.baseUrl() + "/home/TruncateTable",
        data: { table: 'ProductImportPre' }
    })
    .done(function (data) {
        $('#pnlDwn2').html(data.ErrForUser + '<hr/>' + $('#pnlDwn2').html());
        SaveFile(0);
    });
}

function SaveFile(fileIndex) {
    var path = filePaths[fileIndex];

    //console.log('path: ' + path);

    $.ajax({
        method: "GET",
        url: Url.baseUrl() + "/home/SaveProductsFromFileToDB",
        data: { filePath:path  }
    })
    .done(function (data) {
        $('#pnlDwn2').html(data.ErrForUser + $('#pnlDwn2').html());
        ShowProgress($('#spanSaveFilesProg'), 2);

        //check if filelist is done
        if (filePaths.length - 1 >= fileIndex + 1) {
            SaveFile(fileIndex + 1);
        }
        else {
            CleanUpDatabase();
        }
    });
    
}

function DownloadUrl(urlIndex) {
    var url = $(prodUrls[urlIndex]).text();
    var logo = $(logoUrls[urlIndex]).attr('src');
    var arrLogo = logo.split('/');
    logo = arrLogo.length > 0 ? arrLogo[arrLogo.length - 1] : '2b';
    logo = logo.replace('.png', '').replace('.jpg', '').replace('.jpeg', '').replace('.gif', '');
    logo = logo.indexOf('2b') > -1 ? '2b' : logo;

    //console.log('urlIndex: ' + urlIndex + ', url: ' + url + ', logo: ' + logo + ', logo.indexOf(2b): ' + logo.indexOf('2b'));

    if(url){
        $.ajax({
            method: "POST",
            url: Url.baseUrl() + "/home/downloadFileFromUrl",
            data: { url:url, logo:logo }
        })
        .done(function (data) {
            $('#pnlDwn1').html(data.ErrForUser + '<hr/>' + $('#pnlDwn1').html());
            $('#pnlFileDownloads').html('<div class="filePathTD">' + data.ReturnObj + '</div>' + $('#pnlFileDownloads').html());
            ShowProgress($('#spanDwnFilesProg'), 1);

            if (urlIndex + 1 <= prodUrls.length-1) {
                DownloadUrl(urlIndex + 1);
            }
        });
    }
}

function InitializeDownloads() {
    $.ajax({
        method: "GET",
        url: Url.baseUrl() + "/home/ClearFileSaveDir"
    })
    .done(function (data) {
        $('#pnlDwn1').html(data.ErrForUser + '<hr/>' + $('#pnlDwn1').html());
        DownloadUrl(0);
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
    bar.css('width', pct + '%');
    bar.text(pct + '%');
}