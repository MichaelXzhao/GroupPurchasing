<script>
import { onMounted, ref } from 'vue';
import Loading from "../../components/Loading.vue"
import axios from 'axios'
import changeqty from '../../function/changeqty';
import router from '../../router/router';
import shareURL from '../../function/shareURL';
import discountTitle from '../../components/discountTitle.vue';
import * as signalR from "@aspnet/signalr";

export default{
    setup() {
        let data = ref({})
        let memberData = ref()
        let totalqty = ref(0)
        let inputqty = ref(0)
        let imgurl = ref("")
        let loadingDisplay = ref(false)
        let discountData = ref({})
        let hashrecommender = ref("")
        let addqty = (qty)=>{
            inputqty.value = changeqty.addqty(qty)
        }
        let reduceqty = (qty)=>{
            inputqty.value = changeqty.reduceqty(qty)
        }
        let joinGroupBuy = async ()=>{
            console.log(inputqty.value)
            console.log(import.meta.env.VITE_API_URL)
            console.log(data.value)
            await axios.post(import.meta.env.VITE_API_URL+"api/Order/add",{
                "user_name": localStorage.getItem("username"),
                "product_qty": inputqty.value,
                "price": data.value.price,
                "product": data.value.product,
                "picture": data.value.picture,
                "salepageid": data.value.salepageid,
                "shopid": data.value.shopid,
                "skuid": data.value.skuid,
            })
            .then(res=>{
                console.log(res)
            })
            .catch(err=>{
                console.log(err)
            })

            axios.get(import.meta.env.VITE_API_URL+"api/Order/Detail?salepageid=" + localStorage.getItem("salepageid"))
            .then(res=>{
                console.log(res)
                data.value = res.data
                memberData.value = res.data.memberData
                totalqty.value = 0;
                memberData.value.forEach(element => {
                    totalqty.value = totalqty.value + element.qty
                });
                console.log(typeof res.data.skuid)
                console.log(memberData.value)
                imgurl.value = "https:" + res.data.picture
                discountData.value = res.data.discountData
                if(discountData.value.totalPayment!=0&&discountData.value.totalPrice){
                    data.value.price = data.value.price*(discountData.value.totalPayment/discountData.value.totalPrice)
                }
            })
            .catch(err=>{
                console.log(err)
            })
            .finally(()=>{
                window.scrollTo(0, document.body.scrollHeight);
            })
        }
        let shareWeb = ()=>{
            console.log(hashrecommender.value)
            shareURL.shareURL(hashrecommender.value)
        }
        let insertCart = ()=>{
            window.scrollTo(0, 0);
            loadingDisplay.value = true
            axios.post(import.meta.env.VITE_API_URL+"Carts/Checkout",{
                "shopId": data.value.shopid,
                "salePageId": data.value.salepageid,
                "saleProductSKUid": data.value.skuid,
                "qty": totalqty.value
            }).then(res=>{
                console.log(res)
                localStorage.setItem("orderNumber", res.data.orderNumber)
                location.href = res.data.payPageUrl;
            }).catch(err=>{
                console.log(err)
            })
            .finally(()=>{
                loadingDisplay.value = false
                // router.push({path:"/result"})
            })
        }
        let gotoProfile = ()=>{
            router.push({path:"/profile"})
        }
        let gotoDashboard = ()=>{
            window.location.href = import.meta.env.VITE_API_URL+"dashboard"
        }
        onMounted(() => {
            let hubConnection = new signalR.HubConnectionBuilder()
            .withUrl("/GoShopHub/")
            .build();
            // 註冊 MessageHub 的 sendToAllConnections 事件
            
            hubConnection.start()
            .then(() => {
                console.log("Connection started");
                hubConnection.on("SendOrderData", (orderData) => {
                    console.log("Received order data:", orderData);
                    data.value = orderData
                    console.log(data.value)
                    memberData.value = orderData.memberData
                    totalqty.value = 0;
                    memberData.value.forEach(element => {
                        totalqty.value = totalqty.value + element.qty
                    });
                    discountData.value = orderData.discountData
                    if(discountData.value.totalPayment!=0&&discountData.value.totalPrice){
                        data.value.price = data.value.price*(discountData.value.totalPayment/discountData.value.totalPrice)
                    }
                });
            });

            loadingDisplay.value = true
            console.log(localStorage.getItem("salepageid"));
            axios.get(import.meta.env.VITE_API_URL+"api/Order/Detail?salepageid=" + localStorage.getItem("salepageid"))
            .then(res=>{
                console.log(res)
                data.value = res.data
                memberData.value = res.data.memberData
                totalqty.value = 0;
                memberData.value.forEach(element => {
                    totalqty.value = totalqty.value + element.qty
                });
                console.log(typeof res.data.skuid)
                console.log(memberData.value)
                imgurl.value = "https:" + res.data.picture
                discountData.value = res.data.discountData
                if(discountData.value.totalPayment!=0&&discountData.value.totalPrice){
                    data.value.price = data.value.price*(discountData.value.totalPayment/discountData.value.totalPrice)
                }
                res.data.memberData.forEach(element=>{
                    console.log(element.member)
                    if(element.member === localStorage.getItem("username")){
                        console.log(element.sharelink)
                        hashrecommender.value = element.sharelink;
                        return "";
                    }
                })
            })
            .catch(err=>{
                console.log(err)
            })
            .finally(()=>{
                loadingDisplay.value = false
            })
        });
        return {
            data, 
            memberData,
            totalqty,
            inputqty,
            imgurl,
            loadingDisplay,
            discountData,
            addqty,
            reduceqty,
            joinGroupBuy,
            shareWeb,
            insertCart,
            gotoProfile,
            gotoDashboard
        }
    },
    components: { 
        Loading,
        discountTitle 
    }
}
</script>

