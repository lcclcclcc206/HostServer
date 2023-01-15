function FileSizeValidation(id, fileSize) {
    console.log(id);
    var uploadFiles = document.getElementById(id);
    var files = uploadFiles.files;
    if (files.length > 0) {
        for (var i = 0; i <= files.length - 1; i++) {
            var fsize = files.item(i).size;
            // The size of the file.
            if (fsize > fileSize) {
                alert("File is too Big, please select a file less than ".concat(fileSize));
            }
        }
    }
}
//# sourceMappingURL=Validation.js.map