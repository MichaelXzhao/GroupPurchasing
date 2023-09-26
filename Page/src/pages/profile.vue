<script>
import { onMounted, ref } from 'vue';
import axios from 'axios'
import Loading from "../components/Loading.vue"

export default{
    setup() {
        onMounted(() => {
        });
        return {
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
        <div class="profile-point">
            <h1>積分</h1>
            <h1>50</h1>
        </div>
        <div class="profile-recommender">
            <table class="hostGroup-list-table-table">
                <thead class="hostGroup-list-table-table-thead">
                    <tr>
                        <th>名稱</th>
                    </tr>
                </thead>
                <tbody class="hostGroup-list-table-table-tbody">
                    <tr>
                        <td>Micheal</td>
                    </tr>
                    <tr>
                        <td>Irene</td>
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

.profile-content{
    height: calc(100vh - 100px);
}

.profile-point{
    display: flex;
}

.profile-recommender{

}
</style>