<template>
    <div class="layout-header-fix">
        <div class="layout-header-fix-logo">
            <img style="height: 100px;" src="../../img/169545314870657.png" alt="">
            <img class="header-logo" src="../../img/Group2.svg" alt="">
        </div>
        <div v-on:click="gotoProfile">
            <svg v-on:click="gotoDashboard" style="    width: 20px;height: 20px;position: absolute;right: 200px;margin-top: 45px;fill: #ffffff;" version="1.1" id="Capa_1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" 
                viewBox="0 0 487.3 487.3" xml:space="preserve">
                <g>
                    <g>
                        <path d="M487.2,69.7c0,12.9-10.5,23.4-23.4,23.4h-322c-12.9,0-23.4-10.5-23.4-23.4s10.5-23.4,23.4-23.4h322.1
                            C476.8,46.4,487.2,56.8,487.2,69.7z M463.9,162.3H141.8c-12.9,0-23.4,10.5-23.4,23.4s10.5,23.4,23.4,23.4h322.1
                            c12.9,0,23.4-10.5,23.4-23.4C487.2,172.8,476.8,162.3,463.9,162.3z M463.9,278.3H141.8c-12.9,0-23.4,10.5-23.4,23.4
                            s10.5,23.4,23.4,23.4h322.1c12.9,0,23.4-10.5,23.4-23.4C487.2,288.8,476.8,278.3,463.9,278.3z M463.9,394.3H141.8
                            c-12.9,0-23.4,10.5-23.4,23.4s10.5,23.4,23.4,23.4h322.1c12.9,0,23.4-10.5,23.4-23.4C487.2,404.8,476.8,394.3,463.9,394.3z
                            M38.9,30.8C17.4,30.8,0,48.2,0,69.7s17.4,39,38.9,39s38.9-17.5,38.9-39S60.4,30.8,38.9,30.8z M38.9,146.8
                            C17.4,146.8,0,164.2,0,185.7s17.4,38.9,38.9,38.9s38.9-17.4,38.9-38.9S60.4,146.8,38.9,146.8z M38.9,262.8
                            C17.4,262.8,0,280.2,0,301.7s17.4,38.9,38.9,38.9s38.9-17.4,38.9-38.9S60.4,262.8,38.9,262.8z M38.9,378.7
                            C17.4,378.7,0,396.1,0,417.6s17.4,38.9,38.9,38.9s38.9-17.4,38.9-38.9C77.8,396.2,60.4,378.7,38.9,378.7z"/>
                    </g>
                </g>
            </svg>
            <svg style="width: 30px;margin-top: 40px;position: absolute;right: 150px; border-radius: 50%; border-style: solid; border-color: #fff;border-width: 2px;" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" x="0px" y="0px" viewBox="0 0 256 256" enable-background="new 0 0 256 256" xml:space="preserve">
            <g><g><g><path fill="#ffffff" d="M120.8,1c-7.2,0.8-17.6,3.8-24.1,7.1C77,18,63.6,35.5,58.6,57.6c-1.7,7.5-1.7,20.5,0,28.1c4.5,20.1,16.5,36.5,34.5,47c2.4,1.4,3.7,2.4,3.3,2.6c-0.4,0.2-3.4,1.2-6.7,2.4c-27.1,9.2-51.6,29.5-65.2,54c-7.8,14.1-12.6,29.6-14.1,45.6c-0.8,8.2-0.5,12.5,0.9,14.3c3.6,4.9,9.9,4.9,13.4,0c0.9-1.2,1.1-2.6,1.5-8.5c1-16.5,4.6-30,11.7-43.5c15.6-29.6,45-49.7,78.2-53.6c48.4-5.6,93.5,23,108.8,69.1c3.4,10.1,4.6,17.1,5.5,32.3c0.5,8.1,9.8,10.8,14.6,4.2c1-1.3,1.1-2.1,1-8.3c-0.3-27.1-12-54.7-32.1-75.5c-13.1-13.5-30.1-24.4-47.3-30.2c-3.3-1.1-6.3-2.2-6.7-2.3c-0.5-0.2,0.9-1.2,3.3-2.6c18.2-10.6,30.7-28,34.7-48.3c1.3-6.6,1.2-19.9-0.3-26.6c-6-28.1-26.4-48.8-54.4-55.3C138,1.2,126.1,0.5,120.8,1z M139.4,18c21.1,4.5,36.8,20,42.1,41.4c1.3,5.2,1.6,17.4,0.5,22.5c-2.5,11.3-7.1,20.2-14.7,28.1c-21.5,22.3-56.9,22.4-78.3,0.1c-7.7-8.1-12.4-16.9-14.8-28.2c-0.9-4.1-0.8-16.2,0-20.6c2.2-10.8,7.4-20.8,15.1-28.5c7.8-7.8,18.4-13.3,28.7-15c2.1-0.3,4.1-0.7,4.5-0.8C124.4,16.6,136.2,17.3,139.4,18z"/></g></g></g>
            </svg>
            <svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" version="1.1" style="width: 22px;height: 25px;position: absolute;right: 238px;top: 42px;" viewBox="0 0 256 256" xml:space="preserve">
                <defs>
                </defs>
                <g style="stroke: none; stroke-width: 0; stroke-dasharray: none; stroke-linecap: butt; stroke-linejoin: miter; stroke-miterlimit: 10; fill: none; fill-rule: nonzero; opacity: 1;" transform="translate(1.4065934065934016 1.4065934065934016) scale(2.81 2.81)" >
                    <path d="M 67.245 21.439 c -1.104 0 -2 -0.896 -2 -2 V 2.136 c 0 -1.104 0.896 -2 2 -2 s 2 0.896 2 2 v 17.303 C 69.245 20.543 68.35 21.439 67.245 21.439 z" style="stroke: none; stroke-width: 1; stroke-dasharray: none; stroke-linecap: butt; stroke-linejoin: miter; stroke-miterlimit: 10; fill: #ffffff; fill-rule: nonzero; opacity: 1;" transform=" matrix(1 0 0 1 0 0) " stroke-linecap="round" />
                    <path d="M 2 71.518 c -1.104 0 -2 -0.896 -2 -2 V 2.136 c 0 -1.104 0.896 -2 2 -2 s 2 0.896 2 2 v 67.382 C 4 70.622 3.104 71.518 2 71.518 z" style="stroke: none; stroke-width: 1; stroke-dasharray: none; stroke-linecap: butt; stroke-linejoin: miter; stroke-miterlimit: 10; fill: #ffffff; fill-rule: nonzero; opacity: 1;" transform=" matrix(1 0 0 1 0 0) " stroke-linecap="round" />
                    <path d="M 77.622 89.864 c -1.104 0 -2 -0.896 -2 -2 s 0.896 -2 2 -2 c 3.931 0 7.237 -2.721 8.137 -6.377 H 67.245 c -1.104 0 -2 -0.896 -2 -2 s 0.896 -2 2 -2 H 88 c 1.104 0 2 0.896 2 2 C 90 84.312 84.447 89.864 77.622 89.864 z" style="stroke: none; stroke-width: 1; stroke-dasharray: none; stroke-linecap: butt; stroke-linejoin: miter; stroke-miterlimit: 10; fill: #ffffff; fill-rule: nonzero; opacity: 1;" transform=" matrix(1 0 0 1 0 0) " stroke-linecap="round" />
                    <path d="M 67.245 79.487 c -1.104 0 -2 -0.896 -2 -2 V 53.169 c 0 -1.104 0.896 -2 2 -2 s 2 0.896 2 2 v 24.318 C 69.245 78.592 68.35 79.487 67.245 79.487 z" style="stroke: none; stroke-width: 1; stroke-dasharray: none; stroke-linecap: butt; stroke-linejoin: miter; stroke-miterlimit: 10; fill: #ffffff; fill-rule: nonzero; opacity: 1;" transform=" matrix(1 0 0 1 0 0) " stroke-linecap="round" />
                    <path d="M 12.377 89.864 C 5.553 89.864 0 84.312 0 77.487 v -7.97 c 0 -1.104 0.896 -2 2 -2 s 2 0.896 2 2 v 7.97 c 0 4.619 3.758 8.377 8.377 8.377 c 1.104 0 2 0.896 2 2 S 13.482 89.864 12.377 89.864 z" style="stroke: none; stroke-width: 1; stroke-dasharray: none; stroke-linecap: butt; stroke-linejoin: miter; stroke-miterlimit: 10; fill: #ffffff; fill-rule: nonzero; opacity: 1;" transform=" matrix(1 0 0 1 0 0) " stroke-linecap="round" />
                    <path d="M 77.622 89.864 H 12.377 c -1.104 0 -2 -0.896 -2 -2 s 0.896 -2 2 -2 h 65.245 c 1.104 0 2 0.896 2 2 S 78.727 89.864 77.622 89.864 z" style="stroke: none; stroke-width: 1; stroke-dasharray: none; stroke-linecap: butt; stroke-linejoin: miter; stroke-miterlimit: 10; fill: #ffffff; fill-rule: nonzero; opacity: 1;" transform=" matrix(1 0 0 1 0 0) " stroke-linecap="round" />
                    <path d="M 12.377 89.864 c -1.104 0 -2 -0.896 -2 -2 s 0.896 -2 2 -2 c 4.619 0 8.377 -3.758 8.377 -8.377 c 0 -1.104 0.896 -2 2 -2 h 44.49 c 1.104 0 2 0.896 2 2 s -0.896 2 -2 2 H 24.593 C 23.635 85.364 18.521 89.864 12.377 89.864 z" style="stroke: none; stroke-width: 1; stroke-dasharray: none; stroke-linecap: butt; stroke-linejoin: miter; stroke-miterlimit: 10; fill: #ffffff; fill-rule: nonzero; opacity: 1;" transform=" matrix(1 0 0 1 0 0) " stroke-linecap="round" />
                    <path d="M 41.739 52.07 H 15.876 c -1.104 0 -2 -0.896 -2 -2 s 0.896 -2 2 -2 h 25.863 c 1.104 0 2 0.896 2 2 S 42.844 52.07 41.739 52.07 z" style="stroke: none; stroke-width: 1; stroke-dasharray: none; stroke-linecap: butt; stroke-linejoin: miter; stroke-miterlimit: 10; fill: #ffffff; fill-rule: nonzero; opacity: 1;" transform=" matrix(1 0 0 1 0 0) " stroke-linecap="round" />
                    <path d="M 71.038 40.392 c -1.073 0 -1.949 -0.845 -1.998 -1.906 c -1.061 -0.049 -1.906 -0.925 -1.906 -1.998 c 0 -1.104 0.896 -2 2 -2 c 2.153 0 3.904 1.751 3.904 3.904 C 73.038 39.496 72.143 40.392 71.038 40.392 z M 69.134 38.487 h 0.01 H 69.134 z" style="stroke: none; stroke-width: 1; stroke-dasharray: none; stroke-linecap: butt; stroke-linejoin: miter; stroke-miterlimit: 10; fill: #ffffff; fill-rule: nonzero; opacity: 1;" transform=" matrix(1 0 0 1 0 0) " stroke-linecap="round" />
                    <path d="M 67.245 31.748 c -1.104 0 -2 -0.896 -2 -2 V 27.39 c 0 -1.104 0.896 -2 2 -2 s 2 0.896 2 2 v 2.358 C 69.245 30.852 68.35 31.748 67.245 31.748 z" style="stroke: none; stroke-width: 1; stroke-dasharray: none; stroke-linecap: butt; stroke-linejoin: miter; stroke-miterlimit: 10; fill: #ffffff; fill-rule: nonzero; opacity: 1;" transform=" matrix(1 0 0 1 0 0) " stroke-linecap="round" />
                    <path d="M 69.134 38.487 h -3.778 c -2.153 0 -3.904 -1.751 -3.904 -3.904 v -2.931 c 0 -2.153 1.751 -3.904 3.904 -3.904 h 3.778 c 2.153 0 3.904 1.751 3.904 3.904 c 0 1.104 -0.896 2 -2 2 c -1.072 0 -1.948 -0.844 -1.998 -1.904 h -3.589 v 2.74 h 3.683 c 1.104 0 2 0.896 2 2 S 70.238 38.487 69.134 38.487 z" style="stroke: none; stroke-width: 1; stroke-dasharray: none; stroke-linecap: butt; stroke-linejoin: miter; stroke-miterlimit: 10; fill: #ffffff; fill-rule: nonzero; opacity: 1;" transform=" matrix(1 0 0 1 0 0) " stroke-linecap="round" />
                    <path d="M 67.245 47.586 c -1.104 0 -2 -0.896 -2 -2 v -2.358 c 0 -1.104 0.896 -2 2 -2 s 2 0.896 2 2 v 2.358 C 69.245 46.69 68.35 47.586 67.245 47.586 z" style="stroke: none; stroke-width: 1; stroke-dasharray: none; stroke-linecap: butt; stroke-linejoin: miter; stroke-miterlimit: 10; fill: #ffffff; fill-rule: nonzero; opacity: 1;" transform=" matrix(1 0 0 1 0 0) " stroke-linecap="round" />
                    <path d="M 69.134 45.228 h -3.778 c -2.153 0 -3.904 -1.751 -3.904 -3.904 c 0 -1.104 0.896 -2 2 -2 c 1.072 0 1.948 0.844 1.998 1.904 h 3.589 v -2.836 c 0 -1.104 0.896 -2 2 -2 s 2 0.896 2 2 v 2.932 C 73.038 43.476 71.287 45.228 69.134 45.228 z M 65.451 41.323 h 0.01 H 65.451 z" style="stroke: none; stroke-width: 1; stroke-dasharray: none; stroke-linecap: butt; stroke-linejoin: miter; stroke-miterlimit: 10; fill: #ffffff; fill-rule: nonzero; opacity: 1;" transform=" matrix(1 0 0 1 0 0) " stroke-linecap="round" />
                    <path d="M 54.271 64.202 H 15.876 c -1.104 0 -2 -0.896 -2 -2 s 0.896 -2 2 -2 h 38.395 c 1.104 0 2 0.896 2 2 S 55.375 64.202 54.271 64.202 z" style="stroke: none; stroke-width: 1; stroke-dasharray: none; stroke-linecap: butt; stroke-linejoin: miter; stroke-miterlimit: 10; fill: #ffffff; fill-rule: nonzero; opacity: 1;" transform=" matrix(1 0 0 1 0 0) " stroke-linecap="round" />
                    <path d="M 59.09 11.045 c -0.46 0 -0.92 -0.158 -1.293 -0.474 l -6.863 -5.814 l -6.862 5.814 c -0.746 0.632 -1.84 0.632 -2.586 0 l -6.863 -5.814 l -6.863 5.814 c -0.746 0.632 -1.84 0.632 -2.586 0 l -6.863 -5.814 l -6.862 5.814 c -0.746 0.632 -1.84 0.632 -2.586 0 L 0.707 3.662 C -0.136 2.948 -0.24 1.686 0.474 0.843 C 1.188 -0.001 2.45 -0.104 3.293 0.61 l 6.863 5.814 l 6.862 -5.814 c 0.746 -0.632 1.84 -0.632 2.586 0 l 6.863 5.814 L 33.33 0.61 c 0.746 -0.632 1.84 -0.632 2.586 0 l 6.863 5.814 l 6.862 -5.814 c 0.746 -0.632 1.84 -0.632 2.586 0 l 6.863 5.814 l 6.862 -5.814 c 0.843 -0.713 2.104 -0.61 2.819 0.233 c 0.714 0.843 0.609 2.105 -0.233 2.819 l -8.155 6.909 C 60.01 10.887 59.55 11.045 59.09 11.045 z" style="stroke: none; stroke-width: 1; stroke-dasharray: none; stroke-linecap: butt; stroke-linejoin: miter; stroke-miterlimit: 10; fill: #ffffff; fill-rule: nonzero; opacity: 1;" transform=" matrix(1 0 0 1 0 0) " stroke-linecap="round" />
                    <path d="M 67.245 55.169 c -10.402 0 -18.865 -8.463 -18.865 -18.865 s 8.463 -18.865 18.865 -18.865 c 10.401 0 18.864 8.463 18.864 18.865 S 77.646 55.169 67.245 55.169 z M 67.245 21.439 c -8.196 0 -14.865 6.668 -14.865 14.865 s 6.669 14.865 14.865 14.865 S 82.109 44.5 82.109 36.304 C 82.109 28.107 75.441 21.439 67.245 21.439 z" style="stroke: none; stroke-width: 1; stroke-dasharray: none; stroke-linecap: butt; stroke-linejoin: miter; stroke-miterlimit: 10; fill: #ffffff; fill-rule: nonzero; opacity: 1;" transform=" matrix(1 0 0 1 0 0) " stroke-linecap="round" />
                    <path d="M 34.69 39.939 H 15.876 c -1.104 0 -2 -0.896 -2 -2 s 0.896 -2 2 -2 H 34.69 c 1.104 0 2 0.896 2 2 S 35.794 39.939 34.69 39.939 z" style="stroke: none; stroke-width: 1; stroke-dasharray: none; stroke-linecap: butt; stroke-linejoin: miter; stroke-miterlimit: 10; fill: #ffffff; fill-rule: nonzero; opacity: 1;" transform=" matrix(1 0 0 1 0 0) " stroke-linecap="round" />
                    <path d="M 41.739 27.808 H 15.876 c -1.104 0 -2 -0.896 -2 -2 s 0.896 -2 2 -2 h 25.863 c 1.104 0 2 0.896 2 2 S 42.844 27.808 41.739 27.808 z" style="stroke: none; stroke-width: 1; stroke-dasharray: none; stroke-linecap: butt; stroke-linejoin: miter; stroke-miterlimit: 10; fill: #ffffff; fill-rule: nonzero; opacity: 1;" transform=" matrix(1 0 0 1 0 0) " stroke-linecap="round" />
                </g>
            </svg>
        </div>
    </div>
    <div style="background-color: #fff; width: calc(100vw - 17px);height: fit-content;padding-left: 20px;">
        <discountTitle></discountTitle>
    </div>
    <div class="hostGroupBuy-content">
        <div class="hostGroupBuy-content-content">
            <div class="hostGroupBuy-content-left">
                <div class="hostGroupBuy-content-left-img">
                    <div class="hostGroupBuy-content-left-img-div">
                        <img class="hostGroupBuy-content-left-img-img" v-bind:src="imgurl" alt="">
                    </div>
                    <div class="hostGroupBuy-content-left-img-small">
                    </div>
                </div>
            </div>
            <div class="hostGroupBuy-content-right">
                <div class="hostGroupBuy-content-right-title">{{ data.product }}</div>
                <div class="hostGroupBuy-content-right-price">${{ data.price }}</div>
                <div class="hostGroupBuy-content-right-color">
                    <!-- <div class="hostGroupBuy-content-right-color-title">顏色</div> -->
                    <div class="hostGroupBuy-content-right-color-content">
                        <!-- <div class="hostGroupBuy-content-right-color-content-color">
                            <div class="hostGroupBuy-content-right-color-content-color-color">白色</div>
                        </div>
                        <div class="hostGroupBuy-content-right-color-content-color">
                            <div class="hostGroupBuy-content-right-color-content-color-color">藍色</div>
                        </div>
                        <div class="hostGroupBuy-content-right-color-content-color">
                            <div class="hostGroupBuy-content-right-color-content-color-color">灰色</div>
                        </div> -->
                    </div>
                </div>
                <div class="hostGroupBuy-content-right-size">
                    <!-- <div class="hostGroupBuy-content-right-color-title">尺寸</div> -->
                    <div class="hostGroupBuy-content-right-color-content">
                        <!-- <div class="hostGroupBuy-content-right-color-content-color">
                            <div class="hostGroupBuy-content-right-color-content-color-color">S</div>
                        </div>
                        <div class="hostGroupBuy-content-right-color-content-color">
                            <div class="hostGroupBuy-content-right-color-content-color-color">M</div>
                        </div>
                        <div class="hostGroupBuy-content-right-color-content-color">
                            <div class="hostGroupBuy-content-right-color-content-color-color">L</div>
                        </div> -->
                    </div>
                </div>
                <div class="hostGroupBuy-content-right-qty">
                    <div class="hostGroupBuy-content-right-color-title">數量</div>
                    <div class="hostGroupBuy-content-right-qty-content">
                        <button class="hostGroupBuy-content-right-color-content-minus" v-on:click="reduceqty(inputqty)">
                            <svg style="width: 10px;" enable-background="new 0 0 10 10" viewBox="0 0 10 10" x="0" y="0" class="shopee-svg-icon"><polygon points="4.5 4.5 3.5 4.5 0 4.5 0 5.5 3.5 5.5 4.5 5.5 10 5.5 10 4.5"></polygon></svg>
                        </button>
                        <input class="hostGroupBuy-content-right-color-content-input" type="number" v-model="inputqty">
                        <button class="hostGroupBuy-content-right-color-content-plus" v-on:click="addqty(inputqty)">
                            <svg style="width: 10px;" enable-background="new 0 0 10 10" viewBox="0 0 10 10" x="0" y="0" class="shopee-svg-icon icon-plus-sign"><polygon points="10 4.5 5.5 4.5 5.5 0 4.5 0 4.5 4.5 0 4.5 0 5.5 4.5 5.5 4.5 10 5.5 10 5.5 5.5 10 5.5"></polygon></svg>
                        </button>
                    </div>
                </div>
                <div class="hostGroupBuy-content-right-color-content-groupbuy">
                    <button class="hostGroupBuy-content-right-color-content-groupbuy-button" v-on:click="joinGroupBuy">
                        加入團購
                    </button>
                    <button class="hostGroupBuy-content-right-color-content-share-button" v-on:click="shareWeb">
                        分享連結
                    </button>
                </div>
                <div>已選購${{ discountData.totalPrice }}</div>
                <div>目前折扣{{ discountData.totalDiscount }}</div>
                <div style="font-size: 16px;color: #70c1d6;">優惠價{{ discountData.totalPayment }}</div>
            </div>
        </div>
    </div>
    <div class="hostGroup-list">
        <div class="hostGroup-list-table">
            <table class="hostGroup-list-table-table">
                <thead class="hostGroup-list-table-table-thead">
                    <tr>
                        <th>名稱</th>
                        <th>數量</th>
                        <th>金額</th>
                    </tr>
                </thead>
                <tbody class="hostGroup-list-table-table-tbody">
                    <tr v-for="memberDataList in memberData">
                        <td>{{ memberDataList.member }}</td>
                        <td>{{ memberDataList.qty }}</td>
                        <td>{{ memberDataList.qty * data.price }}</td>
                    </tr>
                </tbody>
            </table>
            <div class="hostGroup-list-total">
                <div class="hostGroup-list-total-total">
                    <div class="hostGroup-list-total-content" style="display: flex;">
                        <div style="margin-right: 10px;font-size: 16px;align-items: end;display: flex;">已選購${{ discountData.totalPrice }}</div>
                        <div style="margin-right: 10px;font-size: 16px;align-items: end;display: flex;">目前折扣{{ discountData.totalDiscount }}</div>
                        <div style="margin-right: 10px;color: #70c1d6;align-items: end;display: flex;">優惠價${{ discountData.totalPayment }}</div>
                    </div>
                    <div class="hostGroup-list-total-button">
                        <button class="hostGroup-list-total-button-button" v-on:click="insertCart">提交購物車</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="loading" v-show="loadingDisplay">
        <Loading></Loading>
    </div>
