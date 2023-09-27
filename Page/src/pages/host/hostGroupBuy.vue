<script>
import { onMounted, ref } from 'vue';
import Loading from "../../components/Loading.vue"
import axios from 'axios'
import changeqty from '../../function/changeqty';
import router from '../../router/router';
import shareURL from '../../function/shareURL';
import discountTitle from '../../components/discountTitle.vue';

export default{
    setup() {
        let data = ref({})
        let memberData = ref()
        let totalqty = ref(0)
        let inputqty = ref(0)
        let imgurl = ref("")
        let loadingDisplay = ref(false)
        let discountData = ref({})
        let addqty = (qty)=>{
            inputqty.value = changeqty.addqty(qty)
        }
        let reduceqty = (qty)=>{
            inputqty.value = changeqty.reduceqty(qty)
        }
        let joinGroupBuy = async ()=>{
            console.log(inputqty.value)
            console.log(import.meta.env.VITE_API_URL)
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
            shareURL.shareURL(localStorage.getItem("username"))
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
            }).catch(err=>{
                console.log(err)
            })
            .finally(()=>{
                loadingDisplay.value = false
                router.push({path:"/result"})
            })
        }
        let gotoProfile = ()=>{
            router.push({path:"/profile"})
        }
        onMounted(() => {
            loadingDisplay.value = true
            console.log(import.meta.env.VITE_API_URL);
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
            gotoProfile
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
            <svg style="width: 30px;margin-top: 40px;position: absolute;right: 150px; border-radius: 50%; border-style: solid; border-color: #fff;border-width: 2px;" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" x="0px" y="0px" viewBox="0 0 256 256" enable-background="new 0 0 256 256" xml:space="preserve">
            <g><g><g><path fill="#ffffff" d="M120.8,1c-7.2,0.8-17.6,3.8-24.1,7.1C77,18,63.6,35.5,58.6,57.6c-1.7,7.5-1.7,20.5,0,28.1c4.5,20.1,16.5,36.5,34.5,47c2.4,1.4,3.7,2.4,3.3,2.6c-0.4,0.2-3.4,1.2-6.7,2.4c-27.1,9.2-51.6,29.5-65.2,54c-7.8,14.1-12.6,29.6-14.1,45.6c-0.8,8.2-0.5,12.5,0.9,14.3c3.6,4.9,9.9,4.9,13.4,0c0.9-1.2,1.1-2.6,1.5-8.5c1-16.5,4.6-30,11.7-43.5c15.6-29.6,45-49.7,78.2-53.6c48.4-5.6,93.5,23,108.8,69.1c3.4,10.1,4.6,17.1,5.5,32.3c0.5,8.1,9.8,10.8,14.6,4.2c1-1.3,1.1-2.1,1-8.3c-0.3-27.1-12-54.7-32.1-75.5c-13.1-13.5-30.1-24.4-47.3-30.2c-3.3-1.1-6.3-2.2-6.7-2.3c-0.5-0.2,0.9-1.2,3.3-2.6c18.2-10.6,30.7-28,34.7-48.3c1.3-6.6,1.2-19.9-0.3-26.6c-6-28.1-26.4-48.8-54.4-55.3C138,1.2,126.1,0.5,120.8,1z M139.4,18c21.1,4.5,36.8,20,42.1,41.4c1.3,5.2,1.6,17.4,0.5,22.5c-2.5,11.3-7.1,20.2-14.7,28.1c-21.5,22.3-56.9,22.4-78.3,0.1c-7.7-8.1-12.4-16.9-14.8-28.2c-0.9-4.1-0.8-16.2,0-20.6c2.2-10.8,7.4-20.8,15.1-28.5c7.8-7.8,18.4-13.3,28.7-15c2.1-0.3,4.1-0.7,4.5-0.8C124.4,16.6,136.2,17.3,139.4,18z"/></g></g></g>
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