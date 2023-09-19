<script>
import { onMounted, ref } from 'vue';

export default{
  setup(){
    let profileDisplay = ref(true)
    let groupshoppingDisplay = ref(false)
    let clickSureButton = ()=>{
      profileDisplay.value = false
      groupshoppingDisplay.value = true
    }
    onMounted(()=>{
      console.log("HI")
    })
    return {
      profileDisplay, 
      groupshoppingDisplay,
      clickSureButton 
    }
  }
}
</script>

<template>
  <div class="content">
    <div class="layout-header-fix">
      <img src="./img/headerlogo.png" alt="">
    </div>
    <div class="profile" v-show="profileDisplay">
      <div class="login-form-wrap">
        <h2>填寫基本資料</h2>
        <form class="login-form">
          <p>
            <input type="text" class="username" placeholder="姓名">
          </p>
          <p>
            <div class="sure-button" v-on:click="clickSureButton
            ">確認</div>
          </p>
        </form>
      </div>
    </div>
    <div class="layout-body" v-show="groupshoppingDisplay">
      <div class="layout-body-content">
        <div class="layout-body-content-left">
          <img src="./img/example.jpg" alt="">
        </div>
        <div class="layout-body-content-right">
          <div>溫層測試-冷藏</div>
          <div class="layout-body-content-right-price">NT$1 ~ NT$10</div>
          <div>
            <div>數量</div>
            <div class="qty-wrapper">
              <div class="qty-wrapper-minus">-</div>
              <div>
                <input type="number">
              </div>
              <div class="qty-wrapper-plus">+</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.profile{
  
}
.login-form-wrap{
  background-color: #fff;
    width: 35%;
    margin: 30px auto;
    text-align: center;
    padding: 20px 0 0 0;
    border-radius: 4px;
    box-shadow: 0px 30px 50px 0px rgba(0, 0, 0, 0.2);
}
.username{
  width: 100%;
  padding: 0 0 0 10px;
  margin: 0;
  color: #8a8b8e;
  border: 1px solid #c2c0ca;
  font-style: normal;
  font-size: 16px;
  -webkit-appearance: none;
  -moz-appearance: none;
  appearance: none;
  position: relative;
  display: inline-block;
  background: none;
}
.sure-button{
  cursor: pointer;
}
.content{
  height: calc(100vh - 100px);
}
.layout-header-fix{
  background-color: #e72410;
  height: 100px;
}
.layout-body{
  height: calc(100vh - 100px);
}
.layout-body-content{
  padding: 20px;
  height: 100%;
  display: flex;
}
.layout-body-content-right-price{
  font-weight: 700;
  font-size: 24px;
  color: #70c1d6;
}
.qty-wrapper{
  border: 1px solid #DDD;
  display: flex;
}.qty-wrapper-minus{
  width: 20px;
  height: 20px;
  text-align: center;
}
.qty-wrapper-minus{
  border-right: 1px solid #DDD;
}
.qty-wrapper-plus{
  border-left: 1px solid #DDD;
  width: 20px;
  height: 20px;
}
</style>