</template>

<style scoped>
.layout-header-fix{
  background-color: #e72410;
  height: 100px;
  display: flex;
  position: relative;
}

.layout-header-fix-logo{
    position: relative;
    left: 150px
}

.header-logo{
  width: 50px;
  position: absolute;
  top: 35px;
  left: 250px;
}

.hostGroupBuy-content{
    height: calc(100vh - 100px);
    display: flex;
    background-color: #ececec;
}

.hostGroupBuy-content-content{
    width: 1220px;
    height: 600px;
    background-color: #ffffff;
    margin-left: auto;
    margin-right: auto;
    margin-top: 20px;
    border-radius: 12px;
    display: flex;
}

.hostGroupBuy-content-left{
    padding: 15px;
}

.hostGroupBuy-content-left-img-img{
    width: 450px;
    height: 450px;
}

.hostGroupBuy-content-right{
    padding: 20px;
    padding-right: 35px;
    width: 740px;
    height: 100%;
    font-family: system-ui;
}

.hostGroupBuy-content-right-title{
    font-size: 20px;
}

.hostGroupBuy-content-right-price{
    font-size: 30px;
    color: #e72410;
    margin-top: 30px;
    margin-bottom: 30px;
}

.hostGroupBuy-content-right-color{
    display: flex;
    margin-bottom: 10px;
}

