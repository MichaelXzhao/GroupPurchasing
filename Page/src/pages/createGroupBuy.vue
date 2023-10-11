<script>
import { onMounted, ref } from 'vue';
import Loading from "../components/Loading.vue"
import router from '../router/router.js'
import segmentatesalepageid from "../function/segmentatesalepageid.js"
import axios from 'axios'

export default{
    setup() {
        let username = ref("");
        let groupbuyname = ref("")
        let startdate = ref("")
        let enddate = ref("")
        let salepageid = ref("")
        let createGroupBuy =()=>{
            localStorage.setItem("username", username.value)
            localStorage.setItem("groupbuyname", groupbuyname.value)
            localStorage.setItem("startdate", startdate.value)
            localStorage.setItem("enddate", enddate.value)
            localStorage.setItem("salepageid", salepageid.value)

            axios.post(import.meta.env.VITE_API_URL + "api/Dashboard/Leader",{
                user_name: localStorage.getItem("username"),
                campaign: groupbuyname.value,
                start: startdate.value,
                finish: enddate.value,
                salepageid: localStorage.getItem("salepageid")
            })
            .then(res=>{
                console.log(res)
            })
            .catch(err=>{
                console.log(err)
            })
            .finally(()=>{
                router.push({path:"/createGroupBuy/hostGroupBuy"})
            })
        }
        onMounted(() => {
            // console.log("HI");
            // localStorage.setItem("salepageid", segmentatesalepageid.segmentatesalepageid())
        });
        return {
            username,
            groupbuyname,
            startdate,
            enddate,
            salepageid,
            createGroupBuy
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
    <div class="createGroupBuy-content">
        <div class="createGroupBuy-content-frame">
            <div class="createGroupBuy-content-form">
                <div class="createGroupBuy-content-form-name">
                    <input type="text" class="createGroupBuy-content-form-name-input" placeholder="輸入商品編號" v-model="salepageid">
                </div>
                <div class="createGroupBuy-content-form-name">
                    <input type="text" class="createGroupBuy-content-form-name-input" placeholder="輸入團購名稱" v-model="groupbuyname">
                </div>
                <div class="createGroupBuy-content-form-name">
                    <input type="text" class="createGroupBuy-content-form-name-input" placeholder="輸入名字" v-model="username">
                </div>
                <label style="margin-left: 23px;" for="">輸入開始日期</label>
                <div class="createGroupBuy-content-form-name">
                    <input type="date" class="createGroupBuy-content-form-name-input" placeholder="輸入開始日期" v-model="startdate">
                </div>
                <label style="margin-left: 23px;" for="">輸入結束日期</label>
                <div class="createGroupBuy-content-form-name">
                    <input type="date" class="createGroupBuy-content-form-name-input" placeholder="輸入結束日期" v-model="enddate">
                </div>
            </div>
            <div class="createGroupBuy-content-form-right">
                <div class="createGroupBuy-content-form-logo">
                    <svg class="createGroupBuy-content-form-logo-logo" style="width: 30px; height: 45.5px;" width="297" height="270" viewBox="0 0 297 270" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <path style="fill: #e72410;" class="shrimp-body" d="M297 113.601C297 50.962 246.236 0 183.84 0H10.259C4.593 0 0 4.594 0 10.259C0 11.4 0.125 38.492 11.011 65.957C26.156 104.16 54.361 124.354 92.578 124.354H94.976L85.719 142.869C83.185 147.936 85.239 154.099 90.308 156.633C91.781 157.369 93.345 157.718 94.887 157.718C98.65 157.718 102.274 155.64 104.071 152.044L117.916 124.355H183.702C208.557 124.355 229.556 143.428 229.556 166.005C229.556 186.483 212.459 203.467 190.302 206.27C189.44 201.773 187.865 196.147 184.996 190.445C176.456 173.474 160.371 164.128 139.705 164.128H94.895C89.23 164.128 84.636 168.722 84.636 174.388C84.636 175.693 84.81 187.485 90.857 199.629C94.371 206.688 99.211 212.405 105.129 216.706C99.21 221.007 94.37 226.724 90.857 233.782C84.81 245.927 84.636 257.719 84.636 259.024C84.636 264.69 89.23 269.283 94.895 269.283H139.705C160.371 269.283 176.456 259.936 184.996 242.967C187.892 237.208 189.468 231.532 190.325 227.013C249.716 223.625 297 174.054 297 113.601ZM139.705 184.645C152.539 184.645 161.27 189.388 166.4 199.147C167.519 201.275 168.355 203.436 168.981 205.465H136.055C123.242 205.465 114.551 200.735 109.488 191.002C108.375 188.86 107.547 186.685 106.93 184.645H139.705ZM166.668 233.742C161.581 243.85 152.761 248.764 139.705 248.764H106.935C107.501 246.884 108.246 244.893 109.225 242.926C114.244 232.844 123.02 227.943 136.056 227.943H168.977C168.403 229.811 167.651 231.789 166.668 233.742ZM219.189 27.567L173.427 103.835H122.081V20.519H183.839C196.352 20.519 208.291 23.029 219.189 27.567ZM30.274 58.871C24.508 44.518 22.149 29.919 21.184 20.518H101.564V103.834H92.578C63.223 103.835 42.261 88.708 30.274 58.871ZM196.64 105.031L237.148 37.516C258.592 52.728 273.348 76.849 276.036 104.474L228.49 120.323C219.414 112.481 208.452 107.241 196.64 105.031ZM248.198 180.496C249.423 175.849 250.073 170.995 250.073 166.005C250.073 155.868 247.434 146.078 242.477 137.289L275.626 126.24C272.755 147.403 262.782 166.327 248.198 180.496Z" fill="white"/>
                        <path style="fill: #e72410;" class="shrimp-eye" d="M61.038 35.632C58.34 35.632 55.693 36.73 53.784 38.638C51.876 40.546 50.778 43.194 50.778 45.892C50.778 48.589 51.876 51.237 53.784 53.144C55.703 55.052 58.34 56.151 61.038 56.151C63.735 56.151 66.383 55.052 68.29 53.144C70.209 51.237 71.297 48.589 71.297 45.892C71.297 43.194 70.209 40.546 68.29 38.638C66.393 36.73 63.746 35.632 61.038 35.632Z" fill="white"/>
                    </svg>
                </div>
                <div class="createGroupBuy-content-form-title">團購</div>
                <div style="text-align: center;margin-bottom: 20px;"><p>一起揪團吧!</p></div>
                <div class="createGroupBuy-content-form-button">
                    <button class="createGroupBuy-content-form-button-button" v-on:click="createGroupBuy">開團</button>
                </div>
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
}

.header-logo{
  width: 50px;
  position: absolute;
  top: 35px;
  left: 250px;
}

.createGroupBuy-content{
    height: calc(100vh - 100px);
    display: flex;
}

.createGroupBuy-content-frame{
    display: flex;
    margin: auto;
    border: 1px #DDD solid;
    border-radius: 10px;
}

.createGroupBuy-content-form-right{
    width: 440px;
    /* height: 300px; */
    /* margin: auto; */
    margin-right: auto;
    margin-top: auto;
    margin-bottom: auto;
    /* border: 1px #DDD solid;
    border-radius: 10px; */
    /* box-shadow: 0px 30px 50px 0px rgba(0, 0, 0, 0.2); */
}

.createGroupBuy-content-form{
    width: 440px;
    /* height: 300px; */
    /* margin: auto; */
    /* margin-left: auto;
    margin-top: auto;
    margin-bottom: auto;
    border: 1px #DDD solid;
    border-radius: 10px; */
    border-right: 1px solid #DDD;
    /* box-shadow: 0px 30px 50px 0px rgba(0, 0, 0, 0.2); */
}

.createGroupBuy-content-form-logo{
    margin-top: 20px;
    display: flex;
}

.createGroupBuy-content-form-logo-logo{
    margin: auto;
}

.createGroupBuy-content-form-title{
    text-align: center;
    font-size: xx-large;
    font-weight: 900;
    color: #e72410;
}

.createGroupBuy-content-form-name{
    display: flex;
    margin-bottom: 30px;
    margin-top: 30px;
}

.createGroupBuy-content-form-name-input{
    width: 300px;
    margin: auto;
    position: relative;
}

.createGroupBuy-content-form-name-input{
    width: 90%;
    height: 60px;
    margin: auto;
    border-width: 1px;
    border-style: solid;
    
    /* 使用linear-gradient創建漸變色 */
    border-color: #ddd;
    
    /* 控制邊框圖像如何拉伸和空間填充 */
    border-image-slice: 1;
    border-radius: 5px;
    font-size: 18px;
    padding-left: 16px;
    padding-right: 16px;
}

.createGroupBuy-content-form-button{
    height: 70px;
    display: flex;
}

.createGroupBuy-content-form-button-button{
    height: 50px;
    width: 80px;
    color: #FFF;
    font-size: 18px;
    font-weight: 900;
    font-family: Cambria, Cochin, Georgia, Times, 'Times New Roman', serif;
    margin: auto;
    border-radius: 5px;
    background-image: linear-gradient(105deg, #e22410, #f30063);
}
</style>