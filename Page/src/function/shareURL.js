let shareURL = async(hashname)=>{
    let contentToCopy = import.meta.env.VITE_API_URL + `joinGroupBuy?salepageid=${localStorage.getItem("salepageid")}&recommender=${hashname}`
    // console.log(import.meta.env.VITE_API_URL + `joinGroupBuy?salepageid=${localStorage.getItem("salepageid")}&recommender=${localStorage.getItem("username")}`)
    const textarea = document.createElement("textarea");
    textarea.value = contentToCopy;
    document.body.appendChild(textarea);
    textarea.select();
    try {
        // await navigator.clipboard.writeText(contentToCopy);
        document.execCommand("copy");
        alert('URL 已複製到剪貼簿!');
    } catch (err) {
        alert('複製失敗:', err);
    }
    document.body.removeChild(textarea);
}

export default {shareURL}