.hostGroupBuy-content-right-color-title{
    font-size: 16px;
    color: #75757575;
    margin-bottom: 10px;
    width: 110px;
    padding-top: 5px;
    padding-bottom: 5px;
}

.hostGroupBuy-content-right-color-content{
    display: flex;

}

.hostGroupBuy-content-right-color-content-color{
    text-align: center;
    font-size: 16px;
    margin-right: 8px;
}

.hostGroupBuy-content-right-color-content-color-color{
    border: 1px solid #75757575;
    padding: 5px 12px;
    color: #000000;
    width: 80px;
}

.hostGroupBuy-content-right-size{
    font-size: 16px;
    color: #75757575;
    margin-bottom: 10px;
    display: flex;
}

.hostGroupBuy-content-right-qty{
    font-size: 16px;
    color: #75757575;
    margin-bottom: 10px;
    display: flex;
}

.hostGroupBuy-content-right-qty-content{
    border: 1px solid #75757575;
    border-bottom: 1px solid #75757575;
}

.hostGroupBuy-content-right-color-content-minus{
    height: 40px;
    width: 40px;
    font-size: 14px;
    border-right: 1px solid #75757575;
    
}

.hostGroupBuy-content-right-color-content-input{
    height: 97%;
    font-size: 16px;
    width: 70px;
    text-align: center;
}

