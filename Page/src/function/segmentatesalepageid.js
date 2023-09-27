let segmentatesalepageid = ()=>{
    let url = window.location.href;
    // console.log(url)
    var queryString = url.split('?')[1];
    // console.log(queryString)
    // 使用split()方法分割查询字符串并解析参数
    var paramArray = queryString.split('&');
    // 创建一个对象来存储参数
    var params = {};
    for (var i = 0; i < paramArray.length; i++) {
        var param = paramArray[i].split('=');
        params[param[0]] = decodeURIComponent(param[1]);
    }
    // 获取特定参数的值
    let paramValue = params['salepageid'];
    // console.log(typeof paramValue)

    return paramValue;
}

export default {segmentatesalepageid}