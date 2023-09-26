<script>
import { onMounted, ref } from 'vue';
import axios from 'axios'
import Loading from "../components/Loading.vue"

export default{
    setup() {
        let data = ref({})
        let username = ref(localStorage.getItem("username"))
        let recommendData = ref([])
        onMounted(() => {
            axios.get(import.meta.env.VITE_API_URL + `api/Profile/Recommendation?username=${localStorage.getItem("username")}`)
            .then(res=>{
                console.log(res)
                data.value = res.data;
                recommendData.value = res.data.recommendData;
            })
            .catch(err=>{
                console.log(err)
            })
        });
        return {
            data,
            username,
            recommendData
        }
    },
    components: { Loading }
}
</script>

<template>
    <div class="layout-header-fix">
        <div class="layout-header-fix-logo">
            <img style="height: 100px;" src="../img/169545314870657.png" alt="">
            <img class="header-logo" src="../img/Group2.svg" alt="">
        </div>
    </div>
    <div class="profile-content">
        <div class="profile-content-content">
            <div class="profile-name">
                <div>
                    <h1>{{ username }}</h1>
                </div>
                <div class="profile-point">
                    <h5 class="profile-point-title">積分</h5>
                    <h2 class="profile-point-number">50</h2>
                </div>
            </div>
            <div class="profile-recommender">
                <table class="hostGroup-list-table-table">
                    <thead class="hostGroup-list-table-table-thead">
                        <tr>
                            <th style="width: 100vh;">已推薦用戶</th>
                        </tr>
                    </thead>
                    <tbody class="hostGroup-list-table-table-tbody">
                        <tr v-for="recommender in recommendData">
                            <td>{{ recommender.name }}</td>
                        </tr>
                    </tbody>
                </table>
            </div>
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

.profile-content{
    height: calc(100vh - 100px);
}

.profile-content-content{
    width: 100%;
    padding-left: 150px;
    padding-right: 150px;
}

.profile-name{
    display: flex;
    margin: 20px 0px;
}

.profile-point{
    display: flex;
}

.profile-point-title{
    display: flex;
    align-items: flex-end;
    margin-left: 20px;
    margin-right: 5px;
}

.profile-point-number{
    display: flex;
    align-items: flex-end;
}

.profile-recommender{

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