/* 對所有的 number input 元素 */
input[type="number"]::-webkit-inner-spin-button,
input[type="number"]::-webkit-outer-spin-button {
  -webkit-appearance: none;
  margin: 0;
}

.hostGroupBuy-content-right-color-content-plus{
    height: 40px;
    width: 40px;
    font-size: 14px;
    border-left: 1px solid #75757575;
}

.hostGroupBuy-content-right-color-content-groupbuy{
    margin-top: 20px;
}

.hostGroupBuy-content-right-color-content-groupbuy-button{
    border: #e72410 solid 1px;
    background-color: #e72510db;
    color: #ffffff;
    width: 120px;
    font-size: 16px;
    padding: 8px 12px;
}

.hostGroupBuy-content-right-color-content-share-button{
    border: #1049e7 solid 1px;
    background-color: #1049e7db;
    color: #ffffff;
    width: 120px;
    font-size: 16px;
    padding: 8px 12px;
    margin-left: 20px;
}

.hostGroup-list{
    height: 100vh;
    background-color: #ececec;
    display: flex;
    flex-direction: column;
}

.hostGroup-list-table{
    width: 1220px;
    height: 600px;
    background-color: #ffffff;
    margin-left: auto;
    margin-right: auto;
    margin-top: 20px;
    border-radius: 12px;
    display: flex;
    flex-direction: column;
    position: relative;
}

