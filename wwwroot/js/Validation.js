function FileCountValidation(id) {
    var uploadFiles = document.getElementById(id);
    var files = uploadFiles.files;
    if (files.length > 0)
        return true;
    else
        return false;
}
function FileSizeValidation(id, fileSizeLimit) {
    var uploadFiles = document.getElementById(id);
    var files = uploadFiles.files;
    var contentSize = 0;
    for (var i = 0; i <= files.length - 1; i++) {
        var fsize = files.item(i).size;
        contentSize += fsize;
        // The size of the file.
    }
    if (contentSize > fileSizeLimit) {
        //alert(`File is too Big, please select a file less than ${fileSizeLimit} bytes!`);
        return false;
    }
    else {
        return true;
    }
}
//# sourceMappingURL=Validation.js.map