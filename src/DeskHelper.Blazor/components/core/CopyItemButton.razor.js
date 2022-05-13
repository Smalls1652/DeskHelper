export const copyTextToClipboard = (text) => {
    let blobType = "text/html";
    let blob = new Blob(
        [text],
        {
            type: blobType
        }
    );

    let clipboardData = new ClipboardItem(
        {
            [blobType]: blob
        }
    );

    console.log(blob);
    console.log(clipboardData);
    console.log(text);

    navigator.clipboard.write([clipboardData]);
}