.hostGroup-list-table-table{
}

table {
  border-collapse: collapse;
}

.hostGroup-list-table-table-thead > tr > th{
    border-bottom: #75757575 solid 2px;
    height: 40px;
}

.hostGroup-list-table-table-tbody > tr > td{
    height: 40px ;
    text-align: center;
}

.hostGroup-list-table-page{

}

.hostGroup-list-total{
    width: 1220px;
    height: 80px;
    margin-left: auto;
    margin-right: auto;
    margin-top: 20px;
    border-radius: 12px;
    display: flex;
    justify-content: end;
    position: absolute;
    bottom: 0;
}

.hostGroup-list-total-total{
    display: flex;
    justify-content: end;
    margin-top: auto;
    margin-bottom: auto;
    margin-right: 20px;
    background-color: #ffffff;
    padding: 20px;
    border-radius: 5px;
    margin-left: auto;
    margin-right: auto;
}

.hostGroup-list-total-content{
    font-size: 24px;
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    margin-right: 20px;
    display: flex;
}

.hostGroup-list-total-button{

}

.hostGroup-list-total-button-button{
    background-color: #e72410;
    color: #ffffff;
    padding: 8px 12px;
    font-size: 16px;
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    font-weight: 900;
}

.loading{
    height: calc(200vh - 100px);
    width: 100vw;
    top: 100px;
    position: absolute;
    background-color: #ffffff;
}
</style>