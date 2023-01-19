function ChangeButtonState(id: string, active: boolean) {
    let button = document.getElementById(id) as HTMLButtonElement;
    if (active) {
        button.disabled = false;
    }
    else {
        button.disabled = true;
    }
}