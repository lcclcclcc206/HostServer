function FileCountValidation(id: string): boolean {
    const uploadFiles = document.getElementById(id) as HTMLInputElement;
    const files = uploadFiles.files as FileList;
    if (files.length > 0)
        return true;
    else
        return false;
}

function FileSizeValidation(id: string, fileSizeLimit: number): boolean {
    const uploadFiles = document.getElementById(id) as HTMLInputElement;
    const files = uploadFiles.files as FileList;
    let contentSize: number = 0;

    for (let i = 0; i <= files.length - 1; i++) {
        const fsize = files.item(i).size;
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