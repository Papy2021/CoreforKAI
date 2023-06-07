function confirmDelete(uniqueId, isDeleteClicked) {
    var deleteSpan = 'deleteSpan_' + uniqueId;
    var ConfirmDeleteSpan = 'confirmDeleteSpan_' + uniqueId

    if (isDeleteClicked) {
        $('#' + deleteSpan).hide();
        $('#' + ConfirmDeleteSpan).show()
    }
    else {
        $('#' + deleteSpan).show();
        $('#' + ConfirmDeleteSpan).hide();
    }
}

