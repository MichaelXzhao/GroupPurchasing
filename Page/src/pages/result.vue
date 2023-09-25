<script>
import { onMounted, ref } from 'vue';
import axios from 'axios'
import Loading from "../components/Loading.vue"

export default{
    setup() {
        let loadingDisplay = ref(false)
        let orderNumber = ref("")
        onMounted(() => {
            orderNumber.value = localStorage.getItem("orderNumber")
        });
        return {
            loadingDisplay,
            orderNumber
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
    <div class="hostGroupBuy-content">
        <div style="width: 100vw;">
            <div>
                <h1 style="text-align: center;">訂單完成</h1>
            </div>
            <div style="text-align: center;">
                訂單編號:{{orderNumber}}
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

.animation-shrimp{
    position: absolute;
    animation: bounce 1.5s ease-in-out infinite, rotate 4s linear infinite;;
}

@keyframes bounce {
  0% { bottom: 0; }
  50% { bottom: 20px; }
  100% { bottom: 0; }
}

@keyframes rotate {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.shrimp-body{
    fill: #e72410;
}

.shrimp-eye{
    fill: #e72410;
}

.loading{
    height: calc(100vh - 100px);
    width: 100vw;
    top: 100px;
    position: absolute;
    background-color: #ffffff;
}
</style>