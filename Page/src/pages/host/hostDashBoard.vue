<script>
import { onMounted, ref, computed } from 'vue';
import Loading from "../../components/Loading.vue"
import axios from 'axios'
import changeqty from '../../function/changeqty';
import router from '../../router/router';
import shareURL from '../../function/shareURL';
import * as signalR from "@aspnet/signalr";
import Plotly from 'plotly.js-dist-min'

export default{
    setup() {
        let data = ref([])
        let startDate = ref('2023-10-01');
        let endDate = ref('2023-10-31');
        let gotoProfile = ()=>{
            router.push({path:"/profile"})
        }
        let filteredData = computed(() => {
            return data.value.filter(item => {
                const itemDate = new Date(item.start);
                const sDate = new Date(startDate.value);
                const eDate = new Date(endDate.value);
                return itemDate >= sDate && itemDate <= eDate;
            });
        });
        let gotoGroupBuy = ()=>{
            router.push({path:"/createGroupBuy/hostGroupBuy"})
        }
        onMounted(() => {
            axios.get(import.meta.env.VITE_API_URL + "api/Dashboard/Grouplist")
            .then(res=>{
                console.log(res)
                data.value = res.data
                let plotlyprice = [];
                let plotlyname = [];
                data.value.forEach(element => {
                    plotlyprice.push(element.totaldiscount)
                    plotlyname.push(element.campaign)
                });
                setTimeout(() => {
                    var data = [{
                        values: plotlyprice,
                        labels: plotlyname,
                        type: 'pie'
                    }];

                    var layout = {
                        height: 400,
                        width: 500
                    };

                    Plotly.newPlot('myDiv', data, layout);
                }, 2000);
            })
            .catch(err=>{
                console.log(err)
            })
        });
        return {
            data,
            filteredData,
            startDate,
            endDate,
            gotoProfile,
            gotoGroupBuy
        }
    },
    components: { 
        Loading,
    }
}
</script>

<template>
    <div class="layout-header-fix" v-on:click="gotoGroupBuy">
        <div class="layout-header-fix-logo">
            <img style="height: 100px;" src="../../img/169545314870657.png" alt="">
            <img class="header-logo" src="../../img/Group2.svg" alt="">
        </div>
        <!-- <div v-on:click="gotoProfile">
            <svg style="width: 30px;margin-top: 40px;position: absolute;right: 150px; border-radius: 50%; border-style: solid; border-color: #fff;border-width: 2px;" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" x="0px" y="0px" viewBox="0 0 256 256" enable-background="new 0 0 256 256" xml:space="preserve">
            <g><g><g><path fill="#ffffff" d="M120.8,1c-7.2,0.8-17.6,3.8-24.1,7.1C77,18,63.6,35.5,58.6,57.6c-1.7,7.5-1.7,20.5,0,28.1c4.5,20.1,16.5,36.5,34.5,47c2.4,1.4,3.7,2.4,3.3,2.6c-0.4,0.2-3.4,1.2-6.7,2.4c-27.1,9.2-51.6,29.5-65.2,54c-7.8,14.1-12.6,29.6-14.1,45.6c-0.8,8.2-0.5,12.5,0.9,14.3c3.6,4.9,9.9,4.9,13.4,0c0.9-1.2,1.1-2.6,1.5-8.5c1-16.5,4.6-30,11.7-43.5c15.6-29.6,45-49.7,78.2-53.6c48.4-5.6,93.5,23,108.8,69.1c3.4,10.1,4.6,17.1,5.5,32.3c0.5,8.1,9.8,10.8,14.6,4.2c1-1.3,1.1-2.1,1-8.3c-0.3-27.1-12-54.7-32.1-75.5c-13.1-13.5-30.1-24.4-47.3-30.2c-3.3-1.1-6.3-2.2-6.7-2.3c-0.5-0.2,0.9-1.2,3.3-2.6c18.2-10.6,30.7-28,34.7-48.3c1.3-6.6,1.2-19.9-0.3-26.6c-6-28.1-26.4-48.8-54.4-55.3C138,1.2,126.1,0.5,120.8,1z M139.4,18c21.1,4.5,36.8,20,42.1,41.4c1.3,5.2,1.6,17.4,0.5,22.5c-2.5,11.3-7.1,20.2-14.7,28.1c-21.5,22.3-56.9,22.4-78.3,0.1c-7.7-8.1-12.4-16.9-14.8-28.2c-0.9-4.1-0.8-16.2,0-20.6c2.2-10.8,7.4-20.8,15.1-28.5c7.8-7.8,18.4-13.3,28.7-15c2.1-0.3,4.1-0.7,4.5-0.8C124.4,16.6,136.2,17.3,139.4,18z"/></g></g></g>
            </svg>
        </div> -->
    </div>
    <div class="search-range-content">
        <div class="search-range">
            <div class="search-range-start-end">
                開始時間:
                <input type="date" placeholder="開始時間" v-model="startDate">
            </div>
            <div class="search-range-start-end">
                結束時間:
                <input type="date" placeholder="結束時間" v-model="endDate">
            </div>
            <!-- <div>
                <button class="search-range-start-end-button">尋找</button>
            </div> -->
        </div>
    </div>
    <div class="hostGroup-list">
        <div class="hostGroup-list-table">
            <table class="hostGroup-list-table-table">
                <thead class="hostGroup-list-table-table-thead">
                    <tr>
                        <th>團購活動名稱</th>
                        <th>折扣門檻</th>
                        <th>預計銷售金額</th>
                        <th>實際銷售金額</th>
                        <th>參與人數</th>
                        <th>開始時間</th>
                        <th>結束時間</th>
                        <th>狀態</th>
                    </tr>
                </thead>
                <tbody class="hostGroup-list-table-table-tbody">
                    <tr v-for="list in filteredData" :key="list.id">
                        <td>{{ list.campaign }}</td>
                        <td>{{ list.totaldiscount }}</td>
                        <td>{{ list.totalprice }}</td>
                        <td>{{ list.afterdiscount }}</td>
                        <td>{{ list.membercount }}</td>
                        <td>{{ list.start }}</td>
                        <td>{{ list.finish }}</td>
                        <td>{{ list.status }}</td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
    <div id='myDiv'><!-- Plotly chart will be drawn inside this DIV --></div>
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

.search-range-content{
    display: flex;
    background-color: #ececec;
}

.search-range{
    display: flex;
    width: 1220px;
    margin: auto;
    margin-top: 20px;
}

.search-range-start-end{
    margin-right: 20px;
    font-size: 18px;
}

.search-range-start-end input{
    font-size: 18px;
}

.search-range-start-end-button{
    padding: 5px 10px;
    background-color: #e72410;
    color: #ffffff;
    font-size: 16px;
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
</style>