let shareURL = async()=>{
    let contentToCopy = import.meta.env.VITE_API_URL + `joinGroupBuy?salepageid=${localStorage.getItem("salepageid")}&recommender=${localStorage.getItem("username")}`
    // console.log(import.meta.env.VITE_API_URL + `joinGroupBuy?salepageid=${localStorage.getItem("salepageid")}&recommender=${localStorage.getItem("username")}`)
    try {
        await navigator.clipboard.writeText(contentToCopy);
        alert('URL 已複製到剪貼簿!');
    } catch (err) {
        alert('複製失敗:', err);
    }
}

export default {shareURL}