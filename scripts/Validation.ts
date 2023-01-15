function FileSizeValidation(id: string, fileSize: number) {
    console.log(id);
    const uploadFiles = document.getElementById(id) as HTMLInputElement;
    const files = uploadFiles.files as FileList;

    if (files.length > 0) {
        for (let i = 0; i <= files.length - 1; i++) {
            const fsize = files.item(i).size;
            // The size of the file.
            if (fsize > fileSize) {
                alert(
                    `File is too Big, please select a file less than ${fileSize}`);
            }
        }
    }
}