<script setup>
import HelloWorld from './components/HelloWorld.vue'
</script>

<template>
  <!-- <div class="layout-header-fix">
    <img style="height: 100px;" src="./img/169545314870657.png" alt="">
    <img class="header-logo" src="./img/Group (2).svg" alt="">
  </div> -->
  <RouterView></RouterView>
</template>

<style scoped>
.layout-header-fix{
  background-color: #e72410;
  height: 100px;
  display: flex;
}

.header-logo{
  width: 50px;
  position: absolute;
  top: 35px;
  left: 250px;
}
</style>
