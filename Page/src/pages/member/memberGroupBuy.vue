<script>
import { onMounted } from 'vue';
import Loading from "../../components/Loading.vue"


export default{
    setup() {
        onMounted(() => {
            console.log("HI");
        });
    },
    components: { Loading }
}
</script>

<template>
    <div class="layout-header-fix">
        <div class="layout-header-fix-logo">
            <img style="height: 100px;" src="../../img/169545314870657.png" alt="">
            <img class="header-logo" src="../../img/Group2.svg" alt="">
        </div>
    </div>
    <div class="hostGroupBuy-content">
        <div class="hostGroupBuy-content-content">
            <div class="hostGroupBuy-content-left">
                <div class="hostGroupBuy-content-left-img">
                    <div class="hostGroupBuy-content-left-img-div">
                        <img class="hostGroupBuy-content-left-img-img" src="" alt="">
                    </div>
                    <div class="hostGroupBuy-content-left-img-small">
                    </div>
                </div>
            </div>
            <div class="hostGroupBuy-content-right">
                <div class="hostGroupBuy-content-right-title">【台灣現貨】超有質感Allen 抓一隻來一打 大Ray小Ray落玉盤</div>
                <div class="hostGroupBuy-content-right-price">$300</div>
                <div class="hostGroupBuy-content-right-color">
                    <div class="hostGroupBuy-content-right-color-title">顏色</div>
                    <div class="hostGroupBuy-content-right-color-content">
                        <div class="hostGroupBuy-content-right-color-content-color">
                            <div class="hostGroupBuy-content-right-color-content-color-color">白色</div>
                        </div>
                        <div class="hostGroupBuy-content-right-color-content-color">
                            <div class="hostGroupBuy-content-right-color-content-color-color">藍色</div>
                        </div>
                        <div class="hostGroupBuy-content-right-color-content-color">
                            <div class="hostGroupBuy-content-right-color-content-color-color">灰色</div>
                        </div>
                    </div>
                </div>
                <div class="hostGroupBuy-content-right-size">
                    <div class="hostGroupBuy-content-right-color-title">尺寸</div>
                    <div class="hostGroupBuy-content-right-color-content">
                        <div class="hostGroupBuy-content-right-color-content-color">
                            <div class="hostGroupBuy-content-right-color-content-color-color">S</div>
                        </div>
                        <div class="hostGroupBuy-content-right-color-content-color">
                            <div class="hostGroupBuy-content-right-color-content-color-color">M</div>
                        </div>
                        <div class="hostGroupBuy-content-right-color-content-color">
                            <div class="hostGroupBuy-content-right-color-content-color-color">L</div>
                        </div>
                    </div>
                </div>
                <div class="hostGroupBuy-content-right-qty">
                    <div class="hostGroupBuy-content-right-color-title">數量</div>
                    <div class="hostGroupBuy-content-right-qty-content">
                        <button class="hostGroupBuy-content-right-color-content-minus">
                            <svg style="width: 10px;" enable-background="new 0 0 10 10" viewBox="0 0 10 10" x="0" y="0" class="shopee-svg-icon"><polygon points="4.5 4.5 3.5 4.5 0 4.5 0 5.5 3.5 5.5 4.5 5.5 10 5.5 10 4.5"></polygon></svg>
                        </button>
                        <input class="hostGroupBuy-content-right-color-content-input" type="number">
                        <button class="hostGroupBuy-content-right-color-content-plus">
                            <svg style="width: 10px;" enable-background="new 0 0 10 10" viewBox="0 0 10 10" x="0" y="0" class="shopee-svg-icon icon-plus-sign"><polygon points="10 4.5 5.5 4.5 5.5 0 4.5 0 4.5 4.5 0 4.5 0 5.5 4.5 5.5 4.5 10 5.5 10 5.5 5.5 10 5.5"></polygon></svg>
                        </button>
                    </div>
                </div>
                <div class="hostGroupBuy-content-right-color-content-groupbuy">
                    <button class="hostGroupBuy-content-right-color-content-groupbuy-button">
                        加入團購
                    </button>
                </div>
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
                        <th></th>
                    </tr>
                </thead>
                <tbody class="hostGroup-list-table-table-tbody">
                    <tr>
                        <td>Micheal</td>
                        <td>10</td>
                        <td>3000</td>
                        <td>取消訂單</td>
                    </tr>
                </tbody>
            </table>
        </div>
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
    background-color: #e7251065;
    color: #ffffff;
    width: 120px;
    font-size: 16px;
    padding: 8px 12px;
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